/*global _global, _messages, getParam, floatThead*/
/*exported deleteJobTitle, editJobTile, cancelUpdate, updateJobTitle, enable*/
'use strict';

var canNotUpdate = function (id) {
    $('#txtJobTile_' + id).val($('#spanJobTitle_' + id).text());
};

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'jobtitle/index',
        dataType: 'html',
        method: "POST",
        data: { jobTitle: $('#keyword').val() },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#job-title-detail').html(data);

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
}

function deleteJobTitle(id) {
    confirm('Are you sure you want to delete?', function () {
        $.post(_global.root + 'jobtitle/Delete',
        { id: id },
        function (response) {
            if (response.IsSuccessful) {
                query(false);
            }
            else {
                alert(response.Message);
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
            $.post(_global.root + 'jobtitle/Update', { id: id, name: jobTitle }, function (response) {
                if (response.IsSuccessful) {
                    query(false);
                } else {
                    alert(response.Message);
                    canNotUpdate(id);
                }
            }).error(function () {
                alert(_messages.errorTryAgain);
                canNotUpdate(id);
            });
        } else {
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
        width: 'auto',
        modal: true,
        buttons: [
            {
                text: 'Cancel',
                class: 'btn-cancel waves-effect waves-light m-r--m',
                click: function () {
                    $(this).dialog('close');
                    return false;
                }
            },
            {
                text: 'Create',
                class: 'btn-submit waves-effect waves-light',
                click: function () {
                    var bValid = true;

                    allFields.removeClass('ui-state-error');
                    bValid = bValid && checkLength(name, 'job title', 3, 100);

                    if (bValid) {
                        $.post(_global.root + 'jobtitle/Insert',
                        { name: name.val() },
                        function (response) {
                            if (response.IsSuccessful) {
                                $('#dialog-form').dialog('close');
                                query(false);
                            } else {
                                alert(response.Message);
                            }
                        }).error(function () {
                            alert(_messages.errorTryAgain);
                        });
                    }
                }
            }
        ],
        close: function () {
            updateTips('');
            allFields.val('').removeClass('ui-state-error');
        },
        show: {
            effect: 'fade',
            duration: 300
        },
        hide: {
            effect: 'fade',
            duration: 200
        },
        dialogClass: 'create-job-title'
    });

    $('#btnNewJobTitle').click(function () {
        $('#dialog-form').dialog('open');
    });
});

$(document).ready(function () {
    var enable = true;  // jshint ignore:line

    $('#keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query(false);
        }
    });

    $('#submit').on('click', function () {
        query(false);
    });

    var q = getParam('query');
    if (q != null) {
        $('#keyword').val(q);
    }

    query(true);
});