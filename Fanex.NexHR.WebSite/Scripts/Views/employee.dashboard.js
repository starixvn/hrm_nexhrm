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
} (jQuery));

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
    
    var now;
    setInterval(function () {                
        now = new Date($.now());
        var a = now - firstloadTime;
        a
    }, 60000);
});
