'use strict';

function validateHoliday(e) {
    var holidayName = $("#HolidayName").val(),
        fromDate = new Date($("#FromDate").val()),
        toDate = new Date($("#ToDate").val()),
        dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;

    if (holidayName == null || holidayName === "") {
        alert("Holiday Name is required!");
        e.preventDefault();
    }

    if (dayDiff < 0) {
        alert("To date must be equal or greater than From date.");
        e.preventDefault();
    }
}

$(document).ready(function () {
    initDatePicker();

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

    function setTheDate() {
        var numday = $("#numDay").val(),
            fromDate = $("#FromDate").val(),
            date = Date.parse(fromDate),
            theDate = new Date(date);

        theDate.setDate(theDate.getDate() + parseInt(numday) - 1);
        $("#ToDate").val(theDate.toString('dd MMM, yyyy'));
    }

    $("#btnClear").click(function () {
        $("#HolidayName").val(holidayName);
        $("#ActualFromDate").val(actualFromDate);
        $("#numDay").val(numday);
        $("#FromDate").val(fromDate);
        $("#ToDate").val(toDate);
    });

    $("form").submit(function (e) {
        validateHoliday(e);
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
});