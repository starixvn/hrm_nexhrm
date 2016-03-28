'use strict';
/*global _global */
/* exported initButtonForOTRequest */

function loadOTRequestDetail() {
    $('#OTRequest .loading-process').fadeIn(300);
    $.post(_global.root + "overtime/OvertimeRequestDetail", $("#OTRequest #form").serialize(), function (response) {
        $("#dataOTRequest tbody").html(response);
        $('#OTRequest .loading-process').fadeOut(300);
    });
}

function initButtonForOTRequest() {
    $("#OTRequest #btn-submit").click(function () {
        loadOTRequestDetail();
    });
}