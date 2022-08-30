'use strict';
/*global _global, floatThead*/
/*exported deleteLeaveTrans*/

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var employeeId = $('#EmployeeId').val();
    var year = $('#YearEdit').val();
    var isIncludeRequest = $('#ExcludedRequestTrans').prop("checked")
    var url = _global.root + 'annualleave/employee?employeeId=' + employeeId + '&YearEdit=' + year + '&ExcludedRequestTrans=' + isIncludeRequest;
    if (!isFirstLoad) {
        var orgUrl = url;
        window.history.replaceState(null, null, orgUrl);
    }

    $.ajax({
        url: url,
        type: 'POST',
        dataType: 'html',
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

function initDataFirstQuery(queryString) {
    var urlParams = new URLSearchParams(queryString);
    var employeeIdParam = urlParams.get('employeeId');
    var yearParam = urlParams.get('YearEdit');
    var includeRequestParam = urlParams.get('ExcludedRequestTrans');
    var isIncludeRequestParam = !includeRequestParam ? false : includeRequestParam.toLowerCase() == 'true';
    $("#YearEdit").val(yearParam);
    $(".select--year .select-dropdown").val(yearParam);
    $("#EmployeeId").val(employeeIdParam);
    $("#ExcludedRequestTrans").prop('checked', isIncludeRequestParam);
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