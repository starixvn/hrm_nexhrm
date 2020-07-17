'use strict';
/*global _global, getParam, floatThead, _messages, noInformation, formatJsonDate*/
/* exported index, enable*/

var rowIndex = 1,
    employeeListType = 0,
    sorts = [],
    hasData=false;

function index() {
    return rowIndex++;
}

function query(jobTitleId, isFirstLoad) {
    if (jobTitleId === undefined || jobTitleId === '') {
        jobTitleId = $('#JobTitleId').val();
    }

    var keyword = $('#Keyword').val(),
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
        url: _global.root + 'employee/getemployees',
        data: { query: keyword, teamId: $('#TeamId').val(), enable: enable, isDelete: isDelete, jobTitleId: jobTitleId, sort: JSON.stringify(sorts) },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            employeeListType = data.StatusViewType;
            $('#data tbody').children().remove();
            var listEmployee = data.Employees;
            if (listEmployee && listEmployee.length > 0) {
                rowIndex = 1;
                hasData = true;

                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        formatDate: formatJsonDate
                    },
                    html = tmpl.render(listEmployee, helpers);

                $(html).appendTo('table#data');

                $('.nick').tipsy({
                    className: 'tipsy-avatar',
                    gravity: $.fn.tipsy.autoNS,
                    html: true,
                    title: function () {
                        return '<img src="/employee/avatar/' + $(this).attr('data-code') + '" />';
                    }
                });
                $('table#data').addClass('responsive-table');
            } else {
                hasData = false;
                $('table#data').removeClass('responsive-table');
                $('table#data').floatThead('destroy');
                //if (employeeListType === 0) {
                //    $(noInformation(11)).appendTo('table#data');
                //}
                //else {
                //    $(noInformation(12)).appendTo('table#data');
                //}
                $(noInformation(13)).appendTo('table#data');
            }
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (employeeListType === 0) {
                $('#resignedDate').hide();
                $('.ResignedDate').hide();
            }
            else {
                $('#resignedDate').show();
                $('.ResignedDate').show();
            }
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
            if (hasData) {
                floatThead('table#data');
            }
        }
    });

    $('#Keyword').focus().select();
}

function eventSorting(jobTitleId) {
    if (jobTitleId === undefined || jobTitleId === '') {
        jobTitleId = $('#JobTitleId').val();
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
        $(this).find('div.sorter').removeClass('asc').removeClass('desc');
    });
}

function eventExportExcel(jobTitleId) {
    if (jobTitleId === undefined || jobTitleId === '') {
        jobTitleId = $('#JobTitleId').val();
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

        window.location.href = _global.root + 'Export/EmployeeProfile?query=' + $('#Keyword').val() + '&teamId=' + $('#TeamId').val() +
            '&enable=' + enable + '&isDelete=' + isDelete + '&jobTitleId=' + jobTitleId + '&sort=' + JSON.stringify(sorts);
    });
}

$(document).ready(function () {
    var jobTitleId = $('#JobTitleId').val();

    $("#JobTitleId").val(jobTitleId);
    $('#JobTitleId').material_select(); // jshint ignore:line

    eventSorting(jobTitleId);
    eventExportExcel(jobTitleId);

    $('#Keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query(jobTitleId, false);
        }
    });

    var q = getParam('query');
    if (q !== null) {
        $('#Keyword').val(q);
    }

    //search onload
    query(jobTitleId, true);

    $('#query').click(function (evt) {
        evt.preventDefault();
        refreshSorting();
        query(jobTitleId, false);
    });
});