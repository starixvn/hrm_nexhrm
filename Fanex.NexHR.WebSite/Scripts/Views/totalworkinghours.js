/*global _labels: true, _messages: true*/
'use strict';

var rowIndex = 1;
var dateOption, fromDate, toDate, teamId

function index() {
    return rowIndex++;
}

function validateDate(e) {
    var fromDate = new Date($("#FromDate").val());
    var toDate = new Date($("#ToDate").val());
    var dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;
    if (dayDiff < 0) {
        alert("To Date must be equal or greater than From Date.");
        e.preventDefault();
        $("#btnSubmit").val("Submit");
    }
}

$("#Date").change(function () {
    setDateTimeRange($("#Date").val())
    dateOption = $('#Date').val();
    $(".DateOption").val($("#Date").val());

    $("#FromDate").change();
    $("#ToDate").change();    
});

$("#FromDate").change(function () {
    $(".FromDate").val($("#FromDate").val());    
});

$("#ToDate").change(function () {
    $(".ToDate").val($("#ToDate").val());
});

$("#TeamId").change(function () {
    $(".TeamId").val($("#TeamId").val());
});

$("form").submit(function (e) {
    validateDate(e);
});

$(document).ready(function () {
    initDatePicker("#FromDate");
    initDatePicker("#ToDate");

    //search onload
    query();

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        query();
    });

    $('#btnExport').click(function () {
        document.location = _global.root + "report/ExportExcel?" + $("#form").serialize();
    });    
});

function query() {
    $.ajax({
        url: _global.root + 'report/totalworkinghours',
        type: 'POST',
        data: $("#form").serialize(),
        beforeSend: function () {
            $('.loading-process').fadeIn(300);
        },
        success: function (data) {
            if (data.length > 0) {
                rowIndex = 1;
                $('#data tbody').children().remove();
                $('#template').tmpl(data).appendTo('#data');

                $(".UnpaidLeave").click(function () {
                    var modelSerialize = $(this).parent().find('#form').serialize();
                    document.location = _global.root + "request/hrview?" + modelSerialize;
                });

                $(".SickLeave").click(function () {
                    var leaveType = $(this).parent().parent().find('#unpaidLeave').find('#form').find('#LeaveTypeId');
                    leaveType.val(28);
                    var modelSerialize = leaveType.parent().serialize();
                    document.location = _global.root + "request/hrview?" + modelSerialize;
                });

                $(".AccidentLaborLeave").click(function () {
                    var leaveType = $(this).parent().parent().find('#unpaidLeave').find('#form').find('#LeaveTypeId');
                    leaveType.val(29);
                    var modelSerialize = leaveType.parent().serialize();
                    document.location = _global.root + "request/hrview?" + modelSerialize;
                });

                $(".DateOption").val($("#Date").val());
                $(".FromDate").val($("#FromDate").val());
                $(".ToDate").val($("#ToDate").val());
                $(".TeamId").val($("#TeamId").val());
            } else {
                $('#data tbody').children().remove();
                $(noInformation(11)).appendTo('#data');
            }

            $('.loading-process').fadeOut(300);

            floatThead('table#data');
        },
        error: function () {
            $('.loading-process').fadeOut(300);
            alert(_messages.errorTryAgain);
        }
    });
}