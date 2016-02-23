/*global _global, _messages*/
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'LateEarly/ListLateEarly',
        type: 'GET',
        dataType: 'html',
        data: { empId: _global.employeeId },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#listLateEarly').html(response);            
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
        }
    });
}

$(document).ready(function () {

        query(true);

        $('#submit').on('click', function () {
            query(false);
        });
});