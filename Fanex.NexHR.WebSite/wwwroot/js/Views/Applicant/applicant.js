
'use strict';

function query(isFirstLoad) {
    console.log($('#FromDate').val());

    $.ajax({
        url: _global.root + 'applicants/index',
        dataType: 'html',
        method: "POST",
        data: {
            FullName: $('#FullName').val(),
            PositionId: $('#PositionId').val(),
            Status: $('#Status').val(),
            FromDate: $('#FromDate').val(),
            ToDate: $('#ToDate').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#applicants-detail').html(data);

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

function deleteApplicant(id) {
    confirm('Are you sure you want to delete?', function () {
        $.post(_global.root + 'applicants/Delete',
            { id: id },
            function (response) {
                if (response.IsSuccessful) {
                    query(false);
                }
                else {
                    alert(response.Message);
                }
            }).error(function () {
                alert(_messages.errorTryAgain);
            });
    });

    // confirm('Are you sure you want to delete this team?', function () {
    //     $.post(_global.root + 'team/Delete', { id: id },
    //     function (response) {
    //         if (response.IsSuccessful) {
    //             $('#row' + id).remove();
    //             autoIndex();
    //         }
    //         else {
    //             alert(response.Message);
    //         }
    //     });
    // });
}

$(document).ready(function () {
    $('#FullName').keyup(function (event) {
        if (event.keyCode === 13) {
            query(false);
        }
    });

    $('#submit').on('click', function () {
        query(false);
    });

    var q = getParam('query');
    if (q != null) {
        $('#keyword').val(q);
    }

    query(true);
});