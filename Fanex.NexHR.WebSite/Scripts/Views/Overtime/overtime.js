'use strict';

function LoadOTRequest(isFirstLoad) {
    $.ajax({
        url: _global.root + 'overtime/overtimerequest',
        type: 'GET',
        dataType: 'html',
        async: !isFirstLoad,
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#OTRequest .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#OTRequest').append(response);

            InitFormOTRequest();
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('#OTRequest .loading-process').fadeOut(300);
            }
        }
    });
}

function InitFormOTRequest() {
    initDatePicker("#OTRequest .datepicker");
    InitButtonForOTRequest();
    initAutocomplete("#OTRequest #CreatedName", "#OTRequest #CreatedBy");
    initAutocomplete("#OTRequest #Employee", "#OTRequest #EmployeeId");

    $('#OTRequest #Date').on('change', function () {
        setDateTimeRange($(this).val());
    });

    LoadOTRequestDetail();
}

function LoadMemberOvertime(isFirstLoad) {
    $.ajax({
        url: _global.root + 'overtime/memberovertime',
        type: 'GET',
        dataType: 'html',
        async: !isFirstLoad,
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#memberOT .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#memberOT').append(response);

            InitFormMemberOT();
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('#memberOT .loading-process').fadeOut(300);
            }
        }
    });
}

function InitFormMemberOT() {
    initDatePicker("#memberOT .datepicker");
    InitButtonForMemberOT();
    initAutocomplete("#memberOT #CreatedName", "#memberOT #CreatedBy");
    initAutocomplete("#memberOT #Employee", "#memberOT #EmployeeId");

    $('#memberOT #Date').on('change', function () {
        setDateTimeRange($(this).val());
    });

    LoadMemberOTDetail();

    bindPendingMenu();
    bindApproveMenu();
}

function LoadMyOvertime(isFirstLoad) {
    $.ajax({
        url: _global.root + 'overtime/myovertime',
        type: 'GET',
        dataType: 'html',
        async: !isFirstLoad,
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#myOT .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#myOT').append(response);
            InitButtonForMyOT();
            LoadDetailMyOT(true);
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('#myOT .loading-process').fadeOut(300);
            }
        }
    });
}

$(document).ready(function () {
    LoadMyOvertime(true);

    LoadMemberOvertime(true)

    LoadOTRequest(true)
    //init material select
    $('select').material_select();
});