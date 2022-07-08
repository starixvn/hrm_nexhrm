/*global _global, _messages, bindMenuPending*/
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'WorkFromHomeRequest/GetWorkFromHomeRequestGrid',
        type: 'GET',
        dataType: 'html',
        data: { year: $('#Year').val() },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#work-from-home-request').html(response);
            bindMenuPending();
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
        }
    });
}

function initCreateUpdatePage() {
    var successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
            function () {
                document.location = _global.root + 'WorkFromHomeRequest';
            }
        );
    }
}

$(document).ready(function () {
    if ($('.form-create-update').length > 0) {
        initCreateUpdatePage();
    } else {
        query(true);

        $('#submit').on('click', function () {
            query(false);
        });
    }
});