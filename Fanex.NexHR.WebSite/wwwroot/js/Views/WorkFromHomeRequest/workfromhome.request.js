/*global _global, _messages, bindMenuPending*/
'use strict';

function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    let year = $('.year .select-dropdown').val();
    let url = _global.root + 'WorkFromHomeRequest/GetWorkFromHomeRequestGrid?' + 'Year=' + year;
    if (!isFirstLoad) {
        let orgUrl = _global.root + 'workfromhomerequest?' + 'Year=' + year;
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
    let urlParams = new URLSearchParams(queryString);
    let year = urlParams.get('Year');

    $(".year .select-dropdown").val(year);

}

function initCreateUpdatePage() {
    let successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
            function () {
                document.location = _global.root + 'WorkFromHomeRequest';
            }
        );
    }
}

$(function () {
    if ($('.form-create-update').length > 0) {
        initCreateUpdatePage();
    } else {
        query(true);

        $('#submit').on('click', function () {
            query(false);
        });
    }
});