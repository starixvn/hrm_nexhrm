'use strict';
/*global _global:false, reloadButtonText:false, noInformation:false, getParam:false, checkBadDates:false, actingButton:false,
_messages:false*/
/*exported isShowEdit, editTOEIC, iskExpiredDate*/

var rowIndex = 1;
function query() {
    var keyword = $('#keyword').val();
    var group = $('#group').val();
    var jobTitleId = $('#title').val();

    actingButton('#query', true);
    $.getJSON(
	_global.root + 'api/employee/gettoeic',
	{ query: keyword, groupId: group, titleId: jobTitleId },
	function (data) {
	    $('#data tbody').children().remove();
	    if (data.length > 0) {
	        rowIndex = 1;
	        $('#tplAllSkill').tmpl(data).appendTo('#data');
	        $("#title").trigger("liszt:updated");
	    }
	    else {
	        $(noInformation(12)).appendTo('#data');
	    }

	}).error(function () {
	    alert(_messages.errorTryAgain);
	});

    reloadButtonText('#query');
    $('#keyword').focus().select();
}

function isExpiredDate(date) {
    var dateNow = new Date();
    var expiredDate = new Date(date);

    return (expiredDate.toString("yyyy-MM-dd") < dateNow.toString("yyyy-MM-dd"));
}

function openDialog() {
    $('#dialog-toeic').dialog({
        modal: true,
        width: 500
    });
}

function closeDialog() {
    $('#dialog-toeic').dialog('close');
}

function initAutocomplete() {
    $('#Employee').autocomplete({
        source: function (request, response) {
            var term = request.term;
            $.ajax({
                url: _global.root + 'api/employee/getemployees',
                data: {
                    query: term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.NickName + ' (' + item.FullName + ')',
                            value: item.Id
                        };
                    }));
                }
            });
        },
        select: function (event, ui) {
            event.preventDefault();
            var nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);
        },
        minLength: 1
    });
}

function editTOEIC(id, name) {
    if (id > 0) {
        $.ajax({
            type: 'GET',
            url: _global.root + 'employee/updatefulltoeic?id=' + id + '&name=' + name,
            dataType: 'html',
            success: function (data) {
                if (data) {
                    $('#dialog-toeic').html(data);
                    initEditToeicDialog();
                    openDialog();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }
}

function initEvent() {
    $('#keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query();
        }
    });

    $('#query').click(function (evt) {
        evt.preventDefault();
        query();
    });

    $('#btnNewTOEIC').click(function () {
        $.ajax({
            type: 'GET',
            url: _global.root + 'employee/updatetoeic',
            dataType: 'html',
            success: function (data) {
                if (data) {
                    $('#dialog-toeic').html(data);
                    initAddToeicDialog();
                    openDialog();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    });
}

function initAddToeicDialog() {

    initDatePicker(".datepicker");
    initAutocomplete();
    $('#cancelAddToeic').click(function () {
        closeDialog();
    });

    $('#submitAddToeic').click(function () {
        $.ajax({
            type: 'POST',
            url: _global.root + 'employee/updatetoeic',
            dataType: 'json',
            data: $('#frmUpdateTOEIC').serialize(),
            success: function (data) {
                if (data) {
                    var errorCont = $('#divErrorCont');
                    errorCont.removeClass();
                    errorCont.html('');

                    if (data.code === 1) {
                        errorCont.addClass('success');
                        errorCont.append('Update TOEIC score for Employee successfully');
                        setTimeout(function () { $('#dialog-toeic').dialog("close"); query(); }, 3000);
                    }
                    else {
                        var errors = '<ul>';
                        $(data.error).each(function (i, value) {
                            errors += '<li>' + value.ErrorMessage + '</li>';
                        });
                        errors += '</ul>';

                        errorCont.addClass('validation-summary-errors');
                        errorCont.append(errors);
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    })
}

function initEditToeicDialog() {
    initDatePicker(".datepicker");

    $('#cancelEditToeic').click(function () {
        closeDialog();
    });

    $('#submitEditToeic').click(function () {
        $.ajax({
            type: 'POST',
            url: _global.root + 'employee/updatefulltoeic',
            dataType: 'json',
            data: $('#frmUpdFullTOEIC').serialize(),
            success: function (data) {
                if (data) {
                    var errorCont = $('#divErrorCont');
                    errorCont.removeClass();
                    errorCont.html('');

                    if (data.code === 1) {
                        errorCont.addClass('success');
                        errorCont.append('Update TOEIC score for Employee successfully');
                        setTimeout(function () { $('#dialog-toeic').dialog("close"); query(); }, 3000);
                    }
                    else {
                        var errors = '<ul>';
                        $(data.error).each(function (i, value) {
                            errors += '<li>' + value.ErrorMessage + '</li>';
                        });
                        errors += '</ul>';

                        errorCont.addClass('validation-summary-errors');
                        errorCont.append(errors);
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    })
}

$(document).ready(function () {
    query();
    initEvent();

    //	$('#title').val(jobTitleId);
    //	$('#title').chosen();
    //	$("#title").trigger("liszt:updated");   


});
