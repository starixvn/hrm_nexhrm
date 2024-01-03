
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Promotion/GetPromotions',
        dataType: 'html',
        method: "POST",
        data: {
            FromDate: $('#FromDate').val(),
            ToDate: $('#ToDate').val(),
            EmpId: $('#EmployeeId').val(),
            TeamId: $('#TeamId').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#list-promotion').html(data);

            floatThead('table#data');
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

function deletePromotion(id) {
    confirm('Are you sure you want to delete this promotion?', function () {
        $.post(_global.root + 'Promotion/Delete', { id: id},
            function (response) {
                if (response.IsSuccessful) {
                    $('#row' + id).remove();
                    autoIndex();
                }
                else {
                    alert(response.Message);
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