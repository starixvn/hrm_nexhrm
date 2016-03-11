'use strict';
/*global _global, _messages*/

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'annualleave/employee',
        type: 'POST',
        dataType: 'html',
        data: $('.card-container-base form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#div-content').html(data);

            floatThead('table#data');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
        }
    });
}


function deleteLeaveTrans(leaveTransactionId) {
    confirm('Are you sure you want to delete this leave transaction?', function () {
        $.post(_global.root + 'annualleave/Delete', { leaveTransactionId: leaveTransactionId }, function (respone) {
            if (respone.IsSuccessful) {
                query(false);
            }
            else {
                alert(respone.Message);
            }
        });
    });
}

$(document).ready(function () {
    $('#submit').on('click', function () {
        query(false);
    });

    query(true);
});