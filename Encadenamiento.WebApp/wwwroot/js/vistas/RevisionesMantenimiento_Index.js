

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
let modeloGeneral;
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

            // Columna de fecha con ordenamiento correcto
            {
                "data": "fecha",                
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
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-1"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-info btn-mostrar btn-sm mr-1"><i class="fas fa-eye"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "20px"
            }
        ],
        // Ordenar por la columna de fecha (índice 2) en orden descendente
        order: [[2, "desc"]],
        columnDefs: [
            {
                // Configurar la columna de fecha (índice 2)
                targets: 2,
                render: function (data, type) {
                    // Si el tipo es "display", mostramos la fecha formateada
                    if (type === 'display' && data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('es-NI', {
                            day: '2-digit',
                            month: '2-digit',
                            year: 'numeric'
                        });
                    }
                    // Para otros tipos (como ordenamiento), devolvemos la fecha sin formato
                    return data || "";
                }
            }
        ],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_Revisiones',
            },
            'pageLength',
            {
                extend: 'pdfHtml5',
                title: 'Listado Revisiones'
            },
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


document.getElementById("txtFoto1").addEventListener("change", function (event) {
    const file = event.target.files[0]; // Obtener el primer archivo seleccionado
    const imgElement = document.getElementById("imgFoto1");

    if (file) {
        const reader = new FileReader(); // Crear un objeto FileReader

        // Definir la función a ejecutar cuando se carga el archivo
        reader.onload = function (e) {
            imgElement.src = e.target.result; // Asignar la URL del archivo a la imagen
            imgElement.style.opacity = 1; // Cambiar la opacidad si es necesario
        };

        reader.readAsDataURL(file); // Leer el archivo como URL de datos
    } else {
        // Si no hay archivo, puedes asignar la imagen predeterminada
        imgElement.src = "/images/eog-image-photo-svgrepo-com.svg"; // Ruta de la imagen predeterminada
        imgElement.style.opacity = 0.25; // Establecer opacidad predeterminada
    }
});

document.getElementById("txtFoto2").addEventListener("change", function (event) {
    const file = event.target.files[0]; // Obtener el primer archivo seleccionado
    const imgElement = document.getElementById("imgFoto2");

    if (file) {
        const reader = new FileReader(); // Crear un objeto FileReader

        // Definir la función a ejecutar cuando se carga el archivo
        reader.onload = function (e) {
            imgElement.src = e.target.result; // Asignar la URL del archivo a la imagen
            imgElement.style.opacity = 1; // Cambiar la opacidad si es necesario
        };

        reader.readAsDataURL(file); // Leer el archivo como URL de datos
    } else {
        // Si no hay archivo, puedes asignar la imagen predeterminada
        imgElement.src = "/images/eog-image-photo-svgrepo-com.svg"; // Ruta de la imagen predeterminada
        imgElement.style.opacity = 0.25; // Establecer opacidad predeterminada
    }
});

document.getElementById("txtFoto3").addEventListener("change", function (event) {
    const file = event.target.files[0]; // Obtener el primer archivo seleccionado
    const imgElement = document.getElementById("imgFoto3");

    if (file) {
        const reader = new FileReader(); // Crear un objeto FileReader

        // Definir la función a ejecutar cuando se carga el archivo
        reader.onload = function (e) {
            imgElement.src = e.target.result; // Asignar la URL del archivo a la imagen
            imgElement.style.opacity = 1; // Cambiar la opacidad si es necesario
        };

        reader.readAsDataURL(file); // Leer el archivo como URL de datos
    } else {
        // Si no hay archivo, puedes asignar la imagen predeterminada
        imgElement.src = "/images/eog-image-photo-svgrepo-com.svg"; // Ruta de la imagen predeterminada
        imgElement.style.opacity = 0.25; // Establecer opacidad predeterminada
    }
});

