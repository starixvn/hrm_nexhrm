'use strict';
/*global _global, _messages, floatThead*/

function initAddLeaveDeduction() {
    $('.addLeavededuction').click(function () {
        var employeeId = $(this).attr('data-id'),
            annualBalance = $(this).attr('data-annualbalance');

        $('#EmpId').val(employeeId);
        $('#annualBalance').text(annualBalance);

        $('#insertform').dialog('open');
    });
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/ListLateInEarlyOut',
        type: 'POST',
        dataType: 'html',
        data: $('.card-container-base .form-filter').serialize(),
        beforeSend: function () {
            $('.loading-process').fadeIn(300);
        },
        success: function (response) {
            $('#list-detail').html(response);

            initAddLeaveDeduction();

            floatThead('table#data');
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            $('.loading-process').fadeOut(300);
        }
    });
}

$(document).ready(function () {
    query(true);
    $('#submit').on('click', function () {
        query(false);
    });

    $('#insertform').dialog({
        modal: true,
        autoOpen: false,
        width: 'auto',
        height: 'auto',
        open: function () {
            $('#TotalDays').val('0.5');
            initDatePicker();
        },
        show: {
            effect: "blind",
            duration: 200
        },
        hide: {
            effect: "blind",
            duration: 100
        }
    });

    $('#insertform form').submit(function (evt) {
        evt.preventDefault();

        $.post(_global.root + 'AnnualLeave/AddLeaveDeduction',
            {
                empId: $('#EmpId').val(),
                totalDays: $('#TotalDays').val(),
                deductDate: $('#DeductDate').val(),
                description: $('#Description').val(),
                isAnnual: $("#IsAnnual").prop("checked")
            }, function (response) {
                if (response.IsSuccessful) {
                    $('#insertform').dialog('close');
                    document.location = _global.root + 'report/LateInEarlyOut';
                } else {
                    alert(response.Message);
                }
            });
    });

    $('#cancel').click(function () {
        $('#insertform').dialog('close');
    });
});