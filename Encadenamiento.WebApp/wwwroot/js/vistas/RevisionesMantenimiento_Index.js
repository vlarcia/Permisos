

const VISTA_BUSQUEDA = {
    busquedaFecha: () => {
        $("#txtFechabus").val("")        
        $("#txtFinca").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-finca").hide()
    },
    busquedaFinca: () => {
        $("#txtFechabus").val("")        
        $("#txtFinca").val("")

        $(".busqueda-fecha").hide()
        $(".busqueda-finca").show()
    }

}

let filaseleccionada = null;
let lafinca = null;
let editando = false;
var tablaRequisitos;
$(document).ready(function () {

    tablaRevisiones = $('#tbRevisiones').DataTable({

        responsive: true,
        "ajax": {
            "url": '/Revision/Lista',
            "type": "GET",
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
                "data": "cumplimiento",
                "render": function (data) {
                    if (data) {
                        return parseFloat(data).toFixed(2); // Dos decimales
                    }
                    return "00.00"; // Valor por defecto si no hay dato
                },
                "className": "text-center" // Alinear al centro
            },

            { "data": "tipo" },
            { "data": "comentarios" },

            {
                "defaultContent":   '<button class="btn btn-primary btn-editar btn-sm mr-1"><i class="fas fa-pencil-alt"></i></button>' +
                                    '<button class="btn btn-info btn-mostrar btn-sm mr-1"><i class="fas fa-eye"></i></button>'+
                                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "20px"
            }

        ],
        order: [[2, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_Revisiones',            
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    VISTA_BUSQUEDA["busquedaFecha"]()
    
    $.datepicker.setDefaults($.datepicker.regional["es"])
    $("#txtFechabus").datepicker({ dateFormat: "dd/mm/yy"})
    $("#txtFecha").datepicker({ dateFormat: "dd/mm/yy" })   
    
});

$("#cboBuscarPor").val('');
$("#cboBuscarPor").change(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    } else {
        VISTA_BUSQUEDA["busquedaFinca"]()
    }
})

$("#btnBuscar").click(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        if ($("#txtFechabus").val().trim() == "" || $("#txtFechabus").val().trim() == "") {
            toastr.warning("", "Debe ingresar la fecha de Busqueda!")
            return;
        }
    } else {
        if ($("#txtFinca").val().trim() == "" ) {
            toastr.warning("", "Debe ingresar nombre de finca!")
            return;
        }
    }
    let nombreFinca = $("#txtFinca").val()
    let fechabus = $("#txtFechabus").val()
    
})

