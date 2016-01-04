'use strict';
/*global _global:false, reloadButtonText:false, getParam:false, actingButton:false, _messages:false, noInformation:false*/
/* exported index*/

var rowIndex = 1,
    sorts = [];
function index() {
    return rowIndex++;
}

function query(jobTitleId) {
    if (jobTitleId === undefined || jobTitleId === '') {
        jobTitleId = $('#title').val();
    }

    var keyword = $('#keyword').val();
    actingButton('#query', true);

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

    $.ajax({
        url: _global.root + 'api/employee/getemployees',
        data: { query: keyword, groupId: $('#group').val(), enable: enable, isDelete: isDelete, titleId: jobTitleId, sort: JSON.stringify(sorts) },
        success: function (data) {
            if (data.length > 0) {
                rowIndex = 1;
                $('#data tbody').children().remove();
                $('#itemTemplate').tmpl(data).appendTo('#data');

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

            reloadButtonText('#query');
        },
        error: function () {
            reloadButtonText('#query');
            alert(_messages.errorTryAgain);
        }
    });

    $('#keyword').focus().select();
}

$(document).ready(function () {
    var enable = null,
        jobTitleId = $('#jobtitleid').val();

    $("#title").val(jobTitleId);

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

        query(jobTitleId);
    });

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

    $('#keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query(jobTitleId);
        }
    });

    var q = getParam('query');
    if (q !== null) {
        $('#keyword').val(q);
    }

    //search onload
    query(jobTitleId);

    $('#query').click(function (evt) {
        evt.preventDefault();
        query(jobTitleId);
    });
});