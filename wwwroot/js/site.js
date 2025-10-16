// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Ajustes de validación en español para jQuery Validation y soporte de coma/punto
(function ($) {
    if (!window.jQuery || !$.validator) return;

    // Mensajes en español (refuerza la localización incluso si falla la carga del archivo de mensajes)
    $.extend($.validator.messages, {
        required: "Este campo es obligatorio.",
        number: "Ingrese un número válido.",
        digits: "Solo dígitos.",
        min: $.validator.format("Ingrese un valor mayor o igual a {0}."),
        max: $.validator.format("Ingrese un valor menor o igual a {0}."),
        step: $.validator.format("Ingrese un múltiplo de {0}."),
    });

    // Acepta números con coma o punto como separador decimal
    var numberMethod = function (value, element) {
        if (this.optional(element)) return true;
        var normalized = (value || "").replace(',', '.');
        return /^-?\d+(\.\d+)?$/.test(normalized);
    };
    $.validator.methods.number = numberMethod;
})(jQuery);
