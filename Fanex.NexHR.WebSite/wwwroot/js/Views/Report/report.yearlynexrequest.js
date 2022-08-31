'use strict';

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var data = "empId=" + $('#EmployeeId').val() + "&year=" + $('#Year').val() + "&teamid=" + $('#TeamId').val();
    var teamName = $(".team .select-dropdown").val();

    var url = _global.root + 'Report/GetYearlyNexRequest?' + data;
    if (!isFirstLoad) {
        var orgUrl = _global.root + 'report/yearlynexrequest?' + data + "&empName=" + $('#Employee').val() + "&teamName=" + teamName;
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
        success: function (response) {
            $('#listYearlyNexRequest').html(response);
            floatThead('table#data');
        },

        error: function () {
            notifyError(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
        }
    });
};

function initDataFirstQuery(queryString) {
    var urlParams = new URLSearchParams(queryString);
    var empId = urlParams.get('empId');
    var empName = urlParams.get('empName');
    var year = urlParams.get('year');
    var teamid = urlParams.get('teamid');
    var teamName = urlParams.get('teamName');

    $("#EmployeeId").val(empId);
    $("#Employee").val(empName);
    $("#Year").val(year);
    $("#TeamId").val(teamid);

    $(".year .select-dropdown").val(year);
    $(".team .select-dropdown").val(teamName);
}

$(document).ready(function () {
    query(true);
    $('#submit').on('click', function () {
        query(false);
    }); 
});