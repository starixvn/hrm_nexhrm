/*global _global, _messages, noInformation, autoIndex, formatJsonDate, isHRRole, floatThead */
/* exported index, deleteMemo*/
'use strict';

var rowIndex = 1;
function index() {
    return rowIndex++;
}

function deleteMemo(id) {
    confirm('Are you sure you want to delete this memo?', function () {
        $.post(_global.root + 'memo/deletememo', { id: id }, function (response) {
            if (response.IsSuccessful) {
                $('#row_' + id).remove();
                autoIndex();
                alert("Successfull!");
            }
            else {
                alert(response.Message);
            }
        });
    });
}

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var statusUrl = $('#Status').val() === undefined ? -1 : $('#Status').val();
    var data = "category=" + $('#Category').val() + "&year=" + $('#Year').val() + "&status=" + statusUrl;
    var categoryValue = $(".category .select-dropdown").val();
    var statusValue = $(".status .select-dropdown").val();
    var yearValue = $(".year .select-dropdown").val();

    var url = _global.root + 'memo/getmemos?' + data;
    if (!isFirstLoad) {
        var orgUrl = _global.root + 'memo?' + data + "&categoryValue=" + categoryValue + "&statusValue=" + statusValue + "&yearValue=" + yearValue;
        window.history.replaceState(null, null, orgUrl);
    }

    $.ajax({
        url: url,
        method: "POST",
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
                        formatDate: formatJsonDate,
                        isHRRole: isHRRole
                    },
                    html = tmpl.render(data, helpers);

                $(html).appendTo('table#memo tbody');
            } else {
                $(noInformation(11)).appendTo('table#memo');
            }

            floatThead('table#memo');
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
    var category = urlParams.get('category');
    var year = urlParams.get('year');
    var status = urlParams.get('status');
    var categoryValue = urlParams.get('categoryValue');
    var statusValue = urlParams.get('statusValue');
    var yearValue = urlParams.get('yearValue');

    $("#Category").val(category);
    $("#Year").val(year);
    $("#Status").val(status);


    $(".year .select-dropdown").val(yearValue);
    $(".category .select-dropdown").val(categoryValue);
    $(".status .select-dropdown").val(statusValue);

}

$(document).ready(function () {
    //search onload
    query(true);

    $('#submit').click(function (evt) {
        evt.preventDefault();
        query(false);
    });
});