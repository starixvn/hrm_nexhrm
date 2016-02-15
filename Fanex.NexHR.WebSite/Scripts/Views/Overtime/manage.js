"use strict";

var timeSheets = [];
function initValidationForHour() {
    $("input[type=number]").change(function () {
        if ($(this).val() <= 0) {
            $(this).val(0.5);
        } else if ($(this).val() > 8) {
            $(this).val(8);
        }
    });

    $("input[type=number]").keyup(function () {
        if ($(this).val() <= 0) {
            $(this).val(0.5);
        } else if ($(this).val() > 8) {
            $(this).val(8);
        }
    });
}

function createRow(nick, id) {
    generateFooter();
    var hour = $('#Hour').val();
    timeSheets.push({ EmployeeId: id, DisplayName: nick, Hour: parseFloat(hour) });
    var row = '<tr class="sortedIndex" id="row_' + id + '">' +
                '<td>' + timeSheets.length + '</td>' +
                '<td  class="align-left">' + nick + '</td>' +
                '<td><input class="row_hour" id="row_hour_' + id + '" type="number" value="' + hour + '" step = "0.5" /> </td>' +
                '<td><a href="javascript:;" onclick="deleteRow(' + id + ');"><i class="mdi mdi-delete mdi-dark"></i></a></td>' +
                '</tr>';
    $('#timeSheets').append(row);
    initValidationForHour();
    $("#row_hour_" + id).change(function () {
        changeHour(id, $(this).val());
    })
};
function generateFooter() {
    if (timeSheets.length === 0) {
        $('#timeSheets').html('');
    }
};

function changeHour(employeeId, value) {
    $(timeSheets).each(function (index, element) {
        if (element.EmployeeId === employeeId) {
            element.Hour = parseFloat(value);
        }
    })
};

function deleteRow(id) {
    $('#row_' + id).remove();
    $(timeSheets).each(function (index, element) {
        if (element.EmployeeId === id) {
            timeSheets.splice(index, 1);
        }
    });

    if (timeSheets.length == 0) {
        var row = '<tr><td colspan="4"></td></tr><tr><td colspan="4">No Employee</td></tr>';
        $('#timeSheets').append(row);
    } else {
        generatedIndex();
    }
};

function generatedIndex() {
    var rowIndex = 1;
    $('.sortedIndex').each(function () {
        $(this).children('td').eq(0).html(rowIndex);
        rowIndex++;
    });
};

function checkExist(id) {
    for (var i = 0; i < timeSheets.length; i++) {
        if (timeSheets[i].EmployeeId === id) {
            return true;
        }
    }

    return false;
};

function initEmployeeAutocomplete() {
    $("#Employee").autocomplete({
        source: function (request, response) {
            var term = request.term;
            var index = term.indexOf(' (');

            if (index > 0) {
                term = term.substr(0, index);
            }

            $.ajax({
                url: _global.root + 'api/employee/getemployees',
                dataType: 'json',
                data: {
                    query: term,
                    $top: 10
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        response($.map(data, function (item) {
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
};

function bindingTimeSheetGrid() {
    var timeSheetData = $("#TimeSheetData").val();
    if (timeSheetData != "") {
        timeSheets = JSON.parse(timeSheetData);
        $("#TimeSheetData").val("");
    }
}

$(document).ready(function () {

    initDatePicker(".datepicker");

    initEmployeeAutocomplete();

    $("#form").submit(function () {
        if (timeSheets.length > 0) {
            $("#TimeSheetData").val(JSON.stringify(timeSheets));
        }
    });

    initValidationForHour();

    bindingTimeSheetGrid();
});

