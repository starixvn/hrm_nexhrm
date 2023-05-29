'use strict';
/*global initDatePicker, initAutocomplete*/

const TestDate = "Test Date";
const ExpirationDate = "Expiration Date";
const StartDate = "Start Date";
const EndDate = "End Date";
const Certificates = "Certificates";

function initChangeTextFocusType() {
    const typeid = $("#frmAddTrainingAndDevelopment #typeId option:selected").text();
    if (typeid === Certificates) {
        $(' #fromdate')[0].innerText = TestDate;
        $('#frmAddTrainingAndDevelopment #todate')[0].innerText = ExpirationDate;
        $("#frmAddTrainingAndDevelopment #to-date-require")[0].style.display = 'none';
    } else {
        $("#frmAddTrainingAndDevelopment #to-date-require")[0].style.display = '';
        $('#frmAddTrainingAndDevelopment #fromdate')[0].innerText = StartDate;
        $('#frmAddTrainingAndDevelopment #todate')[0].innerText = EndDate;
    }
}

function setDataEmployee() {
    const displayName = $("#btn-add-training").data('displayname');
    const empId = $("#btn-add-training").data('id');
    $("#frmAddTrainingAndDevelopment #Employee")[0].value = displayName;
    $("#frmAddTrainingAndDevelopment #EmployeeId")[0].value = empId;
    $("#frmAddTrainingAndDevelopment #Employee")[0].disabled = true;
}

function initAddNewDialog() {
    setDataEmployee();
    initDatePicker();
    initAutocomplete('#frmAddTrainingAndDevelopment #Employee', '#frmAddTrainingAndDevelopment #EmployeeId', false);
    $("#frmAddTrainingAndDevelopment #typeId").on("change", () => {
        initChangeTextFocusType();
    });
    $('#dialog-training').dialog({
        modal: true,
        title: 'Add New',
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
                    $(".ui-dialog-buttonset .btn-submit")[0].style.display = 'none';
                    const formData = new FormData();
                    formData.append("EmployeeId", $("#frmAddTrainingAndDevelopment #EmployeeId")[0].value);
                    formData.append("TypeName", $("#frmAddTrainingAndDevelopment #typeId option:selected").text());
                    formData.append("ConductedBy", $("#frmAddTrainingAndDevelopment #ConductedBy")[0].value);
                    formData.append("TypeId", $("#frmAddTrainingAndDevelopment #typeId")[0].value);
                    formData.append("Name", $("#frmAddTrainingAndDevelopment #Name")[0].value);
                    formData.append("Description", $("#frmAddTrainingAndDevelopment #Description")[0].value);
                    formData.append("Organization", $("#frmAddTrainingAndDevelopment #Organization")[0].value);
                    formData.append("FromDate", $("#frmAddTrainingAndDevelopment #FromDatePopup")[0].value);
                    formData.append("ToDate", $("#frmAddTrainingAndDevelopment #ToDatePopup")[0].value);
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

                                if (data.isSuccessful) {
                                    errorCont.addClass('success validation-summary-success');
                                    errorCont.append('Insert successfully');
                                    setTimeout(function () {
                                        $('#dialog-training').dialog("close");
                                        window.location.reload();
                                    }, 1000);
                                }
                                else {
                                    $(".ui-dialog-buttonset .btn-submit")[0].style.display = '';
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
                $('#dialog-training').html(data);
                initAddNewDialog();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    });
});