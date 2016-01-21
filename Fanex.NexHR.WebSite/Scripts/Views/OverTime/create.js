"use strict"

var memberList = [];
function createRow(nick, id) {
    var hour = $('#Hour').val();
    memberList.push({ EmployeeId: id, Hour: parseInt(hour) });
    var row = '<tr class="sortedIndex" id="row' + id + '">' +
                '<td>' + memberList.length + '</td>' +
                '<td>' + nick + '</td>' +
                '<td><input type="number" value="' + hour + '" step = "0.5" /> </td>' +
                '<td><a href="javascript:;" onclick="deleteRow(' + id + ');"><i class="mdi mdi-delete mdi-dark"></i></a></td>' +
                '</tr>';
    $('#memberList').append(row);
};

function deleteRow(id) {
    $('#row' + id).remove();
    $(memberList).each(function (index, element) {
        if (element.EmployeeId == id) {
            memberList.splice(index, 1);
        }
    });

    generatedIndex();
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
        $("#Records").val(JSON.stringify(memberList));
    })
    //    $("#save").click(function () {
    //        var data = { FromDate: $("#FromDate").val(), Description: $("#Description").val(), Records: memberList };

    //        $.ajax({
    //            contentType: 'application/json; charset=utf-8',
    //            dataType: 'json',
    //            type: 'POST',
    //            url: '/OverTime/Create',
    //            data: JSON.stringify({ model: data })
    //        });

    //    });

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
        close: function (event, ui) {
            var nickName = $('#Employee').val();
            var id = $("#EmployeeId").val();
            createRow(nickName, id);
            $('#Employee').val('');
            $('#EmployeeId').val('');
        }
    });
});

