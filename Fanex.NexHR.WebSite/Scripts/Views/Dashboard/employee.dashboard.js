'use strict';
/*global _global, loadNotification */
/*exported index, processRequest*/

var workingHours = 0.0;

var MyDashboard = (function ($) {
    return {
        linkDays: null,
        init: function () {
            MyDashboard.linkDays = $(".contextmenu");

            MyDashboard.linkDays.mouseover(function () {
                $(this).find(".drop-menu").show();
            });

            MyDashboard.linkDays.mouseout(function () {
                $(this).find(".drop-menu").hide();
            });
        }
    };
}(jQuery));

function initTodoList() {
    var empId = _global.employeeId;
    $.ajax({
        url: _global.root + 'dashboard/GetTodoList?employeeId=' + empId,
        type: 'GET',
        dataType: 'html',
        beforeSend: function () {
            //if (!isFirstLoad) {
            //    $('.loading-process').fadeIn(300);
            //}
        },
        success: function (data) {
            if (data) {
                $('#todoList').html(data);
            }
        },
        error: function (textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        },
        complete: function () {
            $(".scrollTodo").mCustomScrollbar({
                scrollButtons: { enable: false },
                theme: "dark",
                scrollbarPosition: "inside"
            });
        }
    });
}

function initTeamWorkload() {
    $.ajax({
        url: _global.root + 'dashboard/GetTeamWorkload',
        type: 'GET',
        dataType: 'html',
        beforeSend: function () {
            //if (!isFirstLoad) {
            //    $('.loading-process').fadeIn(300);
            //}
        },
        success: function (data) {
            if (data) {
                $('#teamWorkload').html(data);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        },
        complete: function () {
        }
    });
}

function initTooltipInformation() {
    var linkDays = $(".contextmenu");

    linkDays.mouseover(function () {
        $(this).find(".drop-menu").show();
    });

    linkDays.mouseout(function () {
        $(this).find(".drop-menu").hide();
    });
}

function processRequest(requestId) {
    confirm('Are you sure you want to Approve?', function () {
        $.post(_global.root + 'request/processrequest',
        { command: "Approve", id: requestId },
            function (response) {
                if (response.IsSuccessful) {
                    initTodoList();
                } else {
                    alert(response.Message);
                    return;
                }
            }
        );
    });
}

function displayTotalWorkingHour(value) {
    var display = '',
        hour = Math.floor(value);

    if (hour >= 0) {
        display = (hour < 10 ? '0' + hour + ':' : hour + ':');
    }

    var minute = Math.round((value - hour) * 60);
    return minute < 10 ? display + '0' + minute : display + minute;
}

function caculateWorkingHours() {
    var now = moment();
    var startBreaktime = moment().hours(11).minutes(30);
    var endBreaktime = moment().hours(12).minutes(30);

    if (!(startBreaktime <= now && endBreaktime >= now)) {
        workingHours = parseFloat($('#workinghour').val());
        workingHours = workingHours + (1 / 60.0);
        $('#displayWorkingHour').html(displayTotalWorkingHour(workingHours));
        $('#workinghour').val(workingHours);
    }
}

$(document).ready(function () {
    MyDashboard.init();

    initTooltipInformation();

    $('.slide').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 2000,
        dots: true,
        nextArrow: false,
        prevArrow: false,
    });

    initTodoList();
    loadNotification();
    initTeamWorkload();
    workingHours = parseFloat($('#workinghour').val());

    setInterval(caculateWorkingHours, 60000);
});

