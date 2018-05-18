'use strict';
/*global _global:false, initDatePicker, initAutocomplete*/
/*exported initMonthPick*/

var leaveSchedule = (function ($) {
	var domElement = {
		MonthPick: "#monthPickr",
		Month: "#Month",
		Year: "#Year",
		BtnExportExcel: "#exportExcel",
		Grid: "#grid-zone",
		Form: ".form-filter form"
	}

	function initMonthPick() {
		$('#monthPickr').monPickr({
			showDateRange: false,
			sortDesc: true,
			today: new Date(),
			minDate: '2010/1/1',
			maxDate: $('#maxDate').val(),
			afterChangeMonth: function () {
				var Month = $(domElement.MonthPick).monPickr('getMonth', 'MM'),
					Year = $(domElement.MonthPick).monPickr('getMonth', 'yyyy');
				console.log(Month + '/' + Year);
				$(domElement.Month).val(Month);
				$(domElement.Year).val(Year);
			}
		});

		$('#monthPickr .monpickr-prev span.ui-icon-circle-triangle-w').empty();
		$('#monthPickr .monpickr-next span.ui-icon-circle-triangle-e').empty();

		$('.row').hover(
			function () {
				$(this).addClass('rowhover');
			}, function () {
				$(this).removeClass('rowhover');
			});
	}

	function registerEventExportExcel() {
		$(domElement.BtnExportExcel).on('click', function (e) {
			e.preventDefault();
			var filter = $(domElement.Form).serialize();
			window.location.href = _global.root + "Export/LeaveScheduleReport?" + filter;
		});
	}

	function getLeaveSchedule() {
		$(domElement.Form).on("submit", function (event) {
			event.preventDefault();
			var form = $(this);
			$.ajax({
				url: form.attr("action"),
				method: form.attr("method"),
				data: form.serialize(),
				success: function (data) {
					$(domElement.Grid).html(data);
					fixTable.init();
				},
				error: function () {
					alert(_messages.errorTryAgain);
				},
				complete: function () {
					$('.loading-process').fadeOut(300);
				}
			});
		});
	}

	function initModule() {
		initMonthPick();
		registerEventExportExcel();
		getLeaveSchedule();
	}

	return {
		initModule,
		getLeaveSchedule
	}
})(jQuery);

$(document).ready(function () {
	leaveSchedule.initModule();
	$(".form-filter form").submit();
});