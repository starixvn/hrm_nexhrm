'use strict';
/*global _global, _messages, autoIndex*/
/* exported deleteRequest */

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'LateEarly/ListLateEarly',
        type: 'GET',
        dataType: 'html',
        data: {
            empId: _global.employeeId,
            month: $('#Month').val(),
            year: $('#Year').val()
        },
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

function deleteRequest(id) {
    confirm('Are you sure you want to delete this request?', function () {
        $.post(_global.root + 'lateearly/delete', { id: id }, function (data) {
            if (data.IsSuccessful) {
                $('#row_' + id).remove();
                autoIndex();
            }
            else {
                alert(data.Message);
            }
        });
    });
}

$(document).ready(function () {

    query(true);

    $('#submit').on('click', function () {
        query(false);
    });
});