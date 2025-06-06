'use strict';

$(document).ready(function () {
    var empId = $('#Id').val();
    var empNickName = $('#NickName').val();

    $('#viewhistoryjobgrade').click(function () {
        var url = '/employee/ViewHistoryJobGrade/' + empId;
        $.get(url, function (data) {
            $('#dialog-show-history-jobgrade').html(data);

            $('#dialog-show-history-jobgrade').dialog({
                modal: true,
                height: 'auto',
                width: 'auto',
                title: empNickName + "'s Job Grade",
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

    $('#viewhistorymanagementgrade').click(function () {
        var url = '/employee/ViewHistoryManagementGrade/' + empId;
        $.get(url, function (data) {
            $('#dialog-show-history-jobgrade').html(data);

            $('#dialog-show-history-jobgrade').dialog({
                modal: true,
                height: 'auto',
                width: 'auto',
                title: empNickName + "'s Management Grade",
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

    $('.tooltip-sd-group').tipsy({
        className: 'tipsy-sd-group',
        gravity: 'nw',
        html: true,
        offset: 5,
        title: function () {
            const titles = $(this).attr('data-title').split(';');
            return "<div>" + titles.map(title => `<p>${title}</p>`).join('') + "</div>";
        }
    });
});