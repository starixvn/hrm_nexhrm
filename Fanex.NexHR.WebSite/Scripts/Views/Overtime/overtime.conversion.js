"use strict";

var overtimeConversion = (function ($) {
    var domElement = {
        editConversion: '.editConversion',
        dialogConversion: "#editConversion",
        formConversion: "#editConversion form",
        deleteConversion: '.deleteConversion'
    };

    function query(isFirstLoad) {
        $.ajax({
            url: _global.root + 'Overtime/OvertimeConversionDetail',
            type: 'POST',
            dataType: 'html',
            data: $('.card-container-base .form form').serialize(),
            beforeSend: function () {
                if (!isFirstLoad) {
                    $('.loading-process').fadeIn(300);
                }
            },
            success: function (response) {
                $('table#data tbody').html(response);
                registerEditConversionEvent();
                registerDeleteConversionEvent();
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

    function registerDeleteConversionEvent() {
        $(domElement.deleteConversion).on("click", function () {
            var id = $(this).attr('data-id');
            confirm('Are you sure you want to delete this overtime conversion?', function () {
                $.ajax({
                    url: _global.root + 'Overtime/DeleteOvertimeConversion',
                    method: "POST",
                    data: {
                        Id: id
                    },
                    success: function (res) {
                        if (res.IsSuccessful) {
                            query(false);
                        } else {
                            alert(res.Message);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert(textStatus + ': ' + errorThrown);
                    }
                });
            });
        });
    }

    function registerEditConversionEvent() {
        $(domElement.editConversion).on("click", function (e) {
            e.preventDefault();
            var id = $(this).attr('data-id');
            var deduct = $(this).attr('data-deduct');
            var deductDate = $(this).attr('data-deductdate');
            var description = $(this).attr('data-description');
            $(domElement.dialogConversion).dialog({
                modal: true,
                width: 'auto',
                height: 'auto',
                open: function () {
                    $('#Id').val(id);
                    $('#Deduct').val(deduct);
                    $('#DeductDate').val(deductDate);
                    $('#Description').val(description);
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

    function initModalEditConversionEvent() {
        $(domElement.formConversion).on("submit", function (e) {
            e.preventDefault();
            $.ajax({
                url: _global.root + 'Overtime/EditOvertimeConversion',
                method: "POST",
                data: {
                    Id: $('#Id').val(),
                    Deduct: $('#Deduct').val(),
                    DeductDate: $('#DeductDate').val(),
                    Description: $('#Description').val()
                },
                success: function (res) {
                    if (res.IsSuccessful) {
                        $(domElement.dialogConversion).dialog("close");
                        query(false);
                    } else {
                        alert(res.Message);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        });


        $('#cancel').on("click", function () {
            $(domElement.dialogConversion).dialog('close');
        });
    }

    function registerSubmitEvent() {
        $('#submit').on('click', function () {
            if (validateFromDateAndToDate())
                query(false);
        });
    }

    function initDateChangeEvent() {
        $('#Date').on('change', function () {
            setDateTimeRange($(this).val());
            $('#DateOption').attr('value', $(this).val());
        });

        $('#FromDate, #ToDate').on('change', function () {
            $('#Date').val('0');
            $('#Date').material_select();   // jshint ignore:line
            $('#DateOption').attr('value', '0');
        });

        $('#Date').val($('#DateOption').val()).change();
    }

    function init() {
        registerSubmitEvent();
        initDateChangeEvent();
        initModalEditConversionEvent();
    }

    return {
        initModule: init,
        query: query,
    };
})(jQuery);

$(document).ready(function () {
    overtimeConversion.query(true);
    overtimeConversion.initModule();
});