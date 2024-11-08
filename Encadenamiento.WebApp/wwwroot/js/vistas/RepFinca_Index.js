
$(document).ready(function () {
    $(".card-body").LoadingOverlay("show");
    fetch("/MaestroFinca/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $(".card-body").LoadingOverlay("hide");
            if (responseJson.data.length > 0) {
                responseJson.data.sort((a, b) => {
                    if (a.codFinca > b.codFinca) return -1;
                    if (a.codFinca < b.codFinca) return 1;
                    return 0;
                })
                responseJson.data.forEach((item) => {
                    // Para cada item de la lista, agrega una opción al combo con el formato 'codigo - nombre'
                    $("#cboFinca").append(
                        $("<option>")
                            .val(item.codFinca) // El valor sigue siendo codFinca
                            .text(item.codFinca + " - " + item.descripcion) // Mostrar 'codFinca - descripcion'
                            .attr("data-nombre", item.descripcion) // Guardar el nombre de la finca
                            .attr("data-idfinca", item.idFinca)
                    );
                    $("#cboFinca").val('');
                });
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        });
    $("#cboFinca").val('');

    $("#cboFinca").change(function () {
        
        var vnombreFinca = $("#cboFinca option:selected").attr("data-nombre");        
        var vidFinca = $("#cboFinca option:selected").attr("data-idfinca");
        var vcodfinca = $("#cboFinca option:selected").val();
        
        $("#txtNombreFinca").val(vnombreFinca);     
        $("#txtCodFinca").val(vcodfinca);
        $("#txtIdFinca").val(vidFinca);

        cargarDatos(vidFinca);
    });
});

