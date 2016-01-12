'use strict';
/*global _global:false, autoIndex: false*/
/*exported deleteHoliday*/

var rowIndex = 1;
function index() {
    return rowIndex++;
}

$(document).ready(function () {
    var year = $('#year').val();

    $('#submit').click(function () {
        query();
    });
});


function deleteHoliday(id) {
	confirm('Are you sure you want to delete this holiday?', function () {
		$.post(_global.root + 'ajax/DeleteHoliday', { id: id }, function () {
			$('#row' + id).remove();
			autoIndex();
		});
	});
}

function query() {
    $.ajax({
        url: _global.root + 'api/holiday/getholidays',
        data: { year: $('#year').val() },
        beforeSend: function () {
            $('.loading-process').fadeIn(400);
        },
        success: function (data) {
            if (data.length > 0) {
                rowIndex = 1;
                $('#data tbody').children().remove();
                $('#template').tmpl(data).appendTo('#data');
            } else {
                $('#data tbody').children().remove();
                $(noInformation(11)).appendTo('#data');
            }

            $('.loading-process').fadeOut(250);
        },
        error: function () {
            $('.loading-process').fadeOut(250);
            alert(_messages.errorTryAgain);
        }
    });
}