/*global _global, _messages, getParam, floatThead, noInformation*/
/* exported index*/
'use strict';

var rowIndex = 1,
    sorts = [];

function index() {
    return rowIndex++;
}

function query(isFirstLoad) {
    var keyword = $('#Keyword').val();

    var enable = null,
        isDelete = null,
        jobTitleId = null;

    $.ajax({
        url: _global.root + 'api/employee/getemployees',
        data: { query: keyword, teamId: $('#TeamId').val(), enable: enable, isDelete: isDelete, jobTitleId: jobTitleId, sort: JSON.stringify(sorts) },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('table#data tbody').children().remove();

            if (data.length > 0) {
                rowIndex = 1;

                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        formatDate: formatDate
                    },
                    html = tmpl.render(data, helpers);

                $(html).appendTo('table#data');

                $('.nick').tipsy({
                    className: 'tipsy-avatar',
                    gravity: $.fn.tipsy.autoNS,
                    html: true,
                    title: function () {
                        var src = _global.root + 'employee/avatar/' + $(this).attr('data-code');

                        return '<img src="' + src + '" />';
                    }
                });
            } else {
                $(noInformation(11)).appendTo('table#data');
            }

            floatThead('table#data');
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

    $('#Keyword').focus().select();
}

function eventSorting() {
    $('th.col-sorter').on('click', function () {
        var $this = $(this).find('div.sorter'),
            property = $this.attr('data-property'),
            direction, index;

        index = sorts.map(function (e) { return e.Property; }).indexOf(property);

        if ($this.hasClass('desc')) {
            $this.removeClass('asc').removeClass('desc').addClass('asc');
            direction = 'asc';
        } else if ($this.hasClass('asc')) {
            $this.removeClass('asc').removeClass('desc').addClass('desc');
            direction = 'desc';
        } else {
            $this.removeClass('asc').removeClass('desc').addClass('desc');
            direction = 'desc';
        }

        if (index !== -1) {
            sorts[index].Direction = direction;
        } else {
            sorts.push({ Property: property, Direction: direction });
        }

        query(false);
    });
}

function refreshSorting() {
    sorts = [];

    $('th.col-sorter').each(function () {
        $(this).find('div.sorter').removeClass('asc').removeClass('desc')
    });
}

function eventExportExcel() {
    $('#exportExcel').on('click', function (evt) {
        evt.preventDefault();

        var enable = null,
            isDelete = null,
            jobTitleId = null;


        window.location.href = _global.root + 'Export/EmployeeProfile?query=' + $('#Keyword').val() + '&teamId=' + $('#TeamId').val() +
            '&enable=' + enable + '&isDelete=' + isDelete + '&jobTitleId=' + jobTitleId + '&sort=' + JSON.stringify(sorts);
    });
}

$(document).ready(function () {
    $('#Keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query(false);
        }
    });

    var q = getParam('query');
    if (q !== null) {
        var decode = decodeURIComponent(q);
        $('#Keyword').val(decode);
    }

    eventSorting();
    eventExportExcel();

    //search onload
    query(true);

    $('#query').click(function (evt) {
        evt.preventDefault();
        refreshSorting();
        query(false);
    });
});