'use strict';
/*global _global:false, initDatePicker, initAutocomplete*/
/*exported initMonthPick*/

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
    initMonthPick();

    $('#btnAddCalendar').click(function () {
        $('#addform').dialog({
            modal: true,
            width: 410,
            height: 550,
            open: function () {
                initDatePicker();
            }
        });
    });

    $('#btnCancel').click(function () {
        $('#addform').dialog('close');
        $(".hide").className = "hide";
    });

    $('#btnAdd').click(function () {
        confirm('Are you sure you add calendars?', function () {
            $.post(_global.root + 'calendar/InsertEmployeeCalendar',
                {
                    AddEmployeeId: $.trim($("#AddEmployee").val()).length > 0 ? $("#AddEmployeeId").val() : null,
                    FromDate: $("#FromDate").val(),
                    ToDate: $("#ToDate").val(),
                    SetOffDay: $("#SetOffDay").is(':checked')
                },
                function (response) {
                    if (response.IsSuccessful) {
                        document.location = _global.root + 'calendar/workingcalendars';
                    }
                    else {
                        alert(response.Message);
                    }
                });
        });
    });

    initAutocomplete('#AddEmployee', '#AddEmployeeId');
});