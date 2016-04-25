function initTimepicker(control) {
    control.timepicker({
        'step': 15,
        'disableTimeRanges': [
            ['11:29am', '12:31pm']
        ],
        'timeFormat': 'H:i'
    });
    control.on('changeTime', function () {
        var value = control.timepicker('getTime');
        //var value = control.val();
        //alert(value);
    });
}

$(document).ready(function () {
    if ($('#chkDuration:checkbox:checked').length > 0) {
        $('#duration').show();
    }

    $('#chkDuration').change(function () {
        if ($('#chkDuration:checkbox:checked').length > 0) {
            $('#duration').show();
        }
        else {
            $('#duration').hide();
        }
    });

    initTimepicker($('#FromTime'));
    initTimepicker($('#ToTime'));

    var successed = $('#successed').val();

    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + 'companyevent/index';
                }
            );
    }
});