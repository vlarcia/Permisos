

const MODELO_DETALLE = {
        
    idVisita: 0,
    idActividad=0,    
    idFinca=0,
    fecha: "",        
    avanceanterior="",
    avances: "",
    estado: "",
    estadoanterior="",
    comentarios: "",
    observaciones: "",
    UrlFoto1:"", 0,

}

let filaeditada = null;
$(document).ready(function () {
    
    // Cargar la lista de fincas usando fetch
  
    $(".card-body").LoadingOverlay("show");
    fetch("/PlanesTrabajo/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $(".card-body").LoadingOverlay("hide");
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    // Para cada item de la lista, agrega una opción al combo con el formato 'codigo - nombre'
                    $("#cboPlan").append(
                        $("<option>")
                            .val(item.idPlan) // El valor sigue siendo codFinca
                            .text(item.idPlan + " - " + item.descripcion.trim() + " -finca: " +nombreFinca.trim())  // Mostrar 'codFinca - descripcion'
                            .attr("data-idplan", item.idPlan)
                            .attr("data-descripcion", item.Descripcion)
                            .attr("data-nombrefinca", item.nombreFinca) // Guardar el nombre de la finca
                            .attr("data-idfinca", item.idFinca)
                    );
                });
            } else
            {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        });

    tablaVisitas = $('#tbVisitas').DataTable({

        responsive: true,
        "ajax": {
            "url": '/Visita/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idVisita" },
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
            { "data": "avance" },
            { "data": "observaciones" },

            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-1"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-info btn-mostrar btn-sm mr-1"><i class="fas fa-eye"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "20px"
            }

        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Planes',
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });


    // Inicializar tabla y pone en modo responsive

        var tablaActividades = $('#tbActividad').DataTable({
            responsive: true, // Hacer que la tabla sea responsive
            columnDefs: [
                {
                    targets: [9],  // Índice de la columna "Requisito" (la décima columna)
                    visible: false // Hacerla invisible
                }
            ]
        });

    // Inicializar el datepicker en los campos de fecha
    $('#txtFecha').datepicker({
        format: "dd/mm/yyyy",   // Establecer el formato
        todayHighlight: true,   // Resaltar la fecha de hoy
        autoclose: true,        // Cerrar automáticamente el calendario al seleccionar
        language: "es"          // Idioma español para los textos
    });

    // Evento change para cuando seleccionen un valor en el combo de fincas
    $("#cboPlan").change(function () {
        // Obtener el nombre de la finca desde el atributo 'data-nombre' de la opción seleccionada
        var vidPlan = $("#cboPlan option:selected").attr("data-idplan")
        var vdescripcion = $("#cboPlan option:selected").attr("data-descripcion")
        var vnombreFinca = $("#cboFinca option:selected").attr("data-nombrefinca");
        var vidFinca = $("#cboFinca option:selected").attr("data-idfinca");    

        // Colocar el nombre de la finca en el campo de texto deshabilitado
        $("#txtIdPlan").val(vidPlan);
        $("#txDescripcionplan").val(vdescripcion);
        $("#txtNombreFinca").val(vnombreFinca);
        $("#txtIdFinca").val(vidFinca);
    });

    
    $('#btnAgregar').on('click', function () {
                
        // Validar los campos del modal antes de agregar la actividad
        const inputsActividad = $("#form-actividad input.input-validar").serializeArray();
        const inputs_sin_valor = inputsActividad.filter((item) => item.value.trim() == "");

        if (inputs_sin_valor.length > 0) {
            const mensaje = `Debe completar el campo : "${inputs_sin_valor[0].name}"`;
            toastr.warning("", mensaje);
            $(`input[name="${inputs_sin_valor[0].name}"]`).focus();
            return;
        }

        $("#modalActividad").find("div.modal-content").LoadingOverlay("show");
        // Obtener los valores de la actividad
        const actividad = {
            descripcion: $('#txtDescripcionActividad').val(),
            tipo: $('#cboTipo').val(),
            fechaini: $('#txtFechainicio').val(),
            fechafin: $('#txtFechafin').val(),
            responsable: $('#txtResponsable').val(),
            recursos: $('#txtRecursos').val(),            
            estado: $('#cboEstado').val(),
            avances: $('#txtAvanceAct').val(),
            comentarios: $('#txtComentarios').val(),
            idRequisito: $('#txtIdRequisito').val()
        };
        if (filaeditada != null) {          
           
            // Reiniciar la variable filaEditada y Cambiar el texto del botón de vuelta a "Agregar"
            filaeditada = null;
            $('#btnAgregar').text('Agregar Actividad');
            // Editar la fila existente en la tabla
            tablaActividades.row(filaeditada).data([
                actividad.descripcion,
                actividad.tipo,
                actividad.fechaini,
                actividad.fechafin,
                actividad.responsable,
                actividad.recursos,
                actividad.estado,
                actividad.avances,
                actividad.comentarios,
                actividad.idRequisito,
                `
            <button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>
            <button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>
            `
            ]).draw(false);  // `draw(false)` asegura que la tabla no se recargue por completo, solo actualiza los datos.            

        } else {
            // Agregar la actividad a la tabla usando DataTables
            tablaActividades.row.add([
                actividad.descripcion,
                actividad.tipo,
                actividad.fechaini,
                actividad.fechafin,
                actividad.responsable,
                actividad.recursos,
                actividad.estado,
                actividad.avances,
                actividad.comentarios,
                actividad.idRequisito,

                `
            <button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>
            <button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>
            `
            ]).draw(false);  // `draw(false)` asegura que la tabla no se recargue por completo, solo actualiza los datos.

        }
        // Cerrar el modal después de agregar la actividad
        $("#modalActividad").find("div.modal-content").LoadingOverlay("hide");
        $('#modalActividad').modal('hide');
    });
    $('#tbActividad tbody').on('click', '.btn-editar', function () {
        if ($(this).closest("tr").hasClass("child")) {
            filaeditada = $(this).closest("tr").prev();
        }
        else {
            filaeditada = $(this).closest("tr");
        }
        
        const data = tablaActividades.row(filaeditada).data();
        $('#btnAgregar').text('Guardar Cambios');

        // Mostrar el modal
        
        mostrarModal(data);
    })

    // Evento para eliminar actividad
    $('#tbActividad tbody').on('click', '.btn-eliminar', function () {
        let fila;

        if ($(this).closest("tr").hasClass("child")) {
            fila = $(this).closest("tr").prev();
        } else {
            fila = $(this).closest("tr");
        }

        const dataelim = tablaActividades.row(fila).data();
        Swal.fire({
            title: "¿Seguro de continuar?",
            text: `Eliminar de la Tabla "${dataelim[0]}"`,
            icon: "warning", // Cambiado de 'type' a 'icon'            
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "No, cancelar",
            customClass: {
                confirmButton: 'btn btn-danger', // Clases CSS personalizadas
                cancelButton: 'btn btn-secondary',

            },
            reverseButtons: true // Cambia el orden de los botones

        }).then((result) => {
            if (result.isConfirmed) { // Si el usuario confirma la acción
                $(".showSweetAlert").LoadingOverlay("show");
                var fila = tablaActividades.row($(this).parents('tr'));
                fila.remove().draw(false); // Eliminar la fila y actualizar la tabla sin recargar
                $(".showSweetAlert").LoadingOverlay("hide");
            }
            
        })
    });

});


