

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

let filaseleccionada = null;
let lafinca = null;
let editar;
$(document).ready(function () {

    tablaPlanes = $('#tbPlanes').DataTable({

        responsive: true,
        "ajax": {
            "url": '/PlanesTrabajo/Lista',
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
            { "data": "avance" },
            { "data": "observaciones" },

            {
                "defaultContent":   '<button class="btn btn-primary btn-editar btn-sm mr-1"><i class="fas fa-pencil-alt"></i></button>' +
                                    '<button class="btn btn-info btn-mostrar btn-sm mr-1"><i class="fas fa-eye"></i></button>'+
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

    VISTA_BUSQUEDA["busquedaFecha"]()
    
    $.datepicker.setDefaults($.datepicker.regional["es"])
    $("#txtFechaIni").datepicker({ dateFormat: "dd/mm/yy"})
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaIniPlan").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFinPlan").datepicker({ dateFormat: "dd/mm/yy" })

    var tablaActividades = $('#tbActividades').DataTable({
        responsive: true, // Hacer que la tabla sea responsive
        searching: false,      // Ocultar el campo de búsqueda
        lengthChange: false,   // Ocultar el campo de selección de entradas
        columnDefs: [
            {
                targets: [0],  // Índice de la columna "IdActividad" (la primer columna)
                visible: false // Hacerla invisible
            }
        ]
    });
});

$("#cboBuscarPor").change(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    } else {
        VISTA_BUSQUEDA["busquedaFinca"]()
    }
})

$("#btnBuscar").click(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        if ($("#txtFechaIni").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debe ingresar fecha desde y fecha hasta!")
            return;
        }
    } else {
        if ($("#txtFinca").val().trim() == "" ) {
            toastr.warning("", "Debe ingresar nombre de finca!")
            return;
        }
    }
    let nombreFinca = $("#txtFinca").val()
    let fechaInicio = $("#txtFechaIni").val()
    let fechaFinal = $("#txtFechaFin").val()
})

$('#tbPlanes tbody').on('click', '.btn-mostrar', function () {
    
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    }
    else {
        filaseleccionada = $(this).closest("tr");
    }
    editar = false;
    const data = tablaPlanes.row(filaseleccionada).data();
    
    $('#btnGuardar').hide();    
    $('#btnEnviarCorreo').show();
    $('#linkImprimir').show();
    $("#linkImprimir").attr("href",`/PlanesTrabajo/MostrarPDFPlan?idPlan=${data.idPlan}`)

    // Mostrar el modal
    mostrarModal(data, true);

})
// Evento para el botón "Enviar por Correo"
// Evento para el botón "Enviar por Correo"
$('#btnEnviarCorreo').on('click', function () {

    const data = tablaPlanes.row(filaseleccionada).data();
    const correoDestino = data.email;  // Obtenemos el email desde la fila seleccionada
    
    if (correoDestino) {
        $("#modalPlanes .modal-body").LoadingOverlay("show");
        $.ajax({
            url: `/PlanesTrabajo/EnviarPlanPorCorreo?idPlan=${data.idPlan}&correoDestino=${correoDestino}`,
            type: 'GET', // O POST si prefieres
            success: function (response) {
                if (response.estado) {
                    Swal.fire("Listo!", "Correo enviado con éxito!", "success");
                    $("#modalPlanes .modal-body").LoadingOverlay("hide");
                } else {
                    Swal.fire("Error", response.mensaje, "error");
                    $("#modalPlanes .modal-body").LoadingOverlay("hide");
                }
            },
            error: function (error) {
                Swal.fire("Error", "No se pudo enviar el correo. " + response.mensaje, "error");
            }
        });        
    }    
});


$('#tbPlanes tbody').on('click', '.btn-editar', function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaseleccionada = $(this).closest("tr").prev();
    } else {
        filaseleccionada = $(this).closest("tr");
    }
    editar = true;
    const data = tablaPlanes.row(filaseleccionada).data();
    lafinca = data.idFinca    //esto es para tener la finca que no cambiará en la edicion
    $('#btnGuardar').show();
    $('#linkImprimir').hide();
    $('#btnEnviarCorreo').hide();

    mostrarModal(data, false); // Puedes reutilizar la función mostrarModal
});