$('#tbRevisiones tbody').on('click', '.btn-mostrar', function () {
    editando = false;  // Asegúrate de estar en modo de solo lectura
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    } else {
        filaseleccionada = $(this).closest("tr");
    }

    const ladata = tablaRevisiones.row(filaseleccionada).data();
    const idFinca = ladata.idFinca;
    const fecha = formatearFecha(ladata.fecha);

    $('#btnGuardar').hide();
    $('#btnEnviarCorreo').show();
    $('#linkImprimir').show();
    $("#linkImprimir").attr("href", `/Revision/MostrarPDFRevision?idFinca=${ladata.idFinca}&fecha=${fecha}`);



    if ($.fn.DataTable.isDataTable('#tbRequisitos')) {
        $('#tbRequisitos').DataTable().clear().destroy();
    }

    // AJAX para obtener las revisiones y crear la tabla
    $.ajax({
        url: `/Revision/ObtenerRevision`,
        type: 'GET',
        data: { idFinca: idFinca, Fecha: fecha, grupo:0 },
        success: function (data) {
            // Crear tabla con los datos recibidos
            tablaRequisitos = $('#tbRequisitos').DataTable({
                responsive: true,
                autoWidth: false,
                data: data.data,
                columns: [
                    { data: 'idRequisito', title: 'IdReq' },
                    {
                        data: 'requisito', title: 'Descripcion',
                        "render": function (data, type, row) {
                            return data.length > 80 ? data.substr(0, 80) + "..." : data;
                        }
                    },
                    { data: 'ambito', title: 'Ambito' },
                    {
                        data: 'estado', title: 'Estado',
                        "render": function (data, type, row) {
                            // Deshabilitar el select si no estamos editando
                            if (!editando) {
                                return `<span>${data}</span>`; // Muestra el estado como texto
                            }
                            return `<select class="form-control estado">                        
                                <option value="CUMPLE" ${data === "CUMPLE" ? "selected" : ""}>Cumple</option>
                                <option value="CUMPLE PARCIAL" ${data === "CUMPLE PARCIAL" ? "selected" : ""}>Cumple Parcial</option>
                                <option value="NO CUMPLE" ${data === "NO CUMPLE" ? "selected" : ""}>No Cumple</option>
                                <option value="NO APLICA" ${data === "NO APLICA" ? "selected" : ""}>No Aplica</option>`;
                        }
                    },
                    {
                        data: 'comentarios', title: 'Comentarios',
                        "render": function (data, type, row) {
                            let comentarios = typeof data === 'string' ? data : '';
                            if (!editando) {
                                return `<span>${comentarios}</span>`; // Muestra el comentario como texto
                            }
                            return `<input type="text" class="form-control comentarios" value="${comentarios}" />`;
                        }
                    }
                ],
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "45%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "20%", "targets": 4 }
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
                "lengthMenu": [15, 25, 35, 45, 53],
                "pageLength": 15,
                order: [[0, "asc"]],
                dom: "lrtip"
            });

            // Ahora, después de que la tabla esté creada con los datos, se muestra el modal
            mostrarModal(data.data[0], true);  // Usa el primer registro como ejemplo
        }
    });
});
// Evento para el botón "Enviar por Correo"
$('#btnEnviarCorreo').on('click', function () {

    const data = tablaRevisiones.row(filaseleccionada).data();
    const correoDestino = data.email;  // Obtenemos el email desde la fila seleccionada
    const fecha=formatearFecha(data.fecha)
    if (correoDestino) {
        $("#modalRevisiones .modal-body").LoadingOverlay("show");
        $.ajax({
            url: `/Revision/EnviarRevisionPorCorreo?idFinca=${data.idFinca}&fecha=${fecha}&correoDestino=${correoDestino}`,
            type: 'GET', // O POST si prefieres
            success: function (response) {
                if (response.estado) {
                    Swal.fire("Listo!", "Correo enviado con éxito!", "success");
                    $("#modalRevisiones .modal-body").LoadingOverlay("hide");
                } else {
                    Swal.fire("Error", response.mensaje, "error");
                    $("#modalRevisiones .modal-body").LoadingOverlay("hide");
                }
            },
            error: function (error) {
                Swal.fire("Error", "No se pudo enviar el correo. " + response.mensaje, "error");
            }
        });        
    }    
});


