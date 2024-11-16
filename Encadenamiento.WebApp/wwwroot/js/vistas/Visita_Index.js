const modelo={
    idVisita: 0,
    idPlan: 0,
    idFinca: 0,
    fecha: "",
    responsable: "",
    mandador: "",
    zafra: 0,
    latitud: "",
    longitud: "",
    observaciones: "",
    urlFoto1: "",
    nombreFoto1: "",
    urlFoto2: "",
    nombreFoto2: "",
    nombreFinca: "",
    codFinca: "",
    descripcionPlan: "",
    senTo: 0,
    aplicado: false,
    sincronizado: false,
    detalleVisita: [],
};

let filaseleccionada;
let editar;
let modeloCargado

$(document).ready(function () {

    // Cargar la lista de fincas usando fetch

    $(".card-body").LoadingOverlay("show");
    try {
        fetch("/PlanesTrabajo/Lista")
            .then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                //console.log(responseJson); // Verifica el contenido de la respuesta
                $(".card-body").LoadingOverlay("hide");
                if (responseJson.data.length > 0) {
                    responseJson.data.forEach((item) => {
                        $("#cboPlan").append(
                            $("<option>")
                                .val(item.idPlan)
                                .text(item.idPlan + " - " + item.descripcion + " -finca: " + item.nombreFinca)
                                .attr("data-idplan", item.idPlan)
                                .attr("data-descripcion", item.descripcion)
                                .attr("data-nombrefinca", item.nombreFinca)
                                .attr("data-codfinca", item.codFinca)
                                .attr("data-idfinca", item.idFinca)
                        );
                    });
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error");
                }
            })
            .catch(error => {
                console.error("Error al obtener la lista de planes:", error);
            });
    } catch (error) {
        console.error("Error en el bloque try:", error);
    }


    tablaVisitas = $('#tbVisitas').DataTable({

        responsive: true,
        "ajax": {
            "url": '/Visita/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idVisita" },
            { "data": "descripcionPlan" },
            { "data": "nombreFinca" },

            // Formatear fechaIni
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

            { "data": "responsable" },
            { "data": "mandador" },
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
                filename: 'Reporte Visitas',
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    //Inicializar tabla Detalles
    var tablaDetalle = $('#tbDetalle').DataTable({
        
        responsive: true, // Hacer que la tabla sea responsive
        searching: false,      // Ocultar el campo de búsqueda
        lengthChange: false,   // Ocultar el campo de selección de entradas*/
        columnDefs: [
            {
                targets: [0, 1],  // Índice de las columnaa "IdReg" y " IdActividad" (la primera y segunda columna)
                visible: false // Hacerlas invisibles
            }
        ]
    });

    // Inicializar el datepicker en los campos de fecha
        

    $('#txtFecha').datepicker({
        format: "dd/mm/yyyy",   // Establecer el formato        
        todayHighlight: true,   // Resaltar la fecha de hoy
        autoclose: true,        // Cerrar automáticamente el calendario al seleccionar
        language: "es",          // Idioma español para los textos  
        
    });
    
    // Evento change para cuando seleccionen un valor en el combo de fincas
    $("#cboPlan").change(function () {
        // Obtener el nombre de la finca desde el atributo 'data-nombre' de la opción seleccionada
        var vidPlan = $("#cboPlan option:selected").attr("data-idplan");
        var vdescripcion = $("#cboPlan option:selected").attr("data-descripcion");
        var vnombreFinca = $("#cboPlan option:selected").attr("data-nombrefinca");
        var vcodFinca = $("#cboPlan option:selected").attr("data-codfinca");
        var vidFinca = $("#cboPlan option:selected").attr("data-idfinca");

        // Colocar el nombre de la finca en el campo de texto deshabilitado
        $("#txtIdPlan").val(vidPlan);
        $("#txtDescripcionplan").val(vdescripcion);
        $("#txtNombreFinca").val(vnombreFinca);
        $("#txtCodFinca").val(vcodFinca);
        $("#txtIdFinca").val(vidFinca);

        // Mostrar el overlay antes de la solicitud AJAX
        $("#tbDetalle").LoadingOverlay("show");
        $("#imgFoto1").LoadingOverlay("show");
        $("#imgFoto2").LoadingOverlay("show");

        // AJAX para obtener las actividades del plan
        $.ajax({
            url: `/PlanesTrabajo/Historial`,  //Ocupo Historial para obtener el detalle del Plan (primer registro de la lista que devuelve historial)
            type: 'GET',
            data: { idPlan: vidPlan },
            success: function (data) {
                if (data && data.length > 0) {
                    const ladata = data[0];  // Usa el primer registro como ejemplo
                    cargardesdePlan(ladata); // Pasa el primer registro a la función cargardesdePlan                    
                } else {
                    Swal.fire("No se encontraron Actividades para el plan seleccionado.");
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error en la solicitud:", error);
            },
            complete: function () {
                // Esto se ejecuta después de que la llamada AJAX haya terminado, sin importar si fue exitosa o no.
                $("#tbDetalle").LoadingOverlay("hide");
                $("#imgFoto1").LoadingOverlay("hide");
                $("#imgFoto2").LoadingOverlay("hide");
            }
        });
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

    $('#tbDetalle').on('click', 'td', function () {
        if (editar) {
            
            var contenidoActual = $(this).text();
            var columnaIndex = $(this).index();
            var fila = $(this).closest('tr');            
            var idActividad = fila.data('id'); // Obtener el ID de la actividad
            var data = tablaDetalle.row(fila).data();
            if (columnaIndex === 4 || columnaIndex === 5) {
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
                    // Actualizar el modelo de datos de la tabla                    
                    data[columnaIndex + 2] = nuevoContenido;    // Se suma 2 x q indice de tbDetalle no toma en cuenta columnas ocultas y
                                                                // estamos accediendo al datatable que si toma en cuenta todas                    
                });
            }

            // Otras columnas: Usar el input normal de texto
            else if (columnaIndex !== 0 && columnaIndex !== 1)
            {
                let input = $('<input type="text" class="form-control form-control-sm">').val(contenidoActual);
                $(this).empty().append(input);
                input.focus();

                input.on('blur', () => {
                    let nuevoContenido = input.val();
                    $(this).text(nuevoContenido);
                    data[columnaIndex+2] = nuevoContenido;                    
                });
                                  
                input.on('keypress', function (e) {
                    if (e.which == 13) {
                        input.blur();
                    }
                });
            }
        }
    });

    //------------- BTN MOSTRAR ------------------------
    $('#tbVisitas tbody').on('click', '.btn-mostrar', function () {

        if ($(this).closest("tr").hasClass("child")) {
            filaseleccionada = $(this).closest("tr").prev();
        }
        else {
            filaseleccionada = $(this).closest("tr");
        }
        editar = false;
        
        modeloCargado = tablaVisitas.row(filaseleccionada).data();
        
        $('#btnGuardar').hide();
        $('#linkImprimir').show();
        $('#btnEnviarCorreo').show();
        $("#linkImprimir").attr("href", `/Visita/MostrarPDFVisita?idVisita=${modelo.idVisita}`)

        // Abrir el modal
        mostrarModal(modeloCargado, true)
        
    })
    // Evento para el botón "Enviar por Correo"
    // Evento para el botón "Enviar por Correo"
    $('#btnEnviarCorreo').on('click', function () {

        const data = tablaVisitas.row(filaseleccionada).data();
        const correoDestino = data.email;  // Obtenemos el email desde la fila seleccionada

        if (correoDestino) {
            $("#modalVisitas .modal-body").LoadingOverlay("show");
            $.ajax({
                url: `/Visita/EnviarVisitaPorCorreo?idVisita=${data.idVisita}&correoDestino=${correoDestino}`,
                type: 'GET', // O POST si prefieres
                success: function (response) {
                    if (response.estado) {
                        Swal.fire("Listo!", "Correo enviado con éxito!", "success");                 
                    } else {
                        Swal.fire("Error", response.mensaje, "error");
                    }
                },
                error: function (error) {
                    Swal.fire("Error", "No se pudo enviar el correo. " + response.mensaje, "error");
                },      
                complete: function () {
                    // Esto se ejecuta después de que la llamada AJAX haya terminado, sin importar si fue exitosa o no.
                    $("#modalVisitas .modal-body").LoadingOverlay("hide");
                }
            });
        }
    });

    //-------------------------- BTN EDITAR-------------------------
    $('#tbVisitas tbody').on('click', '.btn-editar', function () {
        if ($(this).closest("tr").hasClass("child")) {
            filaseleccionada = $(this).closest("tr").prev();
        }
        else {
            filaseleccionada = $(this).closest("tr");
        }
        editar = true;
        modeloCargado = tablaVisitas.row(filaseleccionada).data();

        $('#btnGuardar').text('Guardar Cambios');
        $('#btnGuardar').show();
        $('#linkImprimir').hide();
        $('#btnEnviarCorreo').hide();        
        
        mostrarModal(modeloCargado, false);   // Mostrar el modal
    })

    // Evento para eliminar Visita    
    $('#tbVisitas tbody').on('click', '.btn-eliminar', function () {
        let fila;

        if ($(this).closest("tr").hasClass("child")) {
            fila = $(this).closest("tr").prev();
        } else {
            fila = $(this).closest("tr");
        }

        const data = tablaVisitas.row(fila).data();

        Swal.fire({
            title: "¿Seguro de continuar?",
            text: `Eliminar Visita "${data.idVisita} del plan ${data.descripcionPlan.length > 40 ? data.descripcionPlan.substring(0, 40) + '...' : data.descripcionPlan}" en la finca: "${data.nombreFinca}" `,
            icon: "warning", // Cambiado de 'type' a 'icon'            
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "No, cancelar",
            customClass: {
                confirmButton: 'btn btn-danger', // Clases CSS personalizadas
                cancelButton: 'btn btn-primary',
            },
            reverseButtons: true // Cambia el orden de los botones
        }).then((result) => {
            if (result.isConfirmed) { // Si el usuario confirma la acción
                $(".showSweetAlert").LoadingOverlay("show");
                fetch(`/Visita/Eliminar?IdVisita=${data.idVisita}`, {
                    method: "DELETE",
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaVisitas.row(fila).remove().draw(false);
                            Swal.fire("Listo!", "Visita " + data.idVisita + " en la finca: " + data.nombreFinca + " y su detalle fue eliminada!", "success")
                        } else {
                            Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
                    .catch(error => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        Swal.fire("¡Error!", "Hubo un problema al eliminar la visita. "+error , "error");
                    });
            }
        })
    });
});


//-------------------------- BTN CREAR NUEVA VISITA-------------------------
$('#btnNuevo').on('click', function () {
    // inicializar modelo
    editar = true;
    $('#cboPlan').val('');
    /// Se crea el modelo según el contenido el modal, en esta parte es x q es para agregar
    modeloCargado = structuredClone(modelo);
    
    $('#btnGuardar').text('Guardar Nueva Visita');    
    $('#btnGuardar').show();    
    $('#linkImprimir').hide();
    $('#btnEnviarCorreo').hide();
    

    // Abrir el modal
    mostrarModal(modeloCargado, false)    
});


//-------------------------- BTN GUARDAR NUEVA VISITA Y GUARDAR CAMBIOS------------------------
$('#btnGuardar').on('click', function () {
    // Validar los campos de la Visita
    const inputsVisita = $("#form-visita input.input-validar").serializeArray();
    const inputs_sin_valor = inputsVisita.filter((item) => item.value.trim() == "");

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo : "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus();
        return;
    }
    
    // Cargar Fotos y FormData
    const inputFoto1 = document.getElementById("txtFoto1");
    const inputFoto2 = document.getElementById("txtFoto2");
    const lafoto1 = inputFoto1.files[0] ? inputFoto1.files[0].name : null;
    const lafoto2 = inputFoto2.files[0] ? inputFoto2.files[0].name : null;    
    const sento = modeloCargado.sentTo ? modeloCargado.sentTo : 0;
    const aplica = modeloCargado.aplicado ? modeloCargado.aplicado : false;
    const sincroniza = modeloCargado.sincronizado ? modeloCargado.sincronizado : false;
    
    // Crear el objeto modelovisita a partir del formulario
    const modelovisita = {
        idVisita:   parseInt($("#txtIdVisita").val(), 10),
        idPlan:     parseInt($("#txtIdPlan").val(), 10),
        idFinca:    parseInt($("#txtIdFinca").val(), 10),
        fecha:      convertirFecha($('#txtFecha').val()),
        responsable:$('#txtResponsable').val(),
        mandador:   $('#txtMandador').val(),        
        zafra:      parseInt($("#txtZafra").val(), 10),
        latitud:    $('#txtLatitud').val(),
        longitud:   $('#txtLongitud').val(),   
        observaciones: $('#txtObservaciones').val(),                        
        nombreFoto1: lafoto1,
        nombreFoto2: lafoto2,
        sentTo: sento,     
        aplicado: aplica,
        sincronizado:sincroniza,
        detalleVisita: [] // Aquí agregar el detalle de la visita        
    };

     
    const filas = $('#tbDetalle').DataTable().rows().data();
        
    
    for (let index = 0; index < filas.length; index++) {
        const rowData = filas[index];
        const detalle = {
            idReg: parseInt(rowData[0], 10) ? parseInt(rowData[0], 10) : 0,       // Columna oculta es 0 cuando es nueva
            idActividad: parseInt(rowData[1], 10), // Columna oculta
            fecha: convertirFecha($('#txtFecha').val()),
            avanceanterior: parseFloat(rowData[4]), // Avance anterior
            avances: parseFloat(rowData[5]), // Avance actual
            estadoanterior: rowData[6], // Estado anterior
            estado: rowData[7], // Estado actual
            comentarios: rowData[8], // Comentarios
            observaciones: rowData[9], // Observaciones
            idFinca: parseInt($("#txtIdFinca").val(), 10),
        };
        modelovisita.detalleVisita.push(detalle);
    }

    console.log(modelovisita)    
    const formData = new FormData();    // Agrega las fotos y modelo de data para mandar a Registrar
    
    formData.append("foto1", inputFoto1.files[0] ? inputFoto1.files[0] : null);
    formData.append("foto2", inputFoto2.files[0] ? inputFoto2.files[0] : null);
    formData.append("modelovisita", JSON.stringify(modelovisita));

    // Mostrar overlay
    $("#modalVisitas .modal-body").LoadingOverlay("show");

    if (modelovisita.idVisita == 0) {

        // Enviar datos al servidor para registrar nueva visita
        fetch("/Visita/Registrar", {
            method: "POST",
            body: formData // No establecer 'Content-Type', FormData lo maneja
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire("Listo!", `Registro de Visita ${responseJson.objeto.idVisita} creado con éxito!`, "success");
                    tablaVisitas.row.add(responseJson.objeto).draw(false)  //Revisar esto
                    limpiarFormularioYTabla();
                    $("#modalVisitas").modal("hide")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
                }
            })
            .catch(error => {
                console.log(error)
                Swal.fire("¡Error!", "Hubo un problema al tratar de agregar la visita.", "error");
            })
            .finally(() => {
                $("#modalVisitas .modal-body").LoadingOverlay("hide");
            });
    } else {
        // Enviar datos al servidor para editar una visita existente
        fetch("/Visita/Editar", {
            method: "PUT",
            body: formData // No establecer 'Content-Type', FormData lo maneja
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire("Listo!", `Registro de Visita ${responseJson.objeto.idVisita} editado con éxito!`, "success");
                    tablaVisitas.row(filaseleccionada).data(responseJson.objeto).draw(false);                    
                    limpiarFormularioYTabla();
                    $("#modalVisitas").modal("hide")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
                }
            })
            .catch(error => {
                console.log(error)
                Swal.fire("¡Error!", "Hubo un problema al tratar de modificar la visita.", "error");
            })
            .finally(() => {
                $("#modalVisitas .modal-body").LoadingOverlay("hide");
            });
    }
});


