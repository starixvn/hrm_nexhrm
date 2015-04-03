'use strict';
/*global _global:false, reloadButtonText:false, getParam:false, actingButton:false, _messages:false, moment: false*/
/* exported index*/

var gridOptions = {
	columns: [],
	bodyRows: [],
	showTitle: true,
	titleInfo: {
		showExcelButton: true
	},
	gridExpandHeight: 350,
	sortOption: {}
};

function generateDataHeader() {
	var columns = [
		{ HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Code", Width: 55, Name: "Code", HeaderAlign: "center", CellAlign: "center", Sort: true },
		{ HeaderText: "Nick", Width: 70, Name: "Nick", HeaderAlign: "center", CellAlign: "center", Sort: true },
		{ HeaderText: "Name", Width: 150, Name: "Name", HeaderAlign: "center", CellAlign: "left", Sort: true },
		{ HeaderText: "Team", Width: 70, Name: "Team", HeaderAlign: "center", CellAlign: "center", Sort: true },
		{ HeaderText: "Gender", Width: 70, Name: "Gender", HeaderAlign: "center", CellAlign: "center", Sort: true },
		{ HeaderText: "Job Title", Width: 250, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left", Sort: true },
		{ HeaderText: "DoB", Width: 80, Name: "DateOfBirth", HeaderAlign: "center", CellAlign: "center", Sort: true },
		{ HeaderText: "Joined Date", Width: 100, Name: "JoinedDate", HeaderAlign: "center", CellAlign: "center", Sort: true },
		{ HeaderText: "Edit", Width: 50, Name: "Edit", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Schedule", Width: 50, Name: "Schedule", HeaderAlign: "center", CellAlign: "center" },
	];

	return columns;
}

function generateDataBody(data, isDeleted) {
	var rows = [];
	$.each(data, function (index, item) {
		var gender = item.Gender === 0 ? 'F' : 'M';
		var dateOfBirth = item.DateOfBirth ? moment(item.DateOfBirth).format('DD MMM, YYYY') : '',
			dataDateOfBirth = item.DateOfBirth ? moment(item.DateOfBirth).format('YYYY/MM/DD') : '',
			joinedDate = item.JoinedDate ? moment(item.JoinedDate).format('DD MMM, YYYY') : '',
			dataJoinedDate = item.JoinedDate ? moment(item.JoinedDate).format('YYYY/MM/DD') : '';

		rows.push({
			Columns: [
				{ Name: "No", Value: index + 1 },
				{ Name: "Code", Value: item.Code },
				{ Name: "Nick", Value: "<a data-nick ='" + item.NickName + "' href='/employee/detail/" + item.Id + "' data-code='" + item.Code + "'>"
							+ item.NickName + "</a>", TdClass: 'nick'
				},
				{ Name: "Name", Value: item.FullName },
				{ Name: "Team", Value: "<div class='tooltip' data-team ='" + item.Team.Code.trim().toUpperCase() + "'data-id='" + item.Id
							+ "' title='" + item.Team.Name + "'>" + item.Team.Code + "</div>"
				},
				{ Name: "Gender", Value: "<div data-gender='" + gender + "' data-id='" + item.Id + "'>" + gender + "</div>" },
				{ Name: "JobTitle", Value: "<div data-jobtitle='" + item.JobTitle + "' data-id='" + item.Id + "'>"
							+ item.JobTitle + "</div>"
				},
				{ Name: "DateOfBirth", Value: "<div data-date='" + dataDateOfBirth + "' data-id='" + item.Id + "'>" + dateOfBirth + "</div>" },
				{ Name: "JoinedDate", Value: "<div data-date='" + dataJoinedDate + "' data-id='" + item.Id + "'>" + joinedDate + "</div>" },
				{ Name: "Edit", Value: !isDeleted ? "<a href='/employee/edit/" + item.Id + "'><img src='/Styles/Images/edit-icon.png'></a>" : "-" },
				{ Name: "Schedule", Value: !isDeleted ? "<a href='/employee/defaultschedule/" + item.Id
					+ "'><img src='/Styles/Images/edit-icon.png'></a>" : "-"
				}
			],
			CssClass: item.Enabled ? 'tr-staff-info' : 'disabled'
		});
	});

	return rows;
}

function reRenderBodyAfterSort(rows) {
	$.each(rows, function (index, item) {
		item.Columns[0].Value = index + 1;
	});

	gridOptions.bodyRows = rows;

	$('#gridUserManagement').fanexGrid(gridOptions);

	$('.nick').tooltip({
		track: true,
		delay: 0,
		showURL: false,
		bodyHandler: function () {
			return '<img src="/employee/avatar/' + $(this).attr('data-code') + '" />';
		}
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
		gridOptions.sortOption = {
			afterGridSorted: reRenderBodyAfterSort
		};
		gridOptions.titleInfo.exportExcelFunction = function () {
			window.location.href = _global.root + 'Export/EmployeeProfile?query=' + keyword + '&groupId=' + $('#group').val()
				+ '&enable=' + enable + '&isDelete=' + isDelete + '&titleId=' + jobTitleId;
		};

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
	$("#title").chosen();
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