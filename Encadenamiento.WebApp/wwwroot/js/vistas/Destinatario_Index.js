// Destinatario_Index.js actualizado

let tablaDestinatarios;
let listaAreas = [];
let listaPermisos = [];
let editar = false;


$(document).ready(function () {
    cargarAreas();
    cargarPermisos();
    inicializarTablaDestinatarios();

    $('#btnNuevo').click(function () {
        editar = false;
        limpiarFormulario();
        $('#modalDestinatario').modal('show');
    });

    $('#btnGuardar').click(function () {
        guardarDestinatario();
    });

    $('#btnAgregarPermiso').click(function () {
        agregarFilaPermiso();
    });

    $('#tbPermisosDestinatario tbody').on('click', '.btn-quitar-permiso', function () {
        $(this).closest('tr').remove();
    });

    $('#tbPermisosDestinatario tbody').on('change', '.select-permiso', function () {
        const selectedOption = $(this).find('option:selected');
        const institucion = selectedOption.data('institucion') || '';
        const fecha = selectedOption.data('vencimiento') || '';

        const row = $(this).closest('tr');
        row.find('.input-institucion').val(institucion);
        row.find('.input-fecha').val(fecha);
    });
});



function cargarAreas() {
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
}

function cargarPermisos() {
    $.get('/Permiso/Lista', function (data) {
        if (data && data.data) {
            listaPermisos = data.data;
        }
    });
}

function inicializarTablaDestinatarios() {
    tablaDestinatarios = $('#tbDestinatarios').DataTable({
        ajax: {
            url: '/Destinatario/Lista',
            type: 'GET',
            datatype: 'json'
        },
        columns: [
            { data: 'idDestinatario', visible: false },
            { data: 'nombre' },
            { data: 'correo' },
            { data: 'telefonoWhatsapp' },
            { data: 'nombreArea' },
            {
                data: null,
                render: function (data, type, row) {
                    let badges = '';
                    if (row.recibeAlta) badges += '<span class="badge badge-danger mr-1">Alta</span>';
                    if (row.recibeMedia) badges += '<span class="badge badge-warning mr-1">Media</span>';
                    if (row.recibeBaja) badges += '<span class="badge badge-success mr-1">Baja</span>';
                    return badges || '<span class="text-muted">-</span>';
                }
            },
            {
                data: 'activo',
                render: function (data) {
                    return data ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                defaultContent: `
                    <button class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button>
                    <button class="btn btn-danger btn-sm btn-eliminar"><i class="fas fa-trash"></i></button>
                `,
                orderable: false,
                searchable: false
            }
        ],
        responsive: true
    });

    $('#tbDestinatarios tbody').on('click', '.btn-editar', function () {
        editar = true;
        const row = $(this).closest('tr');
        const data = tablaDestinatarios.row(row).data();
        cargarDatosFormulario(data);
        $('#modalDestinatario').modal('show');
    });

   
}

function cargarDatosFormulario(data) {
    $('#txtIdDestinatario').val(data.idDestinatario);
    $('#txtNombre').val(data.nombre);
    $('#txtCorreo').val(data.correo);
    $('#txtTelefono').val(data.telefonoWhatsapp);
    $('#txtFechaRegistro').val(data.fechaRegistro?.substring(0, 10));
    $('#chkAlta').prop('checked', data.recibeAlta);
    $('#chkMedia').prop('checked', data.recibeMedia);
    $('#chkBaja').prop('checked', data.recibeBaja);
    $('#chkActivo').prop('checked', data.activo);
    $('#cboArea').val(data.idArea);

    $('#tbPermisosDestinatario tbody').empty();
    const permisos = data.permisosPorDestinatarios || [];
    permisos.forEach(p => agregarFilaPermiso(p));
}

function limpiarFormulario() {
    $('#formDestinatario')[0].reset();
    $('#txtIdDestinatario').val('0');
    $('#tbPermisosDestinatario tbody').empty();

    // Establecer la fecha actual como fecha de registro
    const hoy = new Date().toISOString().split("T")[0];
    $('#txtFechaRegistro').val(hoy);
}

function agregarFilaPermiso(permiso = null) {
    const selectedId = permiso?.idPermiso || '';

    // Validación para evitar duplicados
    if (selectedId && $('#tbPermisosDestinatario tbody tr').length > 0) {
        let existe = false;
        $('#tbPermisosDestinatario tbody tr').each(function () {
            const idExistente = parseInt($(this).find('.select-permiso').val()) || 0;
            if (idExistente === parseInt(selectedId)) {
                existe = true;
                return false;
            }
        });
        if (existe) {
            toastr.warning("Este permiso ya ha sido agregado.");
            return;
        }
    }

    const fila = `
        <tr data-id="${permiso?.idPermisoDestinatario || 0}">
            <td>
                <select class="form-control select-permiso">
                    <option value="">[Seleccione]</option>
                    ${listaPermisos.map(p =>
        `<option value="${p.idPermiso}"
                                 data-institucion="${p.institucion}"
                                 data-vencimiento="${p.fechaVencimiento?.substring(0, 10)}"
                                 ${p.idPermiso == selectedId ? 'selected' : ''}>
                            ${p.idPermiso} - ${p.nombre}
                        </option>`).join('')}
                </select>
            </td>
            <td><input type="text" class="form-control input-institucion" value="${permiso?.institucion || ''}" readonly /></td>
            <td><input type="date" class="form-control input-fecha" value="${permiso?.fechaVencimiento?.substring(0, 10) || ''}" readonly /></td>
            <td><button type="button" class="btn btn-danger btn-sm btn-quitar-permiso"><i class="fas fa-trash"></i></button></td>
        </tr>
    `;

    $('#tbPermisosDestinatario tbody').append(fila);

    if (selectedId) {
        $('#tbPermisosDestinatario tbody tr:last .select-permiso').trigger('change');
    }
}



function obtenerModeloDestinatarioDesdeFormulario() {
    const modelo = {
        idDestinatario: parseInt($('#txtIdDestinatario').val()) || 0,
        nombre: $('#txtNombre').val().trim(),
        correo: $('#txtCorreo').val().trim(),
        telefonoWhatsapp: $('#txtTelefono').val().trim(),      
        recibeAlta: $('#chkAlta').is(':checked'),
        recibeMedia: $('#chkMedia').is(':checked'),
        recibeBaja: $('#chkBaja').is(':checked'),
        activo: $('#chkActivo').is(':checked'),
        idArea: parseInt($('#cboArea').val()) || 0,
        fechaRegistro: $('#txtFechaRegistro').val(), // Fecha Registro
        permisosPorDestinatarios: []
    };

    $('#tbPermisosDestinatario tbody tr').each(function () {
        const row = $(this);
        const idPermisoDestinatario = parseInt(row.attr('data-id')) || 0;
        const idPermiso = parseInt(row.find('.select-permiso').val()) || 0;

        if (idPermiso > 0) {
            modelo.permisosPorDestinatarios.push({
                idPermisoDestinatario: idPermisoDestinatario,
                idPermiso: idPermiso,
                idDestinatario: modelo.idDestinatario
            });
        }
    });


    return modelo;
}

function guardarDestinatario() {
    if (!validarFormulario()) return;
    const modelo = obtenerModeloDestinatarioDesdeFormulario();
    const formData = new FormData();
    formData.append("modelo", JSON.stringify(modelo));

    const url = editar ? '/Destinatario/Editar' : '/Destinatario/Crear';

    $("#tbDestinatarios").LoadingOverlay("show");

    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            if (responseJson.estado) {
                Swal.fire("Listo!", `Destinatario ${responseJson.objeto.nombre} registrado con éxito!`, "success");
                $("#modalDestinatario").modal("hide");
                tablaDestinatarios.ajax.reload();
            } else {
                Swal.fire('Error', responseJson.mensaje, 'error');
            }
            $("#tbDestinatarios").LoadingOverlay("hide");
        })
        .catch(error => {
            $("#tbDestinatarios").LoadingOverlay("hide");
            Swal.fire("¡Error!", "Hubo un problema al registrar el destinatario. " + error, "error");
        });
}

