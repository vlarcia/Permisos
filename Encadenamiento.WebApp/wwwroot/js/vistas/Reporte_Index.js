$(document).ready(function () {
    $(".card-body").LoadingOverlay("show");

    fetch("/Revision/Lista")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            $(".card-body").LoadingOverlay("hide");
            if (responseJson.data.length > 0) {
                responseJson.data.sort((a, b) => {
                    if (a.codFinca > b.codFinca) return -1;
                    if (a.codFinca < b.codFinca) return 1;
                    return 0;
                })
                responseJson.data.forEach((item) => {
                    $("#cboRevision").append(
                        $("<option>")
                            .val(item.codFinca)
                            .text(item.codFinca+" - " + formatearFecha(item.fecha) + " - finca: " + item.nombreFinca)
                            .attr("data-fecha", item.fecha)
                            .attr("data-nombrefinca", item.nombreFinca)
                            .attr("data-codfinca", item.codFinca)
                            .attr("data-idfinca", item.idFinca)
                    );
                });
                $("#cboRevision").val('');
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error");
            }
        })
        .catch(error => console.error("Error al obtener la lista de revisiones:", error));

    $("#txtFechaIni").datepicker({ dateFormat: "dd/mm/yy" });
    $("#cboRevision").val('');

    $("#cboRevision").change(function () {
        var vFecha = formatearFecha($("#cboRevision option:selected").attr("data-fecha"));
        var vnombreFinca = $("#cboRevision option:selected").attr("data-nombrefinca");
        var vcodFinca = $("#cboRevision option:selected").attr("data-codfinca");
        var vidFinca = $("#cboRevision option:selected").attr("data-idfinca");

        $("#txtFecha").val(vFecha);
        $("#txtNombreFinca").val(vnombreFinca);
        $("#txtCodFinca").val(vcodFinca);
        $("#txtIdFinca").val(vidFinca);

        cargarDatos(vidFinca, vFecha);
    });
});

function cargarDatos(idFinca, fecha) {
    $(".card-body").LoadingOverlay("show");

    fetch(`/Revision/ObtenerCumplimientoGeneral?idFinca=${idFinca}&fecha=${fecha}`)
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            if (responseJson.estado) {
                // Datos de los porcentajes
                const cantCumple = responseJson.estados.cumple;
                const cantParcial = responseJson.estados.parcial;
                const cantNoCumple = responseJson.estados.noCumple;
                const cantNoAplica = responseJson.estados.noAplica;
                const porcentLaboral = responseJson.estados.porcLaboral;
                const porcentOcupacional = responseJson.estados.porcOcupacional;
                const porcentAmbiental = responseJson.estados.porcAmbiental;
                const porcentRse = responseJson.estados.porcRse;
                // Gráfica
                const ctx = document.getElementById('graficoCumplimiento1').getContext('2d');

                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: ['CUMPLE', 'CUMPLE PARCIAL', 'NO CUMPLE', 'NO APLICA'],
                        datasets: [{
                            label: 'Estado de Requerimientos',
                            data: [cantCumple, cantParcial, cantNoCumple, cantNoAplica],
                            backgroundColor: ['#4CAF50', '#FFC107', '#F44336', '#9E9E9E'] // Agregar un color adicional para "NO APLICA"
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 80,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 11
                                },
                                scaleLabel: {  // Aquí va scaleLabel en lugar de title
                                    display: true,
                                    labelString: 'Cantidad de Requerimientos'  // Cambia text a labelString
                                },
                                padding: 10  // Ajusta este valor para reducir el espacio
                            }],
                        },
                    }
                });

                const ctx2 = document.getElementById('graficoCumplimiento2').getContext('2d');
                new Chart(ctx2, {
                    type: 'bar',
                    data: {
                        labels: ['LABORAL', 'OCUPACIONAL', 'AMBIENTAL', 'RSE'],
                        datasets: [{
                            label: 'Cumplimiento por Ámbito',
                            data: [porcentLaboral, porcentOcupacional, porcentAmbiental, porcentRse],
                            backgroundColor: ['#4CAF50', '#FFC107', '#F44336', '#9E9E9E'] // Agregar un color adicional para "NO APLICA"
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 80,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 10
                                },
                                scaleLabel: {  // Aquí va scaleLabel en lugar de title
                                    display: true,
                                    labelString: 'Porcentaje (%)'  // Cambia text a labelString
                                }
                            }],
                        },
                    }
                });
            
                // Llenar la tabla con los datos de cumplimiento
                $("#txtCumplePorc").val(responseJson.estados.porcCumple.toFixed(2));
                $("#txtParcialPorc").val(responseJson.estados.porcParcial.toFixed(2));
                $("#txtNoCumplePorc").val(responseJson.estados.porcNoCumple.toFixed(2));
                $("#txtNoAplicaPorc").val(responseJson.estados.porcNoAplica.toFixed(2));
                $("#txtCumplimiento1").val(responseJson.cumplimientoGeneral.toFixed(2));
                $("#txtLaboralPorc").val(responseJson.estados.porcLaboral.toFixed(2));
                $("#txtOcupacionalPorc").val(responseJson.estados.porcOcupacional.toFixed(2));
                $("#txtAmbientalPorc").val(responseJson.estados.porcAmbiental.toFixed(2));
                $("#txtRsePorc").val(responseJson.estados.porcRse.toFixed(2));
                $("#txtCumplimiento2").val(responseJson.cumplimientoGeneral.toFixed(2));

            } else {
                Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
            }
        })
        .catch(error => {
            console.error("Error al obtener el cumplimiento general:", error);
            Swal.fire("¡Error!", "Hubo un problema al tratar de obtener los datos.", "error");
        })
        .finally(() => $(".card-body").LoadingOverlay("hide"));
}

function formatearFecha(fechaISO) {
    const fecha = new Date(fechaISO);
    const dia = String(fecha.getDate()).padStart(2, '0');
    const mes = String(fecha.getMonth() + 1).padStart(2, '0');
    const anio = fecha.getFullYear();
    return `${dia}/${mes}/${anio}`;
}
