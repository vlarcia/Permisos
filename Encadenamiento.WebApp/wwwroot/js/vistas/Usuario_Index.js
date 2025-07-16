

const MODELO_BASE = {
    idUsuario: 0,
    nombre: "",
    correo: "",
    telefono: "",
    idRol: 0,
    idArea: 0,
    esActivo: 1,
    urlFoto: ""
}


$(document).ready(function () {

    fetch("/Usuario/ListaRoles")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboRol").append(
                        $("<option>").val(item.idRol).text(item.descripcion)
                    )
                })
            }
        })

    tablaData = $('#tbdata').DataTable({

        responsive: true,
        "ajax": {
            "url": '/Usuario/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idUsuario", "visible": false, "searchable": false },
            {
                "data": "urlFoto", render: function (data) {
                    return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>`
                }
            },
            { "data": "nombre" },
            { "data": "correo" },
            { "data": "telefono" },
            { "data": "nombreRol" },
            { "data": "nombreArea" },

            {
                "data": "esActivo", render: function (data) {
                    if (data == 1)
                        return `<span class="badge badge-info">Activo</span>`;
                    else
                        return `<span class="badge badge-danger">No Activo</span>`;
                }
            },

            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Usuarios',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    fetch("/Area/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data && responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboArea").append(
                        $("<option>").val(item.idArea).text(item.nombre)
                    )
                });
            }
        });

})


function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idUsuario)
    $("#txtNombre").val(modelo.nombre)
    $("#txtCorreo").val(modelo.correo)
    $("#txtTelefono").val(modelo.telefono)
    $("#cboRol").val(modelo.idRol == 0 ? $("#cboRol option:first").val() : modelo.idRol)
    $("#cboArea").val(modelo.idArea == 0 ? $("#cboArea option:first").val() : modelo.idArea);
    $("#cboEstado").val(modelo.esActivo)
    $("#txtFoto").val("")
    $("#imgUsuario").attr("src", modelo.urlFoto)

    $("#modalData").modal("show")
}
$("#btnNuevo").click(function () {
    mostrarModal()
})

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const selects = $("select.input-validar"); // ← obtenemos los selects con clase input-validar

    let campoVacio = null;

    // Validar inputs
    const inputSinValor = inputs.find(item => item.value.trim() === "");
    if (inputSinValor) {
        campoVacio = inputSinValor.name;
    }

    // Validar selects si no hubo inputs vacíos
    if (!campoVacio) {
        selects.each(function () {
            if (!$(this).val() || $(this).val().trim() === "") {
                campoVacio = $(this).attr("name");
                return false; // rompe el .each
            }
        });
    }

    if (campoVacio) {
        const mensaje = `Debe completar el campo: "${campoVacio}"`;
        toastr.warning("", mensaje);

        // Intenta enfocar ya sea input o select
        $(`[name="${campoVacio}"]`).focus();

        return;
    }



    const modelo = structuredClone(MODELO_BASE);
    modelo["idUsuario"] = parseInt($("#txtId").val())
    modelo["nombre"] = $("#txtNombre").val()
    modelo["correo"] = $("#txtCorreo").val()
    modelo["telefono"] = $("#txtTelefono").val()
    modelo["idRol"] = $("#cboRol").val()
    modelo["esActivo"] = $("#cboEstado").val()
    modelo["idArea"] = $("#cboArea").val()


    const inputFoto = document.getElementById("txtFoto")

    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show");
    
    if (modelo.idUsuario == 0) {
        fetch("/Usuario/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    Swal.fire("Listo!","Usuario creado con éxito!","success")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje,"error")
                }
            })
    } else {
        fetch("/Usuario/Editar", {
            method: "POST",
            body: formData
        })
        .then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                filaSeleccionada = null
                $("#modalData").modal("hide")

                Swal.fire("Listo!", "Usuario modificado con éxito!", "success");
                    
            } else {
                Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
            }
        }) 
    }
})


let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    }
    else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();
  
    mostrarModal(data);
})
$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    
    let fila;

    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "¿Seguro de continuar?",
        text: `Eliminar Usuario "${data.nombre}"`,
        icon: "warning", // Cambiado de 'type' a 'icon'
        buttonsStyling: true, // Desactiva el estilo predeterminado de SweetAlert para usar las clases CSS personalizadas
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

            fetch(`/Usuario/Eliminar?IdUsuario=${data.idUsuario}`, {
                method: "POST",
            })
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        tablaData.row(fila).remove().draw(false);

                        Swal.fire("¡Listo!", "Usuario fue eliminado exitosamente.", "success");
                    } else {
                        Swal.fire("¡Lo sentimos!", responseJson.mensaje, "error");
                    }
                })
                .catch(error => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    Swal.fire("¡Error!", "Hubo un problema al eliminar el usuario.", "error");
                });
        }
    });
});
