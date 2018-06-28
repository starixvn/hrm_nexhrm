var fingerprint = (function ($) {
    function init() {
        bindApproveMenu();
        bindPendingMenu();
    }

    function changeFingerprintStatus(command, eventElement) {
        var data = {
            employeeId: $(eventElement).attr("data-employeeId"),
            bioUserId: $(eventElement).attr("data-bioUserId"),
            index: $(eventElement).attr("data-index"),
            templateIndex: $(eventElement).attr("data-templateIndex"),
            status: command
        }
        $.ajax({
            url: _global.root + "FingerprintScanner/ChangeFingerStatus",
            method: "post",
            data: data,
            success: function (response) {
                var result = response.commandResult;
                alert(result.Message);
                if (result.IsSuccessful) {
                    location.reload(true);
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
                            changeFingerprintStatus(key, e.currentTarget);
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
                            changeFingerprintStatus(key, e.currentTarget);
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
    fingerprint.init();
});