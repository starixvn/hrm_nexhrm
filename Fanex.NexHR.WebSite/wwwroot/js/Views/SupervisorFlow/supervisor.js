
'use strict';

function loadTeamSupervisor(isFirstLoad) {
    $.ajax({
        url: _global.root + 'SupervisorFlow/TeamSupervisor',
        dataType: 'html',
        method: "POST",
        // data: {
        //     Keyword: $('#Keyword').val()
        // },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#team .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#team').html(response);
            $('.select-2', '#team').select2({ closeOnSelect: false });
        },
        error: function () {
            notifyError(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('#team .loading-process').fadeOut(300);
            }
        }
    });
}

function submit() {
    $.ajax({
        type: 'POST',
        url: _global.root + 'SupervisorFlow/TeamUpdate',
        dataType: 'json',
        data: $('#frmTeamUpdate').serialize(),
        success: function (data) {
            if (data.Code === 0) {
                setTimeout(function () { $('#dialog-approver').dialog("close"); query(); }, 3000);
            }
            else {
                var errors = '<ul>';
                $(data.error).each(function (i, value) {
                    errors += '<li>' + value.ErrorMessage + '</li>';
                });
                errors += '</ul>';

                errorCont.addClass('validation-summary-errors');
                errorCont.append(errors);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        }
    });
}

function selectTeam(id, name) {
    var selectedElement = document.querySelector('.team-selected');
    selectedElement.setAttribute('id',id);
    selectedElement.textContent = name;
}

$(document).ready(function () {
    loadTeamSupervisor(true);

    let successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
            function () {
                document.location = _global.root + 'SupervisorFlow';
            }
        );
    }
});