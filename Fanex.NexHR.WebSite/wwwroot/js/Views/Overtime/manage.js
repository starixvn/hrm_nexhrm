"use strict";
/*global _global, initDatePicker */
/* exported deleteRow */

var manageOvertime = (function ($) {
    let todayArray = (new Date()).toDateString().split(" ");
    var tempDate = todayArray[2] + ' ' + todayArray[1] + ', ' + todayArray[3];
    var tempHour = 1;

    var timeSheets = [];

    function generateFooter() {
        if ($("#timeSheets").find("tr.sortedIndex").length === 0) {
            $('#timeSheets').html('');
        }
    }

    function createRow(nick, id) {
        var tempHour = 1;

        generateFooter();
        let index = $("#timeSheets").find("tr.sortedIndex").length + 1;
        let names = nick.split(' (');
        let nickName = names[0];
        let fullName = names[1];
        let employee = {
            Id: id,
            NickName: nickName,
            FullName: fullName.substring(0, fullName.length - 1)
        }
        let unique = new Date().getTime();
        $.ajax({
            url: _global.root + 'Overtime/CreateEmployeeRecord',
            type: 'POST',
            dataType: 'html',
            data: {
                Employee: employee,
                Hour: tempHour,
                Date: tempDate,
                Unique: new Date().getTime(),
                index: index
            },
            success: function (res) {
                $('#timeSheets').append(res);
                let elementRow = "tr.sortedIndex#row_" + unique;
                $(elementRow).find('input.datepicker').first().attr('id', 'datepicker_' + unique).attr('name', 'datepicker_' + unique);
                initDatePicker();
                registerChangeNexHourEvent(elementRow, "#row_hour_" + unique);           // hour
                registerChangeNexHourEvent(elementRow, '#datepicker_' + unique);      // date
            },
            error: function (jqXHR, textStatus, errorThrown) {
                notifyError(textStatus + ': ' + errorThrown);
            }
        });
    }

    function getNexHour(elementRow) {
        let employeeId = $($(elementRow).find("input.row_hour").first()).attr("data-id");
        let hour = +$($(elementRow).find("input.row_hour").first()).val();
        const roundedHour = Math.round(hour * 2) / 2;
        if (hour < 0.5 || hour > 24 || roundedHour !== hour) {
            $(elementRow).find(".row_nexhour").first().html(0);
            tempHour = roundedHour;
            $($(elementRow).find("input.row_hour").first()).addClass('input-validation-error').removeClass('valid').attr('aria-invalid', false);
            return;
        } else {
            $($(elementRow).find("input.row_hour").first()).removeClass('input-validation-error').addClass('valid').attr('aria-invalid', true);
        }
        let date = $($(elementRow).find("input.datepicker").first()).val();
        $.ajax({
            url: _global.root + 'Overtime/GetNexHour',
            type: 'GET',
            data: {
                employeeId,
                date,
                hour
            },
            success: function (res) {
                let duration = $($(elementRow).find("input.row_hour").first()).val();
                let nexHour = res.IsWorkDay ? duration * 1.5 : 0;
                $(elementRow).find(".row_nexhour").first().html(nexHour);
                tempHour = duration;
                tempDate = $($(elementRow).find("input.datepicker").first()).val();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                notifyError(textStatus + ': ' + errorThrown);
            }
        });
    }

    function registerChangeNexHourEvent(elementRow, elementInput) {
        $(elementInput).on('change keyup', function () {
            getNexHour(elementRow);
        });
    }

    function generatedIndex() {
        var rowIndex = 1;
        $('.sortedIndex').each(function () {
            $(this).children('td').eq(0).html(rowIndex);
            rowIndex++;
        });
    }

    function deleteRow(unique) {
        $('#row_' + unique).remove();

        if ($("#timeSheets").find("tr.sortedIndex").length === 0) {
            $(noInformation(7)).appendTo('#timeSheets');
        } else {
            generatedIndex();
        }
    }

    function checkExist(id) {
        for (var i = 0; i < timeSheets.length; i++) {
            if (timeSheets[i].EmployeeId === id) {
                return true;
            }
        }

        return false;
    }

    function initEmployeeAutocomplete() {
        $("#Employee").autocomplete({
            source: function (request, response) {
                var term = request.term;
                var index = term.indexOf(' (');

                if (index > 0) {
                    term = term.substr(0, index);
                }

                $.ajax({
                    url: _global.root + 'employee/getemployees',
                    dataType: 'json',
                    data: {
                        query: term,
                        isSpecial: false,
                        $top: 10
                    },
                    success: function (data) {
                        if (data.Employees && data.Employees.length > 0) {
                            response($.map(data.Employees, function (item) {
                                return {
                                    label: item.NickName + ' (' + item.FullName + ')',
                                    value: item.Id
                                };
                            }));
                        } else {
                            $('#EmployeeId').val('');
                        }
                    }
                });
            },
            minLength: 1,
            select: function (event, ui) {
                event.preventDefault();
                var nickName = ui.item.label;
                $('#Employee').val(nickName);
                $('#EmployeeId').val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                var nickName = ui.item.label;
                $('#Employee').val(nickName);
                $('#EmployeeId').val(ui.item.value);
            },
            close: function () {
                var nickName = $('#Employee').val();
                var id = $("#EmployeeId").val();
                if (id !== "" && !checkExist(parseInt(id))) {
                    createRow(nickName, parseInt(id));
                }

                $('#Employee').val('');
                $('#EmployeeId').val('');
            }
        });
    }

    function getTimeRecord(event) {
        let overtimes = $("#timeSheets").find("tr.sortedIndex");
        if (overtimes.length > 0) {
            var timeRecords = [];
            for (var i = 0; i < overtimes.length; i++) {
                let employeeId = $($(overtimes[i]).find("input.row_hour").first()).attr("data-id");
                let nick = $($(overtimes[i]).find(".row_name").first()).html().trim();
                let names = nick.split(' (');
                let nickName = names[0];
                let fullname = names[1].substring(0, names[1].length - 1);
                let date = $($(overtimes[i]).find("input.datepicker").first()).val();
                let hour = +$($(overtimes[i]).find("input.row_hour").first()).val();
                const roundedHour = Math.round(hour * 2) / 2;
                if (roundedHour !== hour) {
                    if ($('#form').find('.validation-summary-errors').length <= 0) {
                        $('#form').find('.validation-summary-valid').first().removeClass('validation-summary-valid').addClass('validation-summary-errors');
                    }
                    if ($('#form').find('.validation-summary-errors').find('ul li.record-error').length === 0)
                        $('#form').find('.validation-summary-errors').find('ul').first().append("<li class='record-error'>Some record invalid</li>");
                    $($(overtimes[i]).find("input.row_hour").first()).addClass('input-validation-error').removeClass('valid').attr('aria-invalid', false);
                    event.preventDefault();
                }
                let nexhour = $($(overtimes[i]).find(".row_nexhour").first()).html().trim();
                let note = $($(overtimes[i]).find(".row_note").first()).val().trim();
                timeRecords.push({
                    Employee: {
                        Id: employeeId,
                        NickName: nickName,
                        FullName: fullname
                    },
                    Date: date,
                    Hour: parseFloat(hour),
                    NexHour: nexhour,
                    Note: note
                });
            }

            $("#TimeSheetData").val(JSON.stringify(timeRecords));
        }
        else {
            $("#TimeSheetData").val("");
        }
    }

    function registersubmit() {
        $("#form").submit(function (event) {
            getTimeRecord(event);
        });
    }

    function initExistRow() {
        let overtimes = $("#timeSheets").find("tr.sortedIndex");
        if (overtimes.length > 0) {
            for (var i = 0; i < overtimes.length; i++) {
                let unique = $(overtimes[i]).attr('data-unique');
                let elementRow = "tr.sortedIndex#row_" + unique;
                $(elementRow).find('input.datepicker').first().attr('id', 'datepicker_' + unique).attr('name', 'datepicker_' + unique);
                $(elementRow).find('input.datepicker').parent().addClass('datepicker-button');
                $(elementRow).find('input.row_hour').attr("type", "number");
                registerChangeNexHourEvent(elementRow, "#row_hour_" + unique);          // hour
                registerChangeNexHourEvent(elementRow, '#datepicker_' + unique);        // date
                getNexHour(elementRow);
            }
            initDatePicker();
        }
    }

    function init() {
        initEmployeeAutocomplete();
        initExistRow();
        registersubmit();
    }

    function notifySuccessed() {
        var successed = $('#successed').val();
        if (successed === 'true') {
            confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + 'Overtime/Index?chosenTab=1';
                }
            );
        }
    }

    return {
        initModule: init,
        notifySuccessed,
        deleteRow
    }

})(jQuery);

function deleteRow(id) {
    manageOvertime.deleteRow(id);
}

$(document).ready(function () {
    manageOvertime.initModule();
    manageOvertime.notifySuccessed();
});