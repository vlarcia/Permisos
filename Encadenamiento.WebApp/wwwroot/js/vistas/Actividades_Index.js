const VISTA_BUSQUEDA = {
    busquedaFecha: () => {
        $("#txtFechaIni").val("")
        $("#txtFechaFin").val("")
        $("#txtFinca").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-finca").hide()
    },
    busquedaFinca: () => {
        $("#txtFechaIni").val("")
        $("#txtFechaFin").val("")
        $("#txtFinca").val("")

        $(".busqueda-fecha").hide()
        $(".busqueda-finca").show()
    }

}

$(document).ready(function () {

    tablaActividades = $('#tbActividades').DataTable({

        responsive: true,
        "ajax": {
            "url": '/PlanesTrabajo/ListaActividad',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idPlan" },
            { "data": "descripcion" },
            { "data": "nombreFinca" },

            // Formatear fechaIni
            {
                "data": "fechaIni",
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

            // Formatear fechaFin
            {
                "data": "fechaFin",
                "render": function (data) {
                    if (data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('es-ES', {
                            day: '2-digit',
                            month: '2-digit',
                            year: 'numeric'
                        });
                    }
                    return ""; // Si no hay fecha, devolver vacío
                }
            },
            { "data": "estado" },
            { "data": "avances" },
            { "data": "comentarios" },
            { "data": "tipo", "visible": false },
            { "data": "responsable", "visible": false },
            { "data": "recursos", "visible": false },
            { "data": "avanceanterior", "visible": false },
            { "data": "fechaUltimarevision", "visible": false },
            { "data": "idActividad", "visible": false },
            { "data": "idFinca", "visible": false },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-1"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-info btn-mostrar btn-sm mr-1"><i class="fas fa-eye"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                
                "orderable": false,
                "searchable": false,
                "width": "60px"
            }
        ],        
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Actividades',
                exportOptions: {
                    columns: [0,1,2,3,4,5,6,7,8,9,10,11,12,13,14]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    // Cargar la lista de Requsitos usando fetch

    $(".card-body").LoadingOverlay("show");
    fetch("/Checklist/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $(".card-body").LoadingOverlay("hide");
            if (responseJson.data.length > 0) {
                // Limpiar ambos selectores antes de agregar los nuevos valores
                $("#cboRequisito, #cboRequisitonuevo").empty();

                // Recorrer la respuesta JSON y añadir las opciones a ambos selectores
                responseJson.data.forEach((item) => {
                    const option = $("<option>")
                        .val(item.idRequisito)
                        .text(item.idRequisito + " - " + item.descripcion.substring(0, 80))  // Mostrar 'idRequisito - descripcion'
                        .attr("data-requisito", item.descripcion.substring(0, 80))            // Guardar descripcion del requisito
                        .attr("data-idReq", item.idRequisito);                                // Guardar id del requisito

                    // Añadir la opción a ambos selectores
                    $("#cboRequisito, #cboRequisitonuevo").append(option);
                });
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error");
            }
        });


    // Cargar la lista de Planes de Trabajo usando fetch
    $(".card-body").LoadingOverlay("show");
    fetch("/PlanesTrabajo/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $(".card-body").LoadingOverlay("hide");
            if (responseJson.data.length > 0) {
                responseJson.data.sort((a, b) => b.idPlan - a.idPlan);
                responseJson.data.forEach((item) => {
                    // Para cada item de la lista, agrega una opción al combo con el formato 'codigo - nombre'
                    $("#cboPlan").append(
                        $("<option>")
                            .val(item.idPlan) 
                            .text(item.idPlan + " - finca: " + item.nombreFinca.substring(0, 25) + " - " + item.descripcion.substring(0, 40)) // Mostrar 'idPlan - finca - descripcion'
                            .attr("data-plan", item.descripcion.substring(0, 70)) // Guardar descripcion del Plan
                            .attr("data-idPlan", item.idPlan)
                            .attr("data-idFinca", item.idFinca)
                    );
                });
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        });


    VISTA_BUSQUEDA["busquedaFecha"]()

    $.datepicker.setDefaults($.datepicker.regional["es"])
    $("#txtFechaIni").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaIniAct").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFinAct").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaUlt").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaininuevo").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechafinnuevo").datepicker({ dateFormat: "dd/mm/yy" })
  
});
$('#tbActividades tbody').on('click', '.btn-mostrar', function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    }
    else {
        filaseleccionada = $(this).closest("tr");
    }

    const data = tablaActividades.row(filaseleccionada).data();
    $('#btnGuardar').hide(); // Oculta el botón 
    
    // Mostrar el modal
    mostrarModal(data, true);
})

