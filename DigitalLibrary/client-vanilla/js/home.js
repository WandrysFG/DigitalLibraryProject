document.addEventListener("DOMContentLoaded", () => {
    const apiBaseUrl = "https://localhost:7169/api";
    let allStories = [];
    let filteredStories = [];
    let allActivities = [];
    let filteredActivities = [];
    let allUsers = [];
    let currentUser = null;

    // ---------- FECHA ACTUAL ----------
    document.getElementById("currentDate").textContent = new Date().toLocaleDateString("es-ES", {
        weekday: "long", year: "numeric", month: "long", day: "numeric"
    });

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

    // ---------- HELPERS ----------
    const fetchJson = async (url, options = {}) => {
        try {
            showLoading(true);
            const response = await fetch(url, options);
            if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
            if (response.status === 204) return null;
            return response.json();
        } catch (error) {
            console.error("Error en fetchJson:", error);
            showAlert("Error", "Error al conectar con el servidor", "error");
            throw error;
        } finally {
            showLoading(false);
        }
    };

    const showAlert = (title, text, icon = "info") => {
        Swal.fire({ title, text, icon, confirmButtonText: "Entendido" });
    };

    const showLoading = (show) => {
        const overlay = document.getElementById("loadingOverlay");
        if (overlay) {
            overlay.classList.toggle("d-none", !show);
        }
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

    // ---------- GESTIÓN DE CUENTOS ----------
    const loadStories = async () => {
        try {
            allStories = await fetchJson(`${apiBaseUrl}/tales`);
            allStories = allStories.filter(story => story.isAvailable);
            applyStoryFilters();
        } catch (error) {
            console.error("Error al cargar cuentos:", error);
            showEmptyState("stories", "Error al cargar los cuentos");
        }
    };

    const applyStoryFilters = () => {
        const themeFilter = document.getElementById("storyThemeFilter")?.value || "";
        const ageFilter = document.getElementById("storyAgeFilter")?.value || "";
        const sortBy = document.getElementById("storySortBy")?.value || "title";

        filteredStories = allStories.filter(story => {
            const matchesTheme = !themeFilter || story.theme === themeFilter;
            let matchesAge = true;
            
            if (ageFilter) {
                const [minAge, maxAge] = ageFilter.split("-").map(Number);
                matchesAge = story.recommendedAge >= minAge && story.recommendedAge <= maxAge;
            }
            
            return matchesTheme && matchesAge;
        });

        filteredStories.sort((a, b) => {
            switch (sortBy) {
                case "title":
                    return a.title.localeCompare(b.title);
                case "title_desc":
                    return b.title.localeCompare(a.title);
                case "age":
                    return a.recommendedAge - b.recommendedAge;
                case "age_desc":
                    return b.recommendedAge - a.recommendedAge;
                default:
                    return 0;
            }
        });

        renderStories(filteredStories);
    };

    const renderStories = (stories) => {
        const grid = document.getElementById("storiesGrid");
        const emptyState = document.getElementById("emptyStoriesState");

        if (!grid) return;

        grid.innerHTML = "";

        if (!stories.length) {
            showEmptyState("stories", "No se encontraron cuentos");
            return;
        }

        hideEmptyState("stories");

        stories.forEach((story, index) => {
            const col = document.createElement("div");
            col.className = "col-lg-4 col-md-6 col-sm-12";
            col.style.animationDelay = `${index * 0.1}s`;

            const publicationDate = story.publicationDate 
                ? new Date(story.publicationDate).toLocaleDateString("es-ES")
                : "Fecha no disponible";

            col.innerHTML = `
                <div class="card story-card fade-in-up" data-story-id="${story.id}">
                    <div class="age-badge">${story.recommendedAge} años</div>
                    ${story.coverImage ? 
                        `<img src="${escapeHtml(story.coverImage)}" class="card-img-top" alt="Portada de ${escapeHtml(story.title)}">` 
                        : `<div class="card-img-top d-flex align-items-center justify-content-center bg-light" style="height: 200px;">
                               <i class="bi bi-book fs-1 text-muted"></i>
                           </div>`
                    }
                    <div class="card-body">
                        <h5 class="card-title">${escapeHtml(story.title)}</h5>
                        <div class="theme-badge ${story.theme}">${escapeHtml(story.theme)}</div>
                        <p class="card-text">${escapeHtml(story.content.substring(0, 100))}${story.content.length > 100 ? "..." : ""}</p>
                        <div class="mt-auto">
                            <small class="text-muted">Publicado: ${publicationDate}</small>
                            <div class="btn-group w-100 mt-2">
                                <button type="button" class="btn btn-read-story" data-story-id="${story.id}">
                                    <i class="bi bi-book-open me-1"></i> Leer Cuento
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            grid.appendChild(col);
        });

        grid.querySelectorAll(".btn-read-story").forEach(btn => {
            btn.addEventListener("click", (e) => {
                e.preventDefault();
                const storyId = parseInt(btn.dataset.storyId);
                openStoryReader(storyId);
            });
        });
    };

    const openStoryReader = (storyId) => {
        const story = allStories.find(s => s.id === storyId);
        if (!story) return;

        document.getElementById("storyReaderTitle").textContent = story.title;
        document.getElementById("storyReaderTheme").textContent = story.theme;
        document.getElementById("storyReaderAge").textContent = story.recommendedAge;
        document.getElementById("storyReaderContent").textContent = story.content;

        const imageEl = document.getElementById("storyReaderImage");
        if (story.coverImage) {
            imageEl.src = story.coverImage;
            imageEl.alt = `Imagen de ${story.title}`;
            imageEl.style.display = "block";
        } else {
            imageEl.style.display = "none";
        }

        const audioEl = document.getElementById("storyAudioPlayer");
        if (story.narrationAudio) {
            audioEl.src = story.narrationAudio;
            audioEl.style.display = "block";
            audioEl.parentElement.style.display = "block";
        } else {
            audioEl.style.display = "none";
            audioEl.parentElement.style.display = "none";
        }

        const favBtn = document.getElementById("addToFavoritesBtn");
        favBtn.dataset.storyId = storyId;

        const modal = new bootstrap.Modal(document.getElementById("storyReaderModal"));
        modal.show();
    };

    // ---------- GESTIÓN DE ACTIVIDADES ----------
    const loadActivities = async () => {
        try {
            allActivities = await fetchJson(`${apiBaseUrl}/activities`);
            await loadStories(); 
            applyActivityFilters();
        } catch (error) {
            console.error("Error al cargar actividades:", error);
            showEmptyState("activities", "Error al cargar las actividades");
        }
    };

    const populateActivityStoryFilter = () => {
        const select = document.getElementById("activityStoryFilter");
        if (select && allStories) {
            const availableStories = allStories.filter(story => story.isAvailable);
            select.innerHTML = `<option value="">Todos los cuentos</option>`;
            availableStories.forEach(story => {
                const option = document.createElement("option");
                option.value = story.id;
                option.textContent = story.title;
                select.appendChild(option);
            });
        }
    };

    const applyActivityFilters = () => {
        const storyFilter = document.getElementById("activityStoryFilter")?.value || "";
        const typeFilter = document.getElementById("homeActivityTypeFilter")?.value || "";

        filteredActivities = allActivities.filter(activity => {
            const matchesStory = !storyFilter || String(activity.taleId) === storyFilter;
            const matchesType = !typeFilter || activity.activityType === typeFilter;
            
            const relatedStory = allStories.find(s => s.id === activity.taleId);
            const storyAvailable = relatedStory && relatedStory.isAvailable;
            
            return matchesStory && matchesType && storyAvailable;
        });

        renderActivities(filteredActivities);
    };

    const renderActivities = (activities) => {
        const grid = document.getElementById("activitiesGrid");
        
        if (!grid) return;

        grid.innerHTML = "";

        if (!activities.length) {
            showEmptyState("activities", "No se encontraron actividades");
            return;
        }

        hideEmptyState("activities");

        activities.forEach((activity, index) => {
            const relatedStory = allStories.find(s => s.id === activity.taleId);
            const storyTitle = relatedStory ? relatedStory.title : `Cuento ID: ${activity.taleId}`;
            
            const col = document.createElement("div");
            col.className = "col-lg-4 col-md-6 col-sm-12";
            col.style.animationDelay = `${index * 0.1}s`;

            const activityIcons = {
                'DotToDot': 'bi-diagram-3',
                'Drawing': 'bi-palette',
                'Quiz': 'bi-question-circle'
            };

            const activityNames = {
                'DotToDot': 'Conectar Puntos',
                'Drawing': 'Dibujo',
                'Quiz': 'Cuestionario'
            };

            const icon = activityIcons[activity.activityType] || 'bi-puzzle';
            const typeName = activityNames[activity.activityType] || activity.activityType;

            col.innerHTML = `
                <div class="card activity-card fade-in-up">
                    <div class="card-body">
                        <div class="activity-icon">
                            <i class="bi ${icon}"></i>
                        </div>
                        <div class="activity-type ${activity.activityType}">${typeName}</div>
                        <h5 class="card-title">${escapeHtml(storyTitle)}</h5>
                        <p class="card-text">${escapeHtml(activity.description || "Actividad divertida")}</p>
                        <button type="button" class="btn btn-play-activity w-100" data-activity-id="${activity.id}">
                            <i class="bi bi-play-circle me-1"></i> Jugar
                        </button>
                    </div>
                </div>
            `;
            grid.appendChild(col);
        });

        grid.querySelectorAll(".btn-play-activity").forEach(btn => {
            btn.addEventListener("click", (e) => {
                e.preventDefault();
                const activityId = parseInt(btn.dataset.activityId);
                openActivityPlayer(activityId);
            });
        });
    };

    const openActivityPlayer = (activityId) => {
        const activity = allActivities.find(a => a.id === activityId);
        if (!activity) return;

        const relatedStory = allStories.find(s => s.id === activity.taleId);
        const storyTitle = relatedStory ? relatedStory.title : `Cuento ID: ${activity.taleId}`;

        const activityNames = {
            'DotToDot': 'Conectar Puntos',
            'Drawing': 'Dibujo',
            'Quiz': 'Cuestionario'
        };

        document.getElementById("activityPlayerTitle").textContent = `${activityNames[activity.activityType] || activity.activityType}`;
        document.getElementById("activityPlayerStory").textContent = storyTitle;
        document.getElementById("activityPlayerType").textContent = activityNames[activity.activityType] || activity.activityType;
        document.getElementById("activityPlayerDescription").textContent = activity.description || "Actividad divertida";

        const contentEl = document.getElementById("activityPlayerContent");
        contentEl.innerHTML = "";

        if (activity.multimediaResource) {
            const multimedia = createMultimediaElement(activity.multimediaResource);
            contentEl.appendChild(multimedia);
        } else {
            contentEl.innerHTML = `
                <div class="text-center">
                    <i class="bi bi-puzzle fs-1 text-muted mb-3"></i>
                    <h5>¡Actividad lista para jugar!</h5>
                    <p class="text-muted">Esta actividad no tiene contenido multimedia específico.</p>
                </div>
            `;
        }

        const modal = new bootstrap.Modal(document.getElementById("activityPlayerModal"));
        modal.show();
    };

    const createMultimediaElement = (url) => {
        const container = document.createElement("div");
        container.className = "activity-multimedia";

        const extension = url.split('.').pop().toLowerCase();
        
        if (['jpg', 'jpeg', 'png', 'gif', 'webp', 'svg'].includes(extension)) {
            const img = document.createElement("img");
            img.src = url;
            img.alt = "Imagen de la actividad";
            img.className = "img-fluid";
            img.onerror = () => {
                container.innerHTML = `
                    <div class="text-center text-muted">
                        <i class="bi bi-image fs-1 mb-3"></i>
                        <p>No se pudo cargar la imagen</p>
                    </div>
                `;
            };
            container.appendChild(img);
        } else if (['mp4', 'webm', 'ogg'].includes(extension)) {
            const video = document.createElement("video");
            video.src = url;
            video.controls = true;
            video.className = "activity-multimedia";
            video.onerror = () => {
                container.innerHTML = `
                    <div class="text-center text-muted">
                        <i class="bi bi-film fs-1 mb-3"></i>
                        <p>No se pudo cargar el video</p>
                    </div>
                `;
            };
            container.appendChild(video);
        } else if (['mp3', 'wav', 'ogg'].includes(extension)) {
            const audio = document.createElement("audio");
            audio.src = url;
            audio.controls = true;
            audio.className = "activity-multimedia";
            audio.onerror = () => {
                container.innerHTML = `
                    <div class="text-center text-muted">
                        <i class="bi bi-music-note fs-1 mb-3"></i>
                        <p>No se pudo cargar el audio</p>
                    </div>
                `;
            };
            container.appendChild(audio);
        } else {
            container.innerHTML = `
                <div class="text-center">
                    <i class="bi bi-link-45deg fs-1 text-primary mb-3"></i>
                    <h5>Recurso de la actividad</h5>
                    <a href="${escapeHtml(url)}" target="_blank" class="btn btn-primary">
                        <i class="bi bi-box-arrow-up-right me-1"></i> Abrir recurso
                    </a>
                </div>
            `;
        }

        return container;
    };

    // ---------- GESTIÓN DE FAVORITOS ----------
    const addToFavorites = async (storyId) => {
        if (!currentUser) {
            showAlert("Información", "Para agregar a favoritos, necesitas estar autenticado como usuario.", "info");
            return;
        }

        try {
            const favorite = {
                userId: currentUser.id,
                taleId: storyId,
                dateAdded: new Date().toISOString().split('T')[0]
            };

            await fetchJson(`${apiBaseUrl}/favorites`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(favorite)
            });

            showAlert("Éxito", "¡Cuento agregado a favoritos!", "success");
            
            const favBtn = document.getElementById("addToFavoritesBtn");
            favBtn.classList.add("favorited");
            favBtn.innerHTML = '<i class="bi bi-heart-fill me-1"></i> En Favoritos';
            
        } catch (error) {
            console.error("Error al agregar a favoritos:", error);
            showAlert("Error", "No se pudo agregar a favoritos", "error");
        }
    };

    // ---------- UTILIDADES ----------
    const showEmptyState = (section, message) => {
        const emptyState = document.getElementById(`empty${section.charAt(0).toUpperCase() + section.slice(1)}State`);
        if (emptyState) {
            emptyState.classList.remove("d-none");
            const messageEl = emptyState.querySelector("h4");
            if (messageEl) messageEl.textContent = message;
        }
    };

    const hideEmptyState = (section) => {
        const emptyState = document.getElementById(`empty${section.charAt(0).toUpperCase() + section.slice(1)}State`);
        if (emptyState) {
            emptyState.classList.add("d-none");
        }
    };

    // ---------- EVENT LISTENERS ----------

    ["storyThemeFilter", "storyAgeFilter", "storySortBy"].forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener("change", applyStoryFilters);
        }
    });

    const clearStoryFiltersBtn = document.getElementById("clearStoryFilters");
    if (clearStoryFiltersBtn) {
        clearStoryFiltersBtn.addEventListener("click", () => {
            document.getElementById("storyThemeFilter").value = "";
            document.getElementById("storyAgeFilter").value = "";
            document.getElementById("storySortBy").value = "title";
            applyStoryFilters();
        });
    }

    ["activityStoryFilter", "homeActivityTypeFilter"].forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener("change", applyActivityFilters);
        }
    });

    const clearActivityFiltersBtn = document.getElementById("clearActivityFilters");
    if (clearActivityFiltersBtn) {
        clearActivityFiltersBtn.addEventListener("click", () => {
            document.getElementById("activityStoryFilter").value = "";
            document.getElementById("homeActivityTypeFilter").value = "";
            applyActivityFilters();
        });
    }

    const addToFavoritesBtn = document.getElementById("addToFavoritesBtn");
    if (addToFavoritesBtn) {
        addToFavoritesBtn.addEventListener("click", () => {
            const storyId = parseInt(addToFavoritesBtn.dataset.storyId);
            addToFavorites(storyId);
        });
    }

    const loadUsers = async () => {
        try {
            allUsers = await fetchJson(`${apiBaseUrl}/users`);
            currentUser = allUsers.find(u => u.isActive && u.userType === 'Reader') || null;
        } catch (error) {
            console.error("Error al cargar usuarios:", error);
        }
    };

    // ---------- INICIALIZACIÓN ----------
    const initializeHome = async () => {
        try {
            if (homeBtn) {
                homeBtn.classList.remove("btn-outline-light");
                homeBtn.classList.add("btn-light");
            }

            await loadUsers();
            await loadStories();
            await loadActivities();
            
        } catch (error) {
            console.error("Error inicializando la aplicación:", error);
            showAlert("Error", "Error al cargar la aplicación", "error");
        }
    };

    initializeHome();
});