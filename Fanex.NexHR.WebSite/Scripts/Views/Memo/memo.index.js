/*global _global:false, getParam:false, floatThead:false, _messages:false, moment: false*/
/* exported index*/
'use strict';
var rowIndex = 1;
function index() {
    return rowIndex++;
}

function deleteMemo(id, fileName, type) {
    confirm('Are you sure you want to delete this memo?', function () {
        $.post(_global.root + 'ajax/deletememo', { id: id }, function () {
            $('#row_' + id).remove();
            autoIndex();
            if (type === 1) {
                deleteMemoFile(fileName);
            }
        });
    });
}

function query() {
    $.ajax({
        url: _global.root + 'api/memo/getmemos',
        data: { category: $('#Category').val(), year: $('#Year').val() },
        beforeSend: function () {
            $('.loading-process').fadeIn(500);
        },
        success: function (data) {
            if (data.length > 0) {
                rowIndex = 1;
                $('#memo tbody').children().remove();
                $('#template').tmpl(data).appendTo('#memo');
            } else {
                $('#memo tbody').children().remove();
                $(noInformation(11)).appendTo('#memo');
            }

            $('.loading-process').fadeOut(500);
        },
        error: function () {
            $('.loading-process').fadeOut(500);
            alert(_messages.errorTryAgain);
        }
    });
}

$(document).ready(function () {

    //search onload
    query();

    $('#submit').click(function (evt) {
        evt.preventDefault();
        query();
    });
});