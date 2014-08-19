'use strict';
/*global _global*/

$(document).ready(function () {
	var managerName = $('#ManagerName').val();

	if (managerName) {
		$('#Manager').val(managerName);
	}

	var nickName;
	$("#Manager").autocomplete({
		source: function (request, response) {
			$.ajax({
				url: _global.root + 'api/employee/getemployees',
				data: {
					query: request.term,
					$top: 10
				},
				success: function (data) {
					response($.map(data, function (item) {
						return {
							label: item.NickName + ' (' + item.FullName + ')',
							value: item.Id
						};
					}));
				}
			});
		},
		minLength: 1,
		select: function (event, ui) {
			nickName = ui.item.label;
			$('#ManagerId').val(ui.item.value);
		},
		focus: function (event, ui) {
			var nickName = ui.item.label;
			$('#Manager').val(nickName);
			$('#ManagerId').val(ui.item.value);
			event.preventDefault();
		},
		change: function (event, ui) {
			nickName = ui.item.label;
			$('#Manager').val(nickName);
			$('#ManagerId').val(ui.item.value);
		},
		close: function () {
			$('#Manager').val(nickName);
		}
	});
});