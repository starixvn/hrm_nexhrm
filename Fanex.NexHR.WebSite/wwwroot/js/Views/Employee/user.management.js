'use strict';
/*global _global, getParam, floatThead, _messages, noInformation, formatJsonDate*/
/* exported index, enable*/

var rowIndex = 1,
    employeeListType = 0,
    sorts = [],
    hasData = false;

function index() {
    return rowIndex++;
}

function query(jobTitleId, isFirstLoad) {
    jobTitleId = $('#JobTitleId').val();

    var keyword = $('#Keyword').val(),
        isDisable = null,
        isDelete = null;

    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }

    if (!$("#IsDisable").prop("checked") && !$("#IsDeleted").prop("checked")) {
        $("#IsActive").prop("checked", true);
    }
    if ($("#IsActive").prop("checked")) {
        isDisable = false;
    } else {
        if ($("#IsDisable").prop("checked")) {
            isDisable = true;
        } else {
            isDisable = null;
        }
    }

    if ($("#IsDeleted").prop("checked")) {
        isDelete = true;
    } else {
        isDelete = null;
    }

    var isSpecial = $('#IsSpecial').val();

    var jobTitleUrl = (!jobTitleId || jobTitleId === "") ? "" : "&jobTitleId=" + jobTitleId
    var data = "query=" + keyword + "&teamId=" + $('#TeamId').val() + "&isDisable=" + isDisable + "&isDelete=" + isDelete + jobTitleUrl + "&sort=" + JSON.stringify(sorts) + "&isSpecial=" + isSpecial;
    var teamName = $(".team .select-dropdown").val();
    var url = _global.root + 'employee/getemployees?' + data;

    var actionOrgUrl = isSpecial === "True" ? "SpecialUserManagement?" : "UserManagement?"

    if (!isFirstLoad) {
        var orgUrl = _global.root + 'employee/' + actionOrgUrl + data + "&teamName=" + teamName;
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
                    gravity: 'n',
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

                $(noInformation(15)).appendTo('table#data');
            }

            if (data.IsShowResignedDate === true) {
                $('#resignedDate').show();
                $('.ResignedDate').show();
            }
            else {
                $('#resignedDate').hide();
                $('.ResignedDate').hide();
            }
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
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

function initDataFirstQuery(queryString) {
    var urlParams = new URLSearchParams(queryString);
    var query = urlParams.get('query');
    var teamId = urlParams.get('teamId');
    var jobTitleId = urlParams.get('jobTitleId');
    var teamName = urlParams.get('teamName');
    //var jobTitleName = urlParams.get('jobTitleName');
    var isDisable = urlParams.get('isDisable');
    var isDelete = urlParams.get('isDelete');

    $('#Keyword').val(query);
    $('#TeamId').val(teamId);
    $('#JobTitleId').val(jobTitleId);
    $(".team .select-dropdown").val(teamName);

    if (isDisable === "true") {
        $("#IsDisable").prop("checked", true);
    } else if (isDelete === "true") {
        $("#IsDeleted").prop("checked", true);
    }
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
            $this.removeClass('desc').addClass('asc');
            direction = 'asc';
        } else if ($this.hasClass('asc')) {
            $this.removeClass('asc').addClass('desc');
            direction = 'desc';
        } else {
            refreshSorting();
            $this.addClass('desc');
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

        var isDisable = null;

        if ($("#IsActive").prop("checked")) {
            isDisable = false;
        } else {
            if ($("#IsDisable").prop("checked")) {
                isDisable = true;
            } else {
                isDisable = null;
            }
        }

        var isDelete = null;
        if ($("#IsDeleted").prop("checked")) {
            isDelete = true;
        } else {
            isDelete = null;
        }

        window.location.href = _global.root + 'Export/EmployeeProfile?query=' + $('#Keyword').val() + '&teamId=' + $('#TeamId').val() +
            '&isDisable=' + isDisable + '&isDelete=' + isDelete + '&jobTitleId=' + jobTitleId + '&sort=' + JSON.stringify(sorts);
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