$('#tbActividades tbody').on('click', '.btn-editar', function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    } else {
        filaseleccionada = $(this).closest("tr");
    }

    const data = tablaActividades.row(filaseleccionada).data();
    lafinca = data.idFinca    //esto es para tener la finca que no cambiará en la edicion

    $('#btnGuardar').show(); // Oculta el botón antes de abrir el modal
    mostrarModal(data, false); // Puedes reutilizar la función mostrarModal
});

//Botones------------
$('#btnGuardar').on('click', function () {   //Boton de Guardar Cambios
    // Obtener toda la fila
    let dataFila = $('#tbActividades').DataTable().row(filaseleccionada).data();
    
    const modeloactividad = {
        idActividad: parseInt($("#txtId").val()),
        idPlan: parseInt(dataFila.idPlan),
        idRequisito:$("#txtIdRequisito").val(),
        descripcion: $("#txtDescripcion").val(),
        tipo: $("#cboTipo").val(),
        fechaIni: convertirFecha($("#txtFechaIniAct").val()),
        fechaFin: convertirFecha($("#txtFechaFinAct").val()),
        responsable: $("#txtResponsable").val(),
        recursos: $("#txtRecursos").val(),
        avances: parseFloat($("#txtAvances").val()),
        avanceanterior: parseFloat($("#txtAvanceAnt").val()),
        estado: $("#cboEstado").val(),
        comentarios: $("#txtComentarios").val(),
        fechaUltimarevision: convertirFecha($("#txtFechaUlt").val()), 
        idfinca: parseInt(dataFila.idFinca),
        };
            
    // Hacer la petición al servidor para actualizar
    fetch("/PlanesTrabajo/EditarActividad", {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(modeloactividad)
    })
        .then(response => response.json())
        .then(responseJson => {
            if (responseJson.estado) {
                Swal.fire("Listo!", "Actividad modificada con éxito!", "success");
                $('#modalActividades').modal('hide');
            } else {
                Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
            }
        });
});

$('#btnAgregar').on('click', function () {   //Boton de Agregar y crear nuevas Actividades

    // Validar los campos del modal antes de agregar la actividad
    const inputsActividad = $("#form-actividadnueva input.input-validar").serializeArray();
    const inputs_sin_valor = inputsActividad.filter((item) => item.value.trim() == "");

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo : "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

    $("#modalNuevaActividad").find("div.modal-content").LoadingOverlay("show");
    // Obtener los valores de la actividad
    const actividad = {
        idActividad:0,
        descripcion: $('#txtDescripcionActnuevo').val(),
        tipo: $('#cboTiponuevo').val(),
        fechaini: convertirFecha($('#txtFechaininuevo').val()),
        fechafin: convertirFecha($('#txtFechafinnuevo').val()),
        fechaUltimarevision: obtenerFechaActual(),
        responsable: $('#txtResponsablenuevo').val(),
        recursos: $('#txtRecursosnuevo').val(),
        estado: $('#cboEstadonuevo').val(),
        avances: $('#txtAvanceActnuevo').val(),
        avanceanterior:0.0,
        comentarios: $('#txtComentariosnuevo').val(),
        idRequisito: $('#txtIdRequisitonuevo').val(),
        idPlan: $('#txtIdPlannuevo').val(),
        idFinca: $('#txtIdFincanuevo').val(),
    }; 
    
        fetch("/PlanesTrabajo/RegistrarActividad", {
            method: "POST",
            headers: { "Content-Type": "application/json;charset=utf-8" }, // Enviar como JSON        
            body: JSON.stringify(actividad) // Convertir el objeto actividad a JSONactividad
        })
            .then(response => {
                $("#modalNuevaActividad").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaActividades.row.add(responseJson.objeto).draw(false)
                    $("#modalNuevaActividad").modal("hide")
                    Swal.fire("Listo!", "Actividad No. "+ responseJson.objeto.idActividad + "  para el plan  "+responseJson.objeto.idPlan+ " fue creada con éxito!", "success")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                }
            })

    
    // Cerrar el modal después de agregar la actividad
    $("#modalActividad").find("div.modal-content").LoadingOverlay("hide");
    $('#modalActividad').modal('hide');
});

