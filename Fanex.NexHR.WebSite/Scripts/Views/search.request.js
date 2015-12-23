/*global _global, shiftChange*/
/*exported */
"use strict";

var nickName;

$(document).ready(function () {
    $("#Employee").autocomplete({
        source: function (request, response) {
            var term = request.term,
                index = term.indexOf(' (');

            if (index > 0) {
                term = term.substr(0, index);
            }

            $.ajax({
                url: _global.root + 'employee/GetEmployees',
                type: 'POST',
                dataType: 'json',
                data: { query: term },
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
        delay: 500,
        minLength: 1,
        select: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmployeeId').val(ui.item.value);
            var empId = parseInt($('#EmployeeId').val());

            if (empId > 0) {
                $.getJSON(_global.root + 'api/employee/GetUnavailDate', { employeeId: empId },
                    function (response) {
                        if (response != null) {
                            var forRequest = parseInt($("#ForRequest").val());
                            switch (forRequest) {
                                case 0: // leave
                                    $("#FromDate, #ToDate").attr("disableddate", response.join(","));
                                    break;
                                case 1: // late in early out
                                    $("#ForDate").attr("disabledDate", response.join(","));
                                    shiftChange();
                                    break;
                                case 2: // check in out
                                    $("#ForDate").attr("disabledDate", response.join(","));
                                    break;
                            }
                        }
                    }
                );
            }
        },
        focus: function (event, ui) {
            event.preventDefault();

            $('#Employee').val(ui.item.label);
        },
        close: function () {
            if (nickName) {
                $('#Employee').val(nickName);
            } else {
                $('#Employee').val(_global.nickName);
            }
        }
    }).autocomplete("widget").addClass("employee-autocomplete");
});