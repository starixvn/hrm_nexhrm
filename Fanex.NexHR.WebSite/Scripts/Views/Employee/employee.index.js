/*global _global:false, getParam:false, floatThead:false, _messages:false, moment: false*/
/* exported index*/
'use strict';

var rowIndex = 1,
    sorts = [];
function index() {
    return rowIndex++;
}

function query() {
    var keyword = $('#keyword').val();

    var enable = null,
        isDelete = null,
        jobTitleId = null;

    $.ajax({
        url: _global.root + 'api/employee/getemployees',
        data: { query: keyword, groupId: $('#group').val(), enable: enable, isDelete: isDelete, titleId: jobTitleId, sort: JSON.stringify(sorts) },
        beforeSend: function () {
            $('.loading-process').fadeIn(300);
        },
        success: function (data) {
            if (data.length > 0) {
                rowIndex = 1;
                $('#data tbody').children().remove();
                $('#template').tmpl(data).appendTo('#data');

                $('.nick').tipsy({
                    className: 'tipsy-avatar',
                    gravity: $.fn.tipsy.autoNS,
                    html: true,
                    title: function () {
                        return '<img src="/employee/avatar/' + $(this).attr('data-code') + '" />';
                    }
                });
            } else {
                $('#data tbody').children().remove();
                $(noInformation(11)).appendTo('#data');
            }

            $('.loading-process').fadeOut(300);

            floatThead('table#data');
        },
        error: function () {
            $('.loading-process').fadeOut(300);
            alert(_messages.errorTryAgain);
        }
    });

    $('#keyword').focus().select();
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

        query();
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


        window.location.href = _global.root + 'Export/EmployeeProfile?query=' + $('#keyword').val() + '&groupId=' + $('#group').val() +
            '&enable=' + enable + '&isDelete=' + isDelete + '&titleId=' + jobTitleId + '&sort=' + JSON.stringify(sorts);
    });
}

$(document).ready(function () {
    $('#keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query();
        }
    });

    var q = getParam('query');
    if (q !== null) {
        var decode = decodeURIComponent(q);
        $('#keyword').val(decode);
    }

    eventSorting();
    eventExportExcel();

    //search onload
    query();

    $('#query').click(function (evt) {
        evt.preventDefault();
        refreshSorting();
        query();
    });
});