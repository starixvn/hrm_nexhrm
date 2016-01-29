$().ready(function () {
    initDatePicker(".datepicker");
    initSubmitButton();
    initAutocomplete("#CreatedName", "#CreatedBy");
    initAutocomplete("#Employee", "#EmployeeId");

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
    });
    bindPendingMenu();
    bindApproveMenu();

    loadSummary();
});

function processRequest(command, eventElement) {
    var requestId = $(eventElement).attr('requestId');

    switch (command) {
        case 'edit':
            location.href = _global.root + "OverTime/Edit/" + requestId;
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
    var element = ".Pending";

    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key) {
            processRequest(key, element);
        },
        items: {
            "approve": { name: "Approve", icon: "approve" },
            "reject": { name: "Reject", icon: "reject" },
            "edit": { name: "Edit", icon: "edit" },
        }
    });
}

function bindApproveMenu() {
    var element = ".Approve";
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key) {
            processRequest(key, element);
        },
        items: {
            "disapprove": { name: "Disapprove", icon: "reject" }
        }
    });
}


function loadSummary() {
    $('.loading-process').fadeIn(200);
    $.post(_global.root + "OverTime/SummaryResult", $("#form").serialize(), function (response) {
        $("#data tbody").html(response);
        $('.loading-process').fadeOut(500);
    });
}

function initSubmitButton() {
    $("#btn-submit").click(function () {
        loadSummary();
    });
}

function approveRequest(id) {
    confirm('<p>Are you sure you want to approve this request?</p>', function () {
        $('.loading-process').fadeIn(200);
        $.post(_global.root + "OverTime/ApproveRequest", { id: id }, function (response) {
            $('.loading-process').fadeOut(500);
            loadSummary();
        });
    });

}

function disapproveRequest(id) {
    confirm('<p>Are you sure you want to disapprove this request?</p>', function () {
        $('.loading-process').fadeIn(200);
        $.post(_global.root + "OverTime/DisapproveRequest", { id: id }, function (response) {
            $('.loading-process').fadeOut(500);
            loadSummary();
        });

    });
}

function rejectRequest(id) {
    confirm('<p>Are you sure you want to reject this request?</p>', function () {
        $('.loading-process').fadeIn(200);
        $.post(_global.root + "OverTime/RejectRequest", { id: id }, function (response) {
            $('.loading-process').fadeOut(500);
            loadSummary();
        });

    });
}