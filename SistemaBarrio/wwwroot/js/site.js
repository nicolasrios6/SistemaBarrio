// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("click", function (e) {

    if (e.target.closest(".toggle-password")) {

        const button = e.target.closest(".toggle-password");
        const input = button.closest(".input-group").querySelector("input");

        const icon = button.querySelector("i");

        if (input.type === "password") {
            input.type = "text";
            icon.classList.remove("bi-eye");
            icon.classList.add("bi-eye-slash");
        }
        else {
            input.type = "password";
            icon.classList.remove("bi-eye-slash");
            icon.classList.add("bi-eye");
        }
    }

});

document.addEventListener("DOMContentLoaded", () => {

    document.querySelectorAll(".tom-select").forEach(select => {

        new TomSelect(select, {

            create: false,

            sortField: {
                field: "text",
                direction: "asc"
            },

            score(search) {

                const busqueda = search
                    .toUpperCase()
                    .replace(/\s+/g, "");

                return function (item) {

                    // Texto original
                    const texto = item.text.toUpperCase();

                    // Ej: "MANZANA A - CASA 3"
                    const normalizado = texto
                        .replace("MANZANA", "")
                        .replace("CASA", "")
                        .replace(/[\s-]/g, "");

                    // Resultado: "A3"

                    if (normalizado.includes(busqueda))
                        return 100;

                    // Si no coincide con A3, usa la búsqueda normal
                    if (texto.includes(busqueda))
                        return 50;

                    return 0;
                };
            }

        });

    });

});