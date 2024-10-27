
$(document).ready(function () {

    $(".container-fluid").LoadingOverlay("show");
    fetch("/Home/ObtenerUsuario")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $(".container-fluid").LoadingOverlay("hide");
            if (responseJson.estado) {
                const d = responseJson.objeto
                $("#txtNombre").val(d.nombre)
                $("#txtCorreo").val(d.correo)
                $("#txTelefono").val(d.telefono)
                $("#txtRol").val(d.nombreRol)                
                $("#imgFoto").attr("src", d.urlFoto)

            }
            else {
                Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
            }
        })

})
$("#btnGuardarCambios").click(function () {
    if ($("#txtCorreo").val().trim() == "") {
        toastr.warning("", "Debe completar el campo Correo")
        $("#txtCorreo").focus()
        return;
    }
    if ($("#txTelefono").val().trim() == "") {
        toastr.warning("", "Debe completar el campo Teléfono")
        $("#txTelefono").focus()
        return;
    }
    
    Swal.fire({
        title: "Desea guardar los cambios?",
        icon: "warning",
        showCancelButton: true,
      
        cancelButtonText: "No",
        confirmButtonText: "Sí, Actualizar",
        reverseButtons: true, 
    }).then((result) => {
        if (result.isConfirmed) { // Si el usuario confirma la acción
            
           $(".showSweetAlert").LoadingOverlay("show");
                 let modelo = {
                 correo: $("#txtCorreo").val().trim(),
                 telefono: $("#txTelefono").val().trim()
                 }

           fetch("/Home/GuardarPerfil", {
                 method: "POST",
                 headers: { "Content-Type": "application/json;charset=utf-8" },
                 body: JSON.stringify(modelo)
           })
           .then(response => {
                 $(".showSweetAlert").LoadingOverlay("hide");
                 return response.ok ? response.json() : Promise.reject(response);
           })
           .then(responseJson => {
               if (responseJson.estado) {                           
                   Swal.fire("Listo!", "Perfil Actualizado!", "success")
               } else {
                      Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                }
           })
           .catch(error => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    Swal.fire("¡Error!", "Hubo un problema al actualizar el perfil", "error");
           });
          }
       })
    })
$("#btnCambiarClave").click(function () {
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")
    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo : "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }
    if ($("#txtClaveNueva").val().trim() != $("#txtConfirmarClave").val().trim()) {
        toastr.warning("", "La nueva contraseña no coincide con la confirmación!")
        $("#txClaveNueva").focus()
        return;
    }

    let modelo = {
        claveActual:$("#txtClaveActual").val().trim(),
        claveNueva: $("#txtClaveNueva").val().trim()
    }

    Swal.fire({
        title: "Desea cambiar la contraseña?",
        icon: "warning",
        showCancelButton: true,

        cancelButtonText: "No",
        confirmButtonText: "Sí, Cambiar",
        reverseButtons: true,
    }).then((result) => {
        if (result.isConfirmed) { // Si el usuario confirma la acción

            $(".showSweetAlert").LoadingOverlay("show");           

            fetch("/Home/CambiarClave", {
                method: "POST",
                headers: { "Content-Type": "application/json;charset=utf-8" },
                body: JSON.stringify(modelo)
            })
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        Swal.fire("Listo!", "Contraseñas Actualizadas!", "success")
                        $("input.input-validar").val("");
                    } else {
                        Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
                    }
                })
                .catch(error => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    Swal.fire("¡Error!", "Hubo un problema al actualizar la contraseña", "error");
                });
        }
    })
})