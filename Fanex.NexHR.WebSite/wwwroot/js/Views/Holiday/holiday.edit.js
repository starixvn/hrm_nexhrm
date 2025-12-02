'use strict';
/*global _global, _messages, actualFromDate */
/*exported index, deleteHoliday*/

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
        var fromDateVal = $("#FromDate").val(),
            toDateVal = $("#ToDate").val();

        // Check if both dates are provided
        if (!fromDateVal || !toDateVal) {
            $("#numDay").val(0);
            return;
        }

        var fromDate = new Date(fromDateVal),
            toDate = new Date(toDateVal);

        // Check if dates are valid
        if (isNaN(fromDate.getTime()) || isNaN(toDate.getTime())) {
            $("#numDay").val(dayDiff);
            return;
        }

        // Calculate difference in days (inclusive)
        var timeDiff = toDate.getTime() - fromDate.getTime();
        var dayDiff = Math.round(timeDiff / (1000 * 60 * 60 * 24)) + 1;

        // Ensure minimum of 1 day
        if (dayDiff < 1) {
            dayDiff = 1;
        }

        $("#numDay").val(dayDiff);
    });

    $('#btnAdd').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: _global.root + 'holiday/Create',
            data: $('.card-body form').serialize(),
            type: 'POST',
            beforeSend: function () {
                $('.loading-process').fadeIn(300);
            },
            success: function (data) {
                if (data.IsSuccessful) {
                    document.location = _global.root + 'holiday/Index';
                }
            },
            error: function () {
                alert(_messages.errorTryAgain);
            },
            complete: function () {
                $('.loading-process').fadeOut(300);
            }
        });
    });

    $('#btnSave').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: _global.root + 'holiday/Edit',
            data: $('.card-body form').serialize(),
            type: 'POST',
            beforeSend: function () {
                $('.loading-process').fadeIn(300);
            },
            success: function (data) {
                if (data.IsSuccessful) {
                    document.location = _global.root + 'holiday/Index';
                }
            },
            error: function () {
                alert(_messages.errorTryAgain);
            },
            complete: function () {
                $('.loading-process').fadeOut(300);
            }
        });
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