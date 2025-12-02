document.addEventListener("DOMContentLoaded", () => {
    const logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", () => {
            Swal.fire({
                title: "¿Deseas cerrar sesión?",
                text: "Se cerrará tu sesión actual",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Sí, cerrar sesión",
                cancelButtonText: "Cancelar"
            }).then((result) => {
                if (result.isConfirmed) {
                    localStorage.removeItem("stellarbooks_user");

                    Swal.fire({
                        icon: "success",
                        title: "Sesión cerrada",
                        text: "Has salido correctamente",
                        showConfirmButton: false,
                        timer: 1500
                    });

                    setTimeout(() => {
                        window.location.href = "login.html";
                    }, 1500);
                }
            });
        });
    }
});
