'use strict';
/*global _global:false , autoIndex*/

function cancelRequest(id) {
    confirm('<p>Are you sure you want to cancel this request?</p>', function () {
        $.post(_global.root + 'request/deleteworkingtimerequest' + "/" + id, function () {
            $('#row' + id).remove();
            autoIndex();
        });
    });
}

function processRequest(command, eventElement) {
    var requestId = $(eventElement).attr('requestId');
    var requestType = $("#RequestType").val();

    switch (command) {
        case 'edit':
            location.href = _global.root + requestType + "/Update/" + requestId;
            break;
        case 'delete':
            cancelRequest(requestId);
            break;
    }
}

function bindMenu(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            processRequest(key, element);
        },
        items: {
            "edit": { name: "Edit", icon: "edit" },
            "delete": { name: "Delete", icon: "delete" }
        }
    });
}

$(document).ready(function () {
    // menu
    bindMenu('.Pending');
});