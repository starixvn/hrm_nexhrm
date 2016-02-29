$(document).ready(function () {
    initDatePicker(".datepicker");
    initSubmitButton();
    initAutocomplete("#CreatedName", "#CreatedBy");
    initAutocomplete("#Employee", "#EmployeeId");

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
    });

    loadSummary();

    bindPendingMenu();
    bindApproveMenu();
});

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
};

function bindPendingMenu() {
    var element = ".Pending";

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
};

function bindApproveMenu() {
    var element = ".Approve";
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
};

function loadSummary() {
    $('.loading-process').fadeIn(200);
    $.post(_global.root + "Overtime/MemberOvertimeData", $("#form").serialize(), function (response) {
        $("#data tbody").html(response);
        initTooltip();
        $('.loading-process').fadeOut(500);
    });
};

function initSubmitButton() {
    $("#btn-submit").click(function () {
        loadSummary();
    });
};

function initTooltip() {
    var linkDays = $(".contextmenu");

    linkDays.mouseover(function () {
        $(this).find(".drop-menu").show();
    });

    linkDays.mouseout(function () {
        $(this).find(".drop-menu").hide();
    });
}

function approveRequest(id) {
    confirm('<p>Are you sure you want to approve this request?</p>', function () {
        $('.loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/ApproveRequest", { id: id }, function (response) {
            if (response.Code == 0) {
                loadSummary();
            } else {
                $('.loading-process').fadeOut(500);
                alert(response.Message);
            }
        });
    });

};

function disapproveRequest(id) {
    confirm('<p>Are you sure you want to disapprove this request?</p>', function () {
        $('.loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/DisapproveRequest", { id: id }, function (response) {
            if (response.Code == 0) {
                loadSummary();
            } else {
                $('.loading-process').fadeOut(500);
                alert(response.Message);
            }
        });
    });
};

function rejectRequest(id) {
    confirm('<p>Are you sure you want to reject this request?</p>', function () {
        $('.loading-process').fadeIn(200);
        $.post(_global.root + "Overtime/RejectRequest", { id: id }, function (response) {
            if (response.Code == 0) {
                loadSummary();
            } else {
                $('.loading-process').fadeOut(500);
                alert(response.Message);
            }
        });
    });
};