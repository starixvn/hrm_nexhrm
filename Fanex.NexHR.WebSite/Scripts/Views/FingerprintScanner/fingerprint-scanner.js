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
            beforeSend: function () {
                $('.loading-process').fadeIn(300);
            },
            success: function (res) {
                $(domElement.TablePlace).html(res);
            },
            error: function () {
                alert(_messages.errorTryAgain);
            },
            complete: function () {
                if ($('.loading-process').length) {
                    $('.loading-process').fadeOut(300);
                }
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