$('#tbDestinatarios tbody').on('click', '.btn-eliminar', function () {

    const row = $(this).closest('tr');
    const data = tablaDestinatarios.row(row).data();
    Swal.fire({
        title: '¿Eliminar Destinatario y todos sus registros asociados?',
        text: `¿Seguro que deseas eliminar el destinatario "${data.nombre}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $("#tbPermisos").LoadingOverlay("show");
            fetch(`/Destinatario/Eliminar?idDestinatario=${data.idDestinatario}`, { method: 'POST' })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(responseJson => {
                    if (responseJson.estado) {
                        Swal.fire("Listo!", "Destinatario " + data.nombre + " fue eliminado!", "success");
                        tablaDestinatarios.ajax.reload();
                    } else {
                        Swal.fire("Lo sentimos!", responseJson.mensaje, "error");
                    }
                    $("#tbDestinatarios").LoadingOverlay("hide");
                })
                .catch(error => {
                    $("#tbDestinatarios").LoadingOverlay("hide");
                    Swal.fire("¡Error!", "Hubo un problema al eliminar el destinatario. " + error, "error");
                });
        }
    });
});

function validarFormulario() {
    let valido = true;

    $('.input-validar').each(function () {
        const tipo = $(this).prop("tagName").toLowerCase(); // input, select o textarea
        const valor = $(this).val();

        // Si el campo está vacío o es solo espacios
        if (!valor || valor.trim() === "") {
            $(this).addClass('is-invalid');

            // Opcional: también podrías mostrar un mensaje personalizado o tooltip
            valido = false;
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    return valido;
}
