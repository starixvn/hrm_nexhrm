/*global _global, _messages, noInformation*/
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

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'api/memo/getmemos',
        data: { category: $('#Category').val(), year: $('#Year').val() },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#memo tbody').children().remove();

            if (data.length > 0) {
                rowIndex = 1;

                $('#template').tmpl(data).appendTo('#memo');
            } else {
                $(noInformation(11)).appendTo('#memo');
            }
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
        }
    });
}

$(document).ready(function () {
    //search onload
    query(true);

    $('#submit').click(function (evt) {
        evt.preventDefault();
        query(false);
    });
});