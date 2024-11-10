$(document).ready(function () {
    // Asigna la función a todos los campos con la clase 'campoDecimal'
    $('.campoDecimal').on('blur', function () {
        formatearDecimal($(this));  // Formatea cuando el campo pierde el foco
    }).on('keypress', function (e) {
        // Solo permitimos números y punto
        var key = e.which || e.keyCode;
        if (!(key >= 48 && key <= 57) && key !== 46) {  // Solo números y punto
            e.preventDefault();
        }
    }).on('input', function () {
        // Eliminar los caracteres no numéricos mientras se escribe
        this.value = this.value.replace(/[^\d.-]/g, '');
    });

    // Función para formatear el número con separadores
    function formatearDecimal(input) {
        var valor = input.val();
        if (valor !== '') {
            // Eliminar cualquier otro formato anterior (comas, puntos) y mantener solo los números y el punto decimal
            valor = valor.replace(/[^0-9.-]+/g, "");

            // Formatear con separadores de miles y dos decimales
            var partes = valor.split(".");
            partes[0] = partes[0].replace(/\B(?=(\d{3})+(?!\d))/g, ","); // Separadores de miles

            if (partes[1]) {
                // Limitar a dos decimales
                partes[1] = partes[1].substring(0, 2);
            }

            // Recombinar las partes formateadas
            input.val(partes.join("."));
        }
    }

});
function convertirFecha(fecha) {

    const partes = fecha.split("/");
    const dia = partes[0];
    const mes = partes[1] - 1; // Restar 1 al mes
    const anio = partes[2];
    return new Date(anio, mes, dia);
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
function obtenerFechaActual() {
    const hoy = new Date();
    const anio = hoy.getFullYear();
    const mes = String(hoy.getMonth() + 1).padStart(2, '0'); // Meses de 0 a 11, sumamos 1
    const dia = String(hoy.getDate()).padStart(2, '0'); // Día con dos dígitos

    return `${anio}-${mes}-${dia}`; // Formato yyyy-MM-dd
}