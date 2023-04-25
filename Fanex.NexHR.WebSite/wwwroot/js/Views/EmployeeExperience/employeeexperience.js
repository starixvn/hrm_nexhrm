'use strict';
/*global _global, getParam, _messages, initDatePicker, initAutocomplete, validateFromDateAndToDate, floatThead*/
/*exported deleteRequest */

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    const data = $('.card-container-base form').serialize();
    const teamParam = $(".team .select-dropdown").val();
    const typeParam = $(".type .select-dropdown").val();

    const url = _global.root + 'EmployeeExperience/GetEmployeeExperience?' + data;
    if (!isFirstLoad) {
        const orgUrl = _global.root + 'EmployeeExperience/Index?' + data + "&TeamParam=" + teamParam + "&typeParam=" + typeParam;
        window.history.replaceState(null, null, orgUrl);
    }

    $.ajax({
        url: url,
        dataType: 'html',
        method: "GET",
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#list-data').html(data);
            initActions();

            if ($('table#data').length > 0) {
                $('table#data').addClass('table-success'); // class to style mobile
            }

            floatThead('table#data');
        },
        error: function (jqXHR, texttype, errorThrown) {
            alert(texttype + ': ' + errorThrown);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
        }
    });
}

function initActions() {
    $('.btn_update').on('click', function () {
        const id = $(this).data('id');
        $.ajax({
            type: 'GET',
            url: _global.root + 'EmployeeExperience/Update?id=' + id,
            dataType: 'html',
            success: function (data) {
                $('#dialog-update').html(data);
                initUpdateDialog();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    });
}

function initDataFirstQuery(queryString) {
    const urlParams = new URLSearchParams(queryString);
    const TeamId = urlParams.get('TeamId');
    const Employee = urlParams.get('Employee');
    const EmployeeId = urlParams.get('EmployeeId');
    const Type = urlParams.get('Type');
    const FromDate = urlParams.get('FromDate');
    const ToDate = urlParams.get('ToDate');
    const teamParam = urlParams.get('TeamParam');
    const typeParam = urlParams.get('TypeParam');

    $("#TeamId").val(TeamId);
    $("#Employee").val(Employee);
    $("#EmployeeId").val(EmployeeId);
    $("#Type").val(Type);
    $("#FromDate").val(FromDate);
    $("#ToDate").val(ToDate);

    $(".team .select-dropdown").val(teamParam);
    $(".typeEmployeeExperience .select-dropdown").val(typeParam);
}

function initChangeTextFocusType() {
    const typeid = document.querySelector("#typeId").value;
    if (typeid === "Certificate") {
        $('#fromdate')[0].innerText = "Exam Date";
        $('#todate')[0].innerText = "Expiration Date";
    } else {
        $('#fromdate')[0].innerText = "Start Date";
        $('#todate')[0].innerText = "End Date";
    }
}

function initUpdateDialog() {
    initDatePicker();
    initAutocompleteDepartment('#frmUpdateExperience #Department', '#frmUpdateExperience #DepartmentId', false);
    initChangeTextFocusType();

    document.querySelector("#typeId").addEventListener("change", () => {
        initChangeTextFocusType();
    });    

    const typeid = document.querySelector("#typeId").value;
    if (typeid === "Certificate") {
        $('#fromdate')[0].innerText = "Exam Date";
        $('#todate')[0].innerText = "Expiration Date";
    } else {
        $('#fromdate')[0].innerText = "Start Date";
        $('#todate')[0].innerText = "End Date";
    }

    $('#dialog-update').dialog({
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
                    if (isValidUpdate()) {
                        $.ajax({
                            type: 'POST',
                            url: _global.root + 'EmployeeExperience/Update',
                            dataType: 'json',
                            data: $('#frmUpdateExperience').serialize(),
                            success: function (data) {
                                if (data) {
                                    let errorCont = $('#frmUpdateExperience #divErrorCont');
                                    errorCont.removeClass();
                                    errorCont.html('');

                                    if (data.Code === 0) {
                                        errorCont.addClass('success validation-summary-success');
                                        errorCont.append('Update successfully');
                                        setTimeout(function () { $('#dialog-update').dialog("close"); query(); }, 3000);
                                    }
                                    else {
                                        let errors = '<ul>';
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

function isValidUpdate() {
    const department = document.querySelector("#frmUpdateExperience #DepartmentId").value;
    const name = document.querySelector("#frmUpdateExperience #Name").value;
    const place = document.querySelector("#frmUpdateExperience #Organization").value;
    const examDate = document.querySelector("#frmUpdateExperience #ExamDate").value;
    const expirationDate = document.querySelector("#frmUpdateExperience #ExpirationDate").value;
    let ErrorMessage = [];
    if (!department) {
        ErrorMessage.push('Department is required!');
    }
    if (!name) {
        ErrorMessage.push('Experience Name is required!');
    }
    if (!place) {
        ErrorMessage.push('Organization is required!');
    }
    if (examDate > expirationDate) {
        ErrorMessage.push('From Date must be less than or equal To Date!');
    }
    if (ErrorMessage.length > 0) {
        let errorCont = $('#frmUpdateExperience #divErrorCont');
        let ul = document.querySelector("#frmUpdateExperience #divErrorCont ul");
        if (ul != null) {
            ul.remove();
        }
        let errors = '<ul>';
        ErrorMessage.map(x => {
            errors += '<li>' + x + '</li>';
        });
        errors += '</ul>';
        errorCont.addClass('validation-summary-errors');
        errorCont.append(errors);
    }
    return ErrorMessage.length === 0;
}

function isValidInsert() {
    const employeeId = document.querySelector("#frmAddExperience #EmployeeId").value;
    const department = document.querySelector("#frmAddExperience #DepartmentId").value;
    const name = document.querySelector("#frmAddExperience #Name").value;
    const place = document.querySelector("#frmAddExperience #Organization").value;
    const examDate = document.querySelector("#frmAddExperience #StartDate").value;
    const expirationDate = document.querySelector("#frmAddExperience #EndDate").value;
    let ErrorMessage = [];
    if (!employeeId) {
        ErrorMessage.push('Employee is required!');
    }
    if (!department) {
        ErrorMessage.push('Department is required!');
    }
    if (!name) {
        ErrorMessage.push('Experience Name is required!');
    }
    if (!place) {
        ErrorMessage.push('Organization is required!');
    }
    if (examDate > expirationDate) {
        ErrorMessage.push('From Date must be less than or equal To Date!');
    }
    if (ErrorMessage.length > 0) {
        let errorCont = $('#frmAddExperience #divErrorCont');
        let ul = document.querySelector("#frmAddExperience #divErrorCont ul");
        if (ul != null) {
            ul.remove();
        }
        let errors = '<ul>';
        ErrorMessage.map(x => {
            errors += '<li>' + x + '</li>';
        });
        errors += '</ul>';
        errorCont.addClass('validation-summary-errors');
        errorCont.append(errors);
    }
    return ErrorMessage.length === 0;
}

function initAddNewDialog() {
    initDatePicker();
    initAutocomplete('#frmAddExperience #Employee', '#frmAddExperience #EmployeeId', false);
    initAutocompleteDepartment('#frmAddExperience #Department', '#frmAddExperience #DepartmentId', false);
    initChangeTextFocusType();

    $('#dialog-addnew').dialog({
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
                    if (isValidInsert()) {
                        $.ajax({
                            type: 'POST',
                            url: _global.root + 'EmployeeExperience/Insert',
                            dataType: 'json',
                            data: $('#frmAddExperience').serialize(),
                            success: function (data) {
                                if (data) {
                                    let errorCont = $('#frmAddExperience #divErrorCont');
                                    errorCont.removeClass();
                                    errorCont.html('');

                                    if (data.Code === 0) {
                                        errorCont.addClass('success validation-summary-success');
                                        errorCont.append('Insert successfully');
                                        setTimeout(function () { $('#dialog-addnew').dialog("close"); query(); }, 3000);
                                    }
                                    else {
                                        let errors = '<ul>';
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

function deleteRequest(id) {
    confirm('Are you sure you want to delete?', function () {
        $.post(_global.root + 'EmployeeExperience/Delete', { id: id }, function (data) {
            if (data.IsSuccessful) {
                alert("Delete successfully!");
                query(false);
            }
            else {
                alert(data.Message);
            }
        });
    });
}

$(document).ready(function () {
    query(true);

    $('#submit').on('click', function () {
        if (validateFromDateAndToDate()) {
            query(false);
        }
    });

    $('#btnAddNew').click(function () {
        $.ajax({
            type: 'GET',
            url: _global.root + 'EmployeeExperience/Insert',
            dataType: 'html',
            success: function (data) {
                $('#dialog-addnew').html(data);
                initAddNewDialog();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    });
});