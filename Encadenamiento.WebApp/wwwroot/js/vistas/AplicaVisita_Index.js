
let filaseleccionada;
let editar;
let modeloCargado

$(document).ready(function () {


    tablaVisitas = $('#tbVisitas').DataTable({

        responsive: true,
        "ajax": {
            "url": '/Sincronizacion/ListaVisita',
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
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-1"><i class="fas fa-exchange-alt"></i> <i class="fas fa-home-alt"></i></button>' ,
                    
                "orderable": false,
                "searchable": false,
                "width": "20px"
            }

        ],
        order: [[0, "desc"]],
        dom: "frtip",
       
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    
    //-------------------------- BTN APLICAR -------------------------
    $('#tbVisitas tbody').on('click', '.btn-editar', function () {
        if ($(this).closest("tr").hasClass("child")) {
            filaseleccionada = $(this).closest("tr").prev();
        }
        else {
            filaseleccionada = $(this).closest("tr");
        }
        editar = true;
        modeloCargado = tablaVisitas.row(filaseleccionada).data();
        const formData = new FormData();    // Agrega el modelo de data para mandar a Aplicar

        formData.append("modelovisita", JSON.stringify(modeloCargado));

        $("#tbVisitas").LoadingOverlay("show");

        fetch("/Sincronizacion/AplicarVisita", {
            method: "PUT",
            body: formData // No establecer 'Content-Type', FormData lo maneja
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire("Listo!", `Visita Aplicada con éxito a Plan de Trabajo ${responseJson.objeto.idPlan} ${responseJson.objeto.descripcionPlan}
                                         para la finca ${responseJson.objeto.codFinca} ${responseJson.objeto.nombreFinca}.`, "success");
                    tablaVisitas.row(filaseleccionada).remove().draw(false);                      
                } else {
                    Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
                }
            })
            .catch(error => {
                console.log(error)
                Swal.fire("¡Error!", "Hubo un problema al tratar de aplicar la visita.", "error");
            })
            .finally(() => {
                $("#tbVisitas").LoadingOverlay("hide");
            });
    })
        
});


//-------------------------- BTN CREAR NUEVA VISITA-------------------------


//-------------------------- FUNCION MOSTRAR MODAL VISITAS-------------------------

