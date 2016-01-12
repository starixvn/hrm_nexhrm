'use strict';
/*global _global:false, reloadButtonText:false, sleep: false, noInformation:false,
getParam:false, actingButton:false, _messages:false*/
/*exported isShowEdit, editTOEIC, iskExpiredDate*/

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

            var index = term.indexOf(' (');

            if (index > 0) {
                term = term.substr(0, index);
            }

            $.ajax({
                url: _global.root + 'api/employee/getemployees',
                data: {
                    query: term
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        response($.map(data, function (item) {
                            return { label: item.NickName + ' (' + item.FullName + ')',
                                value: item.Id
                            };
                        }));
                    } else {
                        $('#EmpId').val('0');
                    }
                }
            });
        },
        select: function (event, ui) {
            event.preventDefault();
            var nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);
        },
        change: function (event, ui) {
            if (ui && ui.item) {
                var nickName = ui.item.label;
                $('#Employee').val(nickName);
                $('#EmpId').val(ui.item.value);
            }
            else {
                $('#EmpId').val('0');
            }
        },
        minLength: 1
    });
}
function editTOEIC(id, name) {
    if (id > 0) {
        $.ajax({
            type: 'GET',
            url: _global.root + 'toeic/updatefulltoeic?id=' + id + '&name=' + name,
            dataType: 'html',
            beforeSend: function () {
                $('.loading-process').fadeIn(200);
            },

            success: function (data) {
                if (data) {
                    $('#dialog-toeic').html(data);
                    initEditToeicDialog();
                    openDialog();
                }
                $('.loading-process').fadeOut(500);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-process').fadeOut(500);
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }
}

function initEvent() {
    $('#keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            $('#submit').submit();
        }
    });

    $('#submit').click(function () {
        actingButton('#submit', true);
        sleep(1000);
        reloadButtonText('#submit');
    });

    $('#btnNewTOEIC').click(function () {
        $.ajax({
            type: 'GET',
            url: _global.root + 'toeic/updatetoeic',
            dataType: 'html',
            beforeSend: function () {
                $('.loading-process').fadeIn(200);
            },

            success: function (data) {
                if (data) {
                    $('#dialog-toeic').html(data);
                    initAddToeicDialog();
                    openDialog();
                }
                $('.loading-process').fadeOut(500);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-process').fadeOut(500);
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
            url: _global.root + 'toeic/updatetoeic',
            dataType: 'json',
            data: $('#frmUpdateTOEIC').serialize(),
            success: function (data) {
                if (data) {
                    var errorCont = $('#divErrorCont');
                    errorCont.removeClass();
                    errorCont.html('');

                    if (data.code === 1) {
                        errorCont.addClass('success validation-summary-success');
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
            url: _global.root + 'toeic/updatefulltoeic',
            dataType: 'json',
            data: $('#frmUpdFullTOEIC').serialize(),
            success: function (data) {
                if (data) {
                    var errorCont = $('#divErrorCont');
                    errorCont.removeClass();
                    errorCont.html('');

                    if (data.code === 1) {
                        errorCont.addClass('success validation-summary-success');
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
    initEvent();
});