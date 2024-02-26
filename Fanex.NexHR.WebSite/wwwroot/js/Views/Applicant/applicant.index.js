
'use strict';

function query(isFirstLoad) {
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

function deleteApplicant(id, fileName) {
    confirm('Are you sure you want to delete this applicant?', function () {
        $.post(_global.root + 'applicants/Delete', { id: id, fileName: fileName},
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

function eventExportExcel() {
    $('#exportExcel').on('click', function (evt) {
        evt.preventDefault();

        window.location.href = _global.root + 'Export/ApplicantsReport?fullName=' + $('#FullName').val() + '&positionId=' + $('#PositionId').val()
        + '&status=' + $('#Status').val() + '&fromDate=' + $('#FromDate').val() + '&toDate=' + $('#ToDate').val();
    });
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

    query(true);
    eventExportExcel();
});