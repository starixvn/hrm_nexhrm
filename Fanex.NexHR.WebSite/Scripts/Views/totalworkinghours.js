/*global _labels: true, _messages: true*/
'use strict';

var rowIndex = 1;
function index() {
    return rowIndex++;
}

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

    //search onload
    //query();

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        query();
    });
});

function query() {
    $.ajax({
        url: _global.root + 'report/totalworkinghours',
        type: 'POST',
        data: $("#form").serialize(),
        beforeSend: function () {
            $('.loading-process').fadeIn(500);
        },
        success: function (data) {
            if (data.length > 0) {
                rowIndex = 1;                
                $('#data tbody').children().remove();
                $('#template').tmpl(data).appendTo('#data');
            } else {
                $('#data tbody').children().remove();
                $(noInformation(11)).appendTo('#data');
            }

            $('.loading-process').fadeOut(500);
        },
        error: function () {
            $('.loading-process').fadeOut(500);
            alert(_messages.errorTryAgain);
        }
    });
}