$('#btnNuevo').on('click', function () {
    // Limpiar los campos del modal
    $('#txtDescripcionActnuevo').val('');
    $('#cboTiponuevo').val('DOCUMENTAL'); // Set default value
    $('#txtFechaininuevo').val('');
    $('#txtFechafinnuevo').val('');
    $('#txtResponsablenuevo').val('');
    $('#txtRecursosnuevo').val('');
    $('#txtAvanceActnuevo').val('');
    $('#cboEstadonuevo').val('INICIADO'); // Set default value
    $('#txtComentariosnuevo').val('');
    $('#cboRequisitonuevo').val('');
    $('#txtRequisitonuevo').val('');
    $('#txtIdRequisitonuevo').val('');
    $('#cboPlan').val('');
    $('#txtPlandescr').val(''); 
    $('#txtIdPlannuevo').val('');
    $('#txtIdFincanuevo').val('');
    
    // Abrir el modal
    $('#modalNuevaActividad').modal('show');
});

$("#tbActividades tbody").on("click", ".btn-eliminar", function () {
    let fila;

    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    }
    else {
        fila = $(this).closest("tr");
    }
    const data = tablaActividades.row(fila).data();
    Swal.fire({
        title: "Seguro de continuar?",
        text: `Eliminar Actividad "${data.idActividad} ${data.descripcion.length > 60 ? data.descripcion.substring(0, 60) + '...' : data.descripcion}"`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true,
        customClass: {
            confirmButton: 'btn btn-danger', // Clases CSS personalizadas
            cancelButton: 'btn btn-secondary',
        },
        reverseButtons: true // Cambia el orden de los botones
    }).then((result) => {
        if (result.isConfirmed) { // Si el usuario confirma la acción
            $(".showSweetAlert").LoadingOverlay("show");
            fetch(`/PlanesTrabajo/EliminarActividad?IdActividad=${data.idActividad}`, {
                method: "DELETE",
            })
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        tablaActividades.row(fila).remove().draw(false);
                        Swal.fire("Listo!", "Actividad No. " + data.idActividad+"  fue eliminada!", "success")
                    } else {
                        Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                    }
                })
                .catch(error => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    Swal.fire("¡Error!", "Hubo un problema al eliminar la actividad.", "error");
                });
        }
    })
})

// Eventos que no son de Botones
$("#cboRequisito").change(function () {
    // Obtener el nombre del Requisito desde el atributo 'data-requisito' de la opción seleccionada
    var vdescrequisito = $("#cboRequisito option:selected").attr("data-requisito");
    var vidReq = $("#cboRequisito option:selected").attr("data-idReq");

    // Colocar el nombre del requisito en el campo de texto deshabilitado
    $("#txtRequisito").val(vdescrequisito);
    $("#txtIdRequisito").val(vidReq);
});
$("#cboRequisitonuevo").change(function () {
    // Obtener el nombre del Requisito desde el atributo 'data-requisito' de la opción seleccionada
    var vdescrequisito = $("#cboRequisitonuevo option:selected").attr("data-requisito");
    var vidReq = $("#cboRequisitonuevo option:selected").attr("data-idReq");

    // Colocar el nombre del requisito en el campo de texto deshabilitado
    $("#txtRequisitonuevo").val(vdescrequisito);
    $("#txtIdRequisitonuevo").val(vidReq);
});

