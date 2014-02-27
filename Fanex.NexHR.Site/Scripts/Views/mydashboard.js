
var MyDashBoard = function ($) {
    return {
        dialogReminder: null,
        hasBirthday: null,
        init: function () {
//            MyDashBoard.dialogReminder = $("#dialog-reminder");
//            MyDashBoard.hasBirthday = $("#HasBirthdayOrFirstWorkingday");

//            MyDashBoard.dialogReminder.dialog({
//                autoOpen: false,
//                width: 770,
//                modal: true,
//                buttons: {
//                    "Ok": function () {
//                        $(this).dialog("close");
//                    }
//                },
//                close: function () {
//                }
//            });

//            if (MyDashBoard.hasBirthday.val() == "True") {
//                MyDashBoard.dialogReminder.dialog("open");
//            }
        }
    };
} (jQuery);

$(document).ready(function () {
    MyDashBoard.init();
});

