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
                // Extraer Cumplimiento General
                const cumplimientoGeneral = responseJson.cumplimientoGeneral.toFixed(2);

                // Datos para la primera gráfica y tabla
                const cantCumple = responseJson.estados.cumple;
                const cantParcial = responseJson.estados.parcial;
                const cantNoCumple = responseJson.estados.noCumple;
                const cantNoAplica = responseJson.estados.noAplica;

                const porcCumple = responseJson.estados.porcCumple.toFixed(2);
                const porcParcial = responseJson.estados.porcParcial.toFixed(2);
                const porcNoCumple = responseJson.estados.porcNoCumple.toFixed(2);
                const porcNoAplica = responseJson.estados.porcNoAplica.toFixed(2);

                const porcentLaboral = responseJson.estados.porcLaboral.toFixed(2);
                const porcentOcupacional = responseJson.estados.porcOcupacional.toFixed(2);
                const porcentAmbiental = responseJson.estados.porcAmbiental.toFixed(2);
                const porcentRse = responseJson.estados.porcRse.toFixed(2);

                // Gráfica de Estado de Requerimientos
                const ctx = document.getElementById('graficoCumplimiento1').getContext('2d');
                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: ['CUMPLE', 'CUMPLE PARCIAL', 'NO CUMPLE', 'NO APLICA'],
                        datasets: [{
                            label: 'Estado de Requerimientos',
                            data: [cantCumple, cantParcial, cantNoCumple, cantNoAplica],
                            backgroundColor: ['#4CAF50', '#FFC107', '#F44336', '#9E9E9E']
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            xAxes: [{ gridLines: { display: false, drawBorder: false }, maxBarThickness: 80 }],
                            yAxes: [{
                                ticks: { min: 0, maxTicksLimit: 11 },
                                scaleLabel: { display: true, labelString: 'Cantidad de Requerimientos' },
                                padding: 10
                            }],
                        },
                    }
                });

                // Gráfica de Cumplimiento por Ámbito
                const ctx2 = document.getElementById('graficoCumplimiento2').getContext('2d');
                new Chart(ctx2, {
                    type: 'bar',
                    data: {
                        labels: ['LABORAL', 'OCUPACIONAL', 'AMBIENTAL', 'RSE'],
                        datasets: [{
                            label: 'Cumplimiento por Ámbito',
                            data: [porcentLaboral, porcentOcupacional, porcentAmbiental, porcentRse],
                            backgroundColor: ['#4CAF50', '#FFC107', '#F44336', '#9E9E9E']
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            xAxes: [{ gridLines: { display: false, drawBorder: false }, maxBarThickness: 80 }],
                            yAxes: [{
                                ticks: { min: 0, maxTicksLimit: 10 },
                                scaleLabel: { display: true, labelString: 'Porcentaje (%)' }
                            }],
                        },
                    }
                });

                // Construir la tabla Cumplimiento
                const tablaCumplimiento = `
                    <table class="table">
                        <thead>
                            <tr>
                                <th style="text-align:center;">Estado</th>
                                <th style="text-align:center;">Porcentaje</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr style="font-weight:bold;">
                                <td>Cumplimiento Global</td>
                                <td style="color:red; text-align:right;">${cumplimientoGeneral}%</td>
                            </tr>
                            <tr><td>CUMPLE</td><td style="text-align:right;">${porcCumple}%</td></tr>
                            <tr><td>CUMPLE PARCIAL</td><td style="text-align:right;">${porcParcial}%</td></tr>
                            <tr><td>NO CUMPLE</td><td style="text-align:right;">${porcNoCumple}%</td></tr>
                            <tr><td>NO APLICA</td><td style="text-align:right;">${porcNoAplica}%</td></tr>
                        </tbody>
                    </table>`;

                document.getElementById("tablaCumplimiento").innerHTML = tablaCumplimiento;

                // Construir la tabla Tipo
                const tablaTipo = `
                    <table class="table">
                        <thead>
                            <tr>
                                <th style="text-align:center;">Tipo</th>
                                <th style="text-align:center;">Porcentaje</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr style="font-weight:bold;">
                                <td>Cumplimiento Global</td>
                                <td style="color:red; text-align:right;">${cumplimientoGeneral}%</td>
                            </tr>
                            <tr><td>LABORAL</td><td style="text-align:right;">${porcentLaboral}%</td></tr>
                            <tr><td>OCUPACIONAL</td><td style="text-align:right;">${porcentOcupacional}%</td></tr>
                            <tr><td>AMBIENTAL</td><td style="text-align:right;">${porcentAmbiental}%</td></tr>
                            <tr><td>RSE</td><td style="text-align:right;">${porcentRse}%</td></tr>
                        </tbody>
                    </table>`;

                document.getElementById("tablaTipo").innerHTML = tablaTipo;

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
