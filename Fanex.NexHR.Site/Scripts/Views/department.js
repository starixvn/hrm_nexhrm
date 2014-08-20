
'use strict';
/*global _global, autoIndex*/
/*exported deleteDepartment*/

function deleteDepartment(departmentId) {
	confirm('Are you sure you want to delete this department?', function () {
		$.post(_global.root + 'ajax/DeleteDepartment', { id: departmentId }, function (commandResult) {
			if (commandResult.Code !== 0) {
				alert(commandResult.Message);
				return;
			}
			else {
				$('#row' + departmentId).remove();
				autoIndex();
			}
		});
	});
}