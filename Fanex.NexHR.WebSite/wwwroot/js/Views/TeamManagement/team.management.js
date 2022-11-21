/*global _global, autoIndex, floatThead, initAutocomplete*/
/*exported deleteTeam*/
'use strict';

function deleteTeam(id) {
    confirm('Are you sure you want to delete this team?', function () {
        $.post(_global.root + 'team/Delete', { id: id },
        function (response) {
            if (response.IsSuccessful) {
                $('#row' + id).remove();
                autoIndex();
            }
            else {
                alert(response.Message);
            }
            
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

    initAutocomplete('#Leader', '#LeaderId', false, null, true);
    initAutocomplete('#AssistantLeader', '#AssistantLeaderId', false, null, true);

    var successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + 'Team/Index';
                }
            );
    }
}

$(document).ready(function () {
    if ($('.form-create-edit').length > 0) {
        initCreateEditPage();
    }

    floatThead('table#data');
});