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
                floatThead('#data');
                initBindingEvent();
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

    function changeStatus(command, eventElement) {
        var data = {
            employeeId: $(eventElement).attr("data-employeeId"),
            bioUserId: $(eventElement).attr("data-bioUserId"),
            status: command
        };

        $.ajax({
            url: _global.root + "Bio/ChangeFingerprintStatus",
            method: "post",
            data: data,
            success: function (response) {
                var result = response.commandResult;
                if (result.IsSuccessful) {
                    query($(domElement.Form));
                }
            },
            error: function (textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            },
        });
    }

    function bindApproveMenu() {
        $.contextMenu({
            selector: '.approve',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key) {
                        confirm('Are you sure you want to change status to Pending?', function () {
                            changeStatus(key, e.currentTarget);
                        });
                    },
                    items: {
                        'Pending': { name: 'Disapprove', icon: 'reject' }
                    }
                };
            }
        });
    }

    function bindPendingMenu() {
        $.contextMenu({
            selector: '.pending',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key) {
                        confirm('Are you sure you want to Approve all fingerprint and card?', function () {
                            changeStatus(key, e.currentTarget);
                        });
                    },
                    items: {
                        'Approve': { name: 'Approve', icon: 'approve' }
                    }
                };
            }
        });
    }

    function initBindingEvent() {
        bindPendingMenu();
        bindApproveMenu();
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