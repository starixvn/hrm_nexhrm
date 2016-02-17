/*global _global, _messages, noInformation*/
/* exported index*/
'use strict';

var rowIndex = 1;
function index() {
    return rowIndex++;
}

function deleteMemo(id, fileName, type) {
    confirm('Are you sure you want to delete this memo?', function () {
        $.post(_global.root + 'memo/deletememo', { id: id }, function () {
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
        data: { category: $('#Category').val(), year: $('#Year').val(), status: $('#Status').val() },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('table#memo tbody').children().remove();

            if (data && data.length > 0) {
                rowIndex = 1;

                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        formatDate: formatDate,
                        isHRRole: isHRRole
                    },
                    html = tmpl.render(data, helpers);

                $(html).appendTo('table#memo');
            } else {
                $(noInformation(11)).appendTo('table#memo');
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