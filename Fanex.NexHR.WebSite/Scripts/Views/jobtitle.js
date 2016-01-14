/// <reference path='../Libs/jquery.min.js' />
/*global _global, _messages, getParam*/
/*exported index, deleteJobTitle, editJobTile, cancelUpdate, updateJobTitle*/
'use strict';

var rowIndex = 1;

function index() {
    return rowIndex++;
}

var canNotUpdate = function (id) {
    $('#txtJobTile_' + id).val($('#spanJobTitle_' + id).text());
};

function deleteJobTitle(id) {
    confirm('Are you sure you want to delete?', function () {
        $.post(_global.root + 'jobtitle/DeleteJobTitle',
        { id: id },
        function (response) {
            if (response) {
                document.location = _global.root + 'jobtitle/jobtitlemanagement';
            }
            else {
                alert(_messages.errorTryAgain);
            }
        }).error(function () {
            alert(_messages.errorTryAgain);
        });
    });
}

function editJobTile(id) {
    $('#spanJobTitle_' + id).hide();
    $('#txtJobTile_' + id).show();
    $('#btnEditJobTitle_' + id).hide();
    $('#btnUpdateJT_' + id).show();
    $('#btnCancelUpdateJT_' + id).show();
}

function cancelUpdate(id) {
    $('#spanJobTitle_' + id).show();
    $('#txtJobTile_' + id).hide();
    $('#btnEditJobTitle_' + id).show();
    $('#btnUpdateJT_' + id).hide();
    $('#btnCancelUpdateJT_' + id).hide();
}

function updateJobTitle(id) {
    var o = $('#txtJobTile_' + id);

    if (o.val().length > 100 || o.val().length < 3) {
        alert('Length of job title must be between ' + 3 + ' and ' + 100 + '.');
        canNotUpdate(id);
        return false;
    }

    confirm('Are you sure you want to change?', function () {
        var jobTitle = $('#txtJobTile_' + id).val();
        if (jobTitle === '') {
            alert(_messages.errorTryAgain);
            canNotUpdate(id);
        }
        var oldTitle = $('#spanJobTitle_' + id).val();
        if (jobTitle !== oldTitle) {
            $.post(_global.root + 'jobtitle/UpdatetJobTitle', { id: id, name: jobTitle }, function (response) {
                if (response) {
                    document.location = _global.root + 'jobtitle/jobtitlemanagement';
                }
                else {
                    alert(_messages.errorTryAgain);
                    canNotUpdate(id);
                }
            }).error(function () {
                alert(_messages.errorTryAgain);
                canNotUpdate(id);
            });
        }
        else {
            alert(_messages.errorTryAgain);
            canNotUpdate(id);
        }
    });
}

$(function () {
    var name = $('#name'),
        allFields = $([]).add(name),
        tips = $('.validateTips');

    function updateTips(t) {
        tips.text(t);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            o.addClass('ui-state-error');
            updateTips('Length of ' + n + ' must be between ' + min + ' and ' + max + '.');
            return false;
        } else {
            return true;
        }
    }

    $('#dialog-form').dialog({
        title: $('#dialog-form').attr('title'),
        autoOpen: false,
        width: 450,
        modal: true,
        buttons: [
            {
                text: 'Create',
                class: 'btn-submit waves-effect waves-light',
                click: function () {
                    var bValid = true;

                    allFields.removeClass('ui-state-error');
                    bValid = bValid && checkLength(name, 'job title', 3, 100);

                    if (bValid) {
                        $.post(_global.root + 'jobtitle/InsertJobTitle',
                        { name: name.val() },
                        function (response) {
                            if (response) {
                                document.location = _global.root + 'jobtitle/jobtitlemanagement';
                            } else {
                                alert(_messages.errorTryAgain);
                            }
                        }).error(function () {
                            alert(_messages.errorTryAgain);
                        });
                    }
                }
            },
            {
                text: 'Cancel',
                class: 'btn-cancel waves-effect waves-light',
                click: function () {
                    $(this).dialog('close');
                    return false;
                }
            }
        ],
        close: function () {
            updateTips('');
            allFields.val('').removeClass('ui-state-error');
        },
        dialogClass: 'create-job-title'
    });

    $('#btnNewJobTitle').click(function () {
        $('#dialog-form').dialog('open');
    });
});

function query() {
    $.ajax({
        url: _global.root + 'jobtitle/JobTitleManagement',
        type: 'POST',
        dataType: 'json',
        data: { jobTitle: $('#keyword').val() },
        beforeSend: function () {
            $('.loading-process').fadeIn(300);
        },
        success: function (response) {
            if (response.data.length > 0) {
                rowIndex = 1;
                $('#data tbody').children().remove();
                $('#template').tmpl(response.data).appendTo('#data');
            } else {
                $('#data tbody').children().remove();
                $(noInformation(10)).appendTo('#data');
            }

            $('.loading-process').fadeOut(300);

            floatThead('table#data');
        },
        error: function () {
            $('.loading-process').fadeOut(300);
            alert(_messages.errorTryAgain);
        }
    });
}

$(document).ready(function () {
    var enable = true;

    $('#submit').on('click', function () {
        query();
    });

    var q = getParam('query');
    if (q != null) {
        $('#keyword').val(q);
        query();
    } else {
        query();
    }
});