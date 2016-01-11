'use strict';
/*global _global*/

var TimeRecord = (function ($) {
    return {
        teamIds: null,
        timeRecordContent: null,
        fromDate: null,
        toDate: null,
        btnSubmit: null,
        init: function () {
            $('#checkin').attr('checked', true);
            TimeRecord.timeRecordContent = $('#TimeRecordContent');
            TimeRecord.fromDate = $('#FromDate');
            TimeRecord.toDate = $('#ToDate');
            TimeRecord.btnSubmit = $('#btnSubmit');

            TimeRecord.fillData();

            TimeRecord.btnSubmit.on('click', function () {
                TimeRecord.fillData();
            });
        },
        fillData: function () {
            var params = {
                'fromDate': TimeRecord.fromDate.val(),
                'toDate': TimeRecord.toDate.val()
            };

            if (TimeRecord.timeRecordContent.length > 0) {
                $.ajax({
                    url: _global.root + 'Report/TimeRecordDetail',
                    type: 'GET',
                    data: params,
                    beforeSend: function () {
                        $('.loading-process').fadeIn(200);
                    },
                    success: function (content) {
                        TimeRecord.timeRecordContent.empty();
                        TimeRecord.timeRecordContent.html(content);

                        floatThead('table#data');

                        $('.loading-process').fadeOut(200);
                    }
                });
            }
        }
    };
} (jQuery));

$(document).ready(function () {
    $('#Date').change(function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#Date').val($('#DateOption').val());
    $('#FromDate, #ToDate').change(function () {
        $('#Date').val('0');
        $('#DateOption').attr('value', '0');
    });

    $('#Date').val($('#DateOption').val());
    setDateTimeRange($('#DateOption').val());

    initDatePicker('#FromDate');
    initDatePicker('#ToDate');

    TimeRecord.init();
});