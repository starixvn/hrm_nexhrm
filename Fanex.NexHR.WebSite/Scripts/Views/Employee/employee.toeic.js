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

function initNumberTextbox() {
    var numbericTextbox = $('input[type=number]');
    numbericTextbox.attr('maxlength', '3');
    numbericTextbox.keydown(function (event) {
        // Allow: backspace, delete, tab, escape, and enter
        if (event.keyCode === 46 || event.keyCode === 8 || event.keyCode === 9 || event.keyCode === 27 || event.keyCode === 13 ||
        // Allow: Ctrl+A
			(event.keyCode === 65 && event.ctrlKey === true) ||
        // Allow: home, end, left, right
			(event.keyCode >= 35 && event.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
            }
        }
    });
}

function isExpiredDate(date) {
    var dateNow = new Date();
    var expiredDate = new Date(date);

    return (expiredDate.toString("yyyy-MM-dd") < dateNow.toString("yyyy-MM-dd"));
}

function initDialog(divContent) {
    divContent.dialog({
        modal: true,
        width: 500,
        buttons: {
            Submit: function () {
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
            },
            Close: function () {
                $(this).dialog('close');
            }
        }
    });
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

function initDatepicker() {
    if ($('.datepicker').length > 0) {
        // get minDate, maxDate, disabledDates
        var mindate = $('.datepicker').attr('mindate');
        var maxdate = $('.datepicker').attr('maxdate');
        var disabledDates = $('.datepicker').attr('disabledDate');

        if (disabledDates.length > 0) {
            $('.datepicker').datepicker({
                dateFormat: "dd M, yy",
                beforeShowDay: checkBadDates,
                showOn: "button",
                buttonImageOnly: true,
                buttonImage: _global.root + "Content/Images/calendar.png"
            });
        }
        else {
            $('.datepicker').datepicker({
                dateFormat: "dd M, yy",
                showOn: "button",
                buttonImageOnly: true,
                buttonImage: _global.root + "Content/Images/calendar.png"
            });
        }

        if (typeof (initDatePicker) !== "function") //check if (isExit include date.filter.js) then (mindate, maxdate include after function initDatePicker)
        {
            if (mindate != 'null') {
                mindate = mindate.split("/");
                $('.datepicker').datepicker("option", "minDate", new Date(mindate[0], mindate[1] - 1, mindate[2]));
            }
            if (maxdate != 'null') {
                maxdate = maxdate.split("/");
                $('.datepicker').datepicker("option", "maxDate", new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
            }
        }
    }
}

function initEditDialog(divContent) {
    divContent.dialog({
        modal: true,
        width: 500,
        buttons: {
            Submit: function () {
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
            },
            Close: function () {
                $(this).dialog('close');
            }
        }
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
                    var divContent = $('#dialog-toeic');
                    divContent.html(data);

                    initNumberTextbox();
                    initDatepicker();
                    initEditDialog(divContent);
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
                    var divContent = $('#dialog-toeic');
                    divContent.html(data);

                    initNumberTextbox();
                    initDatepicker();
                    initAutocomplete();
                    initDialog(divContent);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    });
}

$(document).ready(function () {

    query();

    initEvent();

    //	$('#title').val(jobTitleId);
    //	$('#title').chosen();
    //	$("#title").trigger("liszt:updated");   


});