$('#tbActividades').on('click', 'td', function () {
    if (editar) {
        let contenidoActual = $(this).text();
        let columnaIndex = $(this).index();
        let fila = $(this).closest('tr');
        let idActividad = fila.data('id'); // Obtener el ID de la actividad

        // Columna de fecha (índices 3 y 4)
        if (columnaIndex === 2 || columnaIndex === 3) {
            // Crear el input de tipo texto
            let input = $('<input type="text" class="form-control form-control-sm datepicker">').val(contenidoActual);
            $(this).empty().append(input);
            input.focus();

            // Inicializa el datetimepicker
            input.datepicker({
                dateFormat: 'dd/mm/yy', // Asegúrate de que el formato sea el correcto
                onClose: function () {
                    // Al cerrar el datepicker, se asigna el valor a la celda
                    let nuevoContenido = input.val(); // Obtener el valor del input
                    $(input).parent().text(nuevoContenido); // Asignar el valor a la celda
                }
            });

            // Desplegar el datetimepicker inmediatamente
            input.datepicker("show");

            // Evento para cerrar el datepicker al presionar "Enter"
            input.on('keypress', function (e) {
                if (e.which == 13) {
                    input.datepicker("hide"); // Cerrar el datepicker al presionar Enter
                }
            });
        }

        else if (columnaIndex === 1) {
            // Insertar select con opciones
            let select = $(`
                <select class="form-control form-control-sm">
                    <option value="DOCUMENTAL">Documental</option>
                    <option value="PROCEDIMENTAL">Procedimental</option>
                    <option value="INFRAESTRUCTURA">Infraestructura</option>
                </select>
            `).val(contenidoActual.trim());
            $(this).empty().append(select);
            select.focus();

            select.on('blur change', () => {
                let nuevoContenido = select.val();
                $(this).text(nuevoContenido);
            });
        }
            // Columna de estado (índice 6)
        else if (columnaIndex === 6) {
            // Insertar select con opciones
            let select = $(`
                <select class="form-control form-control-sm">
                    <option value="INICIADO">Iniciado</option>
                    <option value="EN PROCESO">En Proceso</option>
                    <option value="FINALIZADO">Finalizado</option>
                </select>
            `).val(contenidoActual.trim());

            $(this).empty().append(select);
            select.focus();

            // Mostrar la lista de selección al enfocar
            select.on('focus', function () {
                $(this).trigger('mousedown'); // Simula el clic para mostrar la lista
            });

            // Manejar el evento blur y change
            select.on('blur change', () => {
                let nuevoContenido = select.val();
                $(this).text(nuevoContenido);
            });
        }

        // Otras columnas: Usar el input normal de texto
        else {
            let input = $('<input type="text" class="form-control form-control-sm">').val(contenidoActual);
            $(this).empty().append(input);
            input.focus();

            input.on('blur', () => {
                let nuevoContenido = input.val();
                $(this).text(nuevoContenido);
            });

            input.on('keypress', function (e) {
                if (e.which == 13) {
                    input.blur();
                }
            });
        }
    }
});

$('#btnGuardar').on('click', function () {
    $("#modalPlanes .modal-body").LoadingOverlay("show");
    const modeloplan = {            
        idPlan: parseInt($('#txtId').val(),10),
        descripcion: $('#txtDescripcion').val(),
        fechaIni: convertirFecha($('#txtFechaIniPlan').val()),
        fechaFin: convertirFecha($('#txtFechaFinPlan').val()),
        avance: parseFloat($('#txtAvance').val()),
        estado: $('#cboestado').val(),
        observaciones: $('#txtObservaciones').val(),
        idFinca: parseInt(lafinca,10),
        actividades: [] // Aquí irán las actividades
    };
        
    // Obtener actividades del modal
    $('#tbActividades tbody tr').each(function () {
            
        let fila = $(this);
        let idActivity = $('#tbActividades').DataTable().cell(fila, 0).data();
        let actividad = {
                
            idActividad: parseInt(idActivity),
            descripcion: fila.find('td').eq(0).text(),
            tipo: fila.find('td').eq(1).text(),
            fechaIni: convertirFecha(fila.find('td').eq(2).text()),
            fechaFin: convertirFecha(fila.find('td').eq(3).text()),
            responsable: fila.find('td').eq(4).text(),
            recursos: fila.find('td').eq(5).text(),
            estado: fila.find('td').eq(6).text(),
            avances: parseFloat(fila.find('td').eq(7).text()),
            comentarios: fila.find('td').eq(8).text(),
            idRequisito: parseInt(fila.find('td').eq(9).text(), 10),
            idFinca: parseInt(lafinca, 10),
        };
        modeloplan.actividades.push(actividad);
    });
      
        // Hacer la petición al servidor para actualizar
        fetch("/PlanesTrabajo/EditarPlan", {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(modeloplan)
        })
            .then(response => response.json())
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire({title    :"Listo!",
                        text: "Plan de Trabajo modificado con éxito!",
                        icon: "success",
                        confirmButtonColor: "#3085d6",
                        confirmButtonText: `<i class="fa fa-thumbs-up"></i>  OK !`,
                    });

                    tablaPlanes.row(filaseleccionada).data(responseJson.objeto).draw(false);          
                    $('#modalPlanes').modal('hide');
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
                }                          
            })
            .finally(() => {
                $("#modalPlanes .modal-body").LoadingOverlay("hide");
            }); 
});

