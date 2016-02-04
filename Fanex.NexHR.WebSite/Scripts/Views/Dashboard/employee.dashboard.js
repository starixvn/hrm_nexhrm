'use strict';

var firstloadTime = new Date($.now());
var MyDashboard = (function ($) {
    return {
        linkDays: null,
        init: function () {
            MyDashboard.linkDays = $(".lnk-days");

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

            $('.slideTodo').slick({
                slidesToShow: 5,
                slidesToScroll: 5,
                autoplay: true,
                autoplaySpeed: 3500,
                dots: false,
                nextArrow: false,
                prevArrow: false,
                vertical: true,
            });
        }
    });    
}


$(document).ready(function () {
    MyDashboard.init();
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
});
