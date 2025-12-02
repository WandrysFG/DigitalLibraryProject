const apiBaseUrl = "https://localhost:7169/api";

async function fetchJson(url, options = {}) {
    const response = await fetch(url, options);
    if (!response.ok) throw new Error("Error en fetch: " + response.status);
    if (response.status === 204) return null;
    return response.json();
}

document.addEventListener("DOMContentLoaded", () => {
    // LOGIN
    const loginForm = document.getElementById("loginForm");
    if (loginForm) {
        loginForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            
            const email = document.getElementById("email").value.trim();
            const password = document.getElementById("password").value.trim();

            if (!email || !password) {
                Swal.fire("Error", "Debes ingresar email y contraseña", "error");
                return;
            }

            const MAX_EMAIL_LENGTH = 50;
            if (email.length > MAX_EMAIL_LENGTH) {
                Swal.fire("Error", `El correo no puede superar ${MAX_EMAIL_LENGTH} caracteres`, "error");
                return;
            }

            try {
                const users = await fetchJson(`${apiBaseUrl}/users`);
                if (!users || users.length === 0) {
                    Swal.fire("Error", "No hay usuarios registrados", "error");
                    return;
                }

                const user = users.find(u =>
                    u.email.toLowerCase() === email.toLowerCase() &&
                    u.password === password &&
                    u.isActive
                );

                if (!user) {
                    Swal.fire("Error", "Usuario o contraseña incorrectos", "error");
                    return;
                }

                localStorage.setItem("stellarbooks_user", JSON.stringify({
                    id: user.id,
                    firstName: user.firstName,
                    lastName: user.lastName,
                    email: user.email,
                    userType: user.userType
                }));

                Swal.fire({
                    icon: "success",
                    title: `Bienvenido/a, ${user.firstName}`,
                    showConfirmButton: false,
                    timer: 1500,
                    didOpen: () => {
                        document.querySelector(".card").style.position = "relative";
                    }
                });

                setTimeout(() => {
                    window.location.href = "home.html";
                }, 1500);

            } catch (error) {
                console.error(error);
                Swal.fire("Error", "No se pudo conectar con el servidor", "error");
            }
        });
    }

    // REGISTER
    const registerForm = document.getElementById("registerForm");
    if (registerForm) {
        registerForm.addEventListener("submit", async (e) => {
            e.preventDefault();

            const firstName = document.getElementById("firstName").value.trim();
            const lastName = document.getElementById("lastName").value.trim();
            const email = document.getElementById("email").value.trim();
            const password = document.getElementById("password").value.trim();

            if (!firstName || !lastName || !email || !password) {
                Swal.fire("Error", "Todos los campos son obligatorios", "error");
                return;
            }

            try {
                const newUser = {
                    firstName,
                    lastName,
                    email,
                    password,
                    userType: "Reader",
                    registrationDate: new Date().toISOString().split("T")[0],
                    isActive: true
                };

                await fetchJson(`${apiBaseUrl}/users`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(newUser)
                });

                Swal.fire({
                    icon: "success",
                    title: "Usuario registrado correctamente",
                    showConfirmButton: false,
                    timer: 1500
                }).then(() => {
                    window.location.href = "login.html";
                });

            } catch (error) {
                console.error(error);
                Swal.fire("Error", "No se pudo registrar al usuario", "error");
            }
        });
    }
});
