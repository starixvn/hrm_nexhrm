/*global _global, autoIndex, confirm*/
/*exported deleteRequest*/
"use strict";

function deleteRequest(id) {
	var result = confirm('Are you sure you want to delete this request?');
	if (!result) {
		return;
	}

	$.post(_global.root + 'api/employee/deleteworkingtimerequest' + "/" + id, function () {
		$('#row' + id).remove();
		autoIndex();
	});
}