/*global _global, _messages*/
/*exported index*/
'use strict';

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function validateDate() {
    var fromDate = new Date($("#FromDate").val()),
        toDate = new Date($("#ToDate").val());

    if (toDate < fromDate) {
        alert("To Date must be equal or greater than From Date.");
        return false;
    }

    return true;
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'report/totalworkinghours',
        type: 'POST',
        data: $("#form").serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#data tbody').children().remove();

            if (data.length > 0) {
                rowIndex = 1;

                $('#template').tmpl(data).appendTo('table#data');

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
            } else {
                $(noInformation(15)).appendTo('table#data');
            }

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

$(document).ready(function () {
    initDatePicker();

    $("#Date").change(function () {
        setDateTimeRange($("#Date").val());
        dateOption = $('#Date').val();
        $(".DateOption").val($("#Date").val());
    });

    //search onload
    query(true);

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        if (validateDate()) {
            dateOption = $('#DateOption').val();
            fromDate = $('#FromDate').val();
            toDate = $('#ToDate').val();

            query(false);
        }
    });

    $('#btnExport').click(function () {
        document.location = _global.root + "report/ExportExcel?" + $("#form").serialize();
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select();
        $('#DateOption').attr('value', '0');
    });
});