function mostrarModal(modelo = MODELO_DETALLE) {
    
    $("#txtDescripcionActividad").val(modelo[0])
    $("#cboTipo").val(modelo[1])
    $("#txtFechainicio").val(modelo[2])
    $("#txtFechafin").val(modelo[3])
    $("#txtResponsable").val(modelo[4])
    $("#txtRecursos").val(modelo[5])
    $("#cboEstado").val(modelo[6])
    $("#txtAvanceAct").val(modelo[7])
    $("#txtComentarios").val(modelo[8])
    $("#cboRequisito").val(modelo[9])
    $("#txtRequisito").val("")

    $("#modalActividad").modal("show")    

}
$('#btnNuevo').on('click', function () {
    // Limpiar los campos del modal
    $('#txtDescripcionActividad').val('');
    $('#cboTipo').val('Documental'); // Set default value
    $('#txtFechainicio').val('');
    $('#txtFechafin').val('');
    $('#txtResponsable').val('');
    $('#txtRecursos').val('');
    $('#txtAvanceAct').val('');
    $('#cboEstado').val('No Iniciado'); // Set default value
    $('#txtComentarios').val('');
    $('#cboRequisito').val('');
    $('#txtRequisito').val('');

    // Abrir el modal
    $('#modalActividad').modal('show');
});


