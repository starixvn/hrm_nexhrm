'use strict';

function validateHoliday(e) {
    var holidayName = $("#HolidayName").val();
    var fromDate = new Date($("#FromDate").val());
    var toDate = new Date($("#ToDate").val());
    var dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;
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
    initDatePicker('#FromDate');
    initDatePicker('#ToDate');

    var holidayName = $("#HolidayName").val();
    var numday = $("#numDay").val();
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();

    $("#numDay").change(function () {
        setTheDate();
    });

    $("#numDay").click(function () {
        setTheDate();
    });

    function setTheDate() {
        var numday = $("#numDay").val();
        var fromDate = $("#FromDate").val();
        var dat = Date.parse(fromDate);
        var theDate = new Date(dat);
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
        var fromDate = Date.parse($("#FromDate").val());
        var toDate = Date.parse($("#ToDate").val());
        var dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;
        if (dayDiff === 0) {
            dayDiff = 1;
        }
        $("#numDay").val(dayDiff);
    });
});
