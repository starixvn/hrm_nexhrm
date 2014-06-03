/*global actingButton, noInformation, reloadButtonText, _messages*/
/*exported index*/
"use strict";

var rowIndex = 1;
function index() {
	return rowIndex++;
}

function view() {
	var $form = $('form');
	if (!$form.valid()) {
		return;
	}

	actingButton('#view', true);
	$('#nodata').remove();
	$.getJSON("/api/report/timesheet", $('form').serialize(), function (response) {
		if (response.length > 0) {
			rowIndex = 1;
			$('#data tbody').children().remove();
			$('#itemTemplate').tmpl(response).appendTo('#data');
		}
		else {
			$('#data tbody').children().remove();
			$(noInformation(11)).appendTo('#data');
		}
		reloadButtonText('#view');
	}).error(function () {
		reloadButtonText('#view');
		alert(_messages.errorTryAgain);
	});
}

$(document).ready(function () {

	$('#monthPickr').monPickr({
		showDateRange: false,
		sortDesc: true,
		today: new Date(),
		minDate: '2010/1/1',
		maxDate: $('#maxDate').val(),
		afterChangeMonth: function () {
			var Month = $('#monthPickr').monPickr("getMonth", "MM");
			var Year = $('#monthPickr').monPickr("getMonth", "yyyy");
			$('#Month').val(Month);
			$('#Year').val(Year);
		}
	});

	view();

	$('#view').click(function (evt) {
		evt.preventDefault();
		view();
	});
});