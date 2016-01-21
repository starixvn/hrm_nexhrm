'use strict';
/*global _global, noInformation, getParam, _messages*/
/*exported index, isShowEdit, editTOEIC, iskExpiredDate*/

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function isExpiredDate(date) {
    var dateNow = new Date();
    var expiredDate = new Date(date);

    return (expiredDate.toString("yyyy-MM-dd") < dateNow.toString("yyyy-MM-dd"));
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

function initialize() {
    var q = getParam('query');
    if (q != null) {
        $('form.form-filter #Query').val(q);
    }

    $('form.form-filter #Query').keyup(function (event) {
        if (event.keyCode === 13) {
            query();
        }
    });

    $('form.form-filter #submit').on('click', function () {
        query();
    });

    $('#btnNewTOEIC').click(function () {
        $.ajax({
            type: 'GET',
            url: _global.root + 'toeic/updatetoeic',
            dataType: 'html',
            beforeSend: function () {
                $('.loading-process').fadeIn(300);
            },
            success: function (data) {
                if (data) {
                    $('#dialog-toeic').html(data);
                    initAddToeicDialog();
                }

                $('.loading-process').fadeOut(300);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-process').fadeOut(300);
                alert(textStatus + ': ' + errorThrown);
            }
        });
    });
}

function initAddToeicDialog() {
    initDatePicker();
    initAutocomplete();
    initNumberTextbox();

    $('#dialog-toeic').dialog({
        modal: true,
        width: 'auto',
        buttons: [
            {
                text: 'Close',
                class: 'btn-cancel waves-effect waves-light m-r--m',
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: 'Submit',
                class: 'btn-submit waves-effect waves-light',
                click: function () {
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
                }
            }            
        ],
        show: {
            effect: "blind",
            duration: 200
        },
        hide: {
            effect: "blind",
            duration: 100
        }
    });
}

function initEditToeicDialog() {
    initDatePicker();
    initNumberTextbox();

    $('#dialog-toeic').dialog({
        modal: true,
        width: 'auto',
        buttons: [
            {
                text: 'Close',
                class: 'btn-cancel waves-effect waves-light m-r--m',
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: 'Submit',
                class: 'btn-submit waves-effect waves-light',
                click: function () {
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
                }
            }            
        ],
        show: {
            effect: "blind",
            duration: 200
        },
        hide: {
            effect: "blind",
            duration: 100
        }
    });
}

function query() {
    $.ajax({
        url: _global.root + 'toeic/ToeicManagement',
        type: 'POST',
        dataType: 'json',
        data: $('form.form-filter').serialize(),
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
                $(noInformation(13)).appendTo('#data');
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
    initialize();

    query();
});