let tablaPermisos;

$(document).ready(function () {
    tablaPermisos = $('#tbPermisosVencimiento').DataTable({
        responsive: true,
        ajax: {
            url: '/Permiso/ListaPermisos',
            type: "GET",
            dataSrc: "",
            dataType: "json",
            error: function (xhr, error) {
                console.error("Error al cargar datos:", error);
                Swal.fire("Error", "No se pudieron cargar los permisos", "error");
            }
        },
        columns: [
            { "data": "idPermiso", className: "text-center" },
            {
                "data": "nombre",
                render: data => data ? data.replace(/\r\n/g, '<br>') : "N/A"
            },
            { "data": "institucion" },
            {
                "data": "encargado",
                render: data => data ? data.replace(/\n/g, '<br>') : "N/A"
            },
            {
                "data": "fechaVencimiento",
                className: "text-center",
                render: {
                    display: data => {
                        if (!data) return "N/A";
                        const date = new Date(data);
                        return date.toLocaleDateString('es-NI');
                    },
                    sort: data => new Date(data).getTime()
                }
            },
            {
                data: "fechaVencimiento",
                className: "text-center",
                render: {
                    display: (data, type, row) => {
                        if (!data) return "N/A";

                        const hoy = new Date();
                        const vencimiento = new Date(data);
                        const diasGestion = row.diasGestion || 0;

                        // Mostrar "Vencido" si ya pasó la fecha de vencimiento
                        if (vencimiento <= hoy) {
                            return '<span class="badge badge-danger">Vencido</span>';
                        }

                        // Calcular cuándo debería iniciar la gestión
                        const fechaAlerta = new Date(vencimiento);
                        fechaAlerta.setDate(vencimiento.getDate() - diasGestion);

                        const diffDays = Math.ceil((fechaAlerta - hoy) / (1000 * 60 * 60 * 24));

                        if (diffDays <= 0) {
                            return `<span class="badge badge-danger">${diffDays} días</span>`;
                        } else if (diffDays <= 30) {
                            return `<span class="badge badge-warning">${diffDays} días</span>`;
                        } else {
                            return `<span class="badge badge-success">${diffDays} días</span>`;
                        }
                    }
                }
            },
            {
                // Nueva columna oculta para ordenamiento numérico
                "data": "fechaVencimiento",
                visible: false, // Oculta esta columna
                render: data => {
                    const hoy = new Date();
                    const vencimiento = new Date(data);
                    return Math.ceil((vencimiento - hoy) / (1000 * 60 * 60 * 24));
                }
            },
            {
                "defaultContent":
                    '<button class="btn btn-primary btn-correo btn-sm mr-1"><i class="fas fa-envelope"></i></button>' +
                    '<button class="btn btn-whatsapp btn-sm"><i class="fab fa-whatsapp"></i></button>',
                orderable: false,
                searchable: false,
                width: "90px",
                className: "text-center"
            }
        ],
        order: [[6, "asc"]], // Ordenamos por la columna oculta de días restantes
        createdRow: function (row, data) {
            if (!data.fechaVencimiento) return;

            const hoy = new Date();
            const vencimiento = new Date(data.fechaVencimiento);
            const diffDays = Math.ceil((vencimiento - hoy) / (1000 * 60 * 60 * 24));

            if (diffDays <= 30) {
                $(row).addClass('vencimiento-proximo');
            }
            if (diffDays <= 7) {
                $(row).addClass('vencimiento-critico');
            }
        },
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Vencimientos',
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });

    // Envío por correo
    $('#tbPermisosVencimiento tbody').on('click', '.btn-correo', function () {
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaPermisos.row(fila).data();

        $("#tbPermisosVencimiento").LoadingOverlay("show");
        $.ajax({
            url: `/Alerta/EnviarAlerta?idPermiso=${data.idPermiso}`,
            type: 'GET',
            success: function (res) {
                if (res.estado) {
                    Swal.fire("Listo!", "Correo enviado con éxito!", "success");
                    tablaPermisos.row(fila).remove().draw(false);
                } else {
                    Swal.fire("Error", res.mensaje, "error");
                }
            },
            error: () => Swal.fire("Error", "No se pudo enviar el correo", "error"),
            complete: () => $("#tbPermisosVencimiento").LoadingOverlay("hide")
        });
    });

    // Envío por WhatsApp
    $('#tbPermisosVencimiento tbody').on('click', '.btn-whatsapp', function () {
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaPermisos.row(fila).data();

        $("#tbPermisosVencimiento").LoadingOverlay("show");
        $.ajax({
            url: `/Alerta/EnviarAlerta`,
            type: 'GET',
            data: {
                idPermiso: data.idPermiso,
                wasap: 1
            },
            success: function (res) {
                if (res.estado) {
                    Swal.fire({
                        title: "WhatsApp Enviado",
                        text: `Se ha enviado la alerta sobre el permiso: ${data.nombre}`,
                        icon: "success",
                        showClass: {
                            popup: `animate__animated animate__fadeInLeft animate__faster`
                        },
                        hideClass: {
                            popup: `animate__animated animate__fadeOutUp animate__faster`
                        },
                        confirmButtonColor: "#3085d6",
                    });
                    tablaPermisos.row(fila).remove().draw(false);
                } else {
                    Swal.fire("Error", res.mensaje, "error");
                }
            },
            error: err => {
                const msg = err.responseJSON?.mensaje ?? "No se pudo enviar el WhatsApp.";
                Swal.fire("Error", msg, "error");
            },
            complete: () => $("#tbPermisosVencimiento").LoadingOverlay("hide")
        });
    });
});
