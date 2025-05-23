
const MODELO_BASE = {
    idRequisito: 0,    
    descripcion: "",
    ambito: "",
    norma: "",
    bonsucro: "",
    observaciones: ""
}

let tablaData;
$(document).ready(function () {  
    
    tablaData = $('#tbdata').DataTable({

        responsive: true,
        "ajax": {
            "url": '/CheckList/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idRequisito"},            
            { "data": "descripcion" },
            { "data": "ambito" },
            { "data": "norma" },
            { "data": "bonsucro" },
            { "data": "observaciones" },
           
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0,"asc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Requisitos',
                exportOptions: {
                    columns: [0,1,2, 3, 4, 5]
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
    $("#txtDescripcion").val(modelo.descripcion)
    $("#txtAmbito").val(modelo.ambito)
    $("#txtNorma").val(modelo.norma)
    $("#txtBonsucro").val(modelo.bonsucro)
    $("#txtObservaciones").val(modelo.observaciones)
    

    $("#modalData").modal("show")
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
    modelo["idRequisito"] = parseInt($("#txtId").val())
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["ambito"] = $("#txtAmbito").val()
    modelo["norma"] = $("#txtNorma").val()
    modelo["bonsucro"] = $("#txtBonsucro").val()
    modelo["observaciones"] = $("#txtObservaciones").val()
    
    
       
    const formData = new FormData();    
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idRequisito == 0) {
        fetch("/CheckList/Crear", {
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
                    Swal.fire("Listo!", "Requisito creado con éxito!", "success")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/CheckList/Editar", {
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
                    Swal.fire("Listo!", "Requisito modificado con éxito!", "success")
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
    }
    else {
        fila = $(this).closest("tr");
    }
    const data = tablaData.row(fila).data();
    Swal.fire({
        title: "Seguro de continuar?",
        text: `Eliminar Requisito "${data.idRequisito} ${data.descripcion.length > 60 ? data.descripcion.substring(0, 60) + '...' : data.descripcion}"`,        
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
           fetch(`/CheckList/Eliminar?IdRequisito=${data.idRequisito}`, {
                method: "POST",
           })
           .then(response => {
               $(".showSweetAlert").LoadingOverlay("hide");
               return response.ok ? response.json() : Promise.reject(response);
           })
           .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw(false);
                    Swal.fire("Listo!", "Requisito eliminado!", "success")
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                }
           })
           .catch(error => {
               $(".showSweetAlert").LoadingOverlay("hide");
               Swal.fire("¡Error!", "Hubo un problema al eliminar el requisito.", "error");
           });        
        }
    })
})
