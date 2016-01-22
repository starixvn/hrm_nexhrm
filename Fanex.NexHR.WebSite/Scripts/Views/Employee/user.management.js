'use strict';
/*global _global, getParam, floatThead, _messages, noInformation*/
/* exported index*/

var rowIndex = 1,
    sorts = [];

function index() {
    return rowIndex++;
}

function query(jobTitleId, isFirstLoad) {
    if (jobTitleId === undefined || jobTitleId === '') {
        jobTitleId = $('#title').val();
    }

    var keyword = $('#keyword').val(),
        enable = null;

    if ($("#enable").prop("checked")) {
        enable = true;
    } else {
        if ($("#active").prop("checked")) {
            enable = false;
        } else {
            enable = null;
        }
    }

    var isDelete = null;
    if ($("#IsDeleted").prop("checked")) {
        isDelete = true;
    } else {
        isDelete = null;
    }

    $.ajax({
        url: _global.root + 'api/employee/getemployees',
        data: { query: keyword, groupId: $('#group').val(), enable: enable, isDelete: isDelete, titleId: jobTitleId, sort: JSON.stringify(sorts) },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#data tbody').children().remove();

            if (data.length > 0) {
                rowIndex = 1;

                $.template('template', $('#template'));
                $.tmpl('template', data).appendTo('table#data');

                $('.nick').tipsy({
                    className: 'tipsy-avatar',
                    gravity: $.fn.tipsy.autoNS,
                    html: true,
                    title: function () {
                        return '<img src="/employee/avatar/' + $(this).attr('data-code') + '" />';
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

    $('#keyword').focus().select();
}

function eventSorting(jobTitleId) {
    if (jobTitleId === undefined || jobTitleId === '') {
        jobTitleId = $('#title').val();
    }

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

        query(jobTitleId, false);
    });
}

function refreshSorting() {
    sorts = [];

    $('th.col-sorter').each(function () {
        $(this).find('div.sorter').removeClass('asc').removeClass('desc')
    });
}

function eventExportExcel(jobTitleId) {
    if (jobTitleId === undefined || jobTitleId === '') {
        jobTitleId = $('#title').val();
    }

    $('#exportExcel').on('click', function (evt) {
        evt.preventDefault();

        var enable = null;
        if ($("#enable").prop("checked")) {
            enable = true;
        } else {
            if ($("#active").prop("checked")) {
                enable = false;
            } else {
                enable = null;
            }
        }

        var isDelete = null;
        if ($("#IsDeleted").prop("checked")) {
            isDelete = true;
        } else {
            isDelete = null;
        }

        window.location.href = _global.root + 'Export/EmployeeProfile?query=' + $('#keyword').val() + '&groupId=' + $('#group').val() +
            '&enable=' + enable + '&isDelete=' + isDelete + '&titleId=' + jobTitleId + '&sort=' + JSON.stringify(sorts);
    });
}

$(document).ready(function () {
    var enable = null,
        jobTitleId = $('#jobtitleid').val();

    $("#title").val(jobTitleId);
    $('#title').material_select();

    eventSorting(jobTitleId);
    eventExportExcel(jobTitleId);

    $('#keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query(jobTitleId, false);
        }
    });

    var q = getParam('query');
    if (q !== null) {
        $('#keyword').val(q);
    }

    //search onload
    query(jobTitleId, true);

    $('#query').click(function (evt) {
        evt.preventDefault();
        refreshSorting();
        query(jobTitleId, false);
    });
});