$("#cboPlan").change(function () {
    // Obtener el nombre del Plan desde el atributo 'data-plan' de la opción seleccionada
    var vdescrplan = $("#cboPlan option:selected").attr("data-plan");
    var vidPlan = $("#cboPlan option:selected").attr("data-idPlan");
    var vidFinca = $("#cboPlan option:selected").attr("data-idFinca");

    // Colocar el nombre del Plan en el campo de texto deshabilitado
    $("#txtPlandescr").val(vdescrplan);
    $("#txtIdPlannuevo").val(vidPlan);
    $("#txtIdFincanuevo").val(vidFinca);

});

//Funciones y utilidades
function mostrarModal(modelo, edita) {
    
    $("#txtDescripcion").prop('disabled', edita)
    $("#txtFechaIniAct").prop('disabled', edita)
    $("#txtFechaFinAct").prop('disabled', edita)
    $("#txtComentarios").prop('disabled', edita)
    $("#txtAvances").prop('disabled', edita)
    $("#txtAvanceAnt").prop('disabled', edita)
    $("#cboEstado").prop('disabled', edita)
    $("#txtRecursos").prop('disabled', edita)
    $("#txtResponsable").prop('disabled', edita)
    $("#txtResponsable").prop('disabled', edita)
    $("#txtFechaUlt").prop('disabled', edita)
    $("#cboRequisito").prop('disabled', edita)
    $("#cboTipo").prop('disabled', edita)

    $("#txtId").val(modelo.idActividad)
    $("#txtDescripcion").val(modelo.descripcion)
    $("#txtFechaIniAct").val(formatearFecha(modelo.fechaIni))
    $("#txtFechaFinAct").val(formatearFecha(modelo.fechaFin))
    $("#txtFechaUlt").val(formatearFecha(modelo.fechaUltimarevision))
    $("#txtComentarios").val(modelo.comentarios)
    $("#txtAvances").val(parseFloat(modelo.avances))
    $("#cboEstado").val(modelo.estado)
    $("#txtNombreFinca").val(modelo.nombreFinca)
    $("#txtCodFinca").val(modelo.codFinca)
    $("#txtResponsable").val(modelo.responsable)
    $("#txtRecursos").val(modelo.recursos)
    $("#cboTipo").val(modelo.tipo)
    $("#txtIdRequisito").val(modelo.idRequisito)
    $("#cboRequisito").val(modelo.idRequisito)
    $("#txtAvanceAnt").val(modelo.avanceanterior)

    $("#modalActividades").modal("show")
}


function convertirFecha(fecha) {

    const partes = fecha.split("/");
    const dia = partes[0];
    const mes = partes[1] - 1; // Restar 1 al mes
    const anio = partes[2];
    return new Date(anio, mes, dia);
}

function formatearFecha(fechaISO) {
    // Convertir la cadena ISO a un objeto Date
    const fecha = new Date(fechaISO);

    // Extraer día, mes y año
    const dia = String(fecha.getDate()).padStart(2, '0'); // Obtener día con dos dígitos
    const mes = String(fecha.getMonth() + 1).padStart(2, '0'); // Obtener mes, sumando 1 porque los meses en JS van de 0 a 11
    const anio = fecha.getFullYear(); // Obtener año completo

    // Formatear y retornar como dd/MM/yyyy
    return `${dia}/${mes}/${anio}`;
}
function obtenerFechaActual() {
    const hoy = new Date();
    const anio = hoy.getFullYear();
    const mes = String(hoy.getMonth() + 1).padStart(2, '0'); // Meses de 0 a 11, sumamos 1
    const dia = String(hoy.getDate()).padStart(2, '0'); // Día con dos dígitos

    return `${anio}-${mes}-${dia}`; // Formato yyyy-MM-dd
}


