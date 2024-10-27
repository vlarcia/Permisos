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
                $("#totalFincas").text(d.totalFincas)
                $("#totalPlanes").text(d.totalPlanes)            
                $("#totalRevisiones").text(d.totalRevisiones)
                $("#totalVisitas").text(d.totalVisitas)
                $("#totalActividades").text(d.totalActividades)

                // Graficas
                let barchart_labels;
                let barchart_data;
                if (d.fincasVisitadas.length > 0) {
                    barchart_labels = d.fincasVisitadas.map((item) => { return item.llave })
                    barchart_data = d.fincasVisitadas.map((item) => { return item.valor })
                } else {
                    barchart_labels = ["Sin Resultados"]
                    barchart_data=[0]
                }

                let barchart_labels2;
                let barchart_data2;
                if (d.actividadesCompletas.length > 0) {
                    barchart_labels2 = d.actividadesCompletas.map((item) => { return item.llave })
                    barchart_data2 = d.actividadesCompletas.map((item) => { return item.valor })
                } else {
                    barchart_labels2 = ["Sin Resultados"]
                    barchart_data2 = [0]
                }


                // Bar Chart Fincas
                let controlVisita = document.getElementById("chartVisitas");
                let myBarChart = new Chart(controlVisita, {
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
                let controlActividades = document.getElementById("chartActividades");
                let myBarChart2 = new Chart(controlActividades, {
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


})