/*global _global*/
"use strict";

function shiftChange() {
	var empId = parseInt($('#EmployeeId').val());
	if (empId) {
		$.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: empId, date: $('#ForDate').val() },
		function (data) {
			if (data != null) {
				$('input[type=submit]').show();
			}
			else {
				$('input[type=submit]').hide();
			}
		});
	}
}

$(document).ready(function () {
	//Process redirect to My late in/earlt out request.
	var successed = $("#successed").val();
	var year = $("#year").val();
	if (successed === "true") {
		confirm('Successfull!\nPress OK to continue', function () {
			document.location = _global.root + "LateInEarlyOut/EmployeeView/" + year;
		});
	}

		shiftChange();

	$('.datepicker').datepicker("option", "onSelect", function () {
		shiftChange();
	});
});