//-------------------------- FUNCION MOSTRAR MODAL VISITAS-------------------------
function mostrarModal(modelo, edita) {

    $("#txtFecha").prop('disabled', edita);
    $("#txtResponsable").prop('disabled', edita);
    $("#txtMandador").prop('disabled', edita);
    $("#txtZafra").prop('disabled', edita);
    $("#txtLatitud").prop('disabled', edita);
    $("#txtLongitud").prop('disabled', edita);
    $("#txtObservaciones").prop('disabled', edita);
    $("#txtFoto1").prop('disabled', edita);
    $("#txtFoto2").prop('disabled', edita);
    
    $("#txtFecha").val(modelo.fecha != "" ? formatearFecha(modelo.fecha) : "");
    $("#txtResponsable").val(modelo.responsable)
    $("#txtMandador").val(modelo.mandador)
    $("#txtZafra").val(modelo.zafra)
    $("#txtLatitud").val(modelo.latitud)
    $("#txtLongitud").val(modelo.longitud)
    $("#txtObservaciones").val(modelo.observaciones)    
    $("#txtNombreFinca").val(modelo.nombreFinca)
    $("#txtCodFinca").val(modelo.codFinca)
    $("#txtDescripcionplan").val(modelo.descripcionPlan)
    $("#txtIdPlan").val(modelo.idPlan)
    $("#cboPlan").val(modelo.idPlan)
    $("#txtIdFinca").val(modelo.idFinca)
    $("#txtIdVisita").val(modelo.idVisita ? modelo.idVisita :"0");
    
    //$("#txtFoto1").val(modelo.nombreFoto1)
    //$("#txtFoto2").val(modelo.nombreFoto2)

    cargarDetalle(modelo)   // Carga las lineas de la tabla de tbDetalles
    
    // Referencias a los elementos img
    const imgElement1 = document.getElementById("imgFoto1");
    const imgElement2 = document.getElementById("imgFoto2");

    // Validar y asignar imágenes para imgFoto1 e imgFoto2
    if (modelo.nombreFoto1 && modelo.urlFoto1) {
        imgElement1.src = modelo.urlFoto1; // Cargar la imagen desde la URL
        imgElement1.style.opacity = 0.9;
    } else {
        imgElement1.src = "/images/eog-image-photo-svgrepo-com.svg";
        imgElement1.style.opacity = 0.25;
    }    
    if (modelo.nombreFoto2 && modelo.urlFoto2) {
        imgElement2.src = modelo.urlFoto2; 
        imgElement2.style.opacity = 0.9; // Opacidad predeterminada
    } else {
        imgElement2.src = "/images/eog-image-photo-svgrepo-com.svg";
        imgElement2.style.opacity = 0.25; // Opacidad predeterminada
    }

    $("#modalVisitas").modal("show")

} 

