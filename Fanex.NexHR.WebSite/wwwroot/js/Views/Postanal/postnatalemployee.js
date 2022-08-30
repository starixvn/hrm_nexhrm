/*global _global, _messages*/
/* exported deleteRequest */
'use strict';

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var empIdUrl = $('#EmployeeId').val() === "" ? -1 : $('#EmployeeId').val();
    var data = "empId=" + empIdUrl + "&year=" + $('#Year').val();

    var url = _global.root + 'Postnatal/ListPostnatalEmployee?' + data;
    if (!isFirstLoad) {
        var orgUrl = _global.root + 'Postnatal?' + data + "&empName=" + $('#Employee').val();
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
            $('#listPostnatalEmployee').html(response);
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

function initDataFirstQuery(queryString) {
    var urlParams = new URLSearchParams(queryString);
    var empId = urlParams.get('empId');
    var empName = urlParams.get('empName');
    var year = urlParams.get('year');

    $("#EmployeeId").val(empId);
    $("#Employee").val(empName);
    $("#Year").val(year);

    $(".year .select-dropdown").val(year);
}

function deleteRequest(id, fromDate, toDate) {
    confirm('Are you sure you want to delete this item?', function () {
        $.post(_global.root + 'postnatal/delete', { empId: id, fromDate: fromDate, toDate: toDate }, function (response) {
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

    query(true);

    $('#submit').on('click', function () {
        query(false);
    });
});