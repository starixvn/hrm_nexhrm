'use strict';
/*global _global:false, reloadButtonText:false, getParam:false, actingButton:false, _messages:false, moment: false*/
/* exported index*/

var gridOptions = {
	columns: [],
	bodyRows: [],
	gridExpandHeight: 250,
	sortOption: {}
};

function formatDate(currentCell) {
	var date = currentCell.Value ? moment(currentCell.Value).format('DD MMM, YYYY') : '',
		otherDate = currentCell.Value ? moment(currentCell.Value).format('YYYY/MM/DD') : '';
	var cell = "<div data-date='" + otherDate + "'>" + date + "</div>";

	return cell;
}

function generateDataHeader() {
	var columns = [
		{ HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "Nick", Width: 85, Name: "Nick", HeaderAlign: "center", CellAlign: "center", Sort: true },
		{ HeaderText: "Name", Width: 150, Name: "Name", HeaderAlign: "center", CellAlign: "left", Sort: true },
		{ HeaderText: "Team", Width: 60, Name: "Team", HeaderAlign: "center", CellAlign: "center", Sort: true },
		{ HeaderText: "Job Title", Width: 240, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left", Sort: true },
		{ HeaderText: "Skype", Width: 140, Name: "Skype", HeaderAlign: "center", CellAlign: "left", Sort: true },
		{ HeaderText: "Phone", Width: 100, Name: "Phone", HeaderAlign: "center", CellAlign: "center" },
		{ HeaderText: "DoB", Width: 90, Name: "DateOfBirth", HeaderAlign: "center", CellAlign: "center", Sort: true },
		{ HeaderText: "Joined Date", Width: 90, Name: "JoinedDate", HeaderAlign: "center", CellAlign: "center", Sort: true }
	];

	return columns;
}

function generateDataBody(data) {
	var rows = [];
	$.each(data, function (index, item) {
		rows.push({
			Columns: [
				{ Name: "No", Value: index + 1 },
				{ Name: "Nick", Value: "<a data-nick ='" + item.NickName + "' href='/employee/detail/" + item.Id
							+ "' data-code='" + item.Code + "'>" + item.NickName + "</a>", TdClass: 'nick'
				},
				{ Name: "Name", Value: item.FullName },
				{ Name: "Team", Value: "<div class='tooltip' data-team ='" + item.Team.Code.trim().toUpperCase() + "'data-id='" + item.Id
							+ "' title='" + item.Team.Name + "'>" + item.Team.Code + "</div>"
				},
				{ Name: "JobTitle", Value: "<div data-jobtitle='" + item.JobTitle + "' data-id='" + item.Id + "'>"
							+ item.JobTitle + "</div>"
				},
				{ Name: "Skype", Value: item.Skype ? "<a data-skype='" + item.Skype.trim().toUpperCase()
					+ "' href='skype:" + item.Skype.trim() + "?chat'>" + item.Skype.trim()
					+ "</a>" : '<a data-skype="" data-id="' + item.Id + '" href=""></a>'
				},
				{ Name: "Phone", Value: item.Phone },
				{ Name: "DateOfBirth", Value: item.DateOfBirth, CustomCellRender: formatDate },
				{ Name: "JoinedDate", Value: item.JoinedDate, CustomCellRender: formatDate }
			],
			CssClass: 'tr-staff-info'
		});
	});

	return rows;
}

function reRenderBodyAfterSort(rows) {
	$.each(rows, function (index, item) {
		item.Columns[0].Value = index + 1;
	});

	gridOptions.bodyRows = rows;

	$('#gridStaffInfo').fanexGrid(gridOptions);

	if (rows.length > 0) {
		$('.nick').tooltip({
			track: true,
			delay: 0,
			showURL: false,
			bodyHandler: function () {
				return '<img src="/employee/avatar/' + $(this).attr('data-code') + '" />';
			}
		});
	}
	else {
		$('#gridStaffInfo tbody tr td div').text('No information available');
	}

	return rows;
}

function query(jobTitleId) {

	if (jobTitleId === undefined || jobTitleId === '') {
		jobTitleId = $('#title').val();
	}

	var keyword = $('#keyword').val();
	actingButton('#query', true);

	var enable = null;
	var isDelete = null;

	$.getJSON(
	_global.root + 'api/employee/getemployees',
	{ query: keyword, groupId: $('#group').val(), enable: enable, isDelete: isDelete, titleId: jobTitleId },
	function (data) {
		gridOptions.columns = generateDataHeader();
		gridOptions.bodyRows = generateDataBody(data);
		gridOptions.sortOption = {
			afterGridSorted: reRenderBodyAfterSort
		};

		$('#gridStaffInfo').fanexGrid(gridOptions);

		if (data.length > 0) {
			$('.nick').tooltip({
				track: true,
				delay: 0,
				showURL: false,
				bodyHandler: function () {
					return '<img src="/employee/avatar/' + $(this).attr('data-code') + '" />';
				}
			});
		}
		else {
			$('#gridStaffInfo tbody tr td div').text('No information available');
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