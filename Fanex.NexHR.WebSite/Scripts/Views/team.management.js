/*global _global, autoIndex*/
/*exported deleteTeam*/
'use strict';

function deleteTeam(id) {
    confirm('Are you sure you want to delete this team?', function () {
        $.post(_global.root + 'ajax/DeleteTeam', { id: id },
        function (error) {
            if (error.length > 0) {
                alert(error);
                return;
            }
            $('#row' + id).remove();

            autoIndex();
        });
    });
}

function initCreateEditPage() {
    //Validation Leader field
    $('#Leader').rules('add', {
        required: true,
        messages: {
            required: 'The Leader field is required'
        }
    });

    initAutocomplete('#Leader', '#LeaderId');
    initAutocomplete('#AssistantLeader', '#AssistantLeaderId');    
}

$(document).ready(function () {
    if ($('.form-create-edit').length > 0) {
        initCreateEditPage();
    }

    floatThead('table#data');
});