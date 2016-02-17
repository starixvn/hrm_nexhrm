$(document).ready(function () {
    initDatePicker(".datepicker");
    initSubmitButton();

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
    });

    loadSummary();
});

function loadSummary() {
    $('.loading-process').fadeIn(200);
    $.post(_global.root + "report/MyOvertimeResult", $("#form").serialize(), function (response) {
        $("#data tbody").html(response);
        $('.loading-process').fadeOut(500);
    });
};

function initSubmitButton() {
    $("#btn-submit").click(function () {
        loadSummary();
    });
};