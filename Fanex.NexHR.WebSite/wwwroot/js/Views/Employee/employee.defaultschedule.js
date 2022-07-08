'use strict';
/*global _global:false, getParam: false, id:false, formatDate:false*/

$(document).ready(function () {
    var date = formatDate(new Date(), "MM/dd/yyyy");

    if (getParam('date') === date) {
        $('#monthschedule').prop('checked', true);
    }

    if (getParam('date') === null) {
        $('#defaultschedule').prop('checked', true);
    }

    $('#monthschedule').on('click', function () {
        document.location = _global.root + 'employee/monthschedule?employeeId=' + id;
    });

    $('#defaultschedule').on('click', function () {
        document.location = _global.root + 'employee/defaultschedule?employeeId=' + id;
    });
});