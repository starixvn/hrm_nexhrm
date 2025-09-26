'use strict';
/*global _global, _messages, floatThead, initDatePicker*/

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
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var data = $('.card-container-base .form-filter').serialize();
    var teamName = $(".team .select-dropdown").val();

    var url = _global.root + 'Report/ListLateInEarlyOut?' + data;
    var orgUrl = _global.root + 'report/lateinearlyout?' + data + "&teamName=" + teamName;
    window.history.replaceState(null, null, orgUrl);

    $.ajax({
        url: url,
        type: 'POST',
        dataType: 'html',
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

function initDataFirstQuery(queryString) {
    var urlParams = new URLSearchParams(queryString);
    var Year = urlParams.get('Year');
    var TeamId = urlParams.get('TeamId');
    var Employee = urlParams.get('Employee');
    var EmployeeId = urlParams.get('EmployeeId');
    var teamName = urlParams.get('teamName');


    $("#Year").val(Year);
    $("#TeamId").val(TeamId);
    $("#Employee").val(Employee);
    $("#EmployeeId").val(EmployeeId);

    $(".team .select-dropdown").val(teamName);
    $(".year .select-dropdown").val(Year);

}

$(document).ready(function () {
    query(true);
    $('#submit').on('click', function () {
        query(false);
    });

    $('#exportExcel').on('click', function () {
        var data = $('.card-container-base .form-filter').serialize();
        var exportUrl = _global.root + 'Export/LateInEarlyOutReport?' + data;
        window.open(exportUrl, '_blank');
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