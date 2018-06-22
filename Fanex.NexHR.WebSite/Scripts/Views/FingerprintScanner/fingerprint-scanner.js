var fingerprintScanner = (function ($) {
    var domElement = {
        Form: "#fingerprint-scanner",
        TablePlace: "#list-finger-card"
    }

    function query($form) {
        $.ajax({
            url: $form.attr("action"),
            method: $form.attr("method"),
            data: $form.serialize(),
            dataType: "html",
            success: function (res) {
                $(domElement.TablePlace).html(res);
            }
        });
    }

    function registerSubmitEvent() {
        $(domElement.Form).on("submit", function (e) {
            e.preventDefault();
            query($(this));
        });
    }

    function init() {
        query($(domElement.Form));
        registerSubmitEvent();
    }

    return {
        initModule: init
    }
})(jQuery);

$(document).ready(function () {
    fingerprintScanner.initModule();
});