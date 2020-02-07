/*global _global, _messages, noInformation, dateOption, setDateTimeRange, isHRRole, totalHoursToTime, validateFromDateAndToDate, 
 floatThead, displayMinuteForTotalWorking */
/*exported index*/
'use strict';

var rowIndex = 1,
    sorts = [];

function index() {
    return rowIndex++;
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'report/gettotalworkinghour',
        type: 'POST',
        data: $("#form").serialize() + '&sort=' + JSON.stringify(sorts),
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

function eventSorting() {
    $('th.col-sorter').on('click', function () {
        var $this = $(this).find('div.sorter'),
            property = $this.attr('data-property'),
            direction, index;

        index = sorts.map(function (e) { return e.Property; }).indexOf(property);

        if ($this.hasClass('desc')) {
            $this.removeClass('desc').addClass('asc');
            direction = 'asc';
        } else if ($this.hasClass('asc')) {
            $this.removeClass('asc').addClass('desc');
            direction = 'desc';
        } else {
            refreshSorting();
            $this.addClass('desc');
            direction = 'desc';
        }

        if (index !== -1) {
            sorts[index].Direction = direction;
        } else {
            sorts.shift();
            sorts.push({ Property: property, Direction: direction });
        }

        query(false);
    });
}

function refreshSorting() {
    sorts = [];

    $('th.col-sorter').each(function () {
        $(this).find('div.sorter').removeClass('asc').removeClass('desc');
    });
}

$(document).ready(function () {
    $("#Date").change(function () {
        setDateTimeRange($("#Date").val());
        dateOption = $('#Date').val();  // jshint ignore:line
        $(".DateOption").val($("#Date").val());
    });

    eventSorting();

    //search onload
    query(true);

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        refreshSorting();
        if (validateFromDateAndToDate()) {
            query(false);
        }
    });

    $('#btnExport').click(function () {
        var date = 0;
        document.location = _global.root + "export/TotalWorkingHours?DateTimeNow=" + $('#DateTimeNow').val() + '&Date=' + date + '&DateOption=' + $('#DateOption').val() + '&FromDate=' + $('#FromDate').val() + '&ToDate=' + $('#ToDate').val() + '&TeamId=' + $('#TeamId').val() + '&Employee=' + $('#Employee').val() + '&EmployeeId=' + $('#EmployeeId').val() + '&IsExportExcel=' + false + '&sort=' + JSON.stringify(sorts);
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select(); // jshint ignore:line
        $('#DateOption').attr('value', '0');
    });
});