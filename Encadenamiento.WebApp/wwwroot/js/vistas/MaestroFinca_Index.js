
const MODELO_BASE = {
    idFinca: 0,
    codFinca: 0,
    descripcion: "",
    proveedor: "",
    email: "",
    area: 0.0,
    telefono: "",
    grupo:0,
}

let tablaData;
$(document).ready(function () {
    
    tablaData = $('#tbdatafinca').DataTable({

        responsive: true,
        "ajax": {
            "url": '/MaestroFinca/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idFinca", "visible": false, "searchable": false },
            { "data": "codFinca" },
            { "data": "descripcion" },
            { "data": "proveedor" },
            { "data": "email" },
            { "data": "area" },
            { "data": "telefono" },
            { "data": "grupo" },
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
                filename: 'Reporte Fincas',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})
function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idFinca)
    $("#txtCodFinca").val(modelo.codFinca)
    $("#txtDescripcion").val(modelo.descripcion)
    $("#txtProveedor").val(modelo.proveedor)
    $("#txtEmail").val(modelo.email)
    $("#txtArea").val(modelo.area)
    $("#txtTelefono").val(modelo.telefono)    
    $("#cboGrupo").val(modelo.grupo)    

    $("#modalFinca").modal("show")
}
$("#btnNuevo").click(function () {
    mostrarModal()
})

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")
    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo : "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;

    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idFinca"] = parseInt($("#txtId").val())
    modelo["codFinca"] = parseInt($("#txtCodFinca").val())
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["proveedor"] = $("#txtProveedor").val()
    modelo["email"] = $("#txtEmail").val()
    modelo["area"] = parseFloat($("#txtArea").val())
    modelo["telefono"] = $("#txtTelefono").val()
    modelo["grupo"] = parseInt($("#cboGrupo").val())
       
    const formData = new FormData();    
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalFinca").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idFinca == 0) {
        fetch("/MaestroFinca/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalFinca").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalFinca").modal("hide")
                    Swal.fire( "Listo!", "Finca creada con éxito!", "success")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/MaestroFinca/Editar", {
            method: "PUT",
            body: formData
        })
            .then(response => {
                $("#modalFinca").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null
                    $("#modalFinca").modal("hide")
                    Swal.fire("Listo!", "Finca modificada con éxito!", "success")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
})

let filaSeleccionada;
$("#tbdatafinca tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    }
    else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
})
$("#tbdatafinca tbody").on("click", ".btn-eliminar", function () {
    let fila;

    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    }
    else {
        fila = $(this).closest("tr");
    }
    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "Seguro de continuar?",
        text: `Eliminar Finca "${data.codFinca} ${data.descripcion}"`,
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
            fetch(`/MaestroFinca/Eliminar?IdFinca=${data.idFinca}`, {
                    method: "DELETE",

            })
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw(false);

                            Swal.fire("Listo!", "Finca fue eliminada!", "success")
                        } else {
                            Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                        }
                })
                .catch(error => {
                     $(".showSweetAlert").LoadingOverlay("hide");
                     Swal.fire("¡Error!", "Hubo un problema al eliminar la Finca.", "error");
                });
        }
    })
})
