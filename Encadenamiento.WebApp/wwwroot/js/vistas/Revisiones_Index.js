let tablaRequisitos;
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
                    $("#cboFinca").append(
                        $("<option>")
                            .val(item.codFinca)
                            .text(item.codFinca + " - " + item.descripcion)
                            .attr("data-nombre", item.descripcion)
                            .attr("data-id", item.idFinca)
                    ); 
                    $('#cboFinca').val('');    //Para que no muestre valor inicial en el campo
                    $('#txtNombreFinca').val('');
                    $('#txtIdFinca').val('');
                    $('#cboTipo').val('');
                });
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        });

    // Inicializar DataTable
    tablaRequisitos = $('#tbRequisitos').DataTable({
        responsive: true,
        "ajax": {
            "url": '/CheckList/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idRequisito", "className": "id-requisito", "width": "50px" },
            {
                "data": "descripcion",
                "className": "descripcion",
                "render": function (data, type, row) {
                    return data.length > 60 ? data.substr(0, 60) + "..." : data;
                }
            },
            { "data": "ambito", "className": "ambito" },
            {
                "data": null, // Asegúrate de que el campo "estado" exista en tu modelo de datos.
                "render": function (data, type, row) {
                    return `<select class="form-control estado-editable">
                        <option value="" disabled selected>Seleccionar...</option> <!-- Sin selección -->                        
                        <option value="CUMPLE" ${data === "CUMPLE" ? "selected" : ""}>Cumple</option>
                        <option value="CUMPLE PARCIAL" ${data === "CUMPLE PARCIAL" ? "selected" : ""}>Cumple Parcial</option>
                        <option value="NO CUMPLE" ${data === "NO CUMPLE" ? "selected" : ""}>No Cumple</option>
                        <option value="NO APLICA" ${data === "NO APLICA" ? "selected" : ""}>No Aplica</option>
                    </select>`;
                },
                "className": "estado-editable"

            },
            {
                "data": null, // Asegúrate de que el campo "comentarios" exista en tu modelo de datos.
                "render": function (data, type, row) {
                    let comentarios = typeof row.comentarios === 'string' ? row.comentarios : '';
                    return `<input type="text" class="form-control comentarios-editable" value="${comentarios}" />`;
                },
                "className": "comentarios-editable"
            }
        ],
        "createdRow": function (row, data, dataIndex) {
            var ambito = data.ambito;
            if (ambito === "LABORAL") {
                $('td', row).eq(2).css('background-color', '#CCDBFD');
            } else if (ambito === "AMBIENTAL") {
                $('td', row).eq(2).css('background-color', '#D6EADF');
            } else if (ambito === "OCUPACIONAL") {
                $('td', row).eq(2).css('background-color', '#FFCAAF');
            } else if (ambito === "RSE") {
                $('td', row).eq(2).css('background-color', '#CDB4DB');
            } else {
                $('td', row).eq(2).css('background-color', '#FFFEC4');
            }
        },
        "lengthMenu": [15, 20, 30, 40, 53],
        "pageLength": 15,
        order: [[0, "asc"]],
        dom: "lrtip"
    });

    // Inicializar el datepicker
    $('#txtFecha').datepicker({
        format: "dd/mm/yyyy",
        todayHighlight: true,
        autoclose: true,
        language: "es"
    });
});

// Evento de cambio en la columna de "estado"
$('#tbRequisitos tbody').on('change', 'select.estado-editable', function () {
    var rowIdx = tablaRequisitos.row($(this).closest('tr')).index();
    var selectedValue = $(this).val();

    // Actualizar el modelo de datos de la tabla
    var data = tablaRequisitos.row(rowIdx).data();
    data.estado = selectedValue;  // Actualiza el valor de estado

    $(this).closest('tr').find('input.comentarios-editable').focus();
    calcularCumplimientoGlobal();
});

$('#tbRequisitos tbody').on('blur', 'input.comentarios-editable', function () {
    var row = $(this).closest('tr');
    var rowIdx = tablaRequisitos.row(row).index();
    var newText = $(this).val();

    // Actualizar el modelo de datos de la tabla
    var data = tablaRequisitos.row(rowIdx).data();
    data.comentarios = newText;  // Actualiza el valor de comentarios
});