//-------------------------- FUNCION LIMPIAR MODAL -------------------------
function limpiarFormularioYTabla() {
    // Limpiar los campos del formulario Visita
    $('#txtIdPlan').val('');
    $('#txtIdFinca').val('');
    $('#txtFecha').val('');
    $('#txtResponsable').val('');
    $('#txtObservaciones').val('');
    $('#txtMandador').val('');
    $('#cboPlan').val(''); // Si tienes un valor por defecto
    $('#txtNombreFinca').val('');
    $('#txtCodFinca').val('');
    $('#txtDescripcionPlan').val('');
    $('#txtZafra').val('');
    $('#txtLongitud').val('');
    $('#txtLatitud').val('');
    $('#txtFoto1').val('');
    $('#txtFoto2').val('');

    
    const tabla = $('#tbDetalle').DataTable();  // Limpiar la tabla de detalles Asumiendo que usas DataTables
    tabla.clear().draw();                      // Eliminar todas las filas de la tabla

                                            // Si no usas DataTables, puedes limpiar la tabla manualmente así:
                                            // $('#tbActividad tbody').empty();
}


function cargarDetalle(modelo) {
    // Limpiar la tabla antes de agregar nuevas actividades
    $('#tbDetalle').DataTable().clear().draw();

    // Recorrer las actividades en el modelo y agregarlas a la tabla
    modelo.detalleVisita.forEach(function (detalle) {
        $('#tbDetalle').DataTable().row.add([
            detalle.idReg,
            detalle.idActividad,
            detalle.descripcionActividad,
            detalle.tipo,
            detalle.avanceanterior,
            detalle.avances,
            detalle.estadoanterior,
            detalle.estado,            
            detalle.comentarios,
            detalle.observaciones,
        ]).draw(false);
    });
}
function cargardesdePlan(modelo) {
    // Limpiar la tabla antes de agregar nuevas actividades
    $('#tbDetalle').DataTable().clear().draw();

    // Recorrer las actividades en el modelo y agregarlas a la tabla
    modelo.actividades.forEach(function (actividad) {
        if (actividad.estado != "FINALIZADO") {
            $('#tbDetalle').DataTable().row.add([        
                0,                            // Este será el IdReg de detalleVisita, 
                actividad.idActividad,
                actividad.descripcion,
                actividad.tipo,
                actividad.avances,
                "",                
                actividad.estado,
                "",                
                "",
                "",
            ]).draw(false);
        }     
    });
    if ($('#tbDetalle').DataTable().rows().count() === 0) {
        Swal.fire({
            title: "El Plan no tiene Actividades con estatus en proceso, revise en el modulo de planes.",
            showClass: { popup: ` animate__animated animate__fadeInUp animate__faster ` },
            hideClass: { popup: ` animate__animated animate__fadeOutDown animate__faster ` },
            confirmButtonColor: "#3085d6",
        });        
    }
}
