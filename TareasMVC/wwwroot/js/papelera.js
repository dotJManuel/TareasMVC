function papeleraViewModel() {
    var self = this;
    self.tareas = ko.observableArray([]);
    self.cargando = ko.observable(false);

    self.noHayTareas = ko.pureComputed(function () {
        return !self.cargando() && self.tareas().length === 0;
    });
}

function tareaPapeleraViewModel({ id, titulo }) {
    var self = this;
    self.id = ko.observable(id);
    self.titulo = ko.observable(titulo);
}

const papeleraVM = new papeleraViewModel();

async function abrirPapelera() {
    modalPapeleraBootstrap.show();
    papeleraVM.cargando(true);
    papeleraVM.tareas([]);

    const respuesta = await fetch(`${urlTareas}/papelera`, {
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        json.forEach(t => papeleraVM.tareas.push(new tareaPapeleraViewModel(t)));
    } else {
        manejarErrorApi(respuesta);
    }

    papeleraVM.cargando(false);
}

async function restaurarTarea(tarea) {
    const respuesta = await fetch(`${urlTareas}/${tarea.id()}/restaurar`, {
        method: 'POST'
    });

    if (respuesta.ok) {
        papeleraVM.tareas.remove(function (item) { return item.id() == tarea.id(); });
        await obtenerTareas();
    } else {
        manejarErrorApi(respuesta);
    }
}

function manejarClickEliminarPermanente(tarea) {
    confirmarAccion({
        callbackAceptar: () => {
            eliminarPermanente(tarea);
        },
        titulo: `¿Eliminar definitivamente "${tarea.titulo()}"? No se puede deshacer.`
    });
}

async function eliminarPermanente(tarea) {
    const respuesta = await fetch(`${urlTareas}/${tarea.id()}/permanente`, {
        method: 'DELETE'
    });

    if (respuesta.ok) {
        papeleraVM.tareas.remove(function (item) { return item.id() == tarea.id(); });
    } else {
        manejarErrorApi(respuesta);
    }
}