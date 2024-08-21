'use strict';
/*global _global*/

$(document).ready(function () {
    var successed = $('#successed').val();
    var year = $('#AffectedFrom').val().split(", ")[1];
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
            function () {
                document.location = _global.root + 'AnnualLeave/Employee?employeeId=' + $('#EmployeeId').val() + "&Year=" + year + "&ExcludedRequestTrans=" + $('#IncludeRequestTrans').prop("checked");
            }
        );
    }
});