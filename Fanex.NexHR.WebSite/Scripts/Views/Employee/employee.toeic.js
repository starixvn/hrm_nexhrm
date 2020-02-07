'use strict';
/*global _global, getParam, _messages, initDatePicker, initAutocomplete, floatThead */
/*exported index, isShowEdit, editTOEIC, isExpiredDate */

var rowIndex = 1,
    sorts = [];

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

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'toeic/ToeicManagementDetail',
        dataType: 'html',
        data: $('form.form-filter').serialize() + '&sort=' + JSON.stringify(sorts),
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

                                if (data.IsSuccessful) {
                                    errorCont.addClass('success validation-summary-success');
                                    errorCont.append('Update TOEIC score for Employee successfully');
                                    setTimeout(function () { $('#dialog-toeic').dialog("close"); query(); }, 3000);
                                }
                                else {
                                    var errors = '<ul><li>' + data.Message + '</li></ul>';
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

function initAddToeicDialog() {
    initDatePicker();
    initAutocomplete('#Employee', '#EmpId');
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

                                if (data.code === 0) {
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

function calculateToeicScore() {
    var listening = $('form').find('input[name=Listening]').val();
    if (!listening) listening = 0;

    var reading = $('form').find('input[name=Reading]').val();
    if (!reading) reading = 0;

    var total = parseInt(listening) + parseInt(reading);

    $('form').find('input[name=Total]').val(total);
}

function calculateOldToeicScore() {
    var oldListening = $('#frmUpdFullTOEIC').find('input[name=OldListening]').val();
    if (!oldListening) oldListening = 0;

    var oldReading = $('#frmUpdFullTOEIC').find('input[name=OldReading]').val();
    if (!oldReading) oldReading = 0;

    var oldTotal = parseInt(oldListening) + parseInt(oldReading);
    $('#frmUpdFullTOEIC').find('input[name=OldTotal]').val(oldTotal);
}

function eventSorting() {
    $('html').on('click', 'th.col-sorter', function () {
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
            sorts.shift();
            sorts.push({ Property: property, Direction: direction });
        }

        query(false);
    });
}

function refreshSorting() {
    sorts = [];

    $('th.col-sorter').each(function () {
        $(this).find('div.sorter').removeClass('asc').removeClass('desc');
    });
}

$(document).ready(function () {
    initialize();

    eventSorting();

    query(true);
});