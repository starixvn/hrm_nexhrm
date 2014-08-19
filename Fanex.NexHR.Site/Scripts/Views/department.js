
'use strict';
/*global _global, autoIndex*/
/*exported deleteDepartment*/

function deleteDepartment(departmentId) {
	confirm('Are you sure you want to delete this department?', function () {
		$.post(_global.root + 'ajax/DeleteDepartment', { id: departmentId }, function (error) {
			if (error.length > 0) {
				alert(error);
				return;
			}
			$('#row' + departmentId).remove();
			autoIndex();
		});
	});
}