function cargarDatos(idFinca) {
    $(".card-body").LoadingOverlay("show");

    fetch(`/Revision/ObtenerCumplimientoxFinca?idFinca=${idFinca}`)
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            if (responseJson.estado) {
                const seriesDeCumplimiento = responseJson.seriesDeCumplimiento;
                const fechas = [];
                const datasetsCumplimiento = []; // Valores absolutos para la gráfica
                const datasetsTipo = [];
                const datasetsCumplimientoPorcentajes = []; // Porcentajes para la tabla
                const cumplimientoGeneralValues = []; // Array para Cumplimiento General

                seriesDeCumplimiento.forEach((serie, index) => {
                    fechas.push(serie.fecha);

                    // Valores absolutos para la gráfica de cumplimiento
                    datasetsCumplimiento.push([
                        serie.cumple,
                        serie.parcial,
                        serie.noCumple,
                        serie.noAplica
                    ]);

                    // Porcentajes para la tabla de cumplimiento
                    datasetsCumplimientoPorcentajes.push([
                        serie.porcCumple,
                        serie.porcParcial,
                        serie.porcNoCumple,
                        serie.porcNoAplica
                    ]);

                    datasetsTipo.push([serie.porcLaboral, serie.porcOcupacional, serie.porcAmbiental, serie.porcRse]);
                    cumplimientoGeneralValues.push(serie.cumplimientoGeneral); // Guardamos Cumplimiento General
                });

                // Gráfica de Cumplimiento con valores absolutos
                const ctx = document.getElementById('graficoCumplimiento1').getContext('2d');
                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: ['CUMPLE', 'CUMPLE PARCIAL', 'NO CUMPLE', 'NO APLICA'],
                        datasets: datasetsCumplimiento.map((data, index) => ({
                            label: fechas[index],
                            data: data,
                            backgroundColor: ['#4CAF50', '#FFC107', '#F44336', '#9E9E9E']
                        }))
                    },
                    options: {
                        responsive: true,
                        scales: {
                            xAxes: [{ gridLines: { display: false, drawBorder: false }, maxBarThickness: 50 }],
                            yAxes: [{ ticks: { min: 0, maxTicksLimit: 11 }, scaleLabel: { display: true, labelString: 'Cantidad de Requerimientos' }, padding: 10 }],
                        },
                    }
                });

                // Gráfica de Tipos con porcentajes
                const ctx2 = document.getElementById('graficoCumplimiento2').getContext('2d');
                new Chart(ctx2, {
                    type: 'bar',
                    data: {
                        labels: ['LABORAL', 'OCUPACIONAL', 'AMBIENTAL', 'RSE'],
                        datasets: datasetsTipo.map((data, index) => ({
                            label: fechas[index],
                            data: data,
                            backgroundColor: ['#4CAF50', '#FFC107', '#F44336', '#9E9E9E']
                        }))
                    },
                    options: {
                        responsive: true,
                        scales: {
                            xAxes: [{ gridLines: { display: false, drawBorder: false }, maxBarThickness: 50 }],
                            yAxes: [{ ticks: { min: 0, maxTicksLimit: 10 }, scaleLabel: { display: true, labelString: 'Porcentaje (%)' } }],
                        },
                    }
                });

                // Generar tabla de Cumplimiento con porcentajes
                let tablaCumplimientoHTML = '<table class="table table-bordered text-right"><thead><tr><th class="text-center">Fecha</th>';
                fechas.forEach(fecha => {
                    tablaCumplimientoHTML += `<th class="text-center">${fecha}</th>`;
                });
                tablaCumplimientoHTML += '</tr></thead><tbody>';

                // Cumplimiento General en la tabla de Cumplimiento
                tablaCumplimientoHTML += '<tr><td class="text-center font-weight-bold">Cumplimiento Global</td>';
                cumplimientoGeneralValues.forEach(cumplimiento => {
                    tablaCumplimientoHTML += `<td class="text-right font-weight-bold" style="color: red;">${cumplimiento.toFixed(2)}%</td>`;
                });
                tablaCumplimientoHTML += '</tr>';

                const categoriasCumplimiento = ["CUMPLE", "CUMPLE PARCIAL", "NO CUMPLE", "NO APLICA"];
                categoriasCumplimiento.forEach((categoria, index) => {
                    tablaCumplimientoHTML += `<tr><td class="text-center">${categoria}</td>`;
                    datasetsCumplimientoPorcentajes.forEach(data => {
                        tablaCumplimientoHTML += `<td class="text-right">${data[index].toFixed(2)}%</td>`;
                    });
                    tablaCumplimientoHTML += '</tr>';
                });
                tablaCumplimientoHTML += '</tbody></table>';
                document.getElementById("tablaCumplimiento").innerHTML = tablaCumplimientoHTML;

                // Generar tabla de Tipo
                let tablaTipoHTML = '<table class="table table-bordered text-right"><thead><tr><th class="text-center">Fecha</th>';
                fechas.forEach(fecha => {
                    tablaTipoHTML += `<th class="text-center">${fecha}</th>`;
                });
                tablaTipoHTML += '</tr></thead><tbody>';

                // Cumplimiento General en la tabla de Tipo
                tablaTipoHTML += '<tr><td class="text-center font-weight-bold">Cumplimiento Global</td>';
                cumplimientoGeneralValues.forEach(cumplimiento => {
                    tablaTipoHTML += `<td class="text-right font-weight-bold" style="color: red;">${cumplimiento.toFixed(2)}%</td>`;
                });
                tablaTipoHTML += '</tr>';

                const categoriasTipo = ["LABORAL", "OCUPACIONAL", "AMBIENTAL", "RSE"];
                categoriasTipo.forEach((categoria, index) => {
                    tablaTipoHTML += `<tr><td class="text-center">${categoria}</td>`;
                    datasetsTipo.forEach(data => {
                        tablaTipoHTML += `<td class="text-right">${data[index].toFixed(2)}%</td>`;
                    });
                    tablaTipoHTML += '</tr>';
                });
                tablaTipoHTML += '</tbody></table>';
                document.getElementById("tablaTipo").innerHTML = tablaTipoHTML;

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
    // Convertir la cadena ISO a un objeto Date
    const fecha = new Date(fechaISO);

    // Extraer día, mes y año
    const dia = String(fecha.getDate()).padStart(2, '0'); // Obtener día con dos dígitos
    const mes = String(fecha.getMonth() + 1).padStart(2, '0'); // Obtener mes, sumando 1 porque los meses en JS van de 0 a 11
    const anio = fecha.getFullYear(); // Obtener año completo

    // Formatear y retornar como dd/MM/yyyy
    return `${dia}/${mes}/${anio}`;
}
