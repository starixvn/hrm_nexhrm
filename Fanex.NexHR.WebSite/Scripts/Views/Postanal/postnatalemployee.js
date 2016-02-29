/*global _global, _messages*/
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Postnatal/ListPostnatalEmployee',
        type: 'GET',
        dataType: 'html',
        data: {
            empId: $('#EmployeeId').val() == "" ? -1 : $('#EmployeeId').val(),
            year: $('#Year').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#listPostnatalEmployee').html(response);
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

function deleteRequest(id, fromDate, toDate) {
    confirm('Are you sure you want to delete this item?', function () {
        $.post(_global.root + 'postnatal/delete', { empId: id, fromDate: fromDate, toDate: toDate }, function () {
            query(false);
        });
    });
}

$(document).ready(function () {

    query(true);

    $('#submit').on('click', function () {
        query(false);
    });
});