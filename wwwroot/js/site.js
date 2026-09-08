document.addEventListener("DOMContentLoaded", () => {
    const menuButton = document.querySelector(".nav-toggle");
    const menu = document.querySelector(".nav-menu");

    menuButton?.addEventListener("click", () => {
        const isOpen = menu?.classList.toggle("is-open") ?? false;
        menuButton.setAttribute("aria-expanded", String(isOpen));
    });

    document.querySelectorAll(".password-toggle").forEach((button) => {
        button.addEventListener("click", () => {
            const field = button.parentElement?.querySelector("input");
            if (!field) return;

            const shouldShow = field.type === "password";
            field.type = shouldShow ? "text" : "password";
            button.textContent = shouldShow ? "Ocultar" : "Mostrar";
            button.setAttribute("aria-label", shouldShow ? "Ocultar senha" : "Mostrar senha");
        });
    });

    document.querySelectorAll(".alert-close").forEach((button) => {
        button.addEventListener("click", () => button.closest(".alert")?.remove());
    });
});
