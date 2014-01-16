function deleteRequest(id) {
	var result = confirm('Are you sure you want to delete this request?')
	if (!result) return;

	$.post(_global.root + 'api/employee/deleteworkingtimerequest' + "/" + id, function (data) {
		$('#row' + id).remove();
		autoIndex();
	});
}