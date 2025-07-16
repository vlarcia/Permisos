const modelo = {
    idPermiso: 0,
    nombre: "",
    descripcion: "",
    institucion: "",
    encargado: "",
    fechaVencimiento: "",
    diasGestion: 0,
    criticidad: "",
    tipo: "",
    estadoPermiso: "",
    fechaCreacion: "",
    fechaModificacion: null,
    idArea: 0,
    alertas: [],
    urlEvidencia: "",        // URL del PDF en Firebase
    nombreEvidencia: ""      // Nombre del archivo PDF
};

let filaSeleccionada;
let editar = false;

let archivoEvidencia = null;
let archivoEvidencia2 = null;
let archivoEvidencia3 = null;
let urlEvidencia = "";
let urlEvidencia2 = "";
let urlEvidencia3 = "";
let eliminarEvidencia = false;
let eliminarEvidencia2 = false;
let eliminarEvidencia3 = false;

$(document).ready(function () {

    fetch("/Area/Lista")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            if (responseJson.data && responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboArea").append(
                        $("<option>").val(item.idArea).text(item.nombre)
                    );
                });
            }
        });

    const tablaPermisos = $('#tbPermisos').DataTable({
        responsive: true,
        ajax: {
            url: '/Permiso/Lista',
            type: 'GET',
            dataType: 'json'
        },
        columns: [
            { data: 'idPermiso' },
            { data: 'nombre' },
            { data: 'institucion' },
            { data: 'encargado' },
            {
                data: 'fechaVencimiento',
                render: data => data ? new Date(data).toLocaleDateString('es-NI') : ''
            },
            { data: 'estadoPermiso' },
            {
                data: 'fechaVencimiento',
                render: function (data, type, row) {
                    const hoy = new Date();
                    const vencimiento = new Date(data);
                    const diasGestion = row.diasGestion || 0;
                    const fechaAlerta = new Date(vencimiento);
                    fechaAlerta.setDate(vencimiento.getDate() - diasGestion);
                    const diffDays = Math.ceil((fechaAlerta - hoy) / (1000 * 60 * 60 * 24));
                    if (diffDays <= 0) return '<span class="badge badge-danger">🔴</span>';
                    else if (diffDays <= 30) return '<span class="badge badge-warning">🟡</span>';
                    else return '<span class="badge badge-success">🟢</span>';
                }
            },
            {
                defaultContent:
                    '<button class="btn btn-info btn-ver btn-sm mr-1"><i class="fas fa-eye"></i></button>' +
                    '<button class="btn btn-primary btn-editar btn-sm mr-1"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                orderable: false,
                width: '120px'
            }
        ],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_ Permisos',
            }, 'pageLength'
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json'
        }
    });

    $('#txtEvidencia').on('change', function (e) {
        archivoEvidencia = e.target.files[0];
        $('#lblNombreArchivo').text(archivoEvidencia ? archivoEvidencia.name : '');
    });

    $('#txtEvidencia2').on('change', function (e) {
        archivoEvidencia2 = e.target.files[0];
        $('#lblNombreArchivo2').text(archivoEvidencia2 ? archivoEvidencia2.name : '');
    });

    $('#txtEvidencia3').on('change', function (e) {
        archivoEvidencia3 = e.target.files[0];
        $('#lblNombreArchivo3').text(archivoEvidencia3 ? archivoEvidencia3.name : '');
    });

    $.datepicker.setDefaults($.datepicker.regional["es"]);
    $("#txtFechaVencimiento").datepicker({ dateFormat: "dd/mm/yy" });
    $("#txtFechaCreacion").datepicker({ dateFormat: "dd/mm/yy" });

    $('#btnNuevo').click(function () {
        editar = false;
        eliminarEvidencia = false;
        eliminarEvidencia2 = false;
        eliminarEvidencia3 = false;
        $('#formPermiso')[0].reset();
        archivoEvidencia = null;
        archivoEvidencia2 = null;
        archivoEvidencia3 = null;
        $('#txtEvidencia, #txtEvidencia2, #txtEvidencia3').val('');
        $('#lblNombreArchivo, #lblNombreArchivo2, #lblNombreArchivo3').text('');
        $('#btnVerPdf, #btnVerPdf2, #btnVerPdf3, #btnEliminarPdf, #btnEliminarPdf2, #btnEliminarPdf3').hide();
        $('.modal-body input, .modal-body select').prop('disabled', false);
        $('#btnGuardar').show();
        $('#modalPermiso').modal('show');
    });

    $('#tbPermisos tbody').on('click', '.btn-ver', function () {
        const fila = $(this).closest('tr');
        const data = tablaPermisos.row(fila).data();
        $('#txtEvidencia').val('');
        cargarFormulario(data, true);
        $('#btnGuardar').hide();
        $('#modalPermiso').modal('show');
    });

    $('#tbPermisos tbody').on('click', '.btn-editar', function () {
        filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaPermisos.row(filaSeleccionada).data();
        editar = true;
        eliminarEvidencia = false;
        eliminarEvidencia2 = false;
        eliminarEvidencia3 = false;

        urlEvidencia = data.urlEvidencia || "";
        urlEvidencia2 = data.urlEvidencia2 || '';
        urlEvidencia3 = data.urlEvidencia3 || '';

        cargarFormulario(data, false);
        $('#btnGuardar').show();
        $('#modalPermiso').modal('show');
    });

    $('#btnVerPdf').click(function () {
        if (urlEvidencia) {
            window.open(urlEvidencia, '_blank');
        } else {
            Swal.fire('Información', 'No hay archivo PDF cargado para este permiso.', 'info');
        }
    });

    $('#btnVerPdf2').click(() => {
        if (urlEvidencia2) window.open(urlEvidencia2, '_blank');
        else Swal.fire('Información', 'No hay archivo PDF cargado para esta evidencia.', 'info');
    });

    $('#btnVerPdf3').click(() => {
        if (urlEvidencia3) window.open(urlEvidencia3, '_blank');
        else Swal.fire('Información', 'No hay archivo PDF cargado para esta evidencia.', 'info');
    });

    $('#btnEliminarPdf').click(function () {
        eliminarEvidencia = true;
        archivoEvidencia = null;
        $('#lblNombreArchivo').text('');
        $('#txtEvidencia').val('');
        $('#btnVerPdf, #btnEliminarPdf').hide();
    });

    $('#btnEliminarPdf2').click(function () {
        eliminarEvidencia2 = true;
        archivoEvidencia2 = null;
        $('#lblNombreArchivo2').text('');
        $('#txtEvidencia2').val('');
        $('#btnVerPdf2, #btnEliminarPdf2').hide();
    });

    $('#btnEliminarPdf3').click(function () {
        eliminarEvidencia3 = true;
        archivoEvidencia3 = null;
        $('#lblNombreArchivo3').text('');
        $('#txtEvidencia3').val('');
        $('#btnVerPdf3, #btnEliminarPdf3').hide();
    });

    $('#btnGuardar').click(function () {
        if (!validarFormulario()) return;

        const modelo = {
            idPermiso: parseInt($('#txtIdPermiso').val() || 0),
            nombre: $('#txtNombre').val(),
            descripcion: $('#txtDescripcion').val(),
            institucion: $('#txtInstitucion').val(),
            encargado: $('#txtEncargado').val(),
            fechaCreacion: convertirFecha($('#txtFechaCreacion').val()),
            fechaVencimiento: convertirFecha($('#txtFechaVencimiento').val()),
            diasGestion: parseInt($('#txtDiasGestion').val()),
            criticidad: $('#cboCriticidad').val(),
            tipo: $('#cboTipo').val(),
            estadoPermiso: $('#cboEstado').val(),
            idArea: parseInt($('#cboArea').val()),
            nombreEvidencia: archivoEvidencia ? archivoEvidencia.name : $('#lblNombreArchivo').text(),
            nombreEvidencia2: archivoEvidencia2 ? archivoEvidencia2.name : $('#lblNombreArchivo2').text(),
            nombreEvidencia3: archivoEvidencia3 ? archivoEvidencia3.name : $('#lblNombreArchivo3').text(),
            eliminarEvidencia,
            eliminarEvidencia2,
            eliminarEvidencia3
        };

        const formData = new FormData();
        formData.append("modelo", JSON.stringify(modelo));
        if (archivoEvidencia) formData.append("archivoEvidencia", archivoEvidencia);
        if (archivoEvidencia2) formData.append("archivoEvidencia2", archivoEvidencia2);
        if (archivoEvidencia3) formData.append("archivoEvidencia3", archivoEvidencia3);

        const url = editar ? '/Permiso/Editar' : '/Permiso/Crear';
        $("#tbPermisos").LoadingOverlay("show");

        fetch(url, {
            method: 'POST',
            body: formData
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire("Listo!", `Permiso ${responseJson.objeto.nombre} registrado con éxito!`, "success");
                    $("#modalPermiso").modal("hide");
                    tablaPermisos.ajax.reload();
                } else {
                    Swal.fire('Error', responseJson.mensaje, 'error');
                }
                $("#tbPermisos").LoadingOverlay("hide");
            })
            .catch(error => {
                $("#tbPermisos").LoadingOverlay("hide");
                Swal.fire("¡Error!", "Hubo un problema al registrar el permiso. " + error, "error");
            });
    });

    $('#tbPermisos tbody').on('click', '.btn-eliminar', function () {
        const fila = $(this).closest('tr');
        const data = tablaPermisos.row(fila).data();

        Swal.fire({
            title: '¿Eliminar Permiso?',
            text: `¿Seguro que deseas eliminar el permiso "${data.nombre}"?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $("#tbPermisos").LoadingOverlay("show");
                fetch(`/Permiso/Eliminar?idPermiso=${data.idPermiso}`, { method: 'POST' })
                    .then(response => response.ok ? response.json() : Promise.reject(response))
                    .then(responseJson => {
                        if (responseJson.estado) {
                            Swal.fire("Listo!", "Permiso eliminado!", "success");
                            tablaPermisos.ajax.reload();
                        } else {
                            Swal.fire("Error", responseJson.mensaje, "error");
                        }
                        $("#tbPermisos").LoadingOverlay("hide");
                    })
                    .catch(error => {
                        $("#tbPermisos").LoadingOverlay("hide");
                        Swal.fire("¡Error!", "Hubo un problema al eliminar el permiso. " + error, "error");
                    });
            }
        });
    });

    function cargarFormulario(data, soloLectura) {
        $('#txtIdPermiso').val(data.idPermiso);
        $('#txtNombre').val(data.nombre).prop('disabled', soloLectura);
        $('#txtDescripcion').val(data.descripcion).prop('disabled', soloLectura);
        $('#txtInstitucion').val(data.institucion).prop('disabled', soloLectura);
        $('#txtEncargado').val(data.encargado).prop('disabled', soloLectura);
        $('#txtFechaCreacion').val(data.fechaCreacion ? new Date(data.fechaCreacion).toLocaleDateString('es-NI') : '').prop('disabled', soloLectura);
        $('#txtFechaVencimiento').val(data.fechaVencimiento ? new Date(data.fechaVencimiento).toLocaleDateString('es-NI') : '').prop('disabled', soloLectura);
        $('#txtDiasGestion').val(data.diasGestion).prop('disabled', soloLectura);
        $('#cboCriticidad').val(data.criticidad).prop('disabled', soloLectura);
        $('#cboTipo').val(data.tipo).prop('disabled', soloLectura);
        $('#cboEstado').val(data.estadoPermiso).prop('disabled', soloLectura);
        $('#cboArea').val(data.idArea).prop('disabled', soloLectura);
        $('#txtEvidencia').prop('disabled', soloLectura);

        $('#lblNombreArchivo').text(data.nombreEvidencia);
        $('#lblNombreArchivo2').text(data.nombreEvidencia2 || '');
        $('#lblNombreArchivo3').text(data.nombreEvidencia3 || '');

        urlEvidencia = data.urlEvidencia || "";
        urlEvidencia2 = data.urlEvidencia2 || '';
        urlEvidencia3 = data.urlEvidencia3 || '';

        $('#btnVerPdf').toggle(!!urlEvidencia);
        $('#btnVerPdf2').toggle(!!urlEvidencia2);
        $('#btnVerPdf3').toggle(!!urlEvidencia3);
        $('#btnEliminarPdf').toggle(!!urlEvidencia);
        $('#btnEliminarPdf2').toggle(!!urlEvidencia2);
        $('#btnEliminarPdf3').toggle(!!urlEvidencia3);

    }

    function validarFormulario() {
        let valido = true;
        $('.input-validar').each(function () {
            const valor = $(this).val();
            if (!valor || valor.trim() === "") {
                $(this).addClass('is-invalid');
                valido = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        return valido;
    }

    function convertirFecha(fechaStr) {
        const partes = fechaStr.split('/');
        return new Date(partes[2], partes[1] - 1, partes[0]);
    }
});

$(document).on('keydown', 'input, select, textarea', function (e) {
    if (e.key === "Enter") {
        e.preventDefault();
        let focusable = $('input, select, textarea, button')
            .filter(':visible:not([disabled]):not([readonly])');
        let index = focusable.index(this);
        if (index > -1 && index + 1 < focusable.length) {
            focusable.eq(index + 1).focus();
        }
    }
});
