/*global _labels: true, _messages: true*/
'use strict';

function validateDate(e) {
    var fromDate = new Date($("#FromDate").val());
    var toDate = new Date($("#ToDate").val());
    var dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;
    if (dayDiff < 0) {
        alert("To Date must be equal or greater than From Date.");
        e.preventDefault();
        $("#btnSubmit").val("Submit");
    }
}

$("form").submit(function (e) {
    validateDate(e);
});

$(document).ready(function () {
    initDatePicker("#FromDate");
    initDatePicker("#ToDate");
});