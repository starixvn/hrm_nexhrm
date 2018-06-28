var card = (function ($) {
    function init() {
        bindApproveMenu();
        bindPendingMenu();
    }

    function changeCardStatus(command, eventElement) {
        var data = {
            employeeId: $(eventElement).attr("data-employeeId"),
            cardId: $(eventElement).attr("data-cardId"),
            status: command
        }
        $.ajax({
            url: _global.root + "FingerprintScanner/ChangeCardStatus",
            method: "post",
            data: data,
            success: function (response) {
                var result = response.commandResult;
                alert(result.Message);
                if (result.IsSuccessful) {
                    setTimeout(function () {
                        location.reload(true);
                    }, 3000);
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
                            changeCardStatus(key, e.currentTarget);
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
                        confirm('Are you sure you want to Approve this fingerprint?', function () {
                            changeCardStatus(key, e.currentTarget);
                        });
                    },
                    items: {
                        'Approve': { name: 'Approve', icon: 'approve' }
                    }
                };
            }
        });
    }

    return {
        init
    }
})(jQuery);

$(document).ready(function () {
    card.init();
});