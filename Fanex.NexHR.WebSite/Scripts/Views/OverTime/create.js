"use strict";

var memberList = [];
function createRow(nick, id) {
    if (memberList.length == 0) {
        $('#memberList').html('');
    }
    var hour = $('#Hour').val();
    memberList.push({ EmployeeId: id, Hour: parseInt(hour) });
    var row = '<tr class="sortedIndex" id="row' + id + '">' +
                '<td>' + memberList.length + '</td>' +
                '<td>' + nick + '</td>' +
                '<td><input id="row_hour_' + id + '" type="number" value="' + hour + '" step = "0.5" /> </td>' +
                '<td><a href="javascript:;" onclick="deleteRow(' + id + ');"><i class="mdi mdi-delete mdi-dark"></i></a></td>' +
                '</tr>';
    $('#memberList').append(row);
    $("#row_hour_" + id).change(function () {
        changeHour(id, $(this).val());
    })
};

function changeHour(employeeId, value) {
    $(memberList).each(function (index, element) {
        if (element.EmployeeId == employeeId) {
            element.Hour = parseInt(value);
        }
    })
}
function deleteRow(id) {
    $('#row' + id).remove();
    $(memberList).each(function (index, element) {
        if (element.EmployeeId == id) {
            memberList.splice(index, 1);
        }
    });

    if (memberList.length == 0) {
        var row = '<tr><td colspan="6"></td></tr><tr><td colspan="6">No Employee</td></tr>';
        $('#memberList').append(row);
    } else {
        generatedIndex();
    }
}

function generatedIndex() {
    var rowIndex = 1;
    $('.sortedIndex').each(function () {
        $(this).children('td').eq(0).html(rowIndex);
        rowIndex++;
    });
}

$().ready(function () {

    initDatePicker(".datepicker");

    $("#form").submit(function () {
        if (memberList.length > 0) {
            $("#RecordOTSheet").val(JSON.stringify(memberList));
        }
    })

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
        close: function () {
            var nickName = $('#Employee').val();
            var id = $("#EmployeeId").val();
            createRow(nickName, id);
            $('#Employee').val('');
            $('#EmployeeId').val('');
        }
    });
});

