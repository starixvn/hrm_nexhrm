'use strict';
/*global _global, floatThead*/
/* exported deleteRequest */

function query(isFirstLoad) {
    const data = $('.card-container-base form').serialize();
    $.ajax({
        url: _global.root + 'EmployeeWorkFromHome/GetEmployeeWorkFromHomeGrid',
        dataType: 'html',
        method: "GET",
        data: data,
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#list-workfromhome').html(data);
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

function handleSuccessfull() {
    var successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
            function () {
                document.location = _global.root + 'EmployeeWorkFromHome';
            }
        );
    }
}

function deleteRequest(id) {
    confirm('Are you sure you want to remove this employee work from home?', function () {
        $.post(_global.root + 'EmployeeWorkFromHome/Delete', { id }, function (response) {
            if (response.IsSuccessful) {
                query(false);
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
    handleSuccessfull();
});