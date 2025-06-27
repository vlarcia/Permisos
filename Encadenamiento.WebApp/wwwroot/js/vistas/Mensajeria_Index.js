let tablaRevisiones;
$(document).ready(function () {
    $(".table").LoadingOverlay("show");
    tablaVisitas = $('#tbVisitas').DataTable({

        responsive: true,
        "ajax": {
            "url": '/Visita/Lista',
            "type": "GET",
            data: { envio: 1 },
            "datatype": "json"
        },
        "columns": [
            { "data": "idFinca" },
            { "data": "nombreFinca" },

            // Formatear fecha
            {
                "data": "fecha",
                "render": function (data) {
                    if (data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('es-NI', {
                            day: '2-digit',
                            month: '2-digit',
                            year: 'numeric'
                        });
                    }
                    return ""; // Si no hay fecha, devolver vacío
                }
            },

            {
                "defaultContent": '<button class="btn btn-primary btn-correo1 btn-sm mr-1"><i class="fas fa-envelope"></i></button>' +
                    '<button class="btn btn-custom btn-watsap1 btn-sm"><i class="fab fa-whatsapp"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "20px"
            }

        ],      
       
        order: [[1, "asc"]],
        dom: "lrtip",

        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
    tablaRevisiones = $('#tbRevisiones').DataTable({

        responsive: true,
        "ajax": {
            "url": '/Revision/ListaRevisions',
            "type": "GET",
            data: { envio: 1},  // el 1 es para que obtenga solo las revisiones que no se han enviado por correo
            "datatype": "json"
        },
        "columns": [
            { "data": "idFinca" },
            { "data": "nombreFinca" },

            // Formatear fecha
            {
                "data": "fecha",
                "render": function (data) {
                    if (data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('es-NI', {
                            day: '2-digit',
                            month: '2-digit',
                            year: 'numeric'
                        });
                    }
                    return ""; // Si no hay fecha, devolver vacío
                }
            },            

            {               
                "defaultContent": '<button class="btn btn-primary btn-correo2 btn-sm mr-1"><i class="fas fa-envelope"></i></button>' +
                    '<button class="btn btn-custom btn-watsap2 btn-sm"><i class="fab fa-whatsapp"></i></button>' ,                   
                "orderable": false,
                "searchable": false,
                "width": "20px"
            }

        ],
        order: [[1, "asc"]],
        dom: "lrtip",
        
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
   
 
    $(".table").LoadingOverlay("hide");
});

$('#tbVisitas tbody').on('click', '.btn-correo1', function () {
    let filaseleccionada;
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    }
    else {
        filaseleccionada = $(this).closest("tr");
    }

    const data = tablaVisitas.row(filaseleccionada).data();
    const correoDestino = data.email;  // Obtenemos el email desde la fila seleccionada

    if (correoDestino) {
        $("#tbVisitas").LoadingOverlay("show");
        $.ajax({
            url: `/Visita/EnviarVisitaPorCorreo?idVisita=${data.idVisita}&correoDestino=${correoDestino}`,
            type: 'GET', // O POST si prefieres
            success: function (response) {
                if (response.estado) {
                    Swal.fire("Listo!", "Correo enviado con éxito!", "success");
                    tablaVisitas.row(filaseleccionada).remove().draw(false);
                } else {
                    Swal.fire("Error", response.mensaje, "error");
                }
            },
            error: function (error) {
                Swal.fire("Error", "No se pudo enviar el correo. " + response.mensaje, "error");
            },
            complete: function () {
                // Esto se ejecuta después de que la llamada AJAX haya terminado, sin importar si fue exitosa o no.
                $("#tbVisitas").LoadingOverlay("hide");
            }
        });
    }
});
$('#tbVisitas tbody').on('click', '.btn-watsap1', function () {
    let filaseleccionada;
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    }
    else {
        filaseleccionada = $(this).closest("tr");
    }

    const data = tablaVisitas.row(filaseleccionada).data();
    const idvisita = data.idVisita
    const finca = data.nombreFinca;
    const correodestino = data.telefono;  // Obtenemos el telefono  y utilizamos el parametro de la funcion enviaCorreo.

    if (correodestino) {
        $("#tbVisitas").LoadingOverlay("show");
        $.ajax({
            url: `/Visita/EnviarVisitaPorCorreo`,
            type: 'GET', // O POST si prefieres
            data: { idVisita : idvisita, correoDestino : correodestino, wasap: 1},
            success: function (response) {
                if (response.estado) {
                    Swal.fire({
                        title: "WhatsApp Enviado",
                        text: "Se ha enviado por medio de WhatsApp el informe de la visita de la finca" + finca,
                        icon: "success",

                        showClass: {
                            popup: `animate__animated animate__fadeInLeft animate__faster`
                        },
                        hideClass: {
                            popup: `animate__animated animate__fadeOutUp animate__faster`
                        },
                        confirmButtonColor: "#3085d6",
                    });                

                    tablaVisitas.row(filaseleccionada).remove().draw(false);
                } else {
                    Swal.fire("Error", response.mensaje, "error");
                }
            },
            error: function (error) {
                const mensajeError = error.responseJSON && error.responseJSON.mensaje
                    ? error.responseJSON.mensaje
                    : "No se pudo enviar el WhatsApp.";
                Swal.fire("Error", mensajeError, "error");
            },
            complete: function () {
                // Esto se ejecuta después de que la llamada AJAX haya terminado, sin importar si fue exitosa o no.
                $("#tbVisitas").LoadingOverlay("hide");
            }
        });
    }    
});

$('#tbRevisiones tbody').on('click', '.btn-correo2', function () {
    let filaseleccionada;
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    }
    else {
        filaseleccionada = $(this).closest("tr");
    }

    const data = tablaRevisiones.row(filaseleccionada).data();
    const correoDestino = data.email;  // Obtenemos el email desde la fila seleccionada
    const fecha = formatearFecha(data.fecha)
    if (correoDestino) {
        $("#tbRevisiones").LoadingOverlay("show");
        $.ajax({
            url: `/Revision/EnviarRevisionPorCorreo?idFinca=${data.idFinca}&fecha=${fecha}&correoDestino=${correoDestino}`,
            type: 'GET', // O POST si prefieres
            success: function (response) {
                if (response.estado) {
                    Swal.fire("Listo!", "Correo enviado con éxito!", "success");
                    tablaRevisiones.row(filaseleccionada).remove().draw(false);
                } else {
                    Swal.fire("Error", response.mensaje, "error");
                }
            },
            error: function (error) {
                Swal.fire("Error", "No se pudo enviar el correo. " + response.mensaje, "error");
            },
            complete: function () {
                // Esto se ejecuta después de que la llamada AJAX haya terminado, sin importar si fue exitosa o no.
                $("#tbRevisiones").LoadingOverlay("hide");
            }
        });
    }
});

$('#tbRevisiones tbody').on('click', '.btn-watsap2', function () {
    let filaseleccionada;
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    } else {
        filaseleccionada = $(this).closest("tr");
    }

    const data = tablaRevisiones.row(filaseleccionada).data();
    const idfinca = data.idFinca;
    const finca = data.nombreFinca;
    const correodestino = data.telefono;
    const lafecha = formatearFecha(data.fecha);

    if (correodestino) {
        $("#tbRevisiones").LoadingOverlay("show");
        $.ajax({
            url: `/Revision/EnviarRevisionPorCorreo`,
            type: 'GET',
            data: { idFinca: idfinca, fecha: lafecha, correoDestino: correodestino, wasap: 1 },
            success: function (response) {
                if (response.estado) {
                    Swal.fire({
                        title: "WhatsApp Enviado",
                        text: "Se ha enviado por medio de WhatsApp la revisión de la finca " + finca,
                        icon: "success",
                        showClass: { popup: `animate__animated animate__fadeInLeft animate__faster` },
                        hideClass: { popup: `animate__animated animate__fadeOutUp animate__faster` },
                        confirmButtonColor: "#3085d6",
                    });

                    tablaRevisiones.row(filaseleccionada).remove().draw(false);
                } else {
                    Swal.fire("Error", response.mensaje, "error");
                }
            },
            error: function (error) {
                const mensajeError = error.responseJSON && error.responseJSON.mensaje
                    ? error.responseJSON.mensaje
                    : "No se pudo enviar el WhatsApp.";
                Swal.fire("Error", mensajeError, "error");
            },
            complete: function () {
                $("#tbRevisiones").LoadingOverlay("hide");
            }
        });
    }
});
