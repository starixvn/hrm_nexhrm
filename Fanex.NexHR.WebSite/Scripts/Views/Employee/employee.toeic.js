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

function editTOEIC(id, name) {
    if (id > 0) {
        $.ajax({
            type: 'GET',
            url: _global.root + 'toeic/updatefulltoeic?id=' + id + '&name=' + name,
            dataType: 'html',
            success: function (data) {
                $('#dialog-toeic').html(data);
                initEditToeicDialog();
            },
            error: function (jqXHR, textStatus, errorThrown) {
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
            query(false);
        }
    });

    $('form.form-filter #submit').on('click', function () {
        query(false);
    });

    $('#btnNewTOEIC').click(function () {
        $.ajax({
            type: 'GET',
            url: _global.root + 'toeic/updatetoeic',
            dataType: 'html',
            success: function (data) {
                $('#dialog-toeic').html(data);
                initAddToeicDialog();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    });
}

function initAddToeicDialog() {
    initDatePicker();
    initAutocomplete('#Employee', '#EmployeeId');
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

                                if (data.code == 0) {
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
            effect: 'fade',
            duration: 300
        },
        hide: {
            effect: 'fade',
            duration: 200
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

                                if (data.code == 0) {
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
            effect: 'fade',
            duration: 300
        },
        hide: {
            effect: 'fade',
            duration: 200
        }
    });
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'toeic/ToeicManagementDetail',
        dataType: 'html',
        data: $('form.form-filter').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#toeic-detail').html(data);

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

$(document).ready(function () {
    initialize();

    query(true);
});