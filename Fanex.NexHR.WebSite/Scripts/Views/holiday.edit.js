'use strict';

function setTheDate() {
    var numday = $("#numDay").val(),
        fromDate = $("#FromDate").val(),
        date = Date.parse(fromDate),
        theDate = new Date(date);

    theDate.setDate(theDate.getDate() + parseInt(numday) - 1);
    $("#ToDate").val(theDate.toString('dd MMM, yyyy'));
}

$(document).ready(function () {
    var holidayName = $("#HolidayName").val(),
        numday = $("#numDay").val(),
        fromDate = $("#FromDate").val(),
        toDate = $("#ToDate").val();

    $("#numDay").change(function () {
        setTheDate();
    });

    $("#numDay").click(function () {
        setTheDate();
    });

    $("#btnClear").click(function () {
        $("#HolidayName").val(holidayName);
        $("#ActualFromDate").val(actualFromDate);
        $("#numDay").val(numday);
        $("#FromDate").val(fromDate);
        $("#ToDate").val(toDate);
    });

    $("#FromDate, #ToDate").change(function () {
        var fromDate = Date.parse($("#FromDate").val()),
            toDate = Date.parse($("#ToDate").val()),
            dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;

        if (dayDiff === 0) {
            dayDiff = 1;
        }

        $("#numDay").val(dayDiff);
    });

    var successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + 'Holiday/Index';
                }
            );
    }
});