document.getElementById("txtFoto4").addEventListener("change", function (event) {
    const file = event.target.files[0]; // Obtener el primer archivo seleccionado
    const imgElement = document.getElementById("imgFoto4");

    if (file) {
        const reader = new FileReader(); // Crear un objeto FileReader

        // Definir la función a ejecutar cuando se carga el archivo
        reader.onload = function (e) {
            imgElement.src = e.target.result; // Asignar la URL del archivo a la imagen
            imgElement.style.opacity = 1; // Cambiar la opacidad si es necesario
        };

        reader.readAsDataURL(file); // Leer el archivo como URL de datos
    } else {
        // Si no hay archivo, puedes asignar la imagen predeterminada
        imgElement.src = "/images/eog-image-photo-svgrepo-com.svg"; // Ruta de la imagen predeterminada
        imgElement.style.opacity = 0.25; // Establecer opacidad predeterminada
    }
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

    // AJAX para obtener el registro general de revisiones
    $.ajax({
        url: `/Revision/ObtenerRevisionGeneral`,
        type: 'GET',
        data: { idFinca: idFinca, Fecha: fecha},
        success: function (data) {
            modeloGeneral = data.data;                       
        }
    });

    $.ajax({
        url: `/Revision/ObtenerRevision`,
        type: 'GET',
        data: { idFinca: idFinca, Fecha: fecha, grupo: 0 },
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
                dom: "Bfrtip",
                buttons: [
                    {
                        extend: 'excelHtml5',
                        title: 'Revision de Finca',
                        exportOptions: {
                            modifier: {
                                page: 'all'
                            }
                        },
                        customize: function (xlsx) {
                            var sheet = xlsx.xl.worksheets['sheet1.xml'];

                            // Captura valores del formulario
                            var codigoFinca = $('#txtCodFinca').val();
                            var nombreFinca = $('#txtNombreFinca').val();
                            var fecha = $('#txtFecha').val();
                            var tipoRevision = $('#cboTipo').val();
                            var cumplimiento = $('#txtCumplimiento').val();
                            var observaciones = $('#txtObservaciones').val();
                            // Crear una nueva fila con las celdas de cabecera en dos columnas: Descripción y Valor
                            var newRows = `
                                <row r="2">
                                    <c t="inlineStr"><is><t>Código Finca:</t></is></c>
                                    <c t="inlineStr"><is><t>${codigoFinca}</t></is></c>
                                </row>
                                <row r="3">
                                    <c t="inlineStr"><is><t>Nombre Finca:</t></is></c>
                                    <c t="inlineStr"><is><t>${nombreFinca}</t></is></c>
                                </row>
                                <row r="4">
                                    <c t="inlineStr"><is><t>Fecha:</t></is></c>
                                    <c t="inlineStr"><is><t>${fecha}</t></is></c>
                                </row>
                                <row r="5">
                                    <c t="inlineStr"><is><t>Tipo Revisión:</t></is></c>
                                    <c t="inlineStr"><is><t>${tipoRevision}</t></is></c>
                                </row>
                                <row r="6">
                                    <c t="inlineStr"><is><t>Cumplimiento:</t></is></c>
                                    <c t="inlineStr"><is><t>${cumplimiento}</t></is></c>
                                </row>
                                <row r="7">
                                    <c t="inlineStr"><is><t>Observaciones:</t></is></c>
                                    <c t="inlineStr"><is><t>${observaciones}</t></is></c>
                                </row>
                            `;

                            // Insertar las nuevas filas antes de los datos actuales (filas 1-5)
                            var rows = sheet.getElementsByTagName('sheetData')[0];
                            rows.innerHTML = newRows + rows.innerHTML;

                            // Desplazar los datos de la tabla para que comiencen en la fila 8
                            var dataRows = sheet.getElementsByTagName('row');
                            var rowIndex = 8;  // Comenzamos en la fila 8 (el índice en XML es 0-based, así que usamos 7)
                            for (var i = 5; i < dataRows.length; i++) {
                                var row = dataRows[i];
                                var cells = row.getElementsByTagName('c');

                                // Cambiar el atributo 'r' (número de fila) para que los datos empiecen en la fila 8
                                row.setAttribute('r', rowIndex + 1); // Ajustar el número de fila
                                for (var j = 0; j < cells.length; j++) {
                                    var cell = cells[j];
                                    var cellRef = cell.getAttribute('r');

                                    if (cellRef) {
                                        var rowIndexInCell = parseInt(cellRef.match(/\d+/)[0]);
                                        var colRef = cellRef.match(/[A-Z]+/)[0];
                                        var newCellRef = colRef + (rowIndex + 1); // Ajustamos la fila para las celdas
                                        cell.setAttribute('r', newCellRef); // Actualizamos la referencia de la celda
                                    }
                                }
                                rowIndex++;  // Aumentamos el índice de fila
                            }
                        }
                    },
                    {
                        extend: 'pdfHtml5',
                        title: 'Revision de Fincas'
                    },
                    {
                        extend: 'print',
                        title: 'Revision de Fincas',
                        customize: function (win) {
                            // Obtener los valores del formulario
                            var codigoFinca = $('#txtCodFinca').val();
                            var nombreFinca = $('#txtNombreFinca').val();
                            var fecha = $('#txtFecha').val();
                            var tipoRevision = $('#cboTipo').val();
                            var cumplimiento = $('#txtCumplimiento').val();
                            var latitud = $('#txtLatitud').val();
                            var longitud = $('#txtLongitud').val();
                            var observaciones = $('#txtObservaciones').val();

                            // Crear el encabezado con los datos del formulario
                            var headerHTML = `
                    <div style="text-align: center; font-weight: bold;">
                        <h2>Reporte de Requisitos</h2>
                        <p>Código Finca: ${codigoFinca}</p>
                        <p>Nombre Finca: ${nombreFinca}</p>
                        <p>Fecha: ${fecha}</p>
                        <p>Tipo Revisión: ${tipoRevision}</p>
                        <p>Cumplimiento: ${cumplimiento}</p>
                        <p>Latitud: ${latitud}</p>
                        <p>Longitud: ${longitud}</p>
                        <p>Observaciones: ${observaciones}</p>
                    </div>
                    <br />
                `;

                            // Insertar el encabezado antes de la tabla impresa
                            $(win.document.body).find('table').before(headerHTML);
                        }
                    }
                ]
            });

            // Muestra el modal una vez creada la tabla con los datos
            mostrarModal(data.data[0], true, modeloGeneral);  // Usa el primer registro
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

    // AJAX para obtener el registro general de revisiones
    $.ajax({
        url: `/Revision/ObtenerRevisionGeneral`,
        type: 'GET',
        data: { idFinca: idFinca, Fecha: fecha },
        success: function (data) {
            modeloGeneral = data.data;            
            
        }
    });

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
            mostrarModal(data.data[0], false, modeloGeneral);  // Usa el primer registro como ejemplo
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
    // Cargar Fotos y FormData
    const inputFoto1 = document.getElementById("txtFoto1");
    const inputFoto2 = document.getElementById("txtFoto2");
    const inputFoto3 = document.getElementById("txtFoto3");
    const inputFoto4 = document.getElementById("txtFoto4");

    const lafoto1 = inputFoto1.files[0] ? inputFoto1.files[0].name : null;
    const lafoto2 = inputFoto2.files[0] ? inputFoto2.files[0].name : null;    
    const lafoto3 = inputFoto3.files[0] ? inputFoto3.files[0].name : null;    
    const lafoto4 = inputFoto4.files[0] ? inputFoto4.files[0].name : null;    

    modeloGeneral.latitud = $("#txtLatitud").val();
    modeloGeneral.longitud = $("#txtLongitud").val();
    modeloGeneral.observaciones = $("#txtObservaciones").val();
    modeloGeneral.nombrefoto1 = lafoto1;
    modeloGeneral.nombrefoto2 = lafoto2;
    modeloGeneral.nombrefoto3 = lafoto3;
    modeloGeneral.nombrefoto4 = lafoto4;
    modeloGeneral.tipo = tipo;
    modeloGeneral.cumplimiento = cumplimiento;            
    
    const filas = $('#tbRequisitos').DataTable().rows().data();
    const revisiones = [];

    for (let index = 0; index < filas.length; index++) {
        const rowData = filas[index];
        const idRevision = rowData.idRevision;
        const idRequisito = rowData.idRequisito;
        const estado = rowData.estado;        
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
            Comentarios: comentarios,
            Tipo: tipo,
            Cumplimiento: cumplimiento
        });
    }
    // Agrega las fotos y modelo de data para mandar a Registrar
    const formData = new FormData();

    formData.append("revisiones", JSON.stringify(revisiones));
    formData.append("foto1", inputFoto1.files[0] ? inputFoto1.files[0] : null);
    formData.append("foto2", inputFoto2.files[0] ? inputFoto2.files[0] : null);
    formData.append("foto3", inputFoto3.files[0] ? inputFoto3.files[0] : null);
    formData.append("foto4", inputFoto4.files[0] ? inputFoto4.files[0] : null);
    formData.append("modeloGeneral", JSON.stringify(modeloGeneral));

    $(".card-body").LoadingOverlay("show");

    fetch("/Revision/Editar", {
        method: "PUT",                
        body: formData    // No establezco 'Content-Type', FormData lo maneja
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
    let filasmodeloGeneral;
    let rev_general = 0;
    
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    } else {
        filaseleccionada = $(this).closest("tr");
    }
    const ladata = tablaRevisiones.row(filaseleccionada).data();
    const idFinca = ladata.idFinca;
    const fecha = formatearFecha(ladata.fecha);

    const formData = new FormData();
    // Declarar la variable afuera del bloque AJAX
    
    // AJAX para obtener el registro general de revisiones
    $.ajax({
        url: `/Revision/ObtenerRevisionGeneral`,
        type: 'GET',
        data: { idFinca: idFinca, Fecha: fecha},
        success: function (data) {
            if (data) {
                filasmodeloGeneral = data;
                const id_general = filasmodeloGeneral.data.idReg
                rev_general=parseInt(id_general)                
            } else {
                console.log("No se recibió ningún dato.");
            }
        }        
    });
    
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

                    formData.append("rev_eliminar", JSON.stringify(rev_eliminar))
                    formData.append("rev_general", rev_general)

                    fetch(`/Revision/Eliminar`, {
                        method: "DELETE",                        
                        body: formData
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

function mostrarModal(modelo, edita, modeloGeneral) {
    
    $("#txtIdFinca").prop('disabled', true)    
    $("#txtFecha").prop('disabled', true)    
    $("#txtCumplimiento").prop('disabled', edita)
    $("#cboTipo").prop('disabled', edita)
    $("#txtLatitud").prop('disabled', edita)
    $("#txtLongitud").prop('disabled', edita)
    $("#txtObservaciones").prop('disabled', edita)
    $("#txtNombreFinca").prop('disabled', true)
    $("#txtCodFinca").prop('disabled', true)
    $("#txtFoto1").prop('disabled', edita);
    $("#txtFoto2").prop('disabled', edita);
    $("#txtFoto3").prop('disabled', edita);
    $("#txtFoto4").prop('disabled', edita);

    $("#txtIdFinca").val(parseInt(modelo.idFinca))
    $("#txtNombreFinca").val(modelo.nombreFinca)
    $("#txtFecha").val(formatearFecha(modelo.fecha))
    $("#txtCumplimiento").val(parseFloat(modelo.cumplimiento))
    $("#cboTipo").val(modelo.tipo)
    $("#txtNombreFinca").val(modelo.nombreFinca)
    $("#txtCodFinca").val(modelo.codFinca) 
    $("#txtLatitud").val(modeloGeneral.latitud) 
    $("#txtLongitud").val(modeloGeneral.longitud) 
    $("#txtObservaciones").val(modeloGeneral.observaciones) 

    const imgElement1 = document.getElementById("imgFoto1");
    const imgElement2 = document.getElementById("imgFoto2");
    const imgElement3 = document.getElementById("imgFoto3");
    const imgElement4 = document.getElementById("imgFoto4");

    // Validar y asignar imágenes para imgFoto 1, 2 y 3
    if (modeloGeneral.nombrefoto1 && modeloGeneral.urlfoto1) {
        imgElement1.src = modeloGeneral.urlfoto1; // Cargar la imagen desde la URL
        imgElement1.style.opacity = 0.9;
    } else {
        imgElement1.src = "/images/eog-image-photo-svgrepo-com.svg";
        imgElement1.style.opacity = 0.3;
    }
    if (modeloGeneral.nombrefoto2 && modeloGeneral.urlfoto2) {
        imgElement2.src = modeloGeneral.urlfoto2;
        imgElement2.style.opacity = 0.9; // Opacidad predeterminada
    } else {
        imgElement2.src = "/images/eog-image-photo-svgrepo-com.svg";
        imgElement2.style.opacity = 0.3; // Opacidad predeterminada
    }
    if (modeloGeneral.nombrefoto3 && modeloGeneral.urlfoto3) {
        imgElement3.src = modeloGeneral.urlfoto3;
        imgElement3.style.opacity = 0.9; // Opacidad predeterminada
    } else {
        imgElement3.src = "/images/eog-image-photo-svgrepo-com.svg";
        imgElement3.style.opacity = 0.3; // Opacidad predeterminada
    }
    if (modeloGeneral.nombrefoto4 && modeloGeneral.urlfoto4) {
        imgElement4.src = modeloGeneral.urlfoto4;
        imgElement4.style.opacity = 0.9; // Opacidad predeterminada
    } else {
        imgElement4.src = "/images/eog-image-photo-svgrepo-com.svg";
        imgElement4.style.opacity = 0.3; // Opacidad predeterminada
    }


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
