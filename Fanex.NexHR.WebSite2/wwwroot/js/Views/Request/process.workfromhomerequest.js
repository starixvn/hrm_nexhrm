'use strict';
/*global _global:false , autoIndex:false*/
/* exported bindMenuPending */

function cancelRequest(id) {
    confirm('<p>Are you sure you want to cancel this request?</p>', function () {
        $.post(_global.root + 'WorkFromHomeRequest/Delete' + "/" + id, function (data) {
            if (data.IsSuccessful) {
                $('#row_' + id).remove();
                autoIndex();
            }
            else {
                alert(data.Message);
            }
        });
    });
}

function processRequest(command, eventElement) {
    var Id = $(eventElement).attr('requestId')

    switch (command) {
        case 'edit':
            location.href = _global.root + "WorkFromHomeRequest" + "/Edit/" + Id;
            break;
        case 'delete':
            cancelRequest(Id);
            break;
    }
}

function bindMenuPending() {
    $.contextMenu({
        selector: '.Pending',
        trigger: 'left',
        build: function ($trigger, e) {
            return {
                callback: function (key) {
                    processRequest(key, e.currentTarget);
                },
                items: {
                    'edit': { name: 'Edit', icon: 'edit' },
                    'delete': { name: 'Delete', icon: 'delete' }
                }
            };
        }
    });
}