'use strict';

function validateHoliday(e) {
	var holidayName = $("#HolidayName").val();
	var fromDate = new Date($("#FromDate").val());
	var toDate = new Date($("#ToDate").val());
	var dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;
	if (holidayName == null || holidayName === "") {
		alert("Holiday Name is required!");
		e.preventDefault();
	}
	if (dayDiff < 0) {
		alert("To date must be equal or greater than From date.");
		e.preventDefault();
	}
}

$(document).ready(function () {
	$("#ActualFromDate, #ActualToDate, #FromDate, #ToDate").datepicker({
		changeMonth: true,
		changeYear: true
	});
	var holidayName = $("#HolidayName").val();
	var numday = $("#numDay").val();
	var fromDate = $("#FromDate").val();
	var toDate = $("#ToDate").val();
	var actualFromDate = $("#ActualFromDate").val();

	$("#numDay").change(function () {
		setTheDate();
	});

	$("#numDay").click(function () {
		setTheDate();
	});

	function setTheDate() {
		var numday = $("#numDay").val();
		var fromDate = $("#FromDate").val();
		var dat = Date.parse(fromDate);
		var theDate = new Date(dat);
		theDate.setDate(theDate.getDate() + parseInt(numday) - 1);
		$("#ToDate, #ActualToDate").val(theDate.format("dd mmm, yyyy"));
	}

	$("#ActualFromDate").change(function () {
		var numday = $("#numDay").val();
		var fromDate = $("#ActualFromDate").val();
		var dat = Date.parse(fromDate);
		var theDate = new Date(dat);
		theDate.setDate(theDate.getDate() + parseInt(numday) - 1);
		$("#ActualToDate").val(theDate.format("dd mmm, yyyy"));
	});

	$("#btnClear").click(function () {
		$("#HolidayName").val(holidayName);
		$("#ActualFromDate").val(actualFromDate);
		$("#numDay").val(numday);
		$("#FromDate").val(fromDate);
		$("#ToDate").val(toDate);
	});

	$("form").submit(function (e) {
		validateHoliday(e);
	});

	$("#FromDate, #ToDate").change(function () {
		var fromDate = $("#FromDate").datepicker('getDate');
		var toDate = $("#ToDate").datepicker('getDate');
		var dayDiff = (toDate - fromDate) / 1000 / 60 / 60 / 24;
		if (dayDiff === 0) {
			dayDiff = 1;
		}
		$("#numDay").val(dayDiff);

	});
});
