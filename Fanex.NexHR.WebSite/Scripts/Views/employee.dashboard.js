'use strict';

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
});
