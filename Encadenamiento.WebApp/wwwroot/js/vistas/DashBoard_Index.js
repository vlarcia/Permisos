$(document).ready(function () {

    $("div.container-fluid").LoadingOverlay("show");
    fetch("/DashBoard/ObtenerResumen")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $("div.container-fluid").LoadingOverlay("hide");
            if (responseJson.estado) {
                const d = responseJson.objeto
                $("#totalPermisos").text(d.totalPermisos)
                $("#totalDestinatarios").text(d.totalDestinatarios)
                $("#alertasUltimoMes").text(d.alertasUltimoMes)
                $("#vencimientosMes").text(d.vencimientosMes)
                $("#totalPermisosVencidos").text(d.totalPermisosVencidosNoTramite);

                // Evento click en tarjeta "Vencidos sin Trámite"
                $("#cardPermisosVencidos").on("click", function () {
                    mostrarPermisosVencidos();
                });

                // Graficas
                let barchart_labels;
                let barchart_data;
                if (d.renovaciones.length > 0) {
                    barchart_labels = d.renovaciones.map((item) => { return item.llave })
                    barchart_data = d.renovaciones.map((item) => { return item.valor })
                } else {
                    barchart_labels = ["Sin Resultados"]
                    barchart_data = [0]
                }

                let barchart_labels2;
                let barchart_data2;
                if (d.vencimientos.length > 0) {
                    barchart_labels2 = d.vencimientos.map((item) => { return item.llave })
                    barchart_data2 = d.vencimientos.map((item) => { return item.valor })
                } else {
                    barchart_labels2 = ["Sin Resultados"]
                    barchart_data2 = [0]
                }

                // Bar Chart Fincas
                let controlRenovaciones = document.getElementById("chartRenovaciones");
                let myBarChart = new Chart(controlRenovaciones, {
                    type: 'bar',
                    data: {
                        labels: barchart_labels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#4e73df",
                            hoverBackgroundColor: "#2e59d9",
                            borderColor: "#4e73df",
                            data: barchart_data,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });

                // Bar Chart Actividades
                let controlVencimientos = document.getElementById("chartVencimientos");
                let myBarChart2 = new Chart(controlVencimientos, {
                    type: 'bar',
                    data: {
                        labels: barchart_labels2,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#4e73df",
                            hoverBackgroundColor: "#2e59d9",
                            borderColor: "#4e73df",
                            data: barchart_data2,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        onClick: function (evt, elements) {
                            if (elements.length > 0) {
                                const index = elements[0]._index;
                                const fechaSeleccionada = barchart_labels2[index];
                                mostrarTablaPermisosPorFecha(fechaSeleccionada);
                            }
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });
            }
            else {
                Swal.fire("Lo sentimos!", responseJson.mensaje, "error")
            }
        })

    function mostrarPermisosVencidos() {
        $("#fechaSeleccionadaTitulo").text("con estado vencido sin Trámite");
        $("#tablaPermisosFecha tbody").empty();
        $("div.container-fluid").LoadingOverlay("show");

        fetch(`/DashBoard/ObtenerPermisosVencidos`)
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(data => {
                $("div.container-fluid").LoadingOverlay("hide");
                if (data.estado) {
                    if (data.objeto.length > 0) {
                        data.objeto.forEach(permiso => {
                            let fila = `
                            <tr>
                                <td>${permiso.idPermiso}</td>
                                <td>${permiso.nombre.length > 100 ? permiso.nombre.substring(0, 100) + "..." : permiso.nombre}</td>
                                <td>${new Date(permiso.fechaVencimiento).toLocaleDateString()}</td>
                                <td>${permiso.nombreArea}</td>
                                <td>${permiso.estadoPermiso}</td>
                                <td>${permiso.institucion}</td>
                            </tr>`;
                            $("#tablaPermisosFecha tbody").append(fila);
                        });
                    } else {
                        $("#tablaPermisosFecha tbody").append(`<tr><td colspan="6" class="text-center">No hay permisos vencidos.</td></tr>`);
                    }
                    $("#modalPermisosFecha").modal("show");
                } else {
                    Swal.fire("Error", data.mensaje, "error");
                }
            })
            .catch(err => {
                $("div.container-fluid").LoadingOverlay("hide");
                Swal.fire("Error", "No se pudo obtener la información. " + err, "error");
            });
    }

    function mostrarPermisosPorFecha(fechaSeleccionada) {
        const partes = fechaSeleccionada.split('/');
        const fechaObj = new Date(partes[2], partes[1] - 1, partes[0]);
        const fechaISO = fechaObj.toISOString().split('T')[0];

        $("div.container-fluid").LoadingOverlay("show");

        fetch(`/DashBoard/ObtenerPermisosPorFecha?fecha=${encodeURIComponent(fechaISO)}`)
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                $("div.container-fluid").LoadingOverlay("hide");

                if (responseJson.estado) {
                    const permisos = responseJson.objeto;

                    if (permisos.length === 0) {
                        Swal.fire("Sin datos", `No hay permisos que vencen el ${fechaSeleccionada}`, "info");
                        return;
                    }

                    let contenidoHtml = `<div style="text-align:left;">`;

                    permisos.forEach(p => {
                        const fecha = new Date(p.fechaVencimiento).toLocaleDateString('es-NI');
                        const nombreTruncado = p.nombre.length > 100 ? p.nombre.substring(0, 100) + "..." : p.nombre;

                        contenidoHtml += `
                    <div style="margin-bottom:10px;">
                        <strong>ID:</strong> ${p.idPermiso} &nbsp; 
                        <strong> -- Vence:</strong> ${fecha}<br/>
                        <strong>Nombre:</strong> ${nombreTruncado}
                    </div>
                    <hr style="margin: 5px 0;"/>
                `;
                    });

                    contenidoHtml += `</div>`;

                    Swal.fire({
                        title: `Permisos que vencen el ${fechaSeleccionada}`,
                        html: contenidoHtml,
                        width: '600px',
                        confirmButtonText: 'Cerrar'
                    });

                } else {
                    Swal.fire("Error", responseJson.mensaje, "error");
                }
            })
            .catch(error => {
                $("div.container-fluid").LoadingOverlay("hide");
                Swal.fire("Error", "Hubo un problema al obtener los permisos: " + error, "error");
            });
    }

    function mostrarTablaPermisosPorFecha(fecha) {
        $("#fechaSeleccionadaTitulo").text("que se vencen en la fecha " + fecha);
        $("#tablaPermisosFecha tbody").empty();
        $("div.container-fluid").LoadingOverlay("show");

        const partes = fecha.split('/');
        const fechaObj = new Date(partes[2], partes[1] - 1, partes[0]);
        const fechaISO = fechaObj.toISOString().split('T')[0];

        fetch(`/DashBoard/ObtenerPermisosPorFecha?fecha=${encodeURIComponent(fechaISO)}`)
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(data => {
                $("div.container-fluid").LoadingOverlay("hide");
                if (data.estado) {
                    if (data.objeto.length > 0) {
                        data.objeto.forEach(permiso => {
                            let fila = `
                            <tr>
                                <td>${permiso.idPermiso}</td>
                                <td>${permiso.nombre.length > 100 ? permiso.nombre.substring(0, 100) + "..." : permiso.nombre}</td>
                                <td>${new Date(permiso.fechaVencimiento).toLocaleDateString()}</td>
                                <td>${permiso.nombreArea}</td>
                                <td>${permiso.estadoPermiso}</td>
                                <td>${permiso.institucion}</td>
                            </tr>`;
                            $("#tablaPermisosFecha tbody").append(fila);
                        });
                    } else {
                        $("#tablaPermisosFecha tbody").append(`<tr><td colspan="6" class="text-center">No hay permisos vencidos para esta fecha.</td></tr>`);
                    }
                    $("#modalPermisosFecha").modal("show");
                } else {
                    Swal.fire("Error", data.mensaje, "error");
                }
            })
            .catch(err => {
                $("div.container-fluid").LoadingOverlay("hide");
                Swal.fire("Error", "No se pudo obtener la información. " + err, "error");
            });
    }

});