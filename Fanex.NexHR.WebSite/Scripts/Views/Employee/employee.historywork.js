'use strict';

function initAddHistoryWorkDialog() {
    initDatePicker();
    var empId = parseInt($('#empId').val());

    $('#dialog-history').dialog({
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
                        url: _global.root + 'employee/InsertHistoryWork',
                        dataType: 'json',
                        data: 
                            {
                                EmpId: empId,
                                AffectedDate: $("#AffectedDate").val(),
                                GroupName: $("#GroupName").val(),
                                Jobtitlename: $("#JobtitleName").val(),
                            },
                        success: function (){
                            if (data) {
                                var errorCont = $('#divErrorCont');
                                errorCont.removeClass();
                                errorCont.html('');
                                errorCont.addClass('success validation-summary-success');
                                errorCont.append('Update History Work for Employee successfully!');
                                document.location = _global.root + 'employee/workinghistory?employeeId=' + empId;
                            }
                            else {
                                errorCont.addClass('validation-summary-errors');
                                errorCont.append(('Update History Work failed!'));
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
    var empId = parseInt($('#employeeId').val());
    $('#btnInsertHistoryWork').click(function () {
        $.ajax({
            type: 'GET',
            url: _global.root + 'employee/InsertHistoryWork?employeeId=' + empId,
            dataType: 'html',
            success: function (data) {
                $('#dialog-history').html(data);
                initAddHistoryWorkDialog();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    });
}

function editHistoryWork(id) {
    if (id > 0) {
        $.ajax({
            type: 'GET',
            url: _global.root + 'employee/editHistoryWork?id=' + id,
            dataType: 'html',
            success: function (data) {
                $('#dialog-histtory-update').html(data);
                initEditHistoryWorkDialog();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }
}

function initEditHistoryWorkDialog() {

    initDatePicker();
    var empId = parseInt($('#empId').val());
    $('#dialog-histtory-update').dialog({
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
                        url: _global.root + 'employee/editHistoryWork',
                        dataType: 'json',
                        data: $('#frmEditHistoryWork').serialize(),
                        success: function (data) {
                            if (data) {
                                var errorCont = $('#divErrorCont');
                                errorCont.removeClass();
                                errorCont.html('');

                                if (data.IsSuccessful) {
                                    errorCont.addClass('success validation-summary-success');
                                    errorCont.append('Update History Work for Employee successfully');
                                    document.location = _global.root + 'employee/workinghistory?employeeId=' + empId;
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

function query(isFirstLoad) {

    $.ajax({
        type: 'GET',
        url: _global.root + 'employee/HistoryWorkDetail',
        dataType: 'html',
        data: $('form.form-filter').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#history-detail').html(data);
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

function deleteHistoryWork(id,EmpId) {
    confirm('Are you sure you want to delete this History Work?', function () {
        $.post(_global.root + 'employee/DeleteHistoryWork', { id: id },
            function (response) {
                if (response.IsSuccessful) {
                    $('#row' + id).remove();
                    document.location = _global.root + 'employee/workinghistory?employeeId=' + EmpId;
                }
                else {
                    alert(response.Message);
                }
            });
    });
}

$(document).ready(function () {

    initAutocomplete('#FullName', '#EmpId');

    initialize();

    query(true);

    $('#submit').click(function () {
        query(false);
    });
});