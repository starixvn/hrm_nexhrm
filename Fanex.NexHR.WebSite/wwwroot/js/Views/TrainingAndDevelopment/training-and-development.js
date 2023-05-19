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

    const url = _global.root + 'TrainingAndDevelopment/GetTrainingAndDevelopment?' + data;
    if (!isFirstLoad) {
        const orgUrl = _global.root + 'TrainingAndDevelopment/Index?' + data + "&TeamParam=" + teamParam + "&typeParam=" + typeParam;
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
            url: _global.root + 'TrainingAndDevelopment/Update?id=' + id,
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
    $(".typeTrainingAndDevelopment .select-dropdown").val(typeParam);
}

function initChangeTextFocusType() {
    const typeid = $("#frmAddTrainingAndDevelopment #typeId option:selected").text();
    if (typeid === "Certificates") {
        $('#frmAddTrainingAndDevelopment #fromdate')[0].innerText = "Test Date";
        $('#frmAddTrainingAndDevelopment #todate')[0].innerText = "Expiration Date";
    } else {
        $('#frmAddTrainingAndDevelopment #fromdate')[0].innerText = "Start Date";
        $('#frmAddTrainingAndDevelopment #todate')[0].innerText = "End Date";
    }
}

function initChangeTextFocusTypeUpdate() {
    const typeid = $("#frmUpdateTrainingAndDevelopment #typeId option:selected").text();
    if (typeid === "Certificates") {
        $('#frmUpdateTrainingAndDevelopment #fromdate')[0].innerText = "Test Date";
        $('#frmUpdateTrainingAndDevelopment #todate')[0].innerText = "Expiration Date";
    } else {
        $('#frmUpdateTrainingAndDevelopment #fromdate')[0].innerText = "Start Date";
        $('#frmUpdateTrainingAndDevelopment #todate')[0].innerText = "End Date";
    }
}

