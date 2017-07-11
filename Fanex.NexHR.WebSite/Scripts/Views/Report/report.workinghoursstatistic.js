/*global _global, _messages, noInformation, dateOption, setDateTimeRange, isHRRole, totalHoursToTime, validateFromDateAndToDate, 
 floatThead, displayMinuteForTotalWorking */
/*exported index*/
'use strict';

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'report/gettotalworkinghour',
        type: 'POST',
        data: $("#form").serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#data tbody').children().remove();

            if (data && data.length > 0) {
                rowIndex = 1;

                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        displayMinuteForTotalWorking: displayMinuteForTotalWorking,
                        totalHoursToTime: totalHoursToTime,
                        isHRRole: isHRRole,
                        dateOption: $('#DateOption').val(),
                        fromDate: $('#FromDate').val(),
                        toDate: $('#ToDate').val()
                    },
                    html = tmpl.render(data, helpers);

                $(html).appendTo('table#data');

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

function initMonthPick() {
    $('#monthPickr').monPickr({
        showDateRange: false,
        sortDesc: true,
        today: new Date(),
        minDate: '2010/1/1',
        maxDate: $('#maxDate').val(),
        afterChangeMonth: function () {
            var Month = $('#monthPickr').monPickr('getMonth', 'MM'),
                Year = $('#monthPickr').monPickr('getMonth', 'yyyy');
            $('#Month').val(Month);
            $('#Year').val(Year);
        }
    });

    $('#monthPickr .monpickr-prev span.ui-icon-circle-triangle-w').empty();
    $('#monthPickr .monpickr-next span.ui-icon-circle-triangle-e').empty();

    $('.row').hover(
        function () {
            $(this).addClass('rowhover');
        }, function () {
            $(this).removeClass('rowhover');
        });
}

$(document).ready(function () {
    initMonthPick($("#hdtoday").val(), $("#hdmonth").val());

    //search onload
    //query(true);

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();

        if (validateFromDateAndToDate()) {
            query(false);
        }
    });

    $('#btnExport').click(function () {
        document.location = _global.root + "export/TotalWorkingHours?" + $("#form").serialize();
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select(); // jshint ignore:line
        $('#DateOption').attr('value', '0');
    });
});