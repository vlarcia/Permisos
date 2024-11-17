let filaseleccionada;
let editar;
let tablaAndroid
$(document).ready(function () {
    
    tablaAndroid = $('#tbAndroid').DataTable({

        responsive: true,
        "ajax": {
            "url": '/Sincronizacion/ListaAndroid',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idAndroid" },
            { "data": "androidid1" },
            { "data": "dispositivo" },
            { "data": "responsable" },
            { "data": "email" },
            
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
                filename: 'Reporte Dispositivos',
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    //------------- BTN MOSTRAR ------------------------
    $('#tbAndroid tbody').on('click', '.btn-mostrar', function () {

        if ($(this).closest("tr").hasClass("child")) {
            filaseleccionada = $(this).closest("tr").prev();
        }
        else {
            filaseleccionada = $(this).closest("tr");
        }
        editar = false;
        
        modeloCargado = tablaAndroid.row(filaseleccionada).data();
        
        $('#btnGuardar').hide();
        
        // Abrir el modal
        mostrarModal(modeloCargado, true)
        
    })
    
    //-------------------------- BTN EDITAR-------------------------
    $('#tbAndroid tbody').on('click', '.btn-editar', function () {
        if ($(this).closest("tr").hasClass("child")) {
            filaseleccionada = $(this).closest("tr").prev();
        }
        else {
            filaseleccionada = $(this).closest("tr");
        }
        editar = true;
        let modeloCargado = tablaAndroid.row(filaseleccionada).data();

        $('#btnGuardar').text('Guardar Cambios');
        $('#btnGuardar').show();
            
        mostrarModal(modeloCargado, false);   // Mostrar el modal
    })

    // Evento para eliminar Visita    
    $('#tbAndroid tbody').on('click', '.btn-eliminar', function () {
        let fila;

        if ($(this).closest("tr").hasClass("child")) {
            fila = $(this).closest("tr").prev();
        } else {
            fila = $(this).closest("tr");
        }

        const data = tablaAndroid.row(fila).data();

        Swal.fire({
            title: "¿Seguro de continuar?",
            text: `Eliminar Dispositivo Android `,
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
                fetch(`/Sincronizacion/Eliminar?IdAndroid=${data.idAndroid}`, {
                    method: "DELETE",
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaAndroid.row(fila).remove().draw(false);
                            Swal.fire("Listo!", "Dispositivo " + data.idAndroid + " fue eliminado!", "success")
                        } else {
                            Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
                    .catch(error => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        Swal.fire("¡Error!", "Hubo un problema al tatar de eliminar el dispositivo. "+error , "error");
                    });
            }
        })
    });
});


//-------------------------- BTN CREAR NUEVA VISITA-------------------------
$('#btnNuevo').on('click', function () {
    // inicializar modelo
    editar = true;
    
    /// Se crea el modelo según el contenido el modal, en esta parte es x q es para agregar
    let modeloCargado = {
        idAndroid   : 0,
        androidid1  : "",
        dispositivo : "",
        responsable : "",
        email       : "",
    }
        
    $('#btnGuardar').text('Guardar Nuevo Dispositivo');    
    $('#btnGuardar').show();    
    
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
    
    const modeloAndroid = {
        idAndroid: parseInt($("#txtIdAndroid").val(), 10),
        androidid1: $("#txtAndroidId").val(),
        dispositivo: $("#txtDispositivo").val(),
        responsable: $("#txtResponsable").val(),
        email: $("#txtEmail").val(),
    }

    const formData = new FormData();    // Agrega las fotos y modelo de data para mandar a Registrar

    formData.append("modeloAndroid", JSON.stringify(modeloAndroid));

    // Mostrar overlay
    $("#modalAndroid .modal-body").LoadingOverlay("show");

    if (modeloAndroid.idAndroid == 0) {

        // Enviar datos al servidor para registrar nueva visita
        fetch("/Sincronizacion/Crear", {
            method: "POST",
            body: formData // No establecer 'Content-Type', FormData lo maneja
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire("Listo!", `Registro de Dispositivo Android ${responseJson.objeto.idAndroid} ${responseJson.objeto.dispositivo} creado con éxito!`, "success");
                    tablaAndroid.row.add(responseJson.objeto).draw(false)  //Revisar esto
                    limpiarFormulario();
                    $("#modalAndroid").modal("hide")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
                }
            })
            .catch(error => {
                console.log(error)
                Swal.fire("¡Error!", "Hubo un problema al tratar de agregar el dispositivo.", "error");
            })
            .finally(() => {
                $("#modalVisitas .modal-body").LoadingOverlay("hide");
            });
    } else {
        // Enviar datos al servidor para editar una visita existente
        fetch("/Sincronizacion/Editar", {
            method: "PUT",
            body: formData // No establecer 'Content-Type', FormData lo maneja
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire("Listo!", `Registro de Dispositivo Android ${responseJson.objeto.idAndroid} ${responseJson.objeto.dispositivo} editado con éxito!`, "success");
                    tablaAndroid.row(filaseleccionada).data(responseJson.objeto).draw(false);                    
                    limpiarFormulario();
                    $("#modalAndroid").modal("hide")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
                }
            })
            .catch(error => {
                console.log(error)
                Swal.fire("¡Error!", "Hubo un problema al tratar de modificar el dispositivo Android.", "error");
            })
            .finally(() => {
                $("#modalAndroid .modal-body").LoadingOverlay("hide");
            });
    }
});

//-------------------------- FUNCION MOSTRAR MODAL VISITAS-------------------------
function mostrarModal(modelo, edita) {

    $("#txtAndroidId").prop('disabled', edita);   
    $("#txtDispositivo").prop('disabled', edita);
    $("#txtResponsable").prop('disabled', edita);
    $("#txtEmail").prop('disabled', edita);
       

    $("#txtIdAndroid").val(modelo.idAndroid);
    $("#txtAndroidId").val(modelo.androidid1);
    $("#txtDispositivo").val(modelo.dispositivo)
    $("#txtResponsable").val(modelo.responsable)
    $("#txtEmail").val(modelo.email)

    $("#modalAndroid").modal("show")

} 

//-------------------------- FUNCION LIMPIAR MODAL -------------------------
function limpiarFormulario() {
    // Limpiar los campos del formulario Visita

    $('#txtIdAndroid').val('');
    $('#txtAndroidId').val('');    
    $('#txtAndroidId').val('');
    $('#txtResponsable').val('');
    $('#txtEmail').val('');    
}


