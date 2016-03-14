$(document).ready(function () {
    var successed = $('#successed').val();

    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + 'lateearly/index';
                }
            );
    }

    $('#Employee').val(_global.nickName);
    $('#EmployeeId').val(_global.employeeId);
});