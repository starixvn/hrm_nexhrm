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

$(document).ready(function () {
    if ($('.form-create-edit').length > 0) {
        //Validation Leader field
        $('#Leader').rules('add', {
            required: true,
            messages: {
                required: 'The Leader field is required'
            }
        });

        var nickNameLeader, nickNameAssistantLeader;
        //Seach leader
        $('#Leader').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: _global.root + 'api/employee/GetEmployees',
                    dataType: 'json',
                    data: {
                        query: request.term,
                        $top: 10
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.NickName + ' (' + item.FullName + ')',
                                value: item.Id
                            };
                        }));
                    }
                });
            },
            minLength: 1,
            select: function (event, ui) {
                nickNameLeader = ui.item.label;
                $('#Leader').val(nickNameLeader);
                $('#LeaderId').val(ui.item.value);
            },
            focus: function (event, ui) {;
                $('#Leader').val(ui.item.label);
                event.preventDefault();
            },
            close: function () {
                if (nickNameLeader) {
                    $('#Leader').val(nickNameLeader);
                } else {
                    $('#Leader').val('');
                }
            }
        });

        //Seach Assistant Leader
        $('#AssistantLeader').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: _global.root + 'api/employee/GetEmployees',
                    dataType: 'json',
                    data: {
                        query: request.term,
                        $top: 10
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.NickName + ' (' + item.FullName + ')',
                                value: item.Id
                            };
                        }));
                    }
                });
            },
            minLength: 1,
            select: function (event, ui) {
                nickNameAssistantLeader = ui.item.label;
                $('#AssistantLeader').val(nickNameAssistantLeader);
                $('#AssistantLeaderId').val(ui.item.value);
            },
            focus: function (event, ui) {
                $('#AssistantLeader').val(ui.item.label);
                event.preventDefault();
            },
            close: function () {
                if (nickNameAssistantLeader) {
                    $('#AssistantLeader').val(nickNameAssistantLeader);
                } else {
                    $('#AssistantLeader').val('');
                }
            }
        });
    }
});