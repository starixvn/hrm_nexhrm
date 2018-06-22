'use strict';
/*global _global, loadNotification, moment */
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
        success: function (data) {
            if (data) {
                $('#todoList').html(data);
                initTooltipTodoList();
                $('#todoList').mCustomScrollbar({
                    axis: "y",
                    scrollInertia: 100
                });
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

function initLogtimeDialog(issueId) {
    $('#logtime-dialog').dialog({
        modal: true,
        width: 'auto',
        title: 'Log time on issue #' + issueId,
        buttons: [
            {
                text: 'Cancel',
                class: 'btn-cancel waves-effect waves-light m-r--m',
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: 'Submit',
                class: 'btn-submit waves-effect waves-light',
                click: function () {
                    logtimeToIssue();
                }
            }
        ],
        open: function () {
            initDatePicker();
        },
        show: {
            effect: 'blin',
            duration: 300
        },
        hide: {
            effect: 'blin',
            duration: 200
        }
    });
}

function initTooltipTodoList() {
    $('.tooltip-request').tipsy({
        className: 'tipsy-redmine',
        gravity: 'ne',
        html: true,
        title: function () {
            var object = $(this);
            var result = {
                issueId: object.attr('data-issueId'),
                subject: object.attr('data-subject'),
                project: object.attr('data-project'),
                tracker: object.attr('data-tracker'),
                status: object.attr('data-status'),
                startDate: object.attr('data-startdate'),
                dueDate: object.attr('data-duedate'),
                assignee: object.attr('data-assignee'),
                priority: object.attr('data-priority'),
                createdBy: object.attr('data-createdBy'),
            };
            var priorityClass = result.priority.toLowerCase();

            return "<div class='redmine-tooltip'>" +
                        "<p class='subject'><span class='tracker'>" + result.tracker + " #" + result.issueId + ": </span>" + result.subject + "</p>" +
                        "<p><span class='bold'>Project</span>: " + result.project + "</p>" +
                        "<p><span class='bold'>Status</span>: " + result.status + "</p>" +
                        "<p><span class='bold'>Start date</span>: " + result.startDate + "</p>" +
                        "<p><span class='bold'>Due date</span>: " + result.dueDate + "</p>" +
                        "<p><span class='bold'>Created by</span>: " + result.createdBy + "</p>" +
                        "<p><span class='bold'>Assignee</span>: " + result.assignee + "</p>" +
                        "<p><span class='bold'>Priority</span>: <span class='priority "+ priorityClass +"'>" + result.priority + "</span></p>" +
                    "</div>";
        }
    });
}

function displayTotalSpendHourOnDate(onDate) {
    var url = _global.root + 'dashboard/GetTotalLogtimeHourOnDate?onDate=' + onDate;
    var message = "";
    $.ajax({
        url: url,
        method: "GET",
        data: {
            onDate: onDate
        },
        success: function (res) {
            if (res.totalHours > 0) {
                message = "You had input " + res.totalHours + " hour(s) on this day.";
            } else {
                message = "You have not log time on this day."
            }
            $(".total-hour-message").text(message);
        }
    });
}

function logtimeToIssue() {
    $("#formLogtime").find("input").removeClass("input-validation-error");
    $("#formLogtime").find('.field-validation-valid').text("");

    $.ajax({
        type: "POST",
        url: _global.root + 'dashboard/LogtimeIssue',
        data: $("#formLogtime").serialize(),
        success: function (res) {
            if (res.isSuccessful) {
                $("#logtime-dialog").dialog('close');
                alert('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>');
            } else {
                if (res.errors) {
                    showErrorMessage("#formLogtime", res.errors);
                }
                if (res.message) {
                    alert(res.message);
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        }
    });
}

function showErrorMessage(form, errors) {
    errors.forEach(function (error) {
        $(form).find("input[name='" + error.Key + "']").addClass("input-validation-error");
        $(form).find('[data-valmsg-for="' + error.Key + '"]:last').text(error.ErrorMessage);
    });
}

function registerSpentOnEvent() {
    $("#SpentOn").on("change", function () {
        displayTotalSpendHourOnDate($(this).val());
    });
}

function getLogtimeDialog(issueId, issueName) {
    $.ajax({
        type: 'GET',
        url: _global.root + 'dashboard/LogtimeIssue',
        data: {
            issueId,
            issueName
        },
        dataType: 'html',
        success: function (data) {
            $('#logtime-dialog').html(data);
            $('select').material_select();
            initLogtimeDialog(issueId);
            registerSpentOnEvent();
            displayTotalSpendHourOnDate($("#SpentOn").val());
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
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
    var minute = Math.round((value - hour) * 60);
    if (minute >= 60) {
        hour = hour + 1;
        minute = 0;
    }

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
        if (workingHours !== 0) {
            workingHours = workingHours + (1 / 60.0);
            $('#displayWorkingHour').html(displayTotalWorkingHour(workingHours));
            $('#workinghour').val(workingHours);
        }
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