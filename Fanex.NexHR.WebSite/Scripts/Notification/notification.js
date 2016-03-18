function LoadNotification() {
    var empId = _global.employeeId;
    $.ajax({
        url: _global.root + 'dashboard/GetNotification?employeeId=' + empId,
        type: 'GET',
        dataType: 'html',
        beforeSend: function () {
            //if (!isFirstLoad) {
            //    $('.loading-process').fadeIn(300);
            //}
        },
        success: function (data) {
            if (data) {
                $('#notification').html(data);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        },
        complete: function () {
            $('.dropdown-button').dropdown({
                inDuration: 300,
                outDuration: 225,
                constrain_width: false,
                gutter: 0,
                belowOrigin: false,
                alignment: 'left'
            });
        }
    });
}

$(document).ready(function () {
    LoadNotification();
});