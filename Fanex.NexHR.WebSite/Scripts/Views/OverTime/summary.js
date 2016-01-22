$().ready(function () {
    initDatePicker(".datepicker");
    initSubmitButton();
    initAutocomplete("#CreatedName", "#CreatedBy")
    initAutocomplete("#Employee", "#EmployeeId")
})

function loadSummary() {
    $('.loading-process').fadeIn(200);
    $.post(_global.root + "OverTime/Summary", $("#form").serialize(), function (response) {
        $("#data tbody").html(response);
        $('.loading-process').fadeOut(500);
    })

}
function initSubmitButton() {
    $("#btn-submit").click(function () {
        loadSummary();
    })
}

function approveRequest(id) {
    $('.loading-process').fadeIn(200);
    $.post(_global.root + "OverTime/ApproveRequest", { id: id }, function (response) {
        $('.loading-process').fadeOut(500);
        loadSummary();
    })
}

function verifyRequest(id) {
    $('.loading-process').fadeIn(200);
    $.post(_global.root + "OverTime/VerifyRequest", { id: id }, function (response) {
        $('.loading-process').fadeOut(500);
        loadSummary();
    })
}