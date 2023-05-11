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

    const url = _global.root + 'CourseAndCertificate/GetCourseAndCertificate?' + data;
    if (!isFirstLoad) {
        const orgUrl = _global.root + 'CourseAndCertificate/Index?' + data + "&TeamParam=" + teamParam + "&typeParam=" + typeParam;
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
            url: _global.root + 'CourseAndCertificate/Update?id=' + id,
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
    $(".typeCourseAndCertificate .select-dropdown").val(typeParam);
}

function initChangeTextFocusType() {
    const typeid = $("#frmAddCourseAndCertificate #typeId option:selected").text();
    if (typeid === "Certificates") {
        $('#frmAddCourseAndCertificate #fromdate')[0].innerText = "Test Date";
        $('#frmAddCourseAndCertificate #todate')[0].innerText = "Expiration Date";
    } else {
        $('#frmAddCourseAndCertificate #fromdate')[0].innerText = "Start Date";
        $('#frmAddCourseAndCertificate #todate')[0].innerText = "End Date";
    }
}

function initChangeTextFocusTypeUpdate() {
    const typeid = $("#frmUpdateCourseAndCertificate #typeId option:selected").text();
    if (typeid === "Certificates") {
        $('#frmUpdateCourseAndCertificate #fromdate')[0].innerText = "Test Date";
        $('#frmUpdateCourseAndCertificate #todate')[0].innerText = "Expiration Date";
    } else {
        $('#frmUpdateCourseAndCertificate #fromdate')[0].innerText = "Start Date";
        $('#frmUpdateCourseAndCertificate #todate')[0].innerText = "End Date";
    }
}

