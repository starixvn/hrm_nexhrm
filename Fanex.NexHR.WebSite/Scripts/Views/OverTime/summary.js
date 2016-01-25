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
})

function processRequest(command, eventElement) {
    var requestId = $(eventElement).attr('requestId');

    switch (command) {
        case 'edit':
            location.href = _global.root + "OverTime/Edit/" + requestId;
            break;
        case 'approve':
            approveRequest(requestId);
            break;
        case 'verify':
            verifyRequest(requestId);
    }
}

function bindPendingMenu() {
    var element = ".Pending";
    if (isApproveRole) {
        $.contextMenu({
            selector: element,
            trigger: 'left',
            callback: function (key, options) {
                processRequest(key, element);
            },
            items: {
                "edit": { name: "Edit", icon: "edit"},
                "approve": { name: "Approve", icon: "approve"}
            }
        });
    }
}

function bindApproveMenu(element) {
    var element = ".Approved";
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            processRequest(key, element);
        },
        items: {
            "edit": { name: "Edit", icon: "edit", visible: isApproveRole },
            "verify": { name: "Verify", icon: "verify", visible: isHRRole }
        }
    });
}


function loadSummary() {
    $('.loading-process').fadeIn(200);
    $.post(_global.root + "OverTime/Summary", $("#form").serialize(), function (response) {
        $("#data tbody").html(response);
        $('.loading-process').fadeOut(500);
    })

}

function initSubmitButton() {
    $("#btn-submit").click(function () {
        loadSummary();
    })
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

function verifyRequest(id) {
    confirm('<p>Are you sure you want to verify this request?</p>', function () {
        $('.loading-process').fadeIn(200);
        $.post(_global.root + "OverTime/VerifyRequest", { id: id }, function (response) {
            $('.loading-process').fadeOut(500);
            loadSummary();
        });

    });
}