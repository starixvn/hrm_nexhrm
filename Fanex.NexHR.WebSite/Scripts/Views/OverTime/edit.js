"use strict";

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

$(document).ready(function () {

    initDatePicker(".datepicker");

    initAutocomplete("#EmployeeDisplayName", "#EmployeeId");

    initValidationForHour();
});

