'use strict';
/*global _global*/
function initSearch() {
    initAutocomplete('#Employee', '#EmployeeId', null, false);

    // when user clear the textbox, we must set employeeId field to -1 for seaching all employees
    $('form').submit(function () {
        var employee = $('#Employee').val();
        if (employee.length === 0) {
            $('#EmployeeId').val(-1);
        }
        return true;
    });
}

$(document).ready(function () {
    initSearch();
    var successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
            function () {
                document.location = _global.root + 'employee';
            }
        );
    }
});