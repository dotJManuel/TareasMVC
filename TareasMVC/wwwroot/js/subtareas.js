function subtareaViewModel({ id, titulo }) {
    var self = this;
    self.id = ko.observable(id);
    self.titulo = ko.observable(titulo);
}

function manejarClickAgregarSubtarea() {
    Swal.fire({
        title: 'Nueva subtarea',
        input: 'text',
        inputPlaceholder: 'Título de la subtarea',
        showCancelButton: true,
        confirmButtonText: 'Agregar'
    }).then(async (resultado) => {
        if (!resultado.isConfirmed || !resultado.value) {
            return;
        }
        await insertarSubtarea(resultado.value);
    });
}

async function insertarSubtarea(titulo) {
    const respuesta = await fetch(`${urlTareas}/${tareaEditarVM.id}/subtareas`, {
        method: 'POST',
        body: JSON.stringify(titulo),
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        tareaEditarVM.subtareas.push(new subtareaViewModel(json));
    } else {
        manejarErrorApi(respuesta);
    }
}

function manejarClickAbrirSubtarea(subtarea) {
    abrirTareaPorId(subtarea.id());
}

function manejarClickBorrarSubtarea(subtarea) {
    modalEditarTareaBootstrap.hide();
    confirmarAccion({
        callbackAceptar: async () => {
            await borrarSubtarea(subtarea);
            modalEditarTareaBootstrap.show();
        },
        callbackCancelar: () => {
            modalEditarTareaBootstrap.show();
        },
        titulo: `¿Desea enviar esta subtarea a la papelera?`
    });
}

async function borrarSubtarea(subtarea) {
    const respuesta = await fetch(`${urlTareas}/${subtarea.id()}`, {
        method: 'DELETE'
    });

    if (respuesta.ok) {
        tareaEditarVM.subtareas.remove(function (item) { return item.id() == subtarea.id(); });
    } else {
        manejarErrorApi(respuesta);
    }
}