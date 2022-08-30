'use strict';
/*global _global*/

$(document).ready(function () {
    var successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + 'AnnualLeave/Employee?employeeId=' + $('#EmployeeId').val() + "&YearEdit=" + $('#AffectedFrom_Year').val() + "&ExcludedRequestTrans=" + $('#ExcludedRequestTrans').prop("checked");
                }
            );
    }
});