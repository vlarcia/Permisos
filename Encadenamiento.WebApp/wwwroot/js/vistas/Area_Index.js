const modelo = {
    idArea: 0,
    nombre: "",
    descripcion: "",
    activo: true
};

let editar = false;
let filaSeleccionada;

$(document).ready(function () {
    const tablaAreas = $('#tbAreas').DataTable({
        responsive: true,
        ajax: {
            url: '/Area/Lista',
            type: 'GET',
            dataType: 'json'
        },
        columns: [
            { data: 'idArea', title: 'ID' },
            { data: 'nombre', title: 'Nombre' },
            { data: 'descripcion', title: 'Descripción' },
            {
                data: 'activo',
                title: 'Estado',
                render: function (data) {
                    return data
                        ? '<span class="badge badge-success">Activo</span>'
                        : '<span class="badge badge-danger">Inactivo</span>';
                },
                className: 'text-center'
            },
            {
                defaultContent:
                    '<button class="btn btn-info btn-ver btn-sm mr-1"><i class="fas fa-eye"></i></button>' +
                    '<button class="btn btn-primary btn-editar btn-sm mr-1"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                orderable: false,
                width: '120px'
            }
        ],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_Areas',
            }, 'pageLength'
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json'
        }
    });

    // Botón Nuevo
    $('#btnNuevo').click(function () {
        editar = false;
        $('#formArea')[0].reset();
        $('.modal-body input, .modal-body textarea').prop('disabled', false);
        $('#chkActivo').prop('checked', true);
        $('#btnGuardar').show();
        $('#modalArea').modal('show');
    });

    // Botón Ver
    $('#tbAreas tbody').on('click', '.btn-ver', function () {
        const data = tablaAreas.row($(this).closest('tr')).data();
        $('#txtIdArea').val(data.idArea);
        $('#txtNombre').val(data.nombre).prop('disabled', true);
        $('#txtDescripcion').val(data.descripcion).prop('disabled', true);
        $('#chkActivo').prop('checked', data.activo).prop('disabled', true);
        $('#btnGuardar').hide();
        $('#modalArea').modal('show');
    });

    // Botón Editar
    $('#tbAreas tbody').on('click', '.btn-editar', function () {
        editar = true;
        filaSeleccionada = $(this).closest('tr');
        const data = tablaAreas.row(filaSeleccionada).data();
        $('#txtIdArea').val(data.idArea);
        $('#txtNombre').val(data.nombre).prop('disabled', false);
        $('#txtDescripcion').val(data.descripcion).prop('disabled', false);
        $('#chkActivo').prop('checked', data.activo).prop('disabled', false);
        $('#btnGuardar').show();
        $('#modalArea').modal('show');
    });

    // Botón Guardar
    $('#btnGuardar').click(function () {
        if (!validarFormulario()) return;

        const modelo = {
            idArea: parseInt($('#txtIdArea').val() || 0),
            nombre: $('#txtNombre').val(),
            descripcion: $('#txtDescripcion').val(),
            activo: $('#chkActivo').is(':checked')
        };

        const formData = new FormData();
        formData.append("modelo", JSON.stringify(modelo));

        const url = editar ? '/Area/Editar' : '/Area/Crear';
        const metodo = 'POST';

        $("#tbAreas").LoadingOverlay("show");

        fetch(url, {
            method: metodo,
            body: formData
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire("Éxito", "Área guardada correctamente", "success");
                    tablaAreas.ajax.reload();
                    $('#modalArea').modal('hide');
                } else {
                    Swal.fire("Error", responseJson.mensaje || "Error al guardar", "error");
                }
                $("#tbAreas").LoadingOverlay("hide");
            })
            .catch(error => {
                $("#tbAreas").LoadingOverlay("hide");
                Swal.fire("¡Error!", "No se pudo guardar el registro: " + error, "error");
            });
    });

    // Botón Eliminar
    $('#tbAreas tbody').on('click', '.btn-eliminar', function () {
        const fila = $(this).closest('tr');
        const data = tablaAreas.row(fila).data();

        Swal.fire({
            title: '¿Eliminar Área?',
            text: `¿Estás seguro de eliminar "${data.nombre}"?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then(result => {
            if (result.isConfirmed) {
                $("#tbAreas").LoadingOverlay("show");
                fetch(`/Area/Eliminar?id=${data.idArea}`, { method: 'POST' })
                    .then(r => r.ok ? r.json() : Promise.reject(r))
                    .then(responseJson => {
                        if (responseJson.success) {
                            Swal.fire("Eliminado", "Área eliminada correctamente", "success");
                            tablaAreas.ajax.reload();
                        } else {
                            Swal.fire("Error", responseJson.mensaje || "No se pudo eliminar", "error");
                        }
                        $("#tbAreas").LoadingOverlay("hide");
                    })
                    .catch(error => {
                        $("#tbAreas").LoadingOverlay("hide");
                        Swal.fire("¡Error!", "No se pudo eliminar: " + error, "error");
                    });
            }
        });
    });

    // Validación básica
    function validarFormulario() {
        let valido = true;
        $('.input-validar').each(function () {
            if (!$(this).val()) {
                $(this).addClass('is-invalid');
                valido = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        return valido;
    }
});

// ⬇ Navegación con Enter
$(document).on('keydown', 'input, textarea, select', function (e) {
    if (e.key === "Enter") {
        e.preventDefault();
        let focusable = $('input, textarea, select, button')
            .filter(':visible:not([disabled]):not([readonly])');
        let index = focusable.index(this);
        if (index > -1 && index + 1 < focusable.length) {
            focusable.eq(index + 1).focus();
        }
    }
});
