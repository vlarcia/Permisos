$(document).ready(function () {
    // Mostrar el modal al cargar la página
    $('#modalValidarAcceso').modal({
        backdrop: 'static',
        keyboard: false // Deshabilitar el cierre con "Esc"
    });

    // Manejar la validación del formulario
    $('#formValidarAcceso').on('submit', function (e) {
        e.preventDefault();

        const clave = $('#clave').val();

        $.ajax({
            url: '/Acceso/ValidarAcceso', // URL al controlador
            type: 'POST',
            data: { clave: clave },
            success: function (response) {
                if (response.estado) {
                    // Ocultar el modal y mostrar el contenido
                    $('#modalValidarAcceso').modal('hide');
                    $('#contenidoPrincipal').fadeIn();

                    // Inicializar la tabla después de validar
                    inicializarTabla();
                } else {
                    // Mostrar mensaje de error
                    $('#mensajeError').text(response.mensaje).show();
                }
            },
            error: function () {
                $('#mensajeError').text('Error al validar la clave. Inténtelo de nuevo.').show();
            }
        });
    });

    // Botón Cancelar
    $('#btnCancelarValidacion').on('click', function () {
        Swal.fire({
            title: '¿Estás seguro?',
            showClass: { popup: ` animate__animated animate__fadeInUp animate__fast ` },
            hideClass: { popup: ` animate__animated animate__fadeOutRight animate__fast ` },
            text: 'No se validará la clave y no podrás acceder a esta sección.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, volver',
            cancelButtonText: 'No, continuar aquí',
            confirmButtonColor: "#3085d6",

        }).then((result) => {
            if (result.isConfirmed) {
                // Redirigir al menú principal
                window.location.href = '/Home/Index'; // Cambia esto por la ruta de tu menú principal
            } else {
                // Reabrir el modal si decide quedarse
                $('#modalValidarAcceso').modal('show');
            }
        });
    });

    function inicializarTabla() {
        if (!$.fn.DataTable.isDataTable('#tbParametros')) {
            tablaParametros = $('#tbParametros').DataTable({
                responsive: true,
                columnDefs: [
                    {
                        targets: [0],  // Índice de las columnas "IdReg"
                        visible: false // Hacerlas invisibles
                    }
                ],
                "ajax": {
                    "url": '/Acceso/Lista',
                    "type": "GET",
                    "datatype": "json"
                },
                "columns": [
                    { "data": "idReg" },
                    { "data": "recurso" },
                    { "data": "propiedad" },
                    {
                        "data": "valor",
                        "render": function (data, type, row, meta) {
                            // Renderizar la celda como un campo editable
                            return `<input type="text" class="form-control form-control-sm valor-editable" value="${data}" data-row-index="${meta.row}"/>`;
                        }
                    },
                    {
                        "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                            '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                        "orderable": false,
                        "searchable": false,
                        "width": "80px"
                    }
                ],
                order: [[0, "asc"]],
                dom: "Bfrtip",
                buttons: [
                    {}, 'pageLength'
                ],
            });
        }
    }

    // Actualizar la tabla al editar un campo editable
    $('#tbParametros tbody').on('blur', 'input.valor-editable', function () {
        var row = $(this).closest('tr');
        var rowIdx = tablaParametros.row(row).index();
        var newText = $(this).val();

        // Actualizar el modelo de datos de la tabla
        var data = tablaParametros.row(rowIdx).data();
        data.valor = newText; // Actualiza el valor de la propiedad "valor"

        // Opcional: actualizar visualmente la tabla si es necesario
        tablaParametros.row(rowIdx).data(data).draw(false);
    });

    // Botón Guardar Cambios
    $('#btnGuardarCambios').on('click', function () {
        const datosTabla = [];

        // Obtener todos los datos de las filas en el DataTable
        const filas = $('#tbParametros').DataTable().rows().data();
        // Recorrer todas las filas del DataTable
        for (let index = 0; index < filas.length; index++) {
            const rowData = filas[index]; // Obtener datos de la fila actual
            const elidReg = rowData.idReg;
            const elrecurso = rowData.recurso;
            const elpropiedad = rowData.propiedad;
            const elvalor = rowData.valor;

            datosTabla.push({
                idReg: elidReg,
                recurso: elrecurso,
                propiedad: elpropiedad,
                valor: elvalor, // Usar el valor actualizado o el original
            });
        }

        const formData = new FormData();
        formData.append("parametros", JSON.stringify(datosTabla));

        // Enviar datos al servidor
        fetch('/Acceso/GuardarCambios', {
            method: 'PUT',
            body: formData,
        })
            .then(response => response.json())
            .then(data => {
                if (data.estado) {
                    Swal.fire({
                        title: 'Éxito',
                        text: 'Los cambios se guardaron correctamente.',
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                    });

                    // Recargar la tabla
                    tablaParametros.ajax.reload();
                } else {
                    Swal.fire({
                        title: 'Error',
                        text: data.mensaje,
                        icon: 'error',
                        confirmButtonColor: '#d33',
                    });
                }
            })
            .catch(error => {
                Swal.fire({
                    title: 'Error',
                    text: 'Hubo un problema al guardar los cambios.',
                    icon: 'error',
                    confirmButtonColor: '#d33',
                });
            });
    });
});
