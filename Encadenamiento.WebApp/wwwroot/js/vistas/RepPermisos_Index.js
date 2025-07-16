$(document).ready(function () {
    $(".card-body").LoadingOverlay("show");

    fetch("/ReportePermisos/ObtenerResumen")
        .then(res => res.ok ? res.json() : Promise.reject(res))
        .then(data => {
            $(".card-body").LoadingOverlay("hide");

            if (data.estado) {
                const resumen = data.data;

                // Verifica que sea array antes de usar forEach
                if (!Array.isArray(resumen.vencenPronto)) {
                    console.error("❌ vencenPronto no es un array válido", resumen.vencenPronto);
                    Swal.fire("Error", "Los datos de vencimientos no tienen el formato esperado.", "error");
                    return;
                }

                // Gráfico por Estado
                const ctx1 = document.getElementById("graficoEstado").getContext('2d');
                new Chart(ctx1, {
                    type: 'bar',
                    data: {
                        labels: resumen.porEstado.map(x => x.estado),
                        datasets: [{
                            label: "Permisos",
                            backgroundColor: '#4CAF50',
                            data: resumen.porEstado.map(x => x.total)
                        }]
                    }
                });

                // Gráfico por Criticidad
                const ctx2 = document.getElementById("graficoCriticidad").getContext('2d');
                new Chart(ctx2, {
                    type: 'pie',
                    data: {
                        labels: resumen.porCriticidad.map(x => x.criticidad),
                        datasets: [{
                            backgroundColor: ['#F44336', '#FF9800', '#4CAF50'],
                            data: resumen.porCriticidad.map(x => x.total)
                        }]
                    }
                });

                // Tabla de Vencimientos Próximos
                let html = '<table class="table table-bordered table-sm"><thead><tr><th>Nombre</th><th>Fecha Venc.</th><th>Encargado</th><th>Estado</th><th>Criticidad</th></tr></thead><tbody>';
                console.log("✅ vencenPronto:", resumen.vencenPronto);

                resumen.vencenPronto.forEach(p => {
                    html += `<tr>
                        <td>${p.nombre}</td>
                        <td>${new Date(p.fechaVencimiento).toLocaleDateString('es-NI')}</td>
                        <td>${p.encargado}</td>
                        <td>${p.estadoPermiso}</td>
                        <td>${p.criticidad}</td>
                    </tr>`;
                });
                html += '</tbody></table>';
                $("#tablaVencimientos").html(html);

            } else {
                Swal.fire("Lo sentimos!", "No se pudo obtener el resumen", "error");
            }
        })
        .catch(err => {
            window.onerror = function (msg, url, lineNo, columnNo, error) {
                console.error("Global JS error:", msg, "at", url, ":", lineNo, ":", columnNo, error);
            };

            $(".card-body").LoadingOverlay("hide");
            console.error("Error capturado:", err);
            Swal.fire("Error", "Hubo un problema al obtener los datos", "error");
        });
   
});
function exportarExcel() {
    const tabla = document.querySelector("#tablaVencimientos table");
    if (!tabla) {
        Swal.fire("No hay datos", "No se encontró la tabla de datos", "warning");
        return;
    }

    const wb = XLSX.utils.book_new();
    const ws = XLSX.utils.table_to_sheet(tabla);
    XLSX.utils.book_append_sheet(wb, ws, "Permisos");

    XLSX.writeFile(wb, "ReportePermisos.xlsx");
}

function exportarPDF() {
    const tabla = document.getElementById("tablaVencimientos");
    if (!tabla) {
        Swal.fire("No hay datos", "No se encontró la tabla de datos", "warning");
        return;
    }

    const opt = {
        margin: 0.5,
        filename: 'ReportePermisos.pdf',
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2 },
        jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
    };

    html2pdf().set(opt).from(tabla).save();
}

