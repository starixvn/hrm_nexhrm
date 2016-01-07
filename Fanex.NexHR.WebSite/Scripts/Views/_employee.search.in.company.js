'use strict';
/*global _global:false*/
/*exported submitForm*/

$(document).ready(function () {
    $("#Employee").autocomplete({
        source: function (request, response) {
            var term = request.term;
            var index = term.indexOf(' (');

            if (index > 0) {
                term = term.substr(0, index);
            }

            $.ajax({
                url: _global.root + 'ajax/getemployees',
                type: 'POST',
                dataType: 'json',
                data: {
                    query: term,
                    $top: 10
                },
                success: function (data) {
                    if (data.employees && data.employees.length > 0) {
                        response($.map(data.employees, function (item) {
                            return {
                                label: item.NickName + ' (' + item.FullName + ')',
                                value: item.Id
                            };
                        }));
                    }
                    else {
                        $('#EmployeeId').val('0');
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
        change: function (event, ui) {
            if (ui && ui.item) {
                var nickName = ui.item.label;
                $('#Employee').val(nickName);
                $('#EmployeeId').val(ui.item.value);
            } else {
                $('#EmployeeId').val('0');
            }
        }
    });

    // when user clear the textbox, we must set employeeId field to -1 for seaching all employees
    $('form').submit(function () {
        var employee = $('#Employee').val();
        if (employee.length === 0) {
            $('#EmployeeId').val(-1);
        }
        return true;
    });
});

function submitForm(submitType) {
    $("#IsExportExcel").val(submitType);
}