function validarFecha(fecha) {
    // Expresión regular para verificar el formato dd/MM/yyyy
    var regexFecha = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;
    return regexFecha.test(fecha);
}
$('#btnGuardarPlan').on('click', function () {
    // Validar los campos del plan
    const inputsPlan = $("#form-plan input.input-validar").serializeArray();
    const inputs_sin_valor = inputsPlan.filter((item) => item.value.trim() == "");

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo : "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

       // Crear el objeto modeloplan a partir del formulario
    const modeloplan = {
        idFinca: parseInt($("#txtIdFinca").val(), 10), // Convertir a entero
        descripcion: $('#txtDescripcionPlan').val(),
        fechaIni: convertirFecha($('#txtFechainicial').val()), // Convertir a Date
        fechaFin: convertirFecha($('#txtFechafinal').val()),   // Convertir a Date
        observaciones: $('#txtObservaciones').val(),
        avance: parseFloat($('#txtAvance').val()), // Convertir a decimal
        estado: $('#cboEstadoPlan').val(),
        actividades: [] // Aquí agregarás las actividades
    };

    // Recorrer las filas de la tabla y agregar las actividades al modelo
    $('#tbActividad tbody tr').each(function () {
        let fila = $(this);
        let actividad = {
            descripcion: fila.find('td').eq(0).text(),
            tipo: fila.find('td').eq(1).text(),
            fechaIni: convertirFecha(fila.find('td').eq(2).text()),   // Convertir a Date
            fechaFin: convertirFecha(fila.find('td').eq(3).text()),   // Convertir a Date
            responsable: fila.find('td').eq(4).text(),
            recursos: fila.find('td').eq(5).text(),
            estado: fila.find('td').eq(6).text(),
            avances: parseFloat(fila.find('td').eq(7).text()),  // Convertir a decimal
            comentarios: fila.find('td').eq(8).text(),
            idFinca: parseInt($("#txtIdFinca").val(), 10), // Convertir a entero
            fechaUltimarevision: obtenerFechaActual(),
            idRequisito: parseInt($("#txtIdRequisito").val(), 10),
            avanceanterior:0.0
        };

        modeloplan.actividades.push(actividad);
    });
    
    
    // Enviar el modelo completo al servidor como JSON
    $(".showSweetAlert").LoadingOverlay("show");
    fetch("/PlanesTrabajo/RegistrarPlan", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" }, // Enviar como JSON        
        body: JSON.stringify(modeloplan) // Convertir el objeto modeloplan a JSON
        
    })
        .then(response => {
            
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {            

            if (responseJson.estado) {
                Swal.fire("Listo!", "Plan de Trabajo "+responseJson.Objeto.id +""+"creado con éxito!", "success");
                limpiarFormularioYTabla();
            } else {
                Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
            }          
        })
        .catch(error => {
            $(".showSweetAlert").LoadingOverlay("hide");
            Swal.fire("¡Error!", "Hubo un problema al tratar de agregar el Plan.", "error");
        })
    $(".showSweetAlert").LoadingOverlay("hide");
});

function limpiarFormularioYTabla() {
    // Limpiar los campos del formulario del Plan de Trabajo
    $('#txtDescripcionPlan').val('');
    $('#txtIdFinca').val('');
    $('#txtFechainicial').val('');
    $('#txtFechafinal').val('');
    $('#txtObservaciones').val('');
    $('#txtAvance').val('');
    $('#cboEstadoPlan').val('No Iniciado'); // Si tienes un valor por defecto
    $('#txtNombreFinca').val('');

    // Limpiar la tabla de actividades
    const tabla = $('#tbActividad').DataTable(); // Asumiendo que usas DataTables
    tabla.clear().draw(); // Eliminar todas las filas de la tabla

    // Si no usas DataTables, puedes limpiar la tabla manualmente así:
    // $('#tbActividad tbody').empty();
}

function convertirFecha(fecha) {
    const partes = fecha.split("/");
    const dia = partes[0];
    const mes = partes[1] - 1; // Restar 1 al mes
    const anio = partes[2];
    return new Date(anio, mes, dia);
}
function obtenerFechaActual() {
    const hoy = new Date();
    const anio = hoy.getFullYear();
    const mes = String(hoy.getMonth() + 1).padStart(2, '0'); // Meses de 0 a 11, sumamos 1
    const dia = String(hoy.getDate()).padStart(2, '0'); // Día con dos dígitos

    return `${anio}-${mes}-${dia}`; // Formato yyyy-MM-dd
}




// Esto era para menejar fechas con formato amigable



//function convertirFecha(fechaAmigable) {
//    // Mapa de meses
//    const meses = {
//        "Enero": "01", "Febrero": "02", "Marzo": "03", "Abril": "04", "Mayo": "05", "Junio": "06",
//        "Julio": "07", "Agosto": "08", "Septiembre": "09", "Octubre": "10", "Noviembre": "11", "Diciembre": "12"
//    };

//    // Dividir la fecha amigable en partes
//    var partesFecha = fechaAmigable.split('/');

//    var dia = partesFecha[0]; // Día
//    var mesTexto = partesFecha[1]; // Mes en formato de texto
//    var anio = partesFecha[2]; // Año

//    // Reemplazar el nombre del mes por su número correspondiente
//    var mesNumero = meses[mesTexto];

//    // Construir la fecha en formato dd/mm/yyyy
//    var fechaGuardar = dia + "/" + mesNumero + "/" + anio;

//    return fechaGuardar;
//}

//var fechaInicial = $('#txtFechainicial').datepicker('getFormattedDate');
//var fechaFinal = $('#txtFechafinal').datepicker('getFormattedDate');

//// Mostrar la primera alerta
//Swal.fire({
//    title: 'Las fechas como vienen',
//    text: "Inicia: " + fechaInicial + " Finaliza: " + fechaFinal,
//    icon: 'info'
//}).then(() => {
//    // Después de cerrar la primera alerta, continuar con la lógica
//    var fechaIniGuarda = convertirFecha(fechaInicial);
//    var fechaFinGuarda = convertirFecha(fechaFinal);

//    // Mostrar la segunda alerta
//    Swal.fire({
//        title: 'Las fechas como se guardan',
//        text: "Inicia: " + fechaIniGuarda + " Finaliza: " + fechaFinGuarda,
//        icon: 'success'
//    }).then(() => {
//        // Aquí puedes agregar la lógica para guardar los datos
//        // Por ejemplo, enviar una solicitud AJAX o continuar con el proceso
//    });
//});