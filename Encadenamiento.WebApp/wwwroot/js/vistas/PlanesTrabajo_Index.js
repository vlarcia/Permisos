

const MODELO_ACTIVIDAD = {
        
    idFinca: 0,
    descripcion: "",
    tipo: "",
    fecha_ini: "",
    fecha_fin: "",
    responsable: "",
    recursos: "",
    avances: "",
    estado: "",
    comentarios: "",
    idRequisito:0
}

let filaeditada = null;
let tablaActividades;
$(document).ready(function () {
    
    // Cargar la lista de fincas usando fetch
  
    $(".card-body").LoadingOverlay("show");
    fetch("/MaestroFinca/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $(".card-body").LoadingOverlay("hide");
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    // Para cada item de la lista, agrega una opción al combo con el formato 'codigo - nombre'
                    $("#cboFinca").append(
                        $("<option>")
                            .val(item.codFinca) // El valor sigue siendo codFinca
                            .text(item.codFinca + " - " + item.descripcion) // Mostrar 'codFinca - descripcion'
                            .attr("data-nombre", item.descripcion) // Guardar el nombre de la finca
                            .attr("data-id", item.idFinca)
                    );
                });
            } else
            {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
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
                responseJson.data.forEach((item) => {
                    // Para cada item de la lista, agrega una opción al combo con el formato 'codigo - nombre'
                    $("#cboRequisito").append(
                        $("<option>")
                            .val(item.idRequisito) // El valor sigue siendo codFinca
                            .text(item.idRequisito + " - " + item.descripcion.substring(0,80)) // Mostrar 'idRequisito - descripcion'
                            .attr("data-requisito", item.descripcion.substring(0,80)) // Guardar descripcion del requisito
                            .attr("data-idReq", item.idRequisito)
                    );
                });
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        });
    // Inicializar tabla y pone en modo responsive

        tablaActividades = $('#tbActividad').DataTable({
            responsive: true, // Hacer que la tabla sea responsive
            columnDefs: [
                {
                    targets: [9],  // Índice de la columna "Requisito" (la décima columna)
                    visible: false // Hacerla invisible
                }
            ]
        });

    // Inicializar el datepicker en los campos de fecha
    $('#txtFechainicial, #txtFechafinal, #txtFechainicio, #txtFechafin').datepicker({
        format: "dd/mm/yyyy",   // Establecer el formato
        todayHighlight: true,   // Resaltar la fecha de hoy
        autoclose: true,        // Cerrar automáticamente el calendario al seleccionar
        language: "es"          // Idioma español para los textos
    });

    // Evento change para cuando seleccionen un valor en el combo de fincas
    $("#cboFinca").change(function () {
        // Obtener el nombre de la finca desde el atributo 'data-nombre' de la opción seleccionada
        var vnombreFinca = $("#cboFinca option:selected").attr("data-nombre");
        var vidFinca = $("#cboFinca option:selected").attr("data-id");

        // Colocar el nombre de la finca en el campo de texto deshabilitado
        $("#txtNombreFinca").val(vnombreFinca);
        $("#txtIdFinca").val(vidFinca);
    });

    $("#cboRequisito").change(function () {
        // Obtener el nombre del Requisito desde el atributo 'data-requisito' de la opción seleccionada
        var vdescrequisito = $("#cboRequisito option:selected").attr("data-requisito");
        var vidReq = $("#cboRequisito option:selected").attr("data-idReq");

        // Colocar el nombre de la finca en el campo de texto deshabilitado
        $("#txtRequisito").val(vdescrequisito);
        $("#txtIdRequisito").val(vidReq);
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
                $(".card-body").LoadingOverlay("show");
                var fila = tablaActividades.row($(this).parents('tr'));
                fila.remove().draw(false); // Eliminar la fila y actualizar la tabla sin recargar
                $(".card-body").LoadingOverlay("hide");
            }
            
        })
    });

});


function mostrarModal(modelo = MODELO_ACTIVIDAD) {
    
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
    $('#txtIdRequisito').val('');

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
        //Obtener los datos de la fila directamente desde DataTables
        const rowData = tablaActividades.row(this).data();

        // Armar la actividad con rowData[x] en vez de .find('td').eq(x)
        let actividad = {
            descripcion: rowData[0],
            tipo: rowData[1],
            fechaIni: convertirFecha(rowData[2]),
            fechaFin: convertirFecha(rowData[3]),
            responsable: rowData[4],
            recursos: rowData[5],
            estado: rowData[6],
            avances: parseFloat(rowData[7]),
            comentarios: rowData[8],
            idFinca: parseInt($('#txtIdFinca').val(), 10),
            fechaUltimarevision: obtenerFechaActual(),
            idRequisito: parseInt(rowData[9]),   // 👈🏽 ahora sí llega
            avanceanterior: 0.0
        };

        modeloplan.actividades.push(actividad);
    });
    
    
    // Enviar el modelo completo al servidor como JSON
    $(".card-body").LoadingOverlay("show");
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
                Swal.fire("Listo!", "Plan de Trabajo creado con éxito!", "success");
                limpiarFormularioYTabla();
                $(".card-body").LoadingOverlay("hide");
            } else {
                Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
            }          
        })
        .catch(error => {
            $(".card-body").LoadingOverlay("hide");
            Swal.fire("¡Error!", "Hubo un problema al tratar de agregar el Plan.", "error");
            $(".card-body").LoadingOverlay("hide");
        })
  
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
