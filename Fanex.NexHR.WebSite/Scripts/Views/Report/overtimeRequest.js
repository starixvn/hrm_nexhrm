$(document).ready(function () {
    initDatePicker(".datepicker");
    initSubmitButton();
    initAutocomplete("#CreatedName", "#CreatedBy");
    initAutocomplete("#Employee", "#EmployeeId");

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
    });

    loadSummary();
});

function loadSummary() {
    $('.loading-process').fadeIn(200);
    $.post(_global.root + "Report/OvertimeRequestDetail", $("#form").serialize(), function (response) {
        $("#data tbody").html(response);
        $('.loading-process').fadeOut(500);
    });
};

function initSubmitButton() {
    $("#btn-submit").click(function () {
        loadSummary();
    });
};