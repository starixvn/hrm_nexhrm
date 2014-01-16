var nickName;
$(document).ready(function () {
	$("#Employee").autocomplete({
		source: function (request, response) {
			var term = request.term;
			var index = term.indexOf(' (');
			if (index > 0) term = term.substr(0, index);
			$.ajax({
				url: _global.root + 'api/employee/getemployees',
				data: {
					query: term
				},
				success: function (data) {
					if (data && data.length > 0) {
						response($.map(data, function (item) {
							return {
								label: item.NickName + ' (' + item.FullName + ')',
								value: item.Id
							}
						}));
					}
					else {
						$('#EmployeeId').val('0');
					}
				}
			});
		},
		minLength: 1,
		select: function (event, ui) {
			nickName = ui.item.label;
			$('#Employee').val(nickName);
			$('#EmployeeId').val(ui.item.value);
		},
		focus: function (event, ui) {
			nickName = ui.item.label;
			$('#Employee').val(nickName);
			$('#EmployeeId').val(ui.item.value);
			event.preventDefault();
		},
		close: function () {
			$('#Employee').val(nickName);
		}
	});
});