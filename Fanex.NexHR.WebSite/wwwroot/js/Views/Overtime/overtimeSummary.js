'use strict';
/*global _global, _messages, initDatePicker, floatThead*/

var overtimeSummary = (function ($) {
    var domElement = {
        //addLeaveConversion: '.addLeaveConversion',
        //insertformLeaveConversion: '#insertformLeaveConversion',
        //formLeaveConversion: '#insertformLeaveConversion form',
        createConversion: '.createConversion',
        dialogConversion: "#createConversion",
        formConversion: "#createConversion form"
    };

    function registerCreateConversionEvent() {
        $(domElement.createConversion).on("click", function (e) {
            e.preventDefault();
            var employeeId = $(this).attr('data-id');
            $(domElement.dialogConversion).dialog({
                modal: true,
                width: 'auto',
                height: 'auto',
                open: function () {
                    $('#Deduct').val('0.5');
                    $('#Description').val('');
                    $('#EmpId').val(employeeId);
                    initDatePicker();
                },
                show: {
                    effect: 'blind',
                    duration: 200
                },
                hide: {
                    effect: 'blind',
                    duration: 100
                }
            });
        });
    }

    function initModalCreateConversionEvent() {
        $(domElement.formConversion).on("submit", function (e) {
            e.preventDefault();
            $.ajax({
                url: _global.root + 'Overtime/CreateOvertimeConversion',
                method: "POST",
                data: {
                    EmpId: $('#EmpId').val(),
                    Deduct: $('#Deduct').val(),
                    DeductDate: $('#DeductDate').val(),
                    Description: $('#Description').val()
                },
                success: function (res) {
                    if (res.IsSuccessful) {
                        $(domElement.dialogConversion).dialog("close");
                        query(false);
                    } else {
                        notifyError(res.Message);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    notifyError(textStatus + ': ' + errorThrown);
                }
            });
        });
        

        $('#cancel-insert').on("click", function () {
            $(domElement.dialogConversion).dialog('close');
        });
    }

    function query(isFirstLoad) {
        $.ajax({
            url: _global.root + 'Overtime/OvertimeSummary',
            type: 'POST',
            dataType: 'html',
            data: {
                empId: $('#EmployeeId').val(),
                teamid: $('#TeamId').val(),
                year: $('#Year').val()
            },
            beforeSend: function () {
                if (!isFirstLoad) {
                    $('.loading-process').fadeIn(300);
                }
            },
            success: function (response) {
                var curYear = $('#Year').val();
                $('#oldYear').html(curYear - 1);
                $('#curYear').html(curYear);
                $('#listOvertimeSummary').html(response);
                //registerAddLeaveDeductEvent();
                registerCreateConversionEvent();
                floatThead('table#data');
            },

            error: function () {
                notifyError(_messages.errorTryAgain);
            },
            complete: function () {
                if (!isFirstLoad) {
                    $('.loading-process').fadeOut(300);
                }
            }
        });
    }

    //function registerAddLeaveDeductEvent{
    //    $(domElement.addLeaveConversion).on("click", function () {
    //        var employeeId = $(this).attr('data-id');
    //        $('#EmpId').val(employeeId);
    //        $(domElement.insertformLeaveConversion).dialog('open');
    //    });

    //    $(domElement.insertformLeaveConversion).dialog({
    //        modal: true,
    //        autoOpen: false,
    //        width: 'auto',
    //        height: 'auto',
    //        open: function () {
    //            $('#TotalDays').val('0.5');
    //            initDatePicker();
    //        },
    //        show: {
    //            effect: "blind",
    //            duration: 200
    //        },
    //        hide: {
    //            effect: "blind",
    //            duration: 100
    //        }
    //    });
    //}

    //function initAddLeaveConversion() {
    //    $(domElement.formLeaveConversion).on("submit", function (e) {
    //        e.preventDefault();
    //        $.post(_global.root + 'AnnualLeave/AddLeaveConversion',
    //            {
    //                empId: $('#EmpId').val(),
    //                totalDays: $('#TotalDays').val(),
    //                ondate: $('#OnDate').val(),
    //                description: $('#Description').val()
    //            }, function (response) {
    //                if (response.IsSuccessful) {
    //                    $('#insertform').dialog('close');
    //                    document.location = _global.root + 'overtime/OvertimeSummary';
    //                } else {
    //                    alert(response.Message);
    //                }
    //            });
    //    });

    //    $('#cancel').on("click", function () {
    //        $(domElement.insertformLeaveConversion).dialog('close');
    //    });
    //}

    function registerSubmitEvent() {
        $('#submit').on('click', function () {
            query(false);
        }); 
    }

    function init() {
        registerSubmitEvent();
        //initAddLeaveConversion();
        initModalCreateConversionEvent();
    }

    return {
        initModule: init,
        query: query,
    };
})(jQuery);

$(document).ready(function () {
    overtimeSummary.query(true);
    overtimeSummary.initModule();
});