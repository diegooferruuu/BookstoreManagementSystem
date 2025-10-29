﻿(function ($) {
    if (!window.jQuery || !$.validator) return;

    $.extend($.validator.messages, {
        required: "Este campo es obligatorio.",
        number: "Ingrese un número válido.",
        digits: "Solo dígitos.",
        max: $.validator.format("Ingrese un valor menor o igual a {0}."),
        step: $.validator.format("Ingrese un múltiplo de {0}."),
        range: "El precio debe ser mayor a 0.",
        min: $.validator.format("Ingrese un valor mayor o igual a {0}.")
    });

    var numberMethod = function (value, element) {
        if (this.optional(element)) return true;
        var normalized = (value || "").replace(',', '.');
        return /^-?\d+(\.\d+)?$/.test(normalized);
    };
    $.validator.methods.number = numberMethod;

    var originalMin = $.validator.methods.min;
    $.validator.methods.min = function (value, element, param) {
        var result = originalMin.call(this, value, element, param);
        if (!result && element && element.getAttribute) {
            var custom = element.getAttribute('data-msg-min');
            if (custom) {
                var previous = $.validator.messages.min;
                $.validator.messages.min = custom;
                setTimeout(function(){ $.validator.messages.min = previous; },0);
            }
        }
        return result;
    };

    var originalMax = $.validator.methods.max;
    $.validator.methods.max = function (value, element, param) {
        var result = originalMax.call(this, value, element, param);
        if (!result && element && element.getAttribute) {
            var custom = element.getAttribute('data-msg-max');
            if (custom) {
                var previous = $.validator.messages.max;
                $.validator.messages.max = custom;
                setTimeout(function(){ $.validator.messages.max = previous; },0);
            }
        }
        return result;
    };

    if ($.validator.unobtrusive && $.validator.unobtrusive.adapters) {
        $.validator.unobtrusive.adapters.addSingleVal("range", "min");
    }

    var letters = "A-Za-zÁÉÍÓÚÑáéíóúÜüñ";

    $.validator.addMethod("productname", function (value, element) {
        var v = (value || "").trim();
        if (!v) return false;
        var reAll = new RegExp("^["+letters+"0-9. ]+$");
        if (!reAll.test(v)) return false;
        var connectors = ["de","del","para","con","y","en","por","la","las","los","san","santa"]; 
        var units = ["gr","g","kg","ml","l","cm","mm","m"]; 
        var tokens = v.replace(/\s+/g,' ').split(' ');
        var hasMain = false;
        for (var i=0;i<tokens.length;i++){
            var t = tokens[i].trim();
            if (!t) continue;
            if (connectors.indexOf(t.toLowerCase()) >= 0) continue;
            if (t.indexOf('.') !== -1 && t.lastIndexOf('.') !== t.length-1) return false;
            var unit = t.endsWith('.') ? t.substring(0,t.length-1) : t;
            if (units.indexOf(unit.toLowerCase()) >= 0) continue;
            if (/^\d+$/.test(unit)) continue;
            var reLetters = new RegExp("^["+letters+"]+$");
            if (reLetters.test(unit)){
                if (unit.length < 3) return false; else { hasMain = true; continue; }
            }
            var reHasLetter = new RegExp("["+letters+"]");
            if (reHasLetter.test(unit) && /\d/.test(unit)) { hasMain = true; continue; }
            return false;
        }
        return hasMain;
    }, function () {
        return [
            "Solo letras y números.",
            "Mínimo 3 letras por palabra.",
            "Solo puntos y no otros caracteres especiales.",
            "Incluya al menos una palabra con 3 letras."
        ].join(' ');
    });

    $.validator.addMethod("productdesc", function (value, element) {
        var v = (value || "").trim();
        if (!v) return false;
        var reAll = new RegExp("^["+letters+"0-9 ,.]+$");
        if (!reAll.test(v)) return false;
        var connectors = ["de","del","la","las","los","y","en","para","por","con"]; 
        var tokens = v.replace(/\s+/g,' ').split(' ');
        var hasWord = false;
        for (var i=0;i<tokens.length;i++){
            var raw = tokens[i].trim();
            if (!raw) continue;
            var t = raw.replace(/[.,]$/,'')
            if (connectors.indexOf(t.toLowerCase()) >= 0) { continue; }
            if (/^\d+$/.test(t)) { continue; }
            var reLetters = new RegExp("^["+letters+"]+$");
            if (reLetters.test(t)){
                if (t.length < 2) return false;
                hasWord = true; continue;
            }
            var reHasLetter = new RegExp("["+letters+"]");
            if (reHasLetter.test(t) && /\d/.test(t)) { hasWord = true; continue; }
            return false;
        }
        return hasWord;
    }, function(){
        return [
            "Solo letras y números.",
            "Permite puntos y comas.",
            "Incluya al menos una palabra."
        ].join(' ');
    });

    $.validator.addMethod("addresscbba", function (value, element) {
        var v = (value || "").trim().replace(/\s+/g,' ');
        if (!v) return true;
        if (v.length > 60) return false;
        if (!/^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ0-9 .\/]+$/.test(v)) return false;
        var tokens = v.split(' ');
        for (var i=0;i<tokens.length;i++){
            var raw = tokens[i];
            if (/^S\/N$/i.test(raw)) continue;
            var endsWithDot = raw.endsWith('.');
            var t = endsWithDot ? raw.slice(0,-1) : raw;
            var upper = t.toUpperCase();
            if (upper.startsWith('N') || upper.startsWith('NO') || upper.startsWith('NRO')){
                var start = upper.startsWith('NRO') ? (endsWithDot ? 4 : 3) : (upper.startsWith('NO') ? (endsWithDot ? 3 : 2) : (endsWithDot ? 2 : 1));
                var rest = raw.substring(start);
                if (rest.length > 0){ if (!/^\d+$/.test(rest)) return false; else continue; }
                if (i+1 < tokens.length && /^\d+$/.test(tokens[i+1])) { i++; continue; }
                return false;
            }
            if (/^\d+$/.test(t)) continue;
            if (/^[A-Za-zÁÉÍÓÚÑáéíóúÜüñ]+$/.test(t)) { if (t.length < 2) return false; continue; }
            return false;
        }
        return true;
    }, function(){
        return "Dirección inválida. Ej.: 'Av. América Este N1759', 'Calle Corrales S/N', 'No. 1234'. Solo letras, números, espacios, punto y '/'.  ";
    });

})(jQuery);
