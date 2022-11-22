/*global _global, shiftChange, initDatePicker*/
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
                dataType: 'json',
                data: { query: term, isSpecial: false },
                success: function (data) {
                    if (data.Employees && data.Employees.length > 0) {
                        response($.map(data.Employees, function (item) {
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
                $.getJSON(_global.root + 'request/GetUnavailDate', { employeeId: empId },
                    function (response) {
                        if (response != null) {
                            var forRequest = parseInt($("#ForRequest").val());

                            switch (forRequest) {
                                case 0: // leave
                                    $("#FromDate, #ToDate").attr("disableddate", response.join(","));
                                    $("#FromDate, #ToDate").change();
                                    break;
                                case 1: // late in early out
                                    $("#ForDate").attr("disabledDate", response.join(","));
                                    $("#ForDate").change();
                                    shiftChange();
                                    break;
                                case 2: // check in out
                                    $("#ForDate").change();
                                    break;
                            }

                            initDatePicker();
                        }
                    }
                );
            }
        },
        focus: function (event, ui) {
            $('#Employee').val(ui.item.label);
            event.preventDefault();
        },
        close: function () {
            if (nickName) {
                $('#Employee').val(nickName);
            } else {
                $('#Employee').val(_global.nickName);
            }
        }
    });
});