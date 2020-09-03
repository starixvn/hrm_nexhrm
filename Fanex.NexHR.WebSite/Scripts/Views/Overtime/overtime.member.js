"use strict";
/*global _global, isOnlyHRRole*/
/* exported initButtonForMemberOT, bindApproveMenu, bindPendingMenu */

function initTooltip() {
    var linkDays = $("#memberOT .contextmenu");

    linkDays.mouseover(function () {
        $(this).find(".drop-menu").show();
    });

    linkDays.mouseout(function () {
        $(this).find(".drop-menu").hide();
    });
}

function loadMemberOTDetail() {
    $('#memberOT .loading-process').fadeIn(300);
    $.post(_global.root + "Overtime/MemberOvertimeData", $("#memberOT #form").serialize(), function (response) {
        $("#dataMemberOT tbody").html(response);
        initTooltip();
        initDeleteForMemberOT();
        $('#memberOT .loading-process').fadeOut(300);
    });
}

function rejectRequest(id) {
    confirm('<p>Are you sure you want to reject this request?</p>', function () {
        $('#memberOT .loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/RejectRequest", { id }, function (response) {
            if (response.IsSuccessful) {
                loadMemberOTDetail();
            } else {
                $('#memberOT .loading-process').fadeOut(500);
                notifyError(response.Message);
            }
        });
    });
}

function disapproveRequest(id) {
    confirm('<p>Are you sure you want to disapprove this request?</p>', function () {
        $('#memberOT .loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/DisapproveRequest", { id }, function (response) {
            if (response.IsSuccessful) {
                loadMemberOTDetail();
            } else {
                $('#memberOT .loading-process').fadeOut(500);
                notifyError(response.Message);
            }
        });
    });
}

function approveRequest(id, createdBy) {
    confirm('<p>Are you sure you want to approve this request?</p>', function () {
        $('#memberOT .loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/ApproveRequest", { id, createdBy }, function (response) {
            if (response.IsSuccessful) {
                loadMemberOTDetail();
            } else {
                $('#memberOT .loading-process').fadeOut(500);
                notifyError(response.Message);
            }
        });
    });

}

function resetRequest(id) {
    confirm('<p>Are you sure you want to reset this request?</p>', function () {
        $('#memberOT .loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/ResetRequest", { id }, function (response) {
            if (response.IsSuccessful) {
                loadMemberOTDetail();
            } else {
                $('#memberOT .loading-process').fadeOut(500);
                notifyError(response.Message);
            }
        });
    });
}

function processRequest(command, eventElement) {
    var requestId = eventElement.$trigger.attr('requestId');
    var createdBy = eventElement.$trigger.attr('createdBy');
    switch (command) {
        case 'approve':
            approveRequest(requestId, createdBy);
            break;
        case 'disapprove':
            disapproveRequest(requestId);
            break;
        case 'reject':
            rejectRequest(requestId);
            break;
        case 'reset':
            resetRequest(requestId);
            break;
        default: break;
    }
}

function bindPendingMenu() {
    //if (!IsManager)
    //    return;
    var element = "#memberOT .Pending";

    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (itemKey, options) {
            processRequest(itemKey, options);
        },
        items: {
            "approve": { name: "Approve", icon: "approve" },
            "reject": { name: "Reject", icon: "reject" },
        }
    });
}

function bindApproveMenu() {
    //if (!IsManager)
    //    return;
    var element = "#memberOT .Approve";
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            processRequest(key, options);
        },
        items: {
            "disapprove": { name: "Disapprove", icon: "reject" }
        }
    });
}

function bindRejectMenu() {
    var element = "#memberOT .Reject";
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            processRequest(key, options);
        },
        items: {
            "reset": { name: "Reset", icon: "refresh" }
        }
    });
}

function initButtonForMemberOT() {
    $("#memberOT #btn-submit").click(function () {
        if (validateFromDateAndToDate()) {
            loadMemberOTDetail();
        }
    });
}

function initDeleteForMemberOT() {
    $(".delete-overtime").on('click', function () {
        var requestId = $(this).attr('requestId');
        confirm('<p>Are you sure you want to delete this request?</p>', function () {
            $.ajax({
                url: _global.root + 'Overtime/Delete',
                method: "POST",
                data: {
                    id: requestId
                },
                success: function (res) {
                    if (res.IsSuccessful) {
                        loadMemberOTDetail();
                    } else {
                        notifyError(res.Message);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    notifyError(textStatus + ': ' + errorThrown);
                }
            });
        });
    });
}