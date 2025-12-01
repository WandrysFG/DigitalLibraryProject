document.addEventListener("DOMContentLoaded", () => {
    const apiBaseUrl = "https://localhost:7169/api";
    let allTales = [];
    let filteredTales = [];
    let allActivities = [];
    let filteredActivities = [];
    let allUsers = [];
    let filteredUsers = [];
    let allFavorites = [];
    let filteredFavorites = [];

    // ---------- Proteccion de paginas ----------
    const userJson = localStorage.getItem("stellarbooks_user");
    
    if (!userJson) {
        window.location.href = "login.html";
    } else {
        const currentUser = JSON.parse(userJson);
        if (window.location.pathname.includes("index.html") && currentUser.userType !== "Admin") {
            Swal.fire("Acceso denegado", "No tienes permisos para esta página", "warning").then(() => {
                window.location.href = "home.html";
            });
        }
    }

    // ---------- FECHA ACTUAL ----------
    document.getElementById("currentDate").textContent = new Date().toLocaleDateString("es-ES", {
        weekday: "long", year: "numeric", month: "long", day: "numeric"
    });

    // ---------- HELPERS ----------
    const fetchJson = async (url, options = {}) => {
        const response = await fetch(url, options);
        if (!response.ok) throw new Error("Error en la solicitud");
        if (response.status === 204) return null;
        return response.json();
    };

    const showAlert = (title, text, icon = "info") => Swal.fire({ title, text, icon });

    const confirmDelete = async (itemName = "este elemento") => {
        const result = await Swal.fire({
            title: `¿Estás seguro de eliminar ${itemName}?`,
            text: "¡No podrás deshacer esta acción!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#198754',
            cancelButtonColor: '#b72323ff',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });
        return result.isConfirmed;
    };

    const escapeHtml = (str) => {
        if (str == null) return "";
        return String(str)
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#039;");
    };

    const homeBtn = document.getElementById("homeBtn");
    if (homeBtn) {
        homeBtn.addEventListener("click", () => {
            window.location.href = "home.html";
        });
    }

    const indexBtn = document.getElementById("indexBtn");
    if (indexBtn) {
        indexBtn.addEventListener("click", () => {
            window.location.href = "index.html";
        });
    }

    // ---------- FUNCIONES DE CUENTOS ----------

    const clearTaleForm = () => {
        const form = document.getElementById("taleForm");
        if (form) form.reset();
        ["taleId", "taleCoverImage", "taleNarrationAudio", "talePublicationDate"].forEach(id => {
            const element = document.getElementById(id);
            if (element) element.value = "";
        });
        const availableCheckbox = document.getElementById("taleIsAvailable");
        if (availableCheckbox) availableCheckbox.checked = true;
    };

    const loadTales = async () => {
        try {
            allTales = await fetchJson(`${apiBaseUrl}/tales`);
            applyFilters();
        } catch (e) {
            console.error("Error al cargar cuentos:", e);
            showAlert("Error", "No se pudieron cargar los cuentos", "error");
        }
    };

    const applyFilters = () => {
        const category = document.getElementById("taleCategoryFilter").value;
        const status = document.getElementById("taleStatusFilter").value;
        const ageRange = document.getElementById("taleAgeFilter").value;
        const sortBy = document.getElementById("taleSortBy").value;

        filteredTales = allTales.filter(t => {
            const matchesCategory = !category || t.theme === category;
            const matchesStatus = !status || String(t.isAvailable) === status;
            let matchesAge = true;
            if (ageRange) {
                const [minAge, maxAge] = ageRange.split("-").map(Number);
                matchesAge = t.recommendedAge >= minAge && t.recommendedAge <= maxAge;
            }
            return matchesCategory && matchesStatus && matchesAge;
        });

        filteredTales.sort((a, b) => {
            if (sortBy === "title") return a.title.localeCompare(b.title);
            if (sortBy === "title_desc") return b.title.localeCompare(a.title);
            if (sortBy === "date_created_desc") return new Date(b.publicationDate || 0) - new Date(a.publicationDate || 0);
            if (sortBy === "date_created") return new Date(a.publicationDate || 0) - new Date(b.publicationDate || 0);
            return 0;
        });

        renderTales(filteredTales);
    };

    const grid = document.getElementById("talesGrid");

    const renderTales = (tales) => {
    const grid = document.getElementById("talesGrid");
    const emptyState = document.getElementById("emptyState");
    const countBadge = document.getElementById("talesCount");

    countBadge.textContent = `${tales.length} / ${allTales.length}`;

    if (!Array.isArray(tales) || tales.length === 0) {
        grid.classList.add("d-none");
        emptyState.classList.remove("d-none");
        return;
    }

    grid.classList.remove("d-none");
    emptyState.classList.add("d-none");
    grid.innerHTML = "";

    tales.forEach(t => {
        const col = document.createElement("div");
        col.className = "col-md-4";

        let pubDateDisplay = "-";
        if (t.publicationDate) {
            try {
                const d = new Date(t.publicationDate);
                pubDateDisplay = d.toLocaleDateString("es-ES", { year: "numeric", month: "long", day: "numeric" });
            } catch {}
        }

        col.innerHTML = `
            <div class="card h-100">
                ${t.coverImage ? `<img src="${escapeHtml(t.coverImage)}" class="card-img-top" alt="Portada del cuento ${escapeHtml(t.title)}">` : ''}
                <div class="card-body d-flex flex-column">
                    <h5 class="card-title">${escapeHtml(t.title)}</h5>
                    <p class="card-text mb-1"><strong>Tema:</strong> ${escapeHtml(t.theme)}</p>
                    <p class="card-text mb-1"><strong>Edad recomendada:</strong> ${escapeHtml(String(t.recommendedAge))} años</p>
                    <p class="card-text mb-1"><strong>Publicado:</strong> ${escapeHtml(pubDateDisplay)}</p>
                    <p class="card-text mb-1">
                        <strong>Disponibilidad:</strong>
                        <span class="badge ${t.isAvailable ? "bg-success" : "bg-secondary"}">
                        ${t.isAvailable ? "Disponible" : "No disponible"}
                        </span>
                    </p>
                    ${t.narrationAudio ? `
                        <div class="mb-2">
                            <strong>Audio narración:</strong>
                            <div>
                                <audio controls preload="none" aria-label="Audio de narración de ${escapeHtml(t.title)}">
                                <source src="${escapeHtml(t.narrationAudio)}" type="audio/mpeg">
                                Tu navegador no soporta audio.
                                </audio>
                            </div>
                        </div>` : ''}
                    <div class="mt-auto d-flex gap-2">
                        <button type="button" class="btn btn-sm btn-outline-primary edit-tale-btn" data-tale='${JSON.stringify(t).replaceAll("'", "&#39;")}'>
                            <i class="bi bi-pencil"></i> Editar
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-danger delete-tale-btn" data-id="${t.id}">
                            <i class="bi bi-trash"></i> Eliminar
                        </button>
                    </div>
                </div>
            </div>
        `;
        grid.appendChild(col);
    });
    };

    const createFirstTaleBtn = document.getElementById("createFirstTale");
    if (createFirstTaleBtn) {
        createFirstTaleBtn.addEventListener("click", () => {
            clearTaleForm();
            document.getElementById("taleModalAction").textContent = "Nuevo Cuento";
            const modalEl = document.getElementById("taleModal");
            const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        });
    }

    if (grid) {
        grid.addEventListener("click", (e) => {
            const editBtn = e.target.closest(".edit-tale-btn");
            if (editBtn) {
                const tale = JSON.parse(editBtn.dataset.tale);
                editTale(tale);
                return;
            }

            const deleteBtn = e.target.closest(".delete-tale-btn");
            if (deleteBtn) {
                const id = deleteBtn.dataset.id;
                deleteTale(id);
                return;
            }
        });
    }

    const addTaleBtn = document.getElementById("addTaleBtn");
    if (addTaleBtn) {
        addTaleBtn.addEventListener("click", () => {
            clearTaleForm();
            document.getElementById("taleModalAction").textContent = "Nuevo Cuento";
            new bootstrap.Modal(document.getElementById("taleModal")).show();
        });
    }

    window.editTale = (tale) => {
        document.getElementById("taleId").value = tale.id || "";
        document.getElementById("taleTitle").value = tale.title || "";
        document.getElementById("taleRecommendedAge").value = tale.recommendedAge || "";
        document.getElementById("taleTheme").value = tale.theme || "";
        document.getElementById("taleContent").value = tale.content || "";
        document.getElementById("taleCoverImage").value = tale.coverImage || "";
        document.getElementById("taleNarrationAudio").value = tale.narrationAudio || "";
        document.getElementById("talePublicationDate").value = tale.publicationDate ? tale.publicationDate.slice(0, 10) : "";
        document.getElementById("taleIsAvailable").checked = !!tale.isAvailable;
        document.getElementById("taleModalAction").textContent = "Editar Cuento";
        new bootstrap.Modal(document.getElementById("taleModal")).show();
    };

    const saveTaleBtn = document.getElementById("saveTaleBtn");
    if (saveTaleBtn) {
        saveTaleBtn.addEventListener("click", async () => {
            const id = document.getElementById("taleId").value;
            const title = document.getElementById("taleTitle").value.trim();
            const recommendedAge = parseInt(document.getElementById("taleRecommendedAge").value, 10);
            const theme = document.getElementById("taleTheme").value.trim();
            const content = document.getElementById("taleContent").value.trim();
            const coverImage = document.getElementById("taleCoverImage").value.trim();
            const narrationAudio = document.getElementById("taleNarrationAudio").value.trim();
            const publicationDate = document.getElementById("talePublicationDate").value;
            const isAvailable = document.getElementById("taleIsAvailable").checked;

            if (!title || isNaN(recommendedAge) || !theme || !content) {
                return showAlert("Campos requeridos", "Por favor, completa todos los campos obligatorios.", "warning");
            }

            const tale = { id: id ? parseInt(id) : 0, title, recommendedAge, theme, content, isAvailable };
            if (coverImage) tale.coverImage = coverImage;
            if (narrationAudio) tale.narrationAudio = narrationAudio;
            if (publicationDate) tale.publicationDate = publicationDate;

            try {
                await fetchJson(`${apiBaseUrl}/tales${id ? `/${id}` : ""}`, {
                    method: id ? "PUT" : "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(tale)
                });
                bootstrap.Modal.getInstance(document.getElementById("taleModal"))?.hide();
                loadTales();
                showAlert("Éxito", "Cuento guardado correctamente", "success");
            } catch (e) {
                console.error("Error guardando cuento:", e);
                showAlert("Error", e.message || "No se pudo guardar el cuento", "error");
            }
        });
    }

    const deleteTale = async (id) => {
        if (!(await confirmDelete("el cuento"))) return;
        try {
            await fetchJson(`${apiBaseUrl}/tales/${id}`, { method: "DELETE" });
            loadTales();
            showAlert("Éxito", "Cuento eliminado", "success");
        } catch (e) {
            console.error("Error eliminando cuento:", e);
            showAlert("Error", "No se pudo eliminar el cuento", "error");
        }
    };

    ["taleCategoryFilter", "taleStatusFilter", "taleAgeFilter", "taleSortBy"].forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener("change", applyFilters);
        }
    });

    const clearTaleFiltersBtn = document.getElementById("clearTaleFilters");
    if (clearTaleFiltersBtn) {
        clearTaleFiltersBtn.addEventListener("click", () => {
            document.getElementById("taleFiltersForm").reset();
            applyFilters();
        });
    }

    // ---------- FUNCIONES DE ACTIVIDADES ----------

    const clearActivityForm = () => {
        const form = document.getElementById("activityForm");
        if (form) {
            form.reset();
            document.getElementById("activityId").value = "";
        }
    };

    const populateActivityTaleDropdown = () => {
        const select = document.getElementById("activityStoryId");
        if (select) {
            select.innerHTML = `<option value="">Seleccionar cuento...</option>`;
            allTales.forEach(t => {
                const option = document.createElement("option");
                option.value = t.id;
                option.textContent = t.title;
                select.appendChild(option);
            });
        }
    };

    const populateActivityTaleFilterOptions = () => {
        const select = document.getElementById("activityTaleFilter");
        if (select) {
            select.innerHTML = `<option value="">Todos los cuentos</option>`;
            allTales.forEach(t => {
                const option = document.createElement("option");
                option.value = t.id;
                option.textContent = t.title;
                select.appendChild(option);
            });
        }
    };

    const applyActivityFilters = () => {
        const taleFilter = document.getElementById("activityTaleFilter")?.value;
        const typeFilter = document.getElementById("activityTypeFilter")?.value;
        const sortBy = document.getElementById("activitySortBy")?.value || "id";

        filteredActivities = allActivities.filter(a => {
            const matchesTale = !taleFilter || String(a.taleId) === taleFilter;
            const matchesType = !typeFilter || a.activityType === typeFilter;
            return matchesTale && matchesType;
        });

        filteredActivities.sort((a, b) => {
            const taleA = allTales.find(t => t.id === a.taleId);
            const taleB = allTales.find(t => t.id === b.taleId);

            switch (sortBy) {
                case "title":
                    return (taleA?.title || "").localeCompare(taleB?.title || "");
                case "title_desc":
                    return (taleB?.title || "").localeCompare(taleA?.title || "");
                case "date_created_desc":
                    return new Date(b.createdAt) - new Date(a.createdAt);
                case "date_created":
                    return new Date(a.createdAt) - new Date(b.createdAt);
                default:
                    return a.id - b.id;
            }
        });

        renderActivitiesTable(filteredActivities);
    };

    const loadActivities = async () => {
        try {
            allActivities = await fetchJson(`${apiBaseUrl}/activities`);
            populateActivityTaleFilterOptions();
            populateActivityTaleDropdown();
            applyActivityFilters();
        } catch (e) {
            console.error("Error al cargar actividades:", e);
            showAlert("Error", "No se pudieron cargar las actividades", "error");
        }
    };

    const renderActivitiesTable = (activities) => {
        const tbody = document.getElementById("activitiesTableBody");
        const activitiesCount = document.getElementById("activitiesCount");
        const emptyState = document.getElementById("emptyActivitiesState");

        if (!tbody) {
            console.error("No se encontró el elemento activitiesTableBody");
            return;
        }

        tbody.innerHTML = "";

        if (activitiesCount) {
            activitiesCount.textContent = `${activities.length} / ${allActivities.length}`;
        }

        if (!activities.length) {
            if (emptyState) {
                emptyState.classList.remove("d-none");
            }
            tbody.innerHTML = `
                <tr>
                    <td colspan="6" class="text-center text-muted">No hay actividades disponibles</td>
                </tr>
            `;
            return;
        } else {
            if (emptyState) {
                emptyState.classList.add("d-none");
            }
        }

        activities.forEach(a => {
            const tale = allTales.find(t => t.id === a.taleId);
            const taleTitle = tale ? tale.title : `Cuento ID: ${a.taleId}`;

            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${a.id}</td>
                <td>${escapeHtml(taleTitle)}</td>
                <td>${escapeHtml(a.activityType)}</td>
                <td>${escapeHtml(a.description || "")}</td>
                <td>${a.multimediaResource ? `<a href="${escapeHtml(a.multimediaResource)}" target="_blank" class="btn btn-sm btn-outline-info">Ver recurso</a>` : "-"}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-outline-primary edit-activity-btn" 
                            data-activity='${JSON.stringify(a).replaceAll("'", "&#39;")}'>
                        <i class="bi bi-pencil"></i> Editar
                    </button>
                    <button type="button" class="btn btn-sm btn-outline-danger delete-activity-btn" 
                            data-id="${a.id}">
                        <i class="bi bi-trash"></i> Eliminar
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    };

    const activitiesTableBody = document.getElementById("activitiesTableBody");
    if (activitiesTableBody) {
        activitiesTableBody.addEventListener("click", (e) => {
            const editBtn = e.target.closest(".edit-activity-btn");
            if (editBtn) {
                const activity = JSON.parse(editBtn.dataset.activity);
                editActivity(activity);
                return;
            }

            const deleteBtn = e.target.closest(".delete-activity-btn");
            if (deleteBtn) {
                const id = deleteBtn.dataset.id;
                deleteActivity(id);
                return;
            }
        });
    }

    window.editActivity = (activity) => {
        document.getElementById("activityId").value = activity.id || "";
        document.getElementById("activityStoryId").value = activity.taleId || "";
        document.getElementById("activityType").value = activity.activityType || "";
        document.getElementById("activityDescription").value = activity.description || "";
        document.getElementById("activityMultimediaUrl").value = activity.multimediaResource || "";

        document.getElementById("activityModalAction").textContent = "Editar Actividad";
        new bootstrap.Modal(document.getElementById("activityModal")).show();
    };

    const addActivityBtn = document.getElementById("addActivityBtn");
    if (addActivityBtn) {
        addActivityBtn.addEventListener("click", () => {
            clearActivityForm();
            populateActivityTaleDropdown();
            document.getElementById("activityModalAction").textContent = "Nueva Actividad";
            new bootstrap.Modal(document.getElementById("activityModal")).show();
        });
    }

    const createFirstActivityBtn = document.getElementById("createFirstActivity");
    if (createFirstActivityBtn) {
        createFirstActivityBtn.addEventListener("click", () => {
            clearActivityForm();
            populateActivityTaleDropdown();
            document.getElementById("activityModalAction").textContent = "Nueva Actividad";
            new bootstrap.Modal(document.getElementById("activityModal")).show();
        });
    }

    const saveActivityBtn = document.getElementById("saveActivityBtn");
    if (saveActivityBtn) {
        saveActivityBtn.addEventListener("click", async () => {
            const id = document.getElementById("activityId").value;
            const taleId = parseInt(document.getElementById("activityStoryId").value, 10);
            const activityType = document.getElementById("activityType").value;
            const description = document.getElementById("activityDescription").value.trim();
            const multimediaResource = document.getElementById("activityMultimediaUrl").value.trim();

            if (!taleId || !activityType || !description) {
                return showAlert("Campos requeridos", "Por favor, completa todos los campos obligatorios.", "warning");
            }

            const activity = { 
                id: id ? parseInt(id) : 0, 
                taleId, 
                activityType, 
                description
            };

            if (multimediaResource) {
                activity.multimediaResource = multimediaResource;
            }

            try {
                await fetchJson(`${apiBaseUrl}/activities${id ? `/${id}` : ""}`, {
                    method: id ? "PUT" : "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(activity)
                });

                bootstrap.Modal.getInstance(document.getElementById("activityModal"))?.hide();
                await loadActivities();
                showAlert("Éxito", "Actividad guardada correctamente", "success");
            } catch (e) {
                console.error("Error guardando actividad:", e);
                showAlert("Error", e.message || "No se pudo guardar la actividad", "error");
            }
        });
    }

    const deleteActivity = async (id) => {
        if (!(await confirmDelete("la actividad"))) return;
        try {
            await fetchJson(`${apiBaseUrl}/activities/${id}`, { method: "DELETE" });
            await loadActivities();
            showAlert("Éxito", "Actividad eliminada", "success");
        } catch (e) {
            console.error("Error eliminando actividad:", e);
            showAlert("Error", "No se pudo eliminar la actividad", "error");
        }
    };

    ["activityTaleFilter", "activityTypeFilter", "activitySortBy"].forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener("change", applyActivityFilters);
        }
    });

    const clearActivityFiltersBtn = document.getElementById("clearActivityFilters");
    if (clearActivityFiltersBtn) {
        clearActivityFiltersBtn.addEventListener("click", () => {
            const form = document.getElementById("activityFiltersForm");
            if (form) {
                form.reset();
                applyActivityFilters();
            }
        });
    }

    // ---------- FUNCIONES DE FAVORITOS ----------

    const clearFavoriteForm = () => {
        const form = document.getElementById("favoriteForm");
        if (form) {
            form.reset();
            document.getElementById("favoriteId").value = "";
        }
    };

    const populateFavoriteUserDropdown = () => {
        const select = document.getElementById("favoriteUserId");
        if (select) {
            select.innerHTML = `<option value="">Seleccionar usuario...</option>`;
            allUsers.forEach(u => {
                const option = document.createElement("option");
                option.value = u.id;
                option.textContent = `${u.firstName} ${u.lastName} (${u.email})`;
                select.appendChild(option);
            });
        }
    };

    const populateFavoriteTaleDropdown = () => {
        const select = document.getElementById("favoriteTaleId");
        if (select) {
            select.innerHTML = `<option value="">Seleccionar cuento...</option>`;
            allTales.forEach(t => {
                const option = document.createElement("option");
                option.value = t.id;
                option.textContent = t.title;
                select.appendChild(option);
            });
        }
    };

    const populateFavoriteFilterOptions = () => {
        const userSelect = document.getElementById("favoriteUserFilter");
        if (userSelect) {
            userSelect.innerHTML = `<option value="">Todos los usuarios</option>`;
            allUsers.forEach(u => {
                const option = document.createElement("option");
                option.value = u.id;
                option.textContent = `${u.firstName} ${u.lastName}`;
                userSelect.appendChild(option);
            });
        }

        const taleSelect = document.getElementById("favoriteTaleFilter");
        if (taleSelect) {
            taleSelect.innerHTML = `<option value="">Todos los cuentos</option>`;
            allTales.forEach(t => {
                const option = document.createElement("option");
                option.value = t.id;
                option.textContent = t.title;
                taleSelect.appendChild(option);
            });
        }
    };

    const loadFavorites = async () => {
        try {
            allFavorites = await fetchJson(`${apiBaseUrl}/favorites`);
            populateFavoriteFilterOptions();
            populateFavoriteUserDropdown();
            populateFavoriteTaleDropdown();
            applyFavoriteFilters();
        } catch (e) {
            console.error("Error al cargar favoritos:", e);
            showAlert("Error", "No se pudieron cargar los favoritos", "error");
        }
    };

    const applyFavoriteFilters = () => {
        const userFilter = document.getElementById("favoriteUserFilter")?.value || "";
        const taleFilter = document.getElementById("favoriteTaleFilter")?.value || "";
        const dateFilter = document.getElementById("favoriteDate")?.value || "";
        const sortBy = document.getElementById("favoriteSortBy")?.value || "id";

        filteredFavorites = allFavorites.filter(f => {
            const matchesUser = !userFilter || String(f.userId) === userFilter;
            const matchesTale = !taleFilter || String(f.taleId) === taleFilter;
            const matchesDate = !dateFilter || f.dateAdded?.startsWith(dateFilter);
            return matchesUser && matchesTale && matchesDate;
        });

        filteredFavorites.sort((a, b) => {
            switch (sortBy) {
                case "dateAdded_desc":
                    return new Date(b.dateAdded || 0) - new Date(a.dateAdded || 0);
                case "dateAdded":
                    return new Date(a.dateAdded || 0) - new Date(b.dateAdded || 0);
                case "id_desc":
                    return b.id - a.id;
                case "id":
                default:
                    return a.id - b.id;
            }
        });

        renderFavoritesTable(filteredFavorites);
    };

    const renderFavoritesTable = (favorites) => {
        const tbody = document.getElementById("favoritesTableBody");
        const favoritesCount = document.getElementById("favoritesCount");
        const emptyState = document.getElementById("emptyFavoritesState");

        if (!tbody) return;

        tbody.innerHTML = "";
        if (favoritesCount) favoritesCount.textContent = `${favorites.length} / ${allFavorites.length}`;

        if (!favorites.length) {
            if (emptyState) emptyState.classList.remove("d-none");
            tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">No hay favoritos disponibles</td></tr>`;
            return;
        } else {
            if (emptyState) emptyState.classList.add("d-none");
        }

        favorites.forEach(f => {
            const user = allUsers.find(u => u.id === f.userId);
            const tale = allTales.find(t => t.id === f.taleId);

            const userName = user ? `${user.firstName} ${user.lastName}` : `Usuario ID: ${f.userId}`;
            const taleTitle = tale ? tale.title : `Cuento ID: ${f.taleId}`;
            const dateDisplay = f.dateAdded ? new Date(f.dateAdded).toLocaleDateString("es-ES", { year:"numeric", month:"long", day:"numeric" }) : "-";

            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${f.id}</td>
                <td>${escapeHtml(userName)}</td>
                <td>${escapeHtml(taleTitle)}</td>
                <td>${escapeHtml(dateDisplay)}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-outline-primary edit-favorite-btn" 
                            data-favorite='${JSON.stringify(f).replaceAll("'", "&#39;")}'>
                        <i class="bi bi-pencil"></i> Editar
                    </button>
                    <button type="button" class="btn btn-sm btn-outline-danger delete-favorite-btn" 
                            data-id="${f.id}">
                        <i class="bi bi-trash"></i> Eliminar
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    };

    window.editFavorite = (favorite) => {
        document.getElementById("favoriteId").value = favorite.id || "";
        document.getElementById("favoriteUserId").value = favorite.userId || "";
        document.getElementById("favoriteTaleId").value = favorite.taleId || "";
        document.getElementById("favoriteDateAdded").value = favorite.dateAdded ? favorite.dateAdded.slice(0, 10) : "";

        document.getElementById("favoriteModalAction").textContent = "Editar Favorito";
        new bootstrap.Modal(document.getElementById("favoriteModal")).show();
    };

    const deleteFavorite = async (id) => {
        if (!(await confirmDelete("el favorito"))) return;
        try {
            await fetchJson(`${apiBaseUrl}/favorites/${id}`, { method: "DELETE" });
            await loadFavorites();
            showAlert("Éxito", "Favorito eliminado", "success");
        } catch (e) {
            console.error("Error eliminando favorito:", e);
            showAlert("Error", "No se pudo eliminar el favorito", "error");
        }
    };

    const favoritesTableBody = document.getElementById("favoritesTableBody");
    if (favoritesTableBody) {
        favoritesTableBody.addEventListener("click", (e) => {
            const editBtn = e.target.closest(".edit-favorite-btn");
            if (editBtn) {
                const favorite = JSON.parse(editBtn.dataset.favorite);
                editFavorite(favorite);
                return;
            }

            const deleteBtn = e.target.closest(".delete-favorite-btn");
            if (deleteBtn) deleteFavorite(deleteBtn.dataset.id);
        });
    }

    const addFavoriteBtn = document.getElementById("addFavoriteBtn");
    if (addFavoriteBtn) {
        addFavoriteBtn.addEventListener("click", () => {
            clearFavoriteForm();
            populateFavoriteUserDropdown();
            populateFavoriteTaleDropdown();
            document.getElementById("favoriteModalAction").textContent = "Nuevo Favorito";
            new bootstrap.Modal(document.getElementById("favoriteModal")).show();
        });
    }

    const saveFavoriteBtn = document.getElementById("saveFavoriteBtn");
    if (saveFavoriteBtn) {
        saveFavoriteBtn.addEventListener("click", async () => {
            const id = document.getElementById("favoriteId").value;
            const userId = parseInt(document.getElementById("favoriteUserId").value, 10);
            const taleId = parseInt(document.getElementById("favoriteTaleId").value, 10);
            const dateAdded = document.getElementById("favoriteDateAdded").value;

            if (!userId || !taleId) {
                return showAlert("Campos requeridos", "Por favor, selecciona un usuario y un cuento.", "warning");
            }

            const favorite = { id: id ? parseInt(id) : 0, userId, taleId };
            if (dateAdded) favorite.dateAdded = dateAdded;

            try {
                await fetchJson(`${apiBaseUrl}/favorites${id ? `/${id}` : ""}`, {
                    method: id ? "PUT" : "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(favorite)
                });

                bootstrap.Modal.getInstance(document.getElementById("favoriteModal"))?.hide();
                await loadFavorites();
                showAlert("Éxito", "Favorito guardado correctamente", "success");
            } catch (e) {
                console.error("Error guardando favorito:", e);
                showAlert("Error", e.message || "No se pudo guardar el favorito", "error");
            }
        });
    }

    ["favoriteUserFilter", "favoriteTaleFilter", "favoriteDate", "favoriteSortBy"].forEach(id => {
        const element = document.getElementById(id);
        if (element) element.addEventListener("change", applyFavoriteFilters);
    });

    const clearFavoriteFiltersBtn = document.getElementById("clearFavoriteFilters");
    if (clearFavoriteFiltersBtn) {
        clearFavoriteFiltersBtn.addEventListener("click", () => {
            const form = document.getElementById("favoriteFiltersForm");
            if (form) {
                form.reset();
                applyFavoriteFilters();
            }
        });
    }

    // ---------- FUNCIONES DE USUARIOS ----------

    const clearUserForm = () => {
        const form = document.getElementById("userForm");
        if (form) {
            form.reset();
            document.getElementById("userId").value = "";
            document.getElementById("userIsActive").checked = true;
            document.getElementById("userPassword").value = "";
            document.getElementById("userPassword").required = true;
        }
    };

    const loadUsers = async () => {
        try {
            allUsers = await fetchJson(`${apiBaseUrl}/users`);
            applyUserFilters();
        } catch (e) {
            console.error("Error al cargar usuarios:", e);
            showAlert("Error", "No se pudieron cargar los usuarios", "error");
        }
    };

    const applyUserFilters = () => {
        const typeFilter = document.getElementById("userTypeFilter")?.value || "";
        const statusFilter = document.getElementById("userStatusFilter")?.value || "";
        const dateFilter = document.getElementById("userDate")?.value || "";
        const sortBy = document.getElementById("userSortBy")?.value || "id";

        filteredUsers = allUsers.filter(u => {
            const matchesType = !typeFilter || u.userType === typeFilter;
            const matchesStatus = !statusFilter || String(u.isActive) === statusFilter;
            const matchesDate = !dateFilter || (u.registrationDate && u.registrationDate.slice(0, 10) === dateFilter);
            return matchesType && matchesStatus && matchesDate;
        });

        filteredUsers.sort((a, b) => {
            switch (sortBy) {
                case "name": return a.firstName.localeCompare(b.firstName);
                case "name_desc": return b.firstName.localeCompare(a.firstName);
                case "id": return a.id - b.id;
                case "id_desc": return b.id - a.id;
                case "registrationDate_desc": return new Date(b.registrationDate || 0) - new Date(a.registrationDate || 0);
                case "registrationDate": return new Date(a.registrationDate || 0) - new Date(b.registrationDate || 0);
                default: return a.id - b.id;
            }
        });

        renderUsersTable(filteredUsers);
    };

    const renderUsersTable = (users) => {
        const tbody = document.getElementById("usersTableBody");
        const usersCount = document.getElementById("usersCount");
        const emptyState = document.getElementById("emptyUsersState");
        
        if (!tbody) return console.error("No se encontró el elemento usersTableBody");
        
        tbody.innerHTML = "";
        
        if (usersCount) usersCount.textContent = `${users.length} / ${allUsers.length}`;

        if (!users.length) {
            if (emptyState) emptyState.classList.remove("d-none");
            tbody.innerHTML = `<tr><td colspan="8" class="text-center text-muted">No hay usuarios disponibles</td></tr>`;
            return;
        } else if (emptyState) {
            emptyState.classList.add("d-none");
        }

        users.forEach(u => {
            let regDateDisplay = "-";
            if (u.registrationDate) {
                try {
                    const d = new Date(u.registrationDate);
                    regDateDisplay = d.toLocaleDateString("es-ES", { year: "numeric", month: "long", day: "numeric" });
                } catch {}
            }

            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${u.id}</td>
                <td>${escapeHtml(u.firstName)}</td>
                <td>${escapeHtml(u.lastName)}</td>
                <td>${escapeHtml(u.email)}</td>
                <td><span class="badge ${u.userType === 'Admin' ? 'bg-danger' : 'bg-primary'}">${escapeHtml(u.userType)}</span></td>
                <td>${escapeHtml(regDateDisplay)}</td>
                <td>
                    <span class="badge ${u.isActive ? "bg-success" : "bg-secondary"}">
                        ${u.isActive ? "Activo" : "Inactivo"}
                    </span>
                </td>
                <td>
                    <button type="button" class="btn btn-sm btn-outline-primary edit-user-btn" 
                            data-user='${JSON.stringify(u).replaceAll("'", "&#39;")}' aria-label="Editar usuario">
                        <i class="bi bi-pencil"></i> Editar
                    </button>
                    <button type="button" class="btn btn-sm btn-outline-danger delete-user-btn" 
                            data-id="${u.id}" aria-label="Eliminar usuario">
                        <i class="bi bi-trash"></i> Eliminar
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    };

    const addUserBtn = document.getElementById("addUserBtn");
    if (addUserBtn) addUserBtn.addEventListener("click", () => {
        clearUserForm();
        document.getElementById("userModalAction").textContent = "Nuevo Usuario";
        new bootstrap.Modal(document.getElementById("userModal")).show();
    });

    const createFirstUserBtn = document.getElementById("createFirstUser");
    if (createFirstUserBtn) createFirstUserBtn.addEventListener("click", () => {
        clearUserForm();
        document.getElementById("userModalAction").textContent = "Nuevo Usuario";
        new bootstrap.Modal(document.getElementById("userModal")).show();
    });

    const usersTableBody = document.getElementById("usersTableBody");
    if (usersTableBody) {
        usersTableBody.addEventListener("click", (e) => {
            const editBtn = e.target.closest(".edit-user-btn");
            if (editBtn) {
                const user = JSON.parse(editBtn.dataset.user);
                editUser(user);
                return;
            }
            const deleteBtn = e.target.closest(".delete-user-btn");
            if (deleteBtn) {
                const id = deleteBtn.dataset.id;
                deleteUser(id);
            }
        });
    }

    window.editUser = async (user) => {
        await loadUsers();
        const updatedUser = allUsers.find(u => u.id === user.id);
        if (!updatedUser) {
            showAlert("Error", `El usuario con ID ${user.id} ya no existe.`, "error");
            return;
        }
        document.getElementById("userId").value = updatedUser.id ? String(updatedUser.id) : "";
        document.getElementById("userFirstName").value = updatedUser.firstName || "";
        document.getElementById("userLastName").value = updatedUser.lastName || "";
        document.getElementById("userEmail").value = updatedUser.email || "";
        document.getElementById("userPassword").value = updatedUser.password || "";
        document.getElementById("userPassword").required = true;
        document.getElementById("userPassword").placeholder = "Contraseña actual";
        document.getElementById("userType").value = updatedUser.userType || "";
        document.getElementById("userRegistrationDate").value = updatedUser.registrationDate ? updatedUser.registrationDate.slice(0, 10) : "";
        document.getElementById("userIsActive").checked = !!updatedUser.isActive;

        document.getElementById("userModalAction").textContent = "Editar Usuario";
        const modalEl = document.getElementById("userModal");
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
        modalEl.removeAttribute("aria-hidden");
    };

    const userForm = document.getElementById("userForm");
    if (userForm) {
        userForm.addEventListener("submit", async (e) => {
            e.preventDefault();

            const id = document.getElementById("userId").value.trim();
            const userIdNum = id ? parseInt(id, 10) : null;
            const password = document.getElementById("userPassword").value.trim();

            const userData = {
                firstName: document.getElementById("userFirstName").value.trim(),
                lastName: document.getElementById("userLastName").value.trim(),
                email: document.getElementById("userEmail").value.trim(),
                userType: document.getElementById("userType").value || "Reader",
                registrationDate: document.getElementById("userRegistrationDate").value || new Date().toISOString().split("T")[0],
                isActive: document.getElementById("userIsActive").checked
            };

            if (!userIdNum) {
                if (!password) {
                    showAlert("Error", "La contraseña es obligatoria para crear un usuario.", "error");
                    return;
                }
                userData.password = password;
            } else {
                if (!password) {
                    showAlert("Error", "Debes ingresar una contraseña para actualizar el usuario.", "error");
                    return;
                }
                userData.password = password;
            }

            try {
                if (userIdNum) {
                    const userExists = allUsers.some(u => u.id === userIdNum);
                    if (!userExists) {
                        showAlert("Error", `El usuario con ID ${userIdNum} no existe.`, "error");
                        return;
                    }
                }

                const url = userIdNum ? `${apiBaseUrl}/users/${userIdNum}` : `${apiBaseUrl}/users`;
                const method = userIdNum ? "PUT" : "POST";

                console.log("ID numérico a enviar:", userIdNum, typeof userIdNum);
                console.log("URL:", url);
                console.log("Datos:", userData);

                const response = await fetch(url, {
                    method: method,
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(userData)
                });

                if (!response.ok) {
                    const errorText = await response.text();
                    console.error(`Error ${response.status}:`, errorText);
                    throw new Error(`Error ${response.status}: ${errorText}`);
                }

                const result = await response.json();
                console.log("Usuario guardado exitosamente:", result);

                const userModal = bootstrap.Modal.getInstance(document.getElementById("userModal"));
                if (userModal) userModal.hide();

                const message = userIdNum ? "Usuario actualizado correctamente" : "Usuario creado correctamente";
                showAlert("Éxito", message, "success");

                await loadUsers();

            } catch (error) {
                console.error("Error al guardar usuario:", error);
                showAlert("Error", "No se pudo guardar el usuario. Verifica los datos.", "error");
            }
        });
    }

    const deleteUser = async (id) => {
        if (!(await confirmDelete("el usuario"))) return;
        try {
            const response = await fetch(`${apiBaseUrl}/users/${id}`, { method: "DELETE" });
            if (!response.ok) throw new Error(`Error ${response.status}`);
            
            await loadUsers();
            showAlert("Éxito", "Usuario eliminado", "success");
        } catch (error) {
            console.error("Error eliminando usuario:", error);
            showAlert("Error", "No se pudo eliminar el usuario", "error");
        }
    };

    ["userTypeFilter", "userStatusFilter", "userDate", "userSortBy"].forEach(id => {
        const element = document.getElementById(id);
        if (element) element.addEventListener("change", applyUserFilters);
    });

    const clearUserFiltersBtn = document.getElementById("clearUserFilters");
    if (clearUserFiltersBtn) clearUserFiltersBtn.addEventListener("click", () => {
        const form = document.getElementById("userFiltersForm");
        if (form) {
            form.reset();
            applyUserFilters();
        }
    });

    const saveUserBtn = document.getElementById("saveUserBtn");
    if (saveUserBtn) {
        saveUserBtn.addEventListener("click", () => {
            const form = document.getElementById("userForm");
            if (form) {
                form.dispatchEvent(new Event("submit", { cancelable: true, bubbles: true }));
            }
        });
    }

    // ---------- INICIALIZACION ----------
    
    const header = document.querySelector("header");
    if (header) {
        const headerOffset = header.offsetTop;
        window.addEventListener("scroll", () => {
            header.classList.toggle("fixed", window.pageYOffset > headerOffset);
        });
    }

    const initializeApp = async () => {
        try {
            await loadTales();
            await loadActivities();
            await loadFavorites();
            await loadUsers(); 
        } catch (error) {
            console.error("Error inicializando la aplicación:", error);
        }
    };

    initializeApp();
});