$('#tbRevisiones tbody').on('click', '.btn-editar', function () {

    editando = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    } else {
        filaseleccionada = $(this).closest("tr");
    }

    const ladata = tablaRevisiones.row(filaseleccionada).data();
    const idFinca = ladata.idFinca;
    const fecha = formatearFecha(ladata.fecha);

    $('#btnGuardar').show();
    $('#linkImprimir').hide();
    $('#btnEnviarCorreo').hide();

    if ($.fn.DataTable.isDataTable('#tbRequisitos')) {
        $('#tbRequisitos').DataTable().clear().destroy();
    };
    // AJAX para obtener las revisiones y crear la tabla
    $.ajax({
        url: `/Revision/ObtenerRevision`,
        type: 'GET',
        data: { idFinca: idFinca, Fecha: fecha, grupo:0 },
        success: function (data) {
            // Crear tabla con los datos recibidos
            tablaRequisitos = $('#tbRequisitos').DataTable({
                responsive: true,
                data: data.data,  // Asegúrate de usar el array de datos directamente
                columns: [
                    { data: 'idRequisito', title: 'IdReq', width: '6%' },
                    {
                        data: 'requisito', title: 'Descripcion', width: '50%',
                        "render": function (data, type, row) {
                            return data.length > 80 ? data.substr(0, 80) + "..." : data;
                        }
                    },
                    { data: 'ambito', title: 'Ambito', width: '14%' },
                    {
                        data: 'estado', title: 'Estado', width: '10%',
                        "render": function (data, type, row) {
                            return `<select class="form-control estado">                        
                            <option value="CUMPLE" ${data === "CUMPLE" ? "selected" : ""}>Cumple</option>
                            <option value="CUMPLE PARCIAL" ${data === "CUMPLE PARCIAL" ? "selected" : ""}>Cumple Parcial</option>
                            <option value="NO CUMPLE" ${data === "NO CUMPLE" ? "selected" : ""}>No Cumple</option>
                            <option value="NO APLICA" ${data === "NO APLICA" ? "selected" : ""}>No Aplica</option>`;
                        }
                    },
                    {
                        data: 'comentarios', title: 'Comentarios', width: '20%',
                        "render": function (data, type, row) {
                            let comentarios = typeof data === 'string' ? data : '';
                            return `<input type="text" class="form-control comentarios" value="${comentarios}" />`;
                        }
                    }
                ],
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "45%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "20%", "targets": 4 }
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
                "lengthMenu": [15, 25, 35, 45, 53],
                "pageLength": 15,
                order: [[0, "asc"]],
                dom: "lrtip"
            });

            // Ahora, después de que la tabla esté creada con los datos, se muestra el modal
            mostrarModal(data.data[0], false);  // Usa el primer registro como ejemplo
        }
    });
});


// Evento de cambio en la columna de "estado"

$('#tbRequisitos tbody').on('change', 'select.estado', function () {
    if (editando) {
        var rowIdx = tablaRequisitos.row($(this).closest('tr')).index();
        var selectedValue = $(this).val();

        // Actualizar el modelo de datos de la tabla
        var data = tablaRequisitos.row(rowIdx).data();
        data.estado = selectedValue;  // Actualiza el valor de estado

        $(this).closest('tr').find('input.comentarios').focus();
        calcularCumplimientoGlobal();
    }
});

$('#tbRequisitos tbody').on('blur', 'input.comentarios', function () {
    if (editando) {
        var row = $(this).closest('tr');
        var rowIdx = tablaRequisitos.row(row).index();
        var newText = $(this).val();

        // Actualizar el modelo de datos de la tabla
        var data = tablaRequisitos.row(rowIdx).data();
        data.comentarios = newText;  // Actualiza el valor de comentarios
    }
});

// Función para mover el foco al estado de la siguiente fila al presionar Enter en comentarios
$('#tbRequisitos tbody').on('keypress', 'input.comentarios', function (e) {
    if (e.which === 13) {  // Detecta si se presiona la tecla Enter (código 13)
        e.preventDefault(); // Evita que el Enter provoque un comportamiento predeterminado (como submit)

        var row = $(this).closest('tr');
        var nextRow = row.next(); // Selecciona la fila siguiente

        // Verifica si existe una fila siguiente
        if (nextRow.length > 0) {
            // Mueve el foco al select de estado en la fila siguiente
            nextRow.find('select.estado').focus();
        }
    }
});

