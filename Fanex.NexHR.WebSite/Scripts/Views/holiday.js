'use strict';
/*global _global:false, autoIndex: false*/
/*exported index, deleteHoliday*/

var rowIndex = 1;
function index() {
    return rowIndex++;
}

function deleteHoliday(id) {
    confirm('Are you sure you want to delete this holiday?', function () {
        $.post(_global.root + 'holiday/Delete', { id: id }, function (response) {
            if (response.IsSuccessful) {
                document.location = _global.root + 'holiday/Index';
            }
            else {
                alert(response.Message);
            }
        });
    });
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'holiday/getholiday',
        data: { year: $('#year').val() },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#data tbody').children().remove();

            if (data && data.length > 0) {
                rowIndex = 1;
                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        formatDate: formatJsonDate,
                        isHRRole: isHRRole
                    },
                    html = tmpl.render(data, helpers);

                $(html).appendTo('table#data');
            } else {
                $(noInformation(6)).appendTo('#data');
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
    $('#submit').click(function () {
        query(false);
    });

    query(true);
});