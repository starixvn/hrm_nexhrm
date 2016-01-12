/// <reference path="../Libs/jquery.min.js" />
/// <reference path="../Libs/fxHeader.js" />
/*global _global, autoIndex*/
/*exported deleteMemoFile, deleteMemo*/
"use strict";
var rowIndex = 1;    
function index() {
    return rowIndex++;
}

function uploadFailed() {
}

function deleteComplete(evt) {
	if (evt.target.status === 200) {
		$('#attach').hide();
		$("#AttachFile").val("");
		$("#Contents").val("");
	}
}

function deleteMemoFile(fileName) {
	var fd = new FormData();
	fd.append('fileName', fileName);
	var xhr = new XMLHttpRequest();
	xhr.addEventListener("load", deleteComplete, false);
	xhr.addEventListener("error", uploadFailed, false);
	xhr.open("POST", _global.root + "file/Delete");
	xhr.send(fd);
}

function deleteMemo(id, fileName, type) {
	confirm('Are you sure you want to delete this memo?', function () {
		$.post(_global.root + 'ajax/deletememo', { id: id }, function () {
			$('#row_' + id).remove();
			autoIndex();
			if (type === 1) {
				deleteMemoFile(fileName);
			}
		});
	});
}

function query() {   
    var category = null,
        year = null;       

    $.ajax({
        url: _global.root + 'api/memo/getmemos',
        data: { category: category, year: year},
        beforeSend: function () {
            $('.loading-process').fadeIn(200);
        },
        success: function (data) {
            if (data.length > 0) {
                rowIndex = 1;
                $('#memo tbody').children().remove();
                $('#template').tmpl(data).appendTo('#memo');                
            } else {
                $('#memo tbody').children().remove();
                $(noInformation(11)).appendTo('#memo');
            }

            $('.loading-process').fadeOut(500);
        },
        error: function () {
            $('.loading-process').fadeOut(500);
            alert(_messages.errorTryAgain);
        }
    });    
}