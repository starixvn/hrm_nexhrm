'use strict';
/*global initDatePicker, initAutocomplete*/

function initChangeTextFocusType() {
    const typeid = $("#frmAddTrainingAndDevelopment #typeId option:selected").text();
    if (typeid === "Certificates") {
        $(' #fromdate')[0].innerText = "Test Date";
        $('#frmAddTrainingAndDevelopment #todate')[0].innerText = "Expiration Date";
    } else {
        $('#frmAddTrainingAndDevelopment #fromdate')[0].innerText = "Start Date";
        $('#frmAddTrainingAndDevelopment #todate')[0].innerText = "End Date";
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
    if (examDate > expirationDate) {
        if (type === "Certificates") {
            ErrorMessage.push('Test Date must be less than or equal Expiration Date!');
        } else {
            ErrorMessage.push('Start Date must be less than or equal End Date!');
        }
    }
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
    return ErrorMessage.length === 0;
}

function setEmployee() {
    const displayName = $("#btn-add-training").data('displayname');
    const empId = $("#btn-add-training").data('id');
    document.querySelector("#Employee").value = displayName;
    document.querySelector("#EmployeeId").value = empId;
    document.querySelector("#Employee").disabled = true;
}

function initAddNewDialog() {
    setEmployee();
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
                                        setTimeout(function () {
                                            $('#dialog-addnew').dialog("close");
                                            window.location.reload();
                                        }, 1000);
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

$(document).ready(function () {
    $('#btn-add-training').on('click', function () {
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