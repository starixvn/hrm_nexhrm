'use strict';
/*global _global:false, reloadButtonText:false, noInformation:false, getParam:false, actingButton:false, _messages:false*/
/* exported index*/

var rowIndex = 1;

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
		if (data.length > 0) {
			rowIndex = 1;

			$('#data tbody').children().remove();
			$('#itemTemplate').tmpl(data).appendTo('#data');

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
			$('#data tbody').children().remove();
			$(noInformation(12)).appendTo('#data');
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

	$('#title').val(jobTitleId);
	$('#title').chosen();
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


function index() {
	return rowIndex++;
}