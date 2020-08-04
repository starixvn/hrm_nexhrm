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
        generateFooter();
        let hasEmployee = $("tr.sortedIndex#row_" + id).length > 0;
        if (hasEmployee) {
            return;
        }
        let index = $("#timeSheets").find("tr.sortedIndex").length + 1;
        let names = nick.split(' (');
        let nickName = names[0];
        let fullName = names[1];
        let employee = {
            Id: id,
            NickName: nickName,
            FullName: fullName.substring(0, fullName.length - 1)
        }
        $.ajax({
            url: _global.root + 'Overtime/CreateEmployeeRecord',
            type: 'POST',
            dataType: 'html',
            data: {
                Employee: employee,
                Hour: tempHour,
                Date: tempDate,
                index: index
            },
            success: function (res) {
                $('#timeSheets').append(res);
                let elementRow = "tr.sortedIndex#row_" + id;
                $(elementRow).find('input.datepicker').first().attr('id', 'datepicker_' + id).attr('name', 'datepicker_' + id);
                initDatePicker();
                registerChangeNexHourEvent(elementRow, "#row_hour_" + id);           // hour
                registerChangeNexHourEvent(elementRow, '#datepicker_' + id);      // date
            },
            error: function (jqXHR, textStatus, errorThrown) {
                notifyError(textStatus + ': ' + errorThrown);
            }
        });
    }

    function registerChangeNexHourEvent(elementRow, elementInput) {
        $(elementInput).on('change keyup', function () {
            let employeeId = $($(elementRow).find("input.row_hour").first()).attr("data-id");
            let hour = $($(elementRow).find("input.row_hour").first()).val();
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
            })
        });
    }

    function generatedIndex() {
        var rowIndex = 1;
        $('.sortedIndex').each(function () {
            $(this).children('td').eq(0).html(rowIndex);
            rowIndex++;
        });
    }

    function deleteRow(id) {
        $('#row_' + id).remove();

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

    function getTimeRecord() {
        let overtimes = $("#timeSheets").find("tr.sortedIndex");
        if (overtimes.length > 0) {
            var timeRecords = [];
            for (var i = 0; i < overtimes.length; i++) {
                let employeeId = $($(overtimes[i]).find("input.row_hour").first()).attr("data-id");
                let nickName = $($(overtimes[i]).find(".row_name").first()).html().trim();
                let date = $($(overtimes[i]).find("input.datepicker").first()).val();
                let hour = $($(overtimes[i]).find("input.row_hour").first()).val();
                let nexhour = $($(overtimes[i]).find(".row_nexhour").first()).html().trim();
                let note = $($(overtimes[i]).find(".row_note").first()).val().trim();
                timeRecords.push({
                    Employee: {
                        Id: employeeId,
                        NickName: nickName
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
        $("#form").submit(function () {
            getTimeRecord();
        });
    }

    function registerInitEventExistRow() {
        let overtimes = $("#timeSheets").find("tr.sortedIndex");
        if (overtimes.length > 0) {
            for (var i = 0; i < overtimes.length; i++) {
                let id = $($(overtimes[i]).find("input.row_hour").first()).attr("data-id");
                let elementRow = "tr.sortedIndex#row_" + id;
                registerChangeNexHourEvent(elementRow, "#row_hour_" + id);          // hour
                registerChangeNexHourEvent(elementRow, '#datepicker_' + id);        // date
            }
        }
    }

    function init() {
        initEmployeeAutocomplete();
        registerInitEventExistRow();
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

//var timeSheets = [];
//function initValidationForHour() {
//    $("input[type=number]").change(function () {
//        if ($(this).val() <= 0) {
//            $(this).val(0.5);
//        } else if ($(this).val() > 8) {
//            $(this).val(8);
//        }
//    });

//    $("input[type=number]").keyup(function () {
//        if ($(this).val() <= 0) {
//            $(this).val(0.5);
//        } else if ($(this).val() > 8) {
//            $(this).val(8);
//        }
//    });
//}

//function generateFooter() {
//    if ($("#timeSheets").find("tr.sortedIndex").length === 0) {
//        $('#timeSheets').html('');
//    }
//}

//function createRow(nick, id) {
//    generateFooter();
//    let hasEmployee = $("tr.sortedIndex#row_" + id).length > 0;
//    if (hasEmployee) {
//        return;
//    }
//    let hour = 1;
//    let nexhour = 1.5;
//    let index = $("#timeSheets").find("tr.sortedIndex").length + 1;
//    let date = new Date();
//    let names = nick.split(' (');
//    let nickName = names[0];
//    let fullName = names[1];
//    let employee = {
//        Id: id,
//        NickName: nickName,
//        FullName: fullName.substring(0, fullName.length - 1)
//    }
//    $.ajax({
//        url: _global.root + 'Overtime/CreateEmployeeRecord',
//        type: 'POST',
//        dataType: 'html',
//        data: {
//            Employee: employee,
//            //Hour: hour,
//            //NexHour: nexhour,
//            //Date: date,
//            index: index
//        },
//        success: function (res) {
//            $('#timeSheets').append(res);
//            let elementRow = "tr.sortedIndex#row_" + id;
//            $(elementRow).find('input.datepicker').first().attr('id', 'datepicker_' + index).attr('name', 'datepicker_' + index);
//            initDatePicker();
//            registerChangeNexHourEvent(elementRow, "#row_hour_" + id);           // hour
//            registerChangeNexHourEvent(elementRow, '#datepicker_' + index);      // date
//        },
//        error: function (jqXHR, textStatus, errorThrown) {
//            notifyError(textStatus + ': ' + errorThrown);
//        }
//    });
//}

//function registerChangeNexHourEvent(elementRow, elementInput) {
//    $(elementInput).on('change', function () {
//        let employeeId = $($(elementRow).find("input.row_hour").first()).attr("data-id");
//        let hour = $($(elementRow).find("input.row_hour").first()).val();
//        let date = $($(elementRow).find("input.datepicker").first()).val();
//        $.ajax({
//            url: _global.root + 'Overtime/GetNexHour',
//            type: 'GET',
//            data: {
//                employeeId,
//                date,
//                hour
//            },
//            success: function (res) {
//                let nexHour = res.IsWorkDay ? hour * 1.5 : 0;
//                $(elementRow).find(".row_nexhour").first().html(nexHour);
//            },
//            error: function (jqXHR, textStatus, errorThrown) {
//                notifyError(textStatus + ': ' + errorThrown);
//            }
//        })
//    });
//}

//function generatedIndex() {
//    var rowIndex = 1;
//    $('.sortedIndex').each(function () {
//        $(this).children('td').eq(0).html(rowIndex);
//        rowIndex++;
//    });
//}

//function deleteRow(id) {
//    $('#row_' + id).remove();

//    if ($("#timeSheets").find("tr.sortedIndex").length === 0) {
//        $(noInformation(7)).appendTo('#timeSheets');
//    } else {
//        generatedIndex();
//    }
//}

//function checkExist(id) {
//    for (var i = 0; i < timeSheets.length; i++) {
//        if (timeSheets[i].EmployeeId === id) {
//            return true;
//        }
//    }

//    return false;
//}

//function initEmployeeAutocomplete() {
//    $("#Employee").autocomplete({
//        source: function (request, response) {
//            var term = request.term;
//            var index = term.indexOf(' (');

//            if (index > 0) {
//                term = term.substr(0, index);
//            }

//            $.ajax({
//                url: _global.root + 'employee/getemployees',
//                dataType: 'json',
//                data: {
//                    query: term,
//                    $top: 10
//                },
//                success: function (data) {
//                    if (data.Employees && data.Employees.length > 0) {
//                        response($.map(data.Employees, function (item) {
//                            return {
//                                label: item.NickName + ' (' + item.FullName + ')',
//                                value: item.Id
//                            };
//                        }));
//                    } else {
//                        $('#EmployeeId').val('');
//                    }
//                }
//            });
//        },
//        minLength: 1,
//        select: function (event, ui) {
//            event.preventDefault();
//            var nickName = ui.item.label;
//            $('#Employee').val(nickName);
//            $('#EmployeeId').val(ui.item.value);
//        },
//        focus: function (event, ui) {
//            event.preventDefault();
//            var nickName = ui.item.label;
//            $('#Employee').val(nickName);
//            $('#EmployeeId').val(ui.item.value);
//        },
//        close: function () {
//            var nickName = $('#Employee').val();
//            var id = $("#EmployeeId").val();
//            if (id !== "" && !checkExist(parseInt(id))) {
//                createRow(nickName, parseInt(id));
//            }

//            $('#Employee').val('');
//            $('#EmployeeId').val('');
//        }
//    });
//}

//function getTimeRecord()
//{
//    let overtimes = $("#timeSheets").find("tr.sortedIndex");
//    if (overtimes.length > 0) {
//        var timeRecords = [];
//        for (var i = 0; i < overtimes.length; i++) {
//            let employeeId = $($(overtimes[i]).find("input.row_hour").first()).attr("data-id");
//            let nickName = $($(overtimes[i]).find(".row_name").first()).html().trim();
//            let date = $($(overtimes[i]).find("input.datepicker").first()).val();
//            let hour = $($(overtimes[i]).find("input.row_hour").first()).val();
//            let nexhour = $($(overtimes[i]).find(".row_nexhour").first()).html.trim();
//            let note = $($(overtimes[i]).find(".row_note").first()).val().trim();
//            timeRecords.push({
//                Employee: {
//                    Id: employeeId,
//                    NickName: nickName
//                },
//                Date: date,
//                Hour: parseFloat(hour),
//                NexHour: nexhour,
//                Note: note
//            });
//        }

//        $("#TimeSheetData").val(JSON.stringify(timeRecords));
//    }
//    else {
//        $("#TimeSheetData").val("");
//    }
//}

$(document).ready(function () {

    manageOvertime.initModule();
    manageOvertime.notifySuccessed();

});