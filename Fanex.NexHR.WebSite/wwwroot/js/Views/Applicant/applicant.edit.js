'use strict';

function handleFileSelection(event) {
    const fileInput = event.target;
    
    if (fileInput.files.length === 0) {
      console.log('No file selected');
    } else {
      // Perform actions when a file is selected
      const selectedFile = fileInput.files[0];
      console.log('Selected file:', selectedFile);
      // ...
    }
  }

$(document).ready(function () {
    initAutocomplete('#Recruiter', '#RecruiterId', null);

    $('.select-2').select2({ closeOnSelect: false });

    $('.delete-resume').on('click', function () {
        $(this).closest('.resume-container').remove();
    });

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
});