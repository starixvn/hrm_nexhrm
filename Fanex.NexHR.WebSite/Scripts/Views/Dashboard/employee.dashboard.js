'use strict';

var firstloadTime = new Date($.now());
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
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        },
        complete: function () {
            //if (!isFirstLoad) {
            //    $('.loading-process').fadeOut(300);
            //}

            //$('.slideTodo').slick({
            //    slidesToShow: 5,
            //    slidesToScroll: 5,
            //    autoplay: true,
            //    autoplaySpeed: 3500,
            //    dots: false,
            //    nextArrow: false,
            //    prevArrow: false,
            //    vertical: true,
            //});
        }
    });
}

function LoadNotification() {
    var empId = _global.employeeId;
    $.ajax({
        url: _global.root + 'dashboard/GetNotification?employeeId=' + empId,
        type: 'GET',
        dataType: 'html',
        beforeSend: function () {
            //if (!isFirstLoad) {
            //    $('.loading-process').fadeIn(300);
            //}
        },
        success: function (data) {
            if (data) {
                $('#notification').html(data);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        },
        complete: function () {
            $('.dropdown-button').dropdown({
                inDuration: 300,
                outDuration: 225,
                constrain_width: false, // Does not change width of dropdown to that of the activator
                gutter: 0, // Spacing from edge
                belowOrigin: false, // Displays dropdown below the button
                alignment: 'left' // Displays dropdown with edge aligned to the left of button
            });
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

    LoadNotification();
});