// Función para mover el foco al estado de la siguiente fila al presionar Enter en comentarios
$('#tbRequisitos tbody').on('keypress', 'input.comentarios-editable', function (e) {
    if (e.which === 13) {  // Detecta si se presiona la tecla Enter (código 13)
        e.preventDefault(); // Evita que el Enter provoque un comportamiento predeterminado (como submit)

        var row = $(this).closest('tr');
        var nextRow = row.next(); // Selecciona la fila siguiente

        // Verifica si existe una fila siguiente
        if (nextRow.length > 0) {
            // Mueve el foco al select de estado en la fila siguiente
            nextRow.find('select.estado-editable').focus();
        }
    }
});


// Calcula el cumplimiento global
function calcularCumplimientoGlobal() {
    var cantidadCumple = 0;
    var cantidadCumpleParcial = 0;
    var cantidadNoCumple = 0;

    tablaRequisitos.rows().data().each(function (rowData) {
        var estado = rowData.estado;
        if (estado === "CUMPLE") {
            cantidadCumple++;
        } else if (estado === "CUMPLE PARCIAL") {
            cantidadCumpleParcial++;
        } else if (estado === "NO CUMPLE") {
            cantidadNoCumple++;
        }
    });

    var total = cantidadCumple + cantidadCumpleParcial + cantidadNoCumple;
    if (total > 0) {
        var cumplimiento = (cantidadCumple + (cantidadCumpleParcial * 0.5)) / total;
        cumplimiento = (cumplimiento * 100).toFixed(2);
        $('#txtCumplimiento').val(cumplimiento);
    } else {
        $('#txtCumplimiento').val(0);
    }
}
// Evento change para el combo de Finca
$("#cboFinca").change(function () {
    var vnombreFinca = $("#cboFinca option:selected").attr("data-nombre");
    var vidFinca = $("#cboFinca option:selected").attr("data-id");

    $("#txtNombreFinca").val(vnombreFinca);
    $("#txtIdFinca").val(vidFinca);
});

// Evento click para guardar la revisión
$('#btnGuardarRev').on('click', function () {
    const idFinca = parseInt($('#txtIdFinca').val());
    const fecha = convertirFecha($('#txtFecha').val());
    const tipo = $('#cboTipo').val();
    const cumplimiento = parseFloat($('#txtCumplimiento').val());

    if (!idFinca || !fecha || !tipo || !cumplimiento) {
        toastr.warning("Por favor, complete los campos generales (Finca, Fecha, Tipo, Cumplimiento)");
        return;
    }

    const filas = $('#tbRequisitos').DataTable().rows().data();
    const revisiones = [];

    for (let index = 0; index < filas.length; index++) {
        const rowData = filas[index];
        const idRequisito = rowData.idRequisito;
        const estado = rowData.estado;
        const observaciones = rowData.observaciones ? rowData.observaciones.substring(0, 100) : ' ';        
        const comentarios = rowData.comentarios;

        // Si el estado está vacío, mostramos el mensaje y detenemos la función
        if (!estado) {
            toastr.warning(`Debe seleccionar una condición para el estado en el requisito idReq= ${idRequisito}`);
            return;  // Detenemos toda la ejecución de la función
        }

        revisiones.push({
            IdFinca: idFinca,
            IdRequisito: parseInt(idRequisito),
            Fecha: fecha,
            Estado: estado,
            Observaciones: observaciones,
            Comentarios: comentarios,
            Tipo: tipo,
            Cumplimiento: cumplimiento
        });
    }

   $(".card-body").LoadingOverlay("show");

   fetch("/Revision/Crear", {
       method: "POST",
       headers: {
           "Content-Type": "application/json"
       },
       body: JSON.stringify(revisiones)
   })
    .then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        $(".card-body").LoadingOverlay("hide");
        if (responseJson.estado) {
            Swal.fire("Listo!", `Revisión para la finca ${responseJson.objeto.nombreFinca} para la fecha ${responseJson.objeto.fecha} ingresada con éxito!`, "success");
            limpiarFormularioYTabla();

        } else {
            Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
        }
    })
    .catch(error => {
        $(".card-body").LoadingOverlay("hide");
        Swal.fire("¡Error!", "Hubo un problema al agregar la revisión");
    });
})

function limpiarFormularioYTabla() {
    // Limpiar los campos del formulario del Plan de Trabajo
    $('#cboFinca').val('');
    $('#txtNombreFinca').val('');
    $('#txtIdFinca').val('');
    $('#txtFecha').val('');    
    $('#cboTipo').val('');
    $('#txtCumplimiento').val('');    
  
    // Limpiar la tabla de actividades
    tablaRequisitos.ajax.reload();
    
}