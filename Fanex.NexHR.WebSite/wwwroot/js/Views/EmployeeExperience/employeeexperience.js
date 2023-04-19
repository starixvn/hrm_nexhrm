'use strict';
/*global _global, getParam, _messages, initDatePicker, initAutocomplete, floatThead*/
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



    $('#submit').on('click', function () {
        query(false);
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
        $('#address')[0].innerText = "Issue Place";
        $('#fromdate')[0].innerText = "Exam Date";
        $('#todate')[0].innerText = "Expiration Date";
    } else {
        $('#address')[0].innerText = "Study Place";
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
        $('#address')[0].innerText = "Issue Place";
        $('#fromdate')[0].innerText = "Exam Date";
        $('#todate')[0].innerText = "Expiration Date";
    } else {
        $('#address')[0].innerText = "Study Place";
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
                    $.ajax({
                        type: 'POST',
                        url: _global.root + 'EmployeeExperience/Update',
                        dataType: 'json',
                        data: $('#frmUpdateExperience').serialize(),
                        success: function (data) {
                            if (data) {
                                let errorCont = $('#divErrorCont');
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
                    $.ajax({
                        type: 'POST',
                        url: _global.root + 'EmployeeExperience/Insert',
                        dataType: 'json',
                        data: $('#frmAddExperience').serialize(),
                        success: function (data) {
                            if (data) {
                                let errorCont = $('#divErrorCont');
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
        query(false);
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