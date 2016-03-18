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
    var row = '<tr class="sortedIndex" id="row_' + id + '">' +
                '<td>' + ($("#timeSheets").find("tr.sortedIndex").length + 1) + '</td>' +
                '<td  class="align-left row_name">' + nick + '</td>' +
                '<td><input class="row_hour" data-id="'+ id +'" id="row_hour_' + id + '" type="number" value="' + hour + '" step = "0.5" /> </td>' +
                '<td><a href="javascript:;" onclick="deleteRow(' + id + ');"><i class="mdi mdi-delete mdi-dark"></i></a></td>' +
                '</tr>';
    $('#timeSheets').append(row);
    initValidationForHour();
};
function generateFooter() {
    if ($("#timeSheets").find("tr.sortedIndex").length === 0) {
        $('#timeSheets').html('');
    }
};

function deleteRow(id) {
    $('#row_' + id).remove();

    if ($("#timeSheets").find("tr.sortedIndex").length === 0) {
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
                url: _global.root + 'employee/getemployees',
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

function getTimeRecord()
{
    var overtimes = $("#timeSheets").find("tr.sortedIndex");
    if (overtimes.length > 0) {
        var timeRecords = [];
        for (var i = 0; i < overtimes.length; i++) {
            var employeeId = $($(overtimes[i]).find("input.row_hour").first()).attr("data-id");
            var hour = $($(overtimes[i]).find("input.row_hour").first()).val();
            var nickName = $($(overtimes[i]).find(".row_name").first()).html().trim();
            timeRecords.push({
                Employee: {
                    Id: employeeId,
                    NickName: nickName
                }, Hour: parseFloat(hour)
            });
        }

        $("#TimeSheetData").val(JSON.stringify(timeRecords));
    }
    else {
        $("#TimeSheetData").val("");
    }
}

$(document).ready(function () {

    initDatePicker(".datepicker");

    initEmployeeAutocomplete();

    $("#form").submit(function () {
        getTimeRecord();
    });

    initValidationForHour();

    var successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + 'Overtime/Index';
                    //$('ul.tabs').tabs('select_tab', 'memberOT');
                }
            );
    }
});