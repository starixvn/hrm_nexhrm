/*global _global, _messages, setDateTimeRange, floatThead, validateFromDateAndToDate*/
/*exported*/
'use strict';

function query(isFirstLoad) {
    var isLeader = $('#IsLeader').val();

    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var data = $('.card-container-base form.form-filter').serialize();

    var list = isLeader ? 'ListLeaderRequestMonitoring?' : 'ListHRRequestMonitoring?'
    var actionOrgUrl = isLeader ? 'LeaderRequestMonitoring?' : 'HRRequestMonitoring?'
    var url = _global.root + 'report/' + list + data;
    var selectedDateRange = $(".date .select-dropdown").val();
    var rqType = $(".rq-type .select-dropdown").val();
    var maxRequestValue = $(".maxrq .select-dropdown").val();

    if (!isFirstLoad) {
        var orgUrl = _global.root + 'report/' + actionOrgUrl + data + '&DateRange=' + selectedDateRange + '&RqType=' + rqType + '&MaxRequestValue=' + maxRequestValue;
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
        success: function (data) {
            $('#request-monitoring-detail').html(data);
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
    var isLeader = urlParams.get('IsLeader');
    var date = urlParams.get('Date');
    var dateOption = urlParams.get('DateOption');
    var dateRange = urlParams.get('DateRange');
    var rqType = urlParams.get('RqType');
    var fromDate = urlParams.get('FromDate');
    var toDate = urlParams.get('ToDate');
    var requestType = urlParams.get('RequestType');
    var maxRequest = urlParams.get('MaxRequest');
    var maxRequestValue = urlParams.get('MaxRequestValue');

    $("#IsLeader").val(isLeader);
    $("#Date").val(date);
    $("#DateOption").val(dateOption);
    $("#FromDate").val(fromDate);
    $("#ToDate").val(toDate);
    $("#RequestType").val(requestType);
    $("#MaxRequest").val(maxRequest);

    $(".date .select-dropdown").val(dateRange);
    $(".rq-type .select-dropdown").val(rqType);
    $(".maxrq .select-dropdown").val(maxRequestValue);
}

$(document).ready(function () {
    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });
    $('#Date').val($('#DateOption').val()).change();

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select(); // jshint ignore:line
        $('#DateOption').attr('value', '0');
    });

    $('#submit').click(function () {
        if (validateFromDateAndToDate()) {
            query(false);
        }
    });

    query(true);
});