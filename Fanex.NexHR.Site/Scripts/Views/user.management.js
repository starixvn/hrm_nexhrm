'use strict';
/*global _global:false, reloadButtonText:false, getParam:false, actingButton:false, _messages:false, moment: false*/
/* exported index*/

var gridOptions = {
	columns: [],
	bodyRows: [],
	showTitle: true,
	titleInfo: {
		text: "Employee management",
		showExcelButton: true,
		exportExcelFunction: function () {
			alert('a');
		}
	},
	gridExpandHeight: 350
};

function formatDate(currentCell) {
	var date = currentCell.Value ? moment(currentCell.Value).format('DD MMM, YYYY') : '',
		otherDate = currentCell.Value ? moment(currentCell.Value).format('YYYY/MM/DD') : '';
	var cell = "<div data-date='" + otherDate +"'>" + date + "</div>";

	return cell;
}

function generateDataHeader() {
	var columns = [
		{ HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Code", Width: 60, Name: "Code", HeaderAlign: "center", CellAlign: "center"},
		{ HeaderText: "Nick", Width: 80, Name: "Nick", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Name", Width: 160, Name: "Name", HeaderAlign: "center", CellAlign: "left"},
		{ HeaderText: "Team", Width: 70, Name: "Team", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Gender", Width: 50, Name: "Gender", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Job Title", Width: 250, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left" },
		{ HeaderText: "DoB", Width: 80, Name: "DateOfBirth", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Joined Date", Width: 80, Name: "JoinedDate", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Edit", Width: 60, Name: "Edit", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Schedule", Width: 60, Name: "Schedule", HeaderAlign: "center", CellAlign: "center" },
	];

	return columns;
}

function generateDataBody(data, isDeleted) {
	var rows = [];
	$.each(data, function (index, item) {
		rows.push({
			Columns: [
				{ Name: "No", Value: index + 1 },
				{ Name: "Code", Value: item.Code },
				{ Name: "Nick", Value: "<a href='/employee/detail/" + item.Id + "' data-code='" + item.Code + "'>"
							+ item.NickName + "</a>", TdClass: 'nick' },
				{ Name: "Name", Value: item.FullName },
				{ Name: "Team", Value: "<div class='tooltip' title='" + item.Team.Name + "'>" + item.Team.Code + "</div>" },
				{ Name: "Gender", Value: item.Gender === 0 ? "F" : "M" },
				{ Name: "JobTitle", Value: item.JobTitle },
				{ Name: "DateOfBirth", Value: item.DateOfBirth, CustomCellRender: formatDate },
				{ Name: "JoinedDate", Value: item.JoinedDate, CustomCellRender: formatDate },
				{ Name: "Edit", Value: !isDeleted ? "<a href='/employee/edit/" + item.Id + "'><img src='/Styles/Images/edit-icon.png'></a>" : "-" },
				{ Name: "Schedule", Value: !isDeleted ? "<a href='/employee/defaultschedule/" + item.Id 
					+ "'><img src='/Styles/Images/edit-icon.png'></a>" : "-" }
			],
			CssClass: item.Enabled ? 'tr-staff-info' : 'disabled'
		});
	});

	return rows;
}

function query(jobTitleId) {

	if (jobTitleId === undefined || jobTitleId === '') {
		jobTitleId = $('#title').val();
	}

	var keyword = $('#keyword').val();
	actingButton('#query', true);

	var enable = null;
	if ($("#enable").attr("checked")) {
		enable = true;
	}
	else {
		if ($("#active").attr("checked")) {
			enable = false;
		}
		else {
			enable = null;
		}
	}

	var isDelete = null;
	if ($("#IsDeleted").attr("checked")) {
		isDelete = true;
	}
	else {
		isDelete = null;
	}

	$.getJSON(
	_global.root + 'api/employee/getemployees',
	{ query: keyword, groupId: $('#group').val(), enable: enable, isDelete: isDelete, titleId: jobTitleId },
	function (data) {
		gridOptions.columns = generateDataHeader();
		gridOptions.bodyRows = generateDataBody(data, isDelete);
		$('#gridUserManagement').fanexGrid(gridOptions);

		if (data.length > 0) {
			$('.nick').tooltip({
				track: true,
				delay: 0,
				showURL: false,
				bodyHandler: function () {
					return '<img src="/employee/avatar/' + $(this).attr('data-code') + '" />';
				}
			});

			$("#title").trigger("liszt:updated");
		}
		else {
			$('#gridUserManagement tbody tr td div').text('No information available');
		}

		reloadButtonText('#query');
	}).error(function () {
		reloadButtonText('#query');
		alert(_messages.errorTryAgain);
	});
	$('#keyword').focus().select();
}

$(document).ready(function () {

	var jobTitleId = $('#jobtitleid').val();

	$("#title").val(jobTitleId);
	//$("#title").chosen();
	$("#title").trigger("liszt:updated");

	$('#keyword').keyup(function (event) {
		if (event.keyCode === 13) {
			query(jobTitleId);
		}
	});

	var q = getParam('query');
	if (q !== null) {
		$('#keyword').val(q);
	}

	//search onload
	query(jobTitleId);

	$('#query').click(function (evt) {
		evt.preventDefault();
		query(jobTitleId);
	});

});