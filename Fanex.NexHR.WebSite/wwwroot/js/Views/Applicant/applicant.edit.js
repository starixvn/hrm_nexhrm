'use strict';

$(document).ready(function () {
    initAutocomplete('#Recruiter', '#RecruiterId', null);

    $('.select-2').select2({ closeOnSelect: false });

    $('.resume-container a').on('click', function (e) {
        e.preventDefault();
        var newLocation = $(this).attr('href');
        window.location.href = new URL(newLocation, window.location.origin);
    });

    let successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
            function () {
                document.location = _global.root + 'Applicants';
            }
        );
    }

    const fileUpload = document.getElementById('FileUpload');
    const oldFile = document.getElementsByClassName('resume-container')[0];
    const newFile = document.getElementsByClassName('selected-file')[0];

    fileUpload.addEventListener('change', (event) => {
        const selectedFile = event.target.files[0];
        if (selectedFile) {
            oldFile ? oldFile.style.display = 'none' : null;
            newFile ? newFile.style.width = 'auto' : null;
        }
    });

    $('.delete-resume').on('click', function () {
        $(this).closest('.resume-container').remove();
        newFile.style.width = 'auto';
        var resumeInput = document.getElementById('Resume');
        resumeInput.value = null;
    });

    $('#InterviewDate1').datetimepicker({
        controlType: 'select',
        oneLine: true,
        dateFormat: 'dd M, yy',
        timeFormat: 'hh:mm tt',
        timeInput: true
    });

    $('#InterviewDate2').datetimepicker({
        controlType: 'select',
        oneLine: true,
        dateFormat: 'dd M, yy',
        timeFormat: 'hh:mm tt'
    });
});