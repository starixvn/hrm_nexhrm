
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'applicants/index',
        dataType: 'html',
        method: "POST",
        data: {
            FullName: $('#FullName').val(),
            PositionId: $('#PositionId').val(),
            Status: $('#Status').val(),
            FromDate: $('#FromDate').val(),
            ToDate: $('#ToDate').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#applicants-detail').html(data);

            floatThead('table#data');
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

function deleteApplicant(id) {
    confirm('Are you sure you want to delete this applicant?', function () {
        $.post(_global.root + 'applicants/Delete', { id: id },
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

$(document).ready(function () {
    $('#FullName').keyup(function (event) {
        if (event.keyCode === 13) {
            query(false);
        }
    });

    $('#submit').on('click', function () {
        query(false);
    });

    query(true);

    if ($('.form-create-edit').length > 0) {

        initAutocomplete('#Recruiter', '#RecruiterId', null);

        $('.select-2').select2({ closeOnSelect: false });

        let successed = $('#successed').val();
        if (successed === 'true') {
            confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                    function () {
                        document.location = _global.root + 'Applicants/Index';
                    }
                );
        }
    }
});