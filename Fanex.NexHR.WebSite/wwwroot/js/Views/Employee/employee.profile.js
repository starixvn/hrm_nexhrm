'use strict';

$(document).ready(function () {
    var empId = $('#Id').val();

    $('.viewhistoryjobgrade').click(function () {
        var url = '/employee/ViewHistoryJobGrade/' + empId;
        $.get(url, function (data) {
            $('#dialog-show-history-jobgrade').html(data);

            $('#dialog-show-history-jobgrade').dialog({
                modal: true,
                height: 'auto',
                width: 'auto',
                title: 'Job Grade History',
                show: {
                    effect: 'fade',
                    duration: 300
                },
                hide: {
                    effect: 'fade',
                    duration: 200
                }
            });
        });
    });
});