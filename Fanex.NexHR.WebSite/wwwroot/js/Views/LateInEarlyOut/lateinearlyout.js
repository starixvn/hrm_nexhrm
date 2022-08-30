'use strict';
/*global _global, _messages, autoIndex*/
/* exported deleteRequest */

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var data = "empId=" + _global.employeeId + "&year=" + $('#Year').val();
    var url = _global.root + 'LateEarly/ListLateEarly?' + data;
    if (!isFirstLoad) {
        var orgUrl = _global.root + 'LateEarly?' + "Year=" + $('#Year').val();
        window.history.replaceState(null, null, orgUrl);
    }

    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'html',
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

function initDataFirstQuery(queryString) {
    var urlParams = new URLSearchParams(queryString);
    var year = urlParams.get('Year');

    $("#Year").val(year);
    $(".year .select-dropdown").val(year);

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