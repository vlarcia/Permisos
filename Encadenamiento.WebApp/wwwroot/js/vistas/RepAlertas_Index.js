$(document).ready(function () {
    $(".card-body").LoadingOverlay("show");

    let ultimasAlertasGlobal = []; // Guardar datos para exportar

    fetch("/ReporteAlertas/ObtenerResumen")
        .then(res => res.ok ? res.json() : Promise.reject(res))
        .then(data => {
            $(".card-body").LoadingOverlay("hide");
            if (data.estado) {
                const resumen = data.data;
                ultimasAlertasGlobal = resumen.ultimasAlertas;

                // Gráfica por Resultado
                const ctxResultado = document.getElementById("graficoResultado").getContext('2d');
                new Chart(ctxResultado, {
                    type: 'bar',
                    data: {
                        labels: resumen.porResultado.map(x => x.resultado),
                        datasets: [{
                            label: "Alertas",
                            backgroundColor: '#007bff',
                            data: resumen.porResultado.map(x => x.total)
                        }]
                    },
                    options: { responsive: true }
                });

                // Gráfica por Medio
                const ctxMedio = document.getElementById("graficoMedio").getContext('2d');
                new Chart(ctxMedio, {
                    type: 'pie',
                    data: {
                        labels: resumen.porMedio.map(x => x.medio),
                        datasets: [{
                            backgroundColor: ['#28a745', '#ffc107', '#dc3545'],
                            data: resumen.porMedio.map(x => x.total)
                        }]
                    },
                    options: { responsive: true }
                });

                // Tabla de últimas alertas
                renderTablaAlertas(ultimasAlertasGlobal);
            } else {
                Swal.fire("Lo sentimos!", "No se pudo obtener el resumen de alertas", "error");
            }
        })
        .catch(err => {
            $(".card-body").LoadingOverlay("hide");
            Swal.fire("Error", "Hubo un problema al obtener los datos", "error");
        });

    function renderTablaAlertas(data) {
        let html = '<table class="table table-bordered table-sm"><thead><tr><th>ID</th><th>Permiso</th><th>Mensaje</th><th>Resultado</th><th>Medio</th><th>Fecha Envío</th></tr></thead><tbody>';
        data.forEach(a => {
            html += `<tr>
                        <td>${a.idAlerta}</td>
                        <td>${a.idPermiso}</td>
                        <td>${a.mensaje}</td>
                        <td>${a.resultado}</td>
                        <td>${a.medioEnvio}</td>
                        <td>${a.fechaEnvio}</td>
                    </tr>`;
        });
        html += '</tbody></table>';
        $("#tablaAlertas").html(html);
    }

    // Exportar Excel con SheetJS
    $("#btnExportExcel").click(() => {
        if (!ultimasAlertasGlobal.length) {
            Swal.fire("Aviso", "No hay datos para exportar.", "info");
            return;
        }

        const wsData = [
            ["ID", "Permiso", "Mensaje", "Resultado", "Medio", "Fecha Envío"]
        ];

        ultimasAlertasGlobal.forEach(a => {
            wsData.push([a.idAlerta, a.idPermiso, a.mensaje, a.resultado, a.medio, a.fechaEnvio]);
        });

        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet(wsData);
        XLSX.utils.book_append_sheet(wb, ws, "Alertas");

        XLSX.writeFile(wb, "Reporte_Alertas.xlsx");
    });

    // Exportar PDF con jsPDF y autoTable
    $("#btnExportPDF").click(() => {
        if (!ultimasAlertasGlobal.length) {
            Swal.fire("Aviso", "No hay datos para exportar.", "info");
            return;
        }

        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();

        doc.text("Reporte de Alertas", 14, 20);

        const columnas = ["ID", "Permiso", "Mensaje", "Resultado", "Medio", "Fecha Envío"];
        const filas = ultimasAlertasGlobal.map(a => [a.idAlerta, a.idPermiso, a.mensaje, a.resultado, a.medio, a.fechaEnvio]);

        doc.autoTable({
            head: [columnas],
            body: filas,
            startY: 30,
            styles: { fontSize: 8 }
        });

        doc.save("Reporte_Alertas.pdf");
    });
});
