
'use strict';

var canNotUpdate = function (id) {
    $('#txtJobTile_' + id).val($('#spanJobTitle_' + id).text());
};

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'applicants/index',
        dataType: 'html',
        method: "POST",
        data: { jobTitle: $('#keyword').val() },
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

function deleteJobTitle(id) {
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
}

$(document).ready(function () {
    $('#keyword').keyup(function (event) {
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