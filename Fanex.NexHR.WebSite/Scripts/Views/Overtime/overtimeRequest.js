function LoadOTRequestDetail() {
    $('#OTRequest .loading-process').fadeIn(300);
    $.post(_global.root + "overtime/OvertimeRequestDetail", $("#OTRequest #form").serialize(), function (response) {
        $("#dataOTRequest tbody").html(response);
        $('#OTRequest .loading-process').fadeOut(300);
    });
};

function InitButtonForOTRequest() {
    $("#OTRequest #btn-submit").click(function () {
        LoadOTRequestDetail();
    });
};