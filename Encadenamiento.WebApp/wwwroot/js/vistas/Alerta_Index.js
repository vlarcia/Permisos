//const modelo = {
//    idAlerta: 0,
//    idPermiso: 0,
//    idDestinatario: 0,
//    fechaEnvio: "",
//    medioEnvio: "",
//    resultado: "",
//    mensaje: "",
//    permiso: "",
//    destinatario: ""
//};

$(document).ready(function () {
    // Inicializar DataTable
    const tablaAlertas = $('#tbAlertas').DataTable({
        responsive: true,
        ajax: {
            url: '/Alerta/Lista',
            type: 'GET',
            dataType: 'json'
        },
        columns: [
            { data: 'idAlerta' },
            { data: 'permiso' },
            { data: 'destinatario' },
            {
                data: 'fechaEnvio',
                render: function (data) {
                    return data ? new Date(data).toLocaleString('es-NI') : '';
                }
            },
            { data: 'medioEnvio' },
            {
                data: 'resultado',
                render: function (data) {
                    if (!data) return '<span class="badge badge-pendiente">Pendiente</span>';
                    return data.includes('éxito') || data.includes('Exito') || data.includes('exito') ?
                        '<span class="badge badge-enviado">' + data + '</span>' :
                        '<span class="badge badge-fallido">' + data + '</span>';
                }
            },
            {
                defaultContent:
                    '<button class="btn btn-info btn-ver btn-sm mr-1"><i class="fas fa-eye"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                orderable: false,
                width: '90px'
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json'
        },
        order: [[3, 'desc']] // Ordenar por fecha de envío descendente
    });

    // Botón Ver Detalles
    $('#tbAlertas tbody').on('click', '.btn-ver', function () {
        const fila = $(this).closest('tr');
        const data = tablaAlertas.row(fila).data();

        // Llenar el modal con los datos
        $('#txtIdAlerta').val(data.idAlerta);
        $('#txtPermiso').val(data.permiso);
        $('#txtDestinatario').val(data.destinatario);
        $('#txtFechaEnvio').val(data.fechaEnvio ? new Date(data.fechaEnvio).toLocaleString('es-NI') : '');
        $('#txtMedioEnvio').val(data.medioEnvio);
        $('#txtResultado').val(data.resultado || 'Pendiente');
        $('#txtMensaje').val(data.mensaje || 'N/A');

        // Mostrar modal
        $('#modalAlerta').modal('show');
    });

    // Botón Eliminar
    $('#tbAlertas tbody').on('click', '.btn-eliminar', function () {
        const fila = $(this).closest('tr');
        const data = tablaAlertas.row(fila).data();

        Swal.fire({
            title: '¿Eliminar Alerta?',
            text: `¿Seguro que deseas eliminar el registro de alerta del ${new Date(data.fechaEnvio).toLocaleDateString('es-NI')}?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar',
            customClass: {
                confirmButton: 'btn btn-danger',
                cancelButton: 'btn btn-secondary'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                $("#tbAlertas").LoadingOverlay("show");
                fetch(`/Alerta/Eliminar?idAlerta=${data.idAlerta}`, { method: 'POST' })
                    .then(response => response.ok ? response.json() : Promise.reject(response))
                    .then(responseJson => {
                        if (responseJson.estado) {
                            Swal.fire("Éxito", "Registro de alerta eliminado correctamente", "success");
                            tablaAlertas.ajax.reload();
                        } else {
                            Swal.fire("Error", responseJson.mensaje, "error");
                        }
                    })
                    .catch(error => {
                        Swal.fire("Error", "Hubo un problema al eliminar la alerta", "error");
                    })
                    .finally(() => {
                        $("#tbAlertas").LoadingOverlay("hide");
                    });
            }
        });
    });

    // Opcional: Recargar la tabla cada 60 segundos para nuevas alertas
    setInterval(function () {
        tablaAlertas.ajax.reload(null, false); // Recargar sin resetear paginación
    }, 60000);
});