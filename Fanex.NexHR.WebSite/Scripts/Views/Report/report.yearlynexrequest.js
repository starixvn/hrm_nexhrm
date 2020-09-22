'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/GetYearlyNexRequest',
        type: 'POST',
        dataType: 'html',
        data: {
            empId: $('#EmployeeId').val(),
            teamid: $('#TeamId').val(),
            year: $('#Year').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#listYearlyNexRequest').html(response);
            floatThead('table#data');
        },

        error: function () {
            notifyError(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
        }
    });
};

$(document).ready(function () {
    query(true);
    $('#submit').on('click', function () {
        query(false);
    }); 
});