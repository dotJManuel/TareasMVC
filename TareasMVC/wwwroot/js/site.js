function inicializarTema() {
    const boton = document.getElementById('theme-toggle');
    if (!boton) {
        return;
    }

    const icono = boton.querySelector('i');

    function pintarIcono() {
        const tema = document.documentElement.getAttribute('data-bs-theme');
        if (icono) {
            icono.className = tema === 'dark' ? 'bi bi-sun' : 'bi bi-moon-stars';
        }
    }

    pintarIcono();

    boton.addEventListener('click', function () {
        const actual = document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'dark' : 'light';
        const nuevo = actual === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-bs-theme', nuevo);
        try {
            localStorage.setItem('tema', nuevo);
        } catch (e) { }
        pintarIcono();
    });
}

document.addEventListener('DOMContentLoaded', inicializarTema);