$('#btnGuardar').on('click', function () {
    const idFinca = parseInt($('#txtIdFinca').val());
    const fecha = convertirFecha($('#txtFecha').val());
    const tipo = $('#cboTipo').val();
    const cumplimiento = parseFloat($('#txtCumplimiento').val());

    if (!tipo || !cumplimiento) {
        toastr.warning("Por favor, complete los campos generales (Tipo, Cumplimiento)");
        return;
    }

    const filas = $('#tbRequisitos').DataTable().rows().data();
    const revisiones = [];

    for (let index = 0; index < filas.length; index++) {
        const rowData = filas[index];
        const idRevision = rowData.idRevision;
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
            IdRevision:parseInt(idRevision),
            IdFinca: parseInt(idFinca),
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

    fetch("/Revision/Editar", {
        method: "PUT",        
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
                Swal.fire("Listo!", `Revisión para la finca ${responseJson.objeto.nombreFinca} con fecha ${responseJson.objeto.fecha} actualizada con éxito!`, "success");
                $('#modalRevisiones').modal('hide');

            } else {
                Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
            }
        })
        .catch(error => {
            $(".card-body").LoadingOverlay("hide");
            Swal.fire("¡Error!", "Hubo un problema al editar la revisión");
        });
       
});

$('#tbRevisiones tbody').on('click', '.btn-eliminar', function () {    

    let filas;
    let revisiones = [];    
    
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    } else {
        filaseleccionada = $(this).closest("tr");
    }
    const ladata = tablaRevisiones.row(filaseleccionada).data();
    const idFinca = ladata.idFinca;
    const fecha = formatearFecha(ladata.fecha);

    // Declarar la variable afuera del bloque AJAX
    

    $.ajax({
        url: `/Revision/ObtenerRevision`,
        type: 'GET',
        data: { idFinca: idFinca, Fecha: fecha, grupo:0 },
        success: function (data) {
            // Guardar los datos recibidos en la variable externa
            filas = data;

            for (let index = 0; index < filas.data.length; index++) {
                /*const rowData = filas[index];*/
                const idRevision = filas.data[index].idRevision
                revisiones.push({
                    IdRevision: parseInt(idRevision)
                });
            }
            const rev_eliminar = revisiones.map(r => r.IdRevision); // Extrae solo los valores de IdRevision
            
            Swal.fire({
                title: "Seguro de continuar?",
                text: `Eliminar Revision de la finca "${ladata.codFinca} ${ladata.nombreFinca}" con fecha "${fecha}"`,
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
                    console.log(revisiones)
                    fetch(`/Revision/Eliminar`, {
                        method: "DELETE",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify(rev_eliminar)
                    })
                        .then(response => {
                            $(".showSweetAlert").LoadingOverlay("hide");
                            return response.ok ? response.json() : Promise.reject(response);
                        })
                        .then(responseJson => {
                            if (responseJson.estado) {
                                tablaRevisiones.row(filaseleccionada).remove().draw(false);
                                Swal.fire("Listo!", "los registros de la Revision  fueron eliminados!", "success")
                                $('#modalRevisiones').modal('hide');
                            } else {
                                Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                            }
                        })
                        .catch(error => {
                            $(".showSweetAlert").LoadingOverlay("hide");
                            Swal.fire("¡Error!", "Hubo un problema al eliminar la revision.", "error");
                        });
                }
            })
        }
    });
})

function mostrarModal(modelo, edita) {
    
    $("#txtIdFinca").prop('disabled', true)    
    $("#txtFecha").prop('disabled', true)    
    $("#txtCumplimiento").prop('disabled', edita)
    $("#cboTipo").prop('disabled', edita)
    $("#txtNombreFinca").prop('disabled', true)
    $("#txtCodFinca").prop('disabled', true)

    $("#txtIdFinca").val(parseInt(modelo.idFinca))
    $("#txtNombreFinca").val(modelo.nombreFinca)
    $("#txtFecha").val(formatearFecha(modelo.fecha))
    $("#txtCumplimiento").val(parseFloat(modelo.cumplimiento))
    $("#cboTipo").val(modelo.tipo)
    $("#txtNombreFinca").val(modelo.nombreFinca)
    $("#txtCodFinca").val(modelo.codFinca) 
           
    $("#modalRevisiones").modal("show")
}

// Asignar actividades del modelo a la tabla tbActividades

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
