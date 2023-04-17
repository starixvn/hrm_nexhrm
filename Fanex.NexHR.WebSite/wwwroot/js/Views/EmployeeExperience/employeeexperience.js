'use strict';
/*global _global, getParam, _messages, initDatePicker, initAutocomplete, floatThead */
/*exported index, isShowEdit, editTOEIC, isExpiredDate */

function query(isFirstLoad) {


    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    const data = $('.card-container-base form').serialize();
    const teamParam = $(".team .select-dropdown").val();
    const statusParam = $(".status .select-dropdown").val();

    const url = _global.root + 'EmployeeWorkFromHome/GetEmployeeWorkFromHomeGrid?' + data;
    if (!isFirstLoad) {
        const orgUrl = _global.root + 'EmployeeWorkFromHome/Index?' + data + "&TeamParam=" + teamParam + "&StatusParam=" + statusParam;
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
            $('#list-workfromhome').html(data);
            if ($('table#data').length > 0) {
                bindMenuPending();
                bindMenuApproved();
                $('table#data').addClass('table-success'); // class to style mobile

                $('#checkuncheckall').on('click', function () {
                    const isCheckAll = $('#checkuncheckall').prop('checked');

                    $('.chk-request').prop('checked', isCheckAll);
                });
            }

            floatThead('table#data');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
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
    const statusId = urlParams.get('statusId');
    const FromDate = urlParams.get('FromDate');
    const ToDate = urlParams.get('ToDate');
    const teamParam = urlParams.get('TeamParam');
    const statusParam = urlParams.get('StatusParam');

    $("#TeamId").val(TeamId);
    $("#Employee").val(Employee);
    $("#EmployeeId").val(EmployeeId);
    $("#statusId").val(statusId);
    $("#FromDate").val(FromDate);
    $("#ToDate").val(ToDate);

    $(".team .select-dropdown").val(teamParam);
    $(".status .select-dropdown").val(statusParam);
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

function initChangeTextFocusType() {
    document.querySelector("#typeId").addEventListener("change", () => {
        const typeid = document.querySelector("#typeId").value;
        if (typeid === "0" || typeid === "2") {
            $('#address')[0].innerText = "Study Place";
            $('#fromdate')[0].innerText = "Start Date";
            $('#todate')[0].innerText = "End Date";
        } else {
            $('#address')[0].innerText = "Issue Place";
            $('#fromdate')[0].innerText = "Exam Date";
            $('#todate')[0].innerText = "Expiration Date";
        }
    });    
}

function initAddNewDialog() {
    initDatePicker();
    initAutocomplete('#frmAddExperience #Employee', '#frmAddExperience #EmployeeId', false);
    initAutocompleteDepartment('#frmAddExperience #Department', '#frmAddExperience #DepartmentId', false);
    initNumberTextbox();

    document.querySelector("#FromDate").addEventListener('change', () => {
        var a = $(this);
        console.log(this);
    })

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
                        url: _global.root + 'EmployeeExperience/Update',
                        dataType: 'json',
                        data: $('#frmAddExperience').serialize(),
                        success: function (data) {
                            if (data) {
                                let errorCont = $('#divErrorCont');
                                errorCont.removeClass();
                                errorCont.html('');

                                if (data.code === 0) {
                                    errorCont.addClass('success validation-summary-success');
                                    errorCont.append('Update TOEIC score for Employee successfully');
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

$(document).ready(function () {
    query(true);

    $('#submit').on('click', function () {
        query(false);
    });

    $('#btnAddNew').click(function () {
        $.ajax({
            type: 'GET',
            url: _global.root + 'EmployeeExperience/Update',
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