$('#tbPlanes tbody').on('click', '.btn-eliminar', function () {    
    let fila;

    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    }
    else {
        fila = $(this).closest("tr");
    }
    const data = tablaPlanes.row(fila).data();
    Swal.fire({
        title: "Seguro de continuar?",
        text: `Eliminar Plan de Trabajo "${data.idPlan} ${data.descripcion.length > 40 ? data.descripcion.substring(0, 40) + '...' : data.descripcion}" de la finca: "${data.nombreFinca}" `,
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
            fetch(`/PlanesTrabajo/Eliminar?IdPlan=${data.idPlan}`, {
                method: "DELETE",
            })
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        tablaPlanes.row(fila).remove().draw(false);
                        Swal.fire("Listo!", "Plan de Trabajo No. " + data.idPlan +" de la finca: "+ data.nombreFinca + "  fue eliminado!", "success")
                    } else {
                        Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                    }
                })
                .catch(error => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    Swal.fire("¡Error!", "Hubo un problema al eliminar el plan.", "error");
                });
        }
    })
})

function mostrarModal(modelo, edita) {

  
    $("#txtDescripcion").prop('disabled', edita)
    $("#txtFechaIniPlan").prop('disabled', edita)
    $("#txtFechaFinPlan").prop('disabled', edita)
    $("#txtObservaciones").prop('disabled', edita)
    $("#txtAvance").prop('disabled', edita)
    $("#cboestado").prop('disabled', edita)
  
  
    $("#txtId").val(modelo.idPlan)
    $("#txtDescripcion").val(modelo.descripcion)   
    $("#txtFechaIniPlan").val(formatearFecha(modelo.fechaIni))
    $("#txtFechaFinPlan").val(formatearFecha(modelo.fechaFin))
    $("#txtObservaciones").val(modelo.observaciones)
    $("#txtAvance").val(parseFloat(modelo.avance))
    $("#cboestado").val(modelo.estado)
    $("#txtNombreFinca").val(modelo.nombreFinca)
    $("#txtCodFinca").val(modelo.codFinca)
    
    cargarActividades(modelo),
    
    $("#modalPlanes").modal("show")
}

// Asignar actividades del modelo a la tabla tbActividades

function cargarActividades(modelo) {
    // Limpiar la tabla antes de agregar nuevas actividades
    $('#tbActividades').DataTable().clear().draw();

    // Recorrer las actividades en el modelo y agregarlas a la tabla
    modelo.actividades.forEach(function (actividad) {
        $('#tbActividades').DataTable().row.add([
            actividad.idActividad,
            actividad.descripcion,
            actividad.tipo,
            formatearFecha(actividad.fechaIni),  // Formatear la fecha
            formatearFecha(actividad.fechaFin),  // Formatear la fecha
            actividad.responsable,
            actividad.recursos,
            actividad.estado,
            actividad.avances,
            actividad.comentarios,
            actividad.idRequisito,

        ]).draw(false);       
    });
   
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


// Sin uso de Datatable
//function cargarActividades(modelo) {
//    // Limpiar las filas existentes en la tabla
//    $('#tbActividades tbody').empty();

//    // Recorrer cada actividad en el modelo y crear una fila HTML
//    modelo.actividades.forEach(function (actividad) {
//        const fila = `
//            <tr>
//                <td>${actividad.idActividad}</td>
//                <td>${actividad.descripcion}</td>
//                <td>${actividad.tipo}</td>
//                <td>${formatearFecha(actividad.fechaIni)}</td>  <!-- Formatear la fecha -->
//                <td>${formatearFecha(actividad.fechaFin)}</td>  <!-- Formatear la fecha -->
//                <td>${actividad.responsable}</td>
//                <td>${actividad.recursos}</td>
//                <td>${actividad.estado}</td>
//                <td>${actividad.avances}</td>
//                <td>${actividad.comentarios}</td>
//                <td>${actividad.idRequisito}</td>
//            </tr>
//        `;

//        // Agregar la fila al tbody de la tabla
//        $('#tbActividades tbody').append(fila);
//    });
//}