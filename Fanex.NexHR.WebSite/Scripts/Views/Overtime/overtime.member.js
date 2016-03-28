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
        $('#memberOT .loading-process').fadeOut(300);
    });
}

function rejectRequest(id) {
    confirm('<p>Are you sure you want to reject this request?</p>', function () {
        $('#memberOT .loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/RejectRequest", { id: id }, function (response) {
            if (response.IsSuccessful) {
                loadMemberOTDetail();
            } else {
                $('#memberOT .loading-process').fadeOut(500);
                alert(response.Message);
            }
        });
    });
}

function disapproveRequest(id) {
    confirm('<p>Are you sure you want to disapprove this request?</p>', function () {
        $('#memberOT .loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/DisapproveRequest", { id: id }, function (response) {
            if (response.IsSuccessful) {
                loadMemberOTDetail();
            } else {
                $('#memberOT .loading-process').fadeOut(500);
                alert(response.Message);
            }
        });
    });
}

function approveRequest(id) {
    confirm('<p>Are you sure you want to approve this request?</p>', function () {
        $('#memberOT .loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/ApproveRequest", { id: id }, function (response) {
            if (response.IsSuccessful) {
                loadMemberOTDetail();
            } else {
                $('#memberOT .loading-process').fadeOut(500);
                alert(response.Message);
            }
        });
    });

}

function processRequest(command, eventElement) {
    var requestId = eventElement.$trigger.attr('requestId');

    switch (command) {
        case 'edit':
            location.href = _global.root + "Overtime/Edit/" + requestId;
            break;
        case 'approve':
            approveRequest(requestId);
            break;
        case 'disapprove':
            disapproveRequest(requestId);
            break;
        case 'reject':
            rejectRequest(requestId);
            break;
    }
}

function bindPendingMenu() {
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
            "edit": { name: "Edit", icon: "edit", visible: !isOnlyHRRole },
        }
    });
}

function bindApproveMenu() {
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

function initButtonForMemberOT() {
    $("#memberOT #btn-submit").click(function () {
        loadMemberOTDetail();
    });
}