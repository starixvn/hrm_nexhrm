/*global _global, _messages, bindMenuPending*/
'use strict';

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var year = $('.year .select-dropdown').val();
    var url = _global.root + 'WorkFromHomeRequest/GetWorkFromHomeRequestGrid?' + 'Year=' + year;
    if (!isFirstLoad) {
        var orgUrl = _global.root + 'workfromhomerequest?' + 'Year=' + year;
        window.history.replaceState(null, null, orgUrl);
    }

    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'html',
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

function initDataFirstQuery(queryString) {
    var urlParams = new URLSearchParams(queryString);
    var year = urlParams.get('Year');

    $(".year .select-dropdown").val(year);

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