function initUpdateDialog() {
    initDatePicker();
    initChangeTextFocusTypeUpdate();

    document.querySelector("#frmUpdateTrainingAndDevelopment #typeId").addEventListener("change", () => {
        initChangeTextFocusTypeUpdate();
    });    

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
                        const type = $("#frmUpdateTrainingAndDevelopment #typeId option:selected").text();

                        const formData = new FormData();
                        formData.append("Id", document.querySelector("#frmUpdateTrainingAndDevelopment #Id").value);
                        formData.append("EmployeeId", document.querySelector("#frmUpdateTrainingAndDevelopment #EmployeeId").value);
                        formData.append("ConductedBy", document.querySelector("#frmUpdateTrainingAndDevelopment #ConductedBy").value);
                        formData.append("TypeId", document.querySelector("#frmUpdateTrainingAndDevelopment #typeId").value);
                        formData.append("Name", document.querySelector("#frmUpdateTrainingAndDevelopment #Name").value);
                        formData.append("Description", document.querySelector("#frmUpdateTrainingAndDevelopment #Description").value);
                        formData.append("Organization", document.querySelector("#frmUpdateTrainingAndDevelopment #Organization").value);
                        const fromDate = document.querySelector("#ExamDate").value;
                        const toDate = document.querySelector("#ExpirationDate").value;
                        if (type === "Certificates") {
                            formData.append("StartDate", null);
                            formData.append("EndDate", null);
                            formData.append("ExamDate", fromDate);
                            formData.append("ExpirationDate", toDate);
                        } else {
                            formData.append("StartDate", fromDate);
                            formData.append("EndDate", toDate);
                            formData.append("ExamDate", null);
                            formData.append("ExpirationDate", null);
                        }

                        $.ajax({
                            type: 'POST',
                            url: _global.root + 'TrainingAndDevelopment/Update',
                            dataType: 'json',
                            processData: false,
                            contentType: false,
                            data: formData,
                            success: function (data) {
                                if (data) {
                                    let errorCont = $('#frmUpdateTrainingAndDevelopment #divErrorCont');
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
    const name = document.querySelector("#frmUpdateTrainingAndDevelopment #Name").value;
    const place = document.querySelector("#frmUpdateTrainingAndDevelopment #Organization").value;
    const examDate = document.querySelector("#frmUpdateTrainingAndDevelopment #ExamDate").value;
    const expirationDate = document.querySelector("#frmUpdateTrainingAndDevelopment #ExpirationDate").value;
    const type = $("#frmUpdateTrainingAndDevelopment #typeId option:selected").text();
    let ErrorMessage = [];
    if (!name) {
        ErrorMessage.push('Name is required!');
    }
    if (!place) {
        ErrorMessage.push('Organization is required!');
    }
    if (!examDate) {
        if (type === "Certificates") {
            ErrorMessage.push('Test Date is required!');
        } else {
            ErrorMessage.push('Start Date is required!');
        }
    }
    if (!expirationDate) {
        if (type === "Certificates") {
            ErrorMessage.push('Expiration Date is required!');
        } else {
            ErrorMessage.push('End Date is required!');
        }
    }
    if (examDate > expirationDate) {
        if (type === "Certificates") {
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
        let errorCont = $('#frmUpdateTrainingAndDevelopment #divErrorCont');
        let ul = document.querySelector("#frmUpdateTrainingAndDevelopment #divErrorCont ul");
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
    const employeeId = document.querySelector("#frmAddTrainingAndDevelopment #EmployeeId").value;
    const name = document.querySelector("#frmAddTrainingAndDevelopment #Name").value;
    const place = document.querySelector("#frmAddTrainingAndDevelopment #Organization").value;
    const examDate = document.querySelector("#frmAddTrainingAndDevelopment #StartDate").value;
    const expirationDate = document.querySelector("#frmAddTrainingAndDevelopment #EndDate").value;
    const type = $("#frmAddTrainingAndDevelopment #typeId option:selected").text();
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
        if (type === "Certificates") {
            ErrorMessage.push('Test Date is required!');
        } else {
            ErrorMessage.push('Start Date is required!');
        }
    }
    if (!expirationDate) {
        if (type === "Certificates") {
            ErrorMessage.push('Expiration Date is required!');
        } else {
            ErrorMessage.push('End Date is required!');
        }
    }
    if (examDate > expirationDate) {
        if (type === "Certificates") {
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
        let errorCont = $('#frmAddTrainingAndDevelopment #divErrorCont');
        let ul = document.querySelector("#frmAddTrainingAndDevelopment #divErrorCont ul");
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
    initAutocomplete('#frmAddTrainingAndDevelopment #Employee', '#frmAddTrainingAndDevelopment #EmployeeId', false);
    $("#frmAddTrainingAndDevelopment #typeId").on("change", () => {
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
                        const type = $("#frmAddTrainingAndDevelopment #typeId option:selected").text();

                        const formData = new FormData();
                        formData.append("EmployeeId", document.querySelector("#frmAddTrainingAndDevelopment #EmployeeId").value);
                        formData.append("ConductedBy", document.querySelector("#frmAddTrainingAndDevelopment #ConductedBy").value);
                        formData.append("TypeId", document.querySelector("#frmAddTrainingAndDevelopment #typeId").value);
                        formData.append("Name", document.querySelector("#frmAddTrainingAndDevelopment #Name").value);
                        formData.append("Description", document.querySelector("#frmAddTrainingAndDevelopment #Description").value);
                        formData.append("Organization", document.querySelector("#frmAddTrainingAndDevelopment #Organization").value);
                        const fromDate = document.querySelector("#StartDate").value;
                        const toDate = document.querySelector("#EndDate").value;
                        if (type === "Certificates") {
                            formData.append("StartDate", null);
                            formData.append("EndDate", null);
                            formData.append("ExamDate", fromDate);
                            formData.append("ExpirationDate", toDate);
                        } else {
                            formData.append("StartDate", fromDate);
                            formData.append("EndDate", toDate);
                            formData.append("ExamDate", null);
                            formData.append("ExpirationDate", null);
                        }


                        $.ajax({
                            type: 'POST',
                            url: _global.root + 'TrainingAndDevelopment/Create',
                            dataType: 'json',
                            processData: false,
                            contentType: false,
                            data: formData,
                            success: function (data) {
                                if (data) {
                                    let errorCont = $('#frmAddTrainingAndDevelopment #divErrorCont');
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
        $.post(_global.root + 'TrainingAndDevelopment/Delete', { id: id }, function (data) {
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
        Materialize.toast('<i class="mdi mdi-close-circle"></i><span class="m-l--m">From Date and To Date is require!<span>', 2000);

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
            url: _global.root + 'TrainingAndDevelopment/Create',
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