function initUpdateDialog() {
    initDatePicker();
    initChangeTextFocusTypeUpdate();

    document.querySelector("#frmUpdateCourseAndCertificate #typeId").addEventListener("change", () => {
        initChangeTextFocusTypeUpdate();
    });    

    const typeid = document.querySelector("#typeId").value;
    if (typeid === "Certificate") {
        $('#fromdate')[0].innerText = "Test Date";
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
                        document.querySelector(".ui-dialog-buttonpane .btn-submit").style.display = 'none';
                        $.ajax({
                            type: 'POST',
                            url: _global.root + 'CourseAndCertificate/Update',
                            dataType: 'json',
                            data: $('#frmUpdateCourseAndCertificate').serialize(),
                            success: function (data) {
                                if (data) {
                                    let errorCont = $('#frmUpdateCourseAndCertificate #divErrorCont');
                                    errorCont.removeClass();
                                    errorCont.html('');

                                    if (data.Code === 0) {
                                        errorCont.addClass('success validation-summary-success');
                                        errorCont.append('Update successfully');
                                        setTimeout(function () { $('#dialog-update').dialog("close"); query(); }, 1000);
                                    }
                                    else {
                                        document.querySelector(".ui-dialog-buttonpane .btn-submit").style.display = '';
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
    const name = document.querySelector("#frmUpdateCourseAndCertificate #Name").value;
    const place = document.querySelector("#frmUpdateCourseAndCertificate #Organization").value;
    const examDate = document.querySelector("#frmUpdateCourseAndCertificate #ExamDate").value;
    const expirationDate = document.querySelector("#frmUpdateCourseAndCertificate #ExpirationDate").value;
    const typeid = document.querySelector("#frmUpdateCourseAndCertificate #typeId").value;
    let ErrorMessage = [];
    if (!name) {
        ErrorMessage.push('Name is required!');
    }
    if (!place) {
        ErrorMessage.push('Organization is required!');
    }
    if (!examDate) {
        if (typeid === "Certificate") {
            ErrorMessage.push('Test Date is required!');
        } else {
            ErrorMessage.push('Start Date is required!');
        }
    }
    if (!expirationDate) {
        if (typeid === "Certificate") {
            ErrorMessage.push('Expiration Date is required!');
        } else {
            ErrorMessage.push('End Date is required!');
        }
    }
    if (examDate > expirationDate) {
        if (typeid === "Certificate") {
            ErrorMessage.push('Test Date must be less than or equal Expiration Date!');
        } else {
            ErrorMessage.push('Start Date must be less than or equal End Date!');
        }
    }
    sendErrorUpdateMessage(ErrorMessage);
    return ErrorMessage.length === 0;
}

function sendErrorUpdateMessage(ErrorMessage) {
    if (ErrorMessage.length > 0) {
        let errorCont = $('#frmUpdateCourseAndCertificate #divErrorCont');
        let ul = document.querySelector("#frmUpdateCourseAndCertificate #divErrorCont ul");
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
}

function isValidInsert() {
    const employeeId = document.querySelector("#frmAddCourseAndCertificate #EmployeeId").value;
    const name = document.querySelector("#frmAddCourseAndCertificate #Name").value;
    const place = document.querySelector("#frmAddCourseAndCertificate #Organization").value;
    const examDate = document.querySelector("#frmAddCourseAndCertificate #StartDate").value;
    const expirationDate = document.querySelector("#frmAddCourseAndCertificate #EndDate").value;
    const typeid = document.querySelector("#frmAddCourseAndCertificate #typeId").value;
    let ErrorMessage = [];
    if (!employeeId) {
        ErrorMessage.push('Employee is required!');
    }
    if (!name) {
        ErrorMessage.push('Name is required!');
    }
    if (!place) {
        ErrorMessage.push('Organization is required!');
    }
    if (!examDate) {
        if (typeid === "Certificate") {
            ErrorMessage.push('Test Date is required!');
        } else {
            ErrorMessage.push('Start Date is required!');
        }
    }
    if (!expirationDate) {
        if (typeid === "Certificate") {
            ErrorMessage.push('Expiration Date is required!');
        } else {
            ErrorMessage.push('End Date is required!');
        }
    }
    if (examDate > expirationDate) {
        if (typeid === "Certificate") {
            ErrorMessage.push('Test Date must be less than or equal Expiration Date!');
        } else {
            ErrorMessage.push('Start Date must be less than or equal End Date!');
        }
    }
    sendErrorInserMessage(ErrorMessage);
    return ErrorMessage.length === 0;
}

function sendErrorInserMessage(ErrorMessage) {
    if (ErrorMessage.length > 0) {
        let errorCont = $('#frmAddCourseAndCertificate #divErrorCont');
        let ul = document.querySelector("#frmAddCourseAndCertificate #divErrorCont ul");
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
}

function initAddNewDialog() {
    initDatePicker();
    initAutocomplete('#frmAddCourseAndCertificate #Employee', '#frmAddCourseAndCertificate #EmployeeId', false);
    $("#frmAddCourseAndCertificate #typeId").on("change", () => {
        initChangeTextFocusType();
    }); 
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
                        document.querySelector(".ui-dialog-buttonpane .btn-submit").style.display = 'none';
                        $.ajax({
                            type: 'POST',
                            url: _global.root + 'CourseAndCertificate/Insert',
                            dataType: 'json',
                            data: $('#frmAddCourseAndCertificate').serialize(),
                            success: function (data) {
                                if (data) {
                                    let errorCont = $('#frmAddCourseAndCertificate #divErrorCont');
                                    errorCont.removeClass();
                                    errorCont.html('');

                                    if (data.Code === 0) {
                                        errorCont.addClass('success validation-summary-success');
                                        errorCont.append('Insert successfully');
                                        setTimeout(function () { $('#dialog-addnew').dialog("close"); query(); }, 1000);
                                    }
                                    else {
                                        document.querySelector(".ui-dialog-buttonpane .btn-submit").style.display = '';
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
        $.post(_global.root + 'CourseAndCertificate/Delete', { id: id }, function (data) {
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

function validate() {
    let fromDate = $("#FromDate").val(),
        toDate = $("#ToDate").val(),
        isValid = true;

    if (!fromDate || !toDate) {
        $('#toast-container').children().remove();
        Materialize.toast('<i class="mdi mdi-close-circle"></i><span class="m-l--m">From date and to date id require!<span>', 2000);

        isValid = false;
    }

    return isValid;
};

$(document).ready(function () {
    query(true);

    $('#submit').on('click', function () {
        if (validateFromDateAndToDate() && validate()) {
            query(false);
        }
    });

    $('#btnAddNew').click(function () {
        $.ajax({
            type: 'GET',
            url: _global.root + 'CourseAndCertificate/Insert',
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