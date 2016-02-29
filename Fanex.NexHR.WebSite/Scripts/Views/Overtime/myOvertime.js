$(document).ready(function () {
    initSubmitButton();
    loadSummary();
});

function loadSummary() {
    $('.loading-process').fadeIn(200);
    $.post(_global.root + "overtime/MyOvertimeData", { year: $('#Year').val() }, function (response) {
        $("#data tbody").html(response);
        $('.loading-process').fadeOut(500);
    });
};

function initSubmitButton() {
    $("#btn-submit").click(function () {
        loadSummary();
    });
};