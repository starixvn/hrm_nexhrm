'use strict';
/*global _global:false */

$(document).ready(function () {
    initAutocomplete('#Employee', '#EmployeeId');

    // when user clear the textbox, we must set employeeId field to -1 for seaching all employees
    $('form').submit(function () {
        var employee = $('#Employee').val();
        if (employee.length === 0) {
            $('#EmployeeId').val(-1);
        }
        return true;
    });
});