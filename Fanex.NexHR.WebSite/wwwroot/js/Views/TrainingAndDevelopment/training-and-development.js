'use strict';
/*global _global, getParam, _messages, initDatePicker, initAutocomplete, validateFromDateAndToDate, floatThead*/
/*exported deleteRequest */

const TestDate = "Test Date";
const ExpirationDate = "Expiration Date";
const StartDate = "Start Date";
const EndDate = "End Date";
const Certificates = "Certificates";

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    const data = $('.card-container-base form').serialize();
    const teamParam = $(".team .select-dropdown").val();
    const typeParam = $(".type .select-dropdown").val();

    const url = _global.root + 'LearningAndDevelopment/GetTrainingAndDevelopment?' + data;
    if (!isFirstLoad) {
        const orgUrl = _global.root + 'LearningAndDevelopment/Index?' + data + "&TeamParam=" + teamParam + "&typeParam=" + typeParam;
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

            $('.btn_update').on('click', function () {
                const id = $(this).data('id');
                $.ajax({
                    type: 'GET',
                    url: _global.root + 'LearningAndDevelopment/Update?id=' + id,
                    dataType: 'html',
                    success: function (data) {
                        $('#dialog-training').html(data);
                        initUpdateDialog();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert(textStatus + ': ' + errorThrown);
                    }
                });
            });

            viewCertificate();

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

function initChangeTextFocusType(frm) {
    const typeid = $(frm + " #typeId option:selected").text();
    if (typeid === Certificates) {
        $(frm + ' #fromdate')[0].innerText = TestDate;
        $(frm + ' #todate')[0].innerText = ExpirationDate;
        $(frm + " #to-date-require")[0].style.display = 'none';
    } else {
        $(frm + " #to-date-require")[0].style.display = '';
        $(frm + ' #fromdate')[0].innerText = StartDate;
        $(frm + ' #todate')[0].innerText = EndDate;
    }
}

function viewCertificate() {
    $('.icon-certificate').on('click', function (e) {
        e.preventDefault();
        var newLocation = $(this).attr('href');
        var openNewTab = new URL(newLocation, window.location.origin);
        window.open(openNewTab, '_blank');
    });
}

function changeFileUpload() {
    $('.delete-certificate').on('click', function () {
        $(this).closest('.certificate-container').remove();
        newFile.style.width = 'auto';
        var certificateInput = document.getElementById('Certificate');
        certificateInput.value = null;
    });

    const fileUpload = document.getElementById('FileUpload');
    const oldFile = document.getElementsByClassName('certificate-container')[0];
    const newFile = document.getElementsByClassName('selected-file')[0];

    fileUpload.addEventListener('change', (event) => {
        const selectedFile = event.target.files[0];
        if (selectedFile) {
            oldFile ? oldFile.style.display = 'none' : null;
            newFile ? newFile.style.width = 'auto' : null;
        }
    });
}

function initUpdateDialog() {
    initDatePicker();
    initChangeTextFocusType("#frmUpdateTrainingAndDevelopment");

    $("#frmUpdateTrainingAndDevelopment #typeId").on("change", () => {
        initChangeTextFocusType("#frmUpdateTrainingAndDevelopment");
    });

    $('#dialog-training').dialog({
        modal: true,
        title: 'Update',
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
                    formData.append("Id", $("#frmUpdateTrainingAndDevelopment #Id")[0].value);
                    formData.append("EmployeeId", $("#frmUpdateTrainingAndDevelopment #EmployeeId")[0].value);
                    formData.append("TypeName", $("#frmUpdateTrainingAndDevelopment #typeId option:selected").text());
                    formData.append("ConductedBy", $("#frmUpdateTrainingAndDevelopment #ConductedBy")[0].value);
                    formData.append("TypeId", $("#frmUpdateTrainingAndDevelopment #typeId")[0].value);
                    formData.append("Publish", $("#frmUpdateTrainingAndDevelopment #publish")[0].value);
                    formData.append("Name", $("#frmUpdateTrainingAndDevelopment #Name")[0].value);
                    formData.append("Description", $("#frmUpdateTrainingAndDevelopment #Description")[0].value);
                    formData.append("Organization", $("#frmUpdateTrainingAndDevelopment #Organization")[0].value);
                    formData.append("FromDate", $("#frmUpdateTrainingAndDevelopment #FromDatePopup")[0].value);
                    formData.append("ToDate", $("#frmUpdateTrainingAndDevelopment #ToDatePopup")[0].value);

                    const certificateInput = $("#frmUpdateTrainingAndDevelopment #Certificate")[0];
                    if (certificateInput) {
                        formData.append("Certificate", certificateInput.value);
                    }

                    const fileInput = $("#frmUpdateTrainingAndDevelopment #FileUpload")[0];
                    if (fileInput && fileInput.files.length > 0) {
                        formData.append("FileUpload", fileInput.files[0]);
                    }

                    $.ajax({
                        type: 'POST',
                        url: _global.root + 'LearningAndDevelopment/Update',
                        dataType: 'json',
                        processData: false,
                        contentType: false,
                        data: formData,
                        success: function (data) {
                            if (data) {
                                let errorCont = $('#frmUpdateTrainingAndDevelopment #divErrorCont');
                                errorCont.removeClass();
                                errorCont.html('');

                                if (data.isSuccessful) {
                                    errorCont.addClass('success validation-summary-success');
                                    errorCont.append('Update successfully');
                                    setTimeout(function () { $('#dialog-training').dialog("close"); query(); }, 1000);
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

    viewCertificate();
    changeFileUpload();
}

function initAddNewDialog() {
    initDatePicker();
    initAutocomplete('#frmAddTrainingAndDevelopment #Employee', '#frmAddTrainingAndDevelopment #EmployeeId', false);
    $("#frmAddTrainingAndDevelopment #typeId").on("change", () => {
        initChangeTextFocusType("#frmAddTrainingAndDevelopment");
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
                    formData.append("Publish", $("#frmAddTrainingAndDevelopment #publish")[0].value);
                    formData.append("Name", $("#frmAddTrainingAndDevelopment #Name")[0].value);
                    formData.append("Description", $("#frmAddTrainingAndDevelopment #Description")[0].value);
                    formData.append("Organization", $("#frmAddTrainingAndDevelopment #Organization")[0].value);
                    formData.append("FromDate", $("#frmAddTrainingAndDevelopment #FromDatePopup")[0].value);
                    formData.append("ToDate", $("#frmAddTrainingAndDevelopment #ToDatePopup")[0].value);

                    const fileInput = $("#frmAddTrainingAndDevelopment #FileUpload")[0];
                    if (fileInput != null && fileInput.files.length > 0) {
                        formData.append("FileUpload", fileInput.files[0]);
                    }

                    $.ajax({
                        type: 'POST',
                        url: _global.root + 'LearningAndDevelopment/Create',
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
                                    setTimeout(function () { $('#dialog-training').dialog("close"); query(); }, 1000);
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

function deleteRequest(id, fileName) {
    confirm('Are you sure you want to delete?', function () {
        $.post(_global.root + 'LearningAndDevelopment/Delete', { id: id, fileName: fileName }, function (data) {
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
            url: _global.root + 'LearningAndDevelopment/Create',
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