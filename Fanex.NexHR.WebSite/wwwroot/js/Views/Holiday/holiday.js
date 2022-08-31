'use strict';
/*global _global:false, _messages, formatJsonDate, isHRRole, noInformation */
/*exported index, deleteHoliday*/

var rowIndex = 1;
function index() {
    return rowIndex++;
}

function deleteHoliday(id) {
    confirm('Are you sure you want to delete this holiday?', function () {
        $.ajax({
            url: _global.root + 'holiday/Delete/',
            data: { id: id },
            type: 'POST',
            dataType: 'json',
            beforeSend: function () {
                $('.loading-process').fadeIn(300);
            },
            success: function (data) {
                if (data.IsSuccessful) {
                    document.location = _global.root + 'holiday/Index';
                }
            },
            error: function () {
                alert(_messages.errorTryAgain);
            },
            complete: function () {
                $('.loading-process').fadeOut(300);
            }
        });
    });
}

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var data = "year=" + $('#year').val();
    var url = _global.root + 'holiday/getholiday?' + data;
    if (!isFirstLoad) {
        var orgUrl = _global.root + 'holiday?' + data;
        window.history.replaceState(null, null, orgUrl);
    }

    $.ajax({
        url: url,
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

function initDataFirstQuery(queryString) {
    var urlParams = new URLSearchParams(queryString);
    var year = urlParams.get('year');

    $("#year").val(year);
    $(".year .select-dropdown").val(year);

}

$(document).ready(function () {
    $('#submit').click(function () {
        query(false);
    });

    query(true);
});