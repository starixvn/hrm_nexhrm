'use strict';

function LoadMemberOvertime(isFirstLoad) {
    $.ajax({
        url: _global.root + 'overtime/memberovertime',
        type: 'GET',
        dataType: 'html',
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
});