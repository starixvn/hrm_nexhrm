'use strict';
/*global _global:false*/
/*global _messages:false*/
/*exported initMonthPick*/

function initMonthPick(today, month) {
	//Month Picker
	var dateRangePicker = $('#DateControl');

	dateRangePicker.monPickr({
		showDateRange: false,
		sortDesc: true,
		today: today,
		month: month,
		minDate: '2010/1/1',
		maxDate: $('#maxDate').val(),
		afterChangeMonth: function () {
			var Month = dateRangePicker.monPickr("getMonth", "MM");
			var Year = dateRangePicker.monPickr("getMonth", "yyyy");
			$('#Month').val(Month);
			$('#Year').val(Year);
		}
	});

	$('.row').hover(
		function () {
			$(this).addClass('rowhover');
		}, function () {
			$(this).removeClass('rowhover');
		});
	}

	$(document).ready(function () {
		initMonthPick($("#hdtoday").val(), $("#hdmonth").val());

		$('#btnAddCalendar').click(function () {
			$('#addform').dialog({
				modal: true,
				width: 410,
				height: 275,
				open: function () {
					// $('#TotalDays').val('');
				}
			});
		});

		$('#btnCancel').click(function () {
			$('#addform').dialog('close');
		});

		$('#btnAdd').click(function () {
			confirm('Are you sure you add calendars?', function () {
				$.post(_global.root + 'ajax/InsertWorkingCalendar',
				{ EmployeeId: $.trim($("#Employee").val()).length > 0 ? $("#EmployeeId").val() : null, 
					FromDate: $("#FromDate").val(), ToDate: $("#ToDate").val() },
				function (response) {
					if (response) {
						document.location = _global.root + 'employee/workingcalendars';
					}
					else {
						alert(_messages.errorTryAgain);
					}
				});
			});
		});
	});