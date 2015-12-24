/// <reference path="jquery.min.js" />
/// <reference path="date.js" />

(function() {
    var proxied = window.alert;

    window.alert = function() {
        $('<div>' + '<p>' + arguments[0] + '</p>').dialog({
            title       : 'NexHRM',
            modal       : true,
            resizable   : false,
            buttons     : [{
                    text    : 'OK',
                    class   : 'btn-ok button-hrm waves-effect waves-light btn green',
                    click   : function() {
                        $(this).dialog('close');
                        return true;
                    }
                }
            ],
            show        : 'fade',
            hide        : 'fade',
            minHeight   : 50,
            dialogClass : 'modal-shadow'
        });
    };
})();

$.extend({
    getOrCreateDialog: function(id) {
        $box = $('#' + id);
        if (!$box.length) {
            $box = $('<div id="' + id + '"><p></p></div>').hide().appendTo('body');
        }

        return $box;
    }
});

function confirm(message, callback, options) {
    var defaults = {
        title       : 'NexHRM',
        modal       : true,
        resizable   : false,
        buttons     : [
            {
                text: 'OK',
                class: 'btn-ok button-hrm waves-effect waves-light btn green',
                click: function() {
                    $(this).dialog('close');
                    if(callback != null)
                    {
                        callback();
                    }
                    return true;
                }
            },
            {
                text: 'Cancel',
                class: 'btn-cancel button-hrm waves-effect waves-light btn grey',
                click: function() {
                    $(this).dialog('close');
                    return false;
                }
            }
        ],
        show        : 'fade',
        hide        : 'fade',
        minHeight   : 50,
        dialogClass : 'modal-shadow'
    }

    $confirm = $.getOrCreateDialog('confirm');
    // set message
    $($confirm).html(message);
    // init dialog
    $confirm.dialog($.extend({}, defaults, options));
}

function initDatePicker(el) {
    var $input = $(el).pickadate({ selectYears: 2 }),
        picker = $input.pickadate('picker');

    // get minDate, maxDate, disabledDates
    var mindate = $(el).attr('mindate'),
        maxdate = $(el).attr('maxdate'),
        disabledDates = $(el).attr('disabledDate');

    if (mindate !== 'null') {
        mindate = mindate.split('/');
        picker.set('min', new Date(mindate[0], mindate[1] - 1, mindate[2]));
    }

    if (maxdate !== 'null') {
        maxdate = maxdate.split('/');
        picker.set('max', new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
    }

    if (disabledDates.length > 0) {
        var disabledDays = [],
            disabledDates = disabledDates.split(',');

        $.each(disabledDates, function (index, disabledDate) {
            var date = new Date(disabledDate).format('yyyy/mm/dd').split('/');
            disabledDays.push(new Date(date[0], date[1] - 1, date[2]));
        });

        picker.set('disable', disabledDays);
    }

    return picker;
}

$(document).ready(function(){
    var currentUrl = $("#controller").val() + "_" + $("#action").val();
    var selectNavItem = $("#" + currentUrl);
    if (selectNavItem !== undefined) {
        selectNavItem.addClass("active");
    }

    $('.el[data-hashovermenu=True], .el[data-hashovermenu=data-hashovermenu]').hover(
        function () {
            $(this).find('.hoveraction').removeClass('hide').fadeIn(400);
        }, function () {
            $(this).find('.hoveraction').addClass('hide').hide();
        }
    );

    $('.tooltip').tipsy({
        live: true,
        gravity: $.fn.tipsy.autoNS
    });

    $('.slider').slider({full_width: true});
    $('.parallax').parallax();
    $('.modal-trigger').leanModal();
    $('.scrollspy').scrollSpy();
    $('.button-collapse').sideNav({'edge': 'left'});
    $('select').not('.disabled').material_select();

    $('.flash').animate({opacity: 0.25},3500, function(){
        $(this).slideUp().remove();
    });

    $('#searchbox').keyup(function (event) {
        if (event.keyCode === 13) {
            document.location = _global.root + 'employee/index?query=' + $(this).val();
        }
    });

    $('#searchbox').focus(function () {
        $('.search_bar').addClass('focused');
    });

    $('#searchbox').blur(function () {
        if (!$(this).val()) {
            $('.search_bar').removeClass('focused');
        }
    });

    //Handle event when click search icon
    $("#searchIcon").click(function(){
        document.location = _global.root + 'employee/index?query=' + $("#searchbox").val();
    });

    $("input[type=number]").keydown(function(event) {
        // Allow: backspace, delete, tab, escape, and enter
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode === 13 ||
            // Allow: Ctrl+A
            (event.keyCode === 65 && event.ctrlKey === true) ||
            // Allow: home, end, left, right
            (event.keyCode >= 35 && event.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105 )) {
                event.preventDefault();
            }
        }
    });

    var clonedHeaderRow;
    $(".persist-area").each(function () {
        clonedHeaderRow = $(".persist-header", this);
        clonedHeaderRow
            .before(clonedHeaderRow.clone())
            .css("width", clonedHeaderRow.width())
            .addClass("floatingHeader")
            .removeClass("persist-header");
    });

    $(".content_scroll").scroll(updateTableHeaders).trigger("scroll");

    $(window).resize(function() {
        updateTableHeaders();
    });

    $('.processing-submit').click(function (evt) {
        actingButton('.processing-submit', true);
    });
});

function updateTableHeaders() {
    if($(".persist-area").val() != undefined)
    {
        var i = 0,
            leftspace= 191,
            widthspace = $(".persist-area").width() + 18;
        widthspace = widthspace - 17;

        if(widthspace < 1051) {
            widthspace = 1051;
        }

        $('.floatingHeader').width(widthspace);
        var left = $('table thead tr:first').position()["left"];

        $(".persist-area").each(function () {
            var el = $(this),
                offset = el.offset(),
                scrollTop = $(window).scrollTop(),
                floatingHeader = $(".floatingHeader", this);

            if ((scrollTop > offset.top - 60) && (scrollTop < offset.top + el.height())) {
                floatingHeader.css({
                    "visibility": "visible",
                    "left": (leftspace + left)
                });
            } else {
                floatingHeader.css({
                    "visibility": "hidden"
                });
            };
        });

        $("table .persist-header th").each(function () {
            var x = $(this).width();
            $($("table .floatingHeader th")[i]).width(x);
            i++;
        });
    }
}

function totalHoursToTime(value) {
    var display = '',
        hour = Math.floor(value);

    if(hour > 0)
    {
        display = hour < 10 ? '0' + hour + 'h' : hour + 'h';
    }

    var minute = Math.round((value - hour) * 60);
    return minute < 10 ? display + '0' + minute + '\'' : display + minute + '\'';
}

function displayMinute(minutes) {
    var hour = Math.floor(minutes / 60),
        minute = minutes % 60;

    if(hour == 0)
    {
        return minute > 9 ? '00:' + minute : '00:0' + minute;
    }
    return (hour > 9 ? hour : '0' + hour) + ':' + (minute > 9 ? minute : '0' + minute);
}

function checkBadDates(mydate) {
    var isEnable = true;
    var disabledDates = $('.datepicker').attr('disabledDate');
    var myBadDates = disabledDates.split(",");

    var checkdate = new Date(mydate).format("dd mmm, yyyy");
    for (var i = 0; i < myBadDates.length; i++) {
        var date = new Date(myBadDates[i]).format("dd mmm, yyyy");
        if (date == checkdate) {
            isEnable = false;
        }
    }

    return [isEnable];
}

//required: jquery.tipsy.js
function tooltip(element, enable) {
    if (enable) {
        $(element).tipsy({
        live: false,
        gravity: $.fn.tipsy.autoNS
    });
    }
    else {
        $(element).tipsy("hide");
    }
}

function getParam(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return null;
    else
        return results[1];
}

function stringToBool(value)
{
    switch(value)
    {
        case 'null':
            return null;
        case 'false':
            return false;
        case 'true':
            return true;
    }
}

function showMessage(message) {
    $('#message').html(message + ' <img src="/images/ajax-loader.gif" />').css('left',$(window).width()/2);
    $('#message').slideDown();
}

function fadeOutMessage() {
    $('#message').fadeOut(500);
}

function hideMessage() {
    $('#message').text('Done');
    $('#message').fadeOut(500);
}

function flashMessage(message) {
    $('#message').hide();
    $('#message').html(message);
    $('#message').slideDown(function () {
        $('#message').fadeOut(3500);
    });
}

function actingButton(element, disabled)
{
    $(element).attr("origin-value", $(element).attr("value"));
    $(element).attr("value", 'Processing');
    $(element).addClass("disabled");
}

function reloadButtonText(element)
{
    $(element).attr("value", $(element).attr("origin-value"));
    $(element).removeClass("disabled");
}

function getDomain(url) {
    return url.match(/:\/\/(www\.)?(.[^/:]+)/)[2];
}

function getPlainText(text) {
    if(text == null || typeof(text) != "string") return null;
    return text.replace(/<[^>]*>/g, '');
}

function getEncodePlainText(text) {
    if(text == null || typeof(text) != "string") return null;
    return encodeURIComponent(text.replace(/<[^>]*>/g, ''));
}

function formatJsonDate(jsonDate, mask) {
    var date = new Date(parseInt(jsonDate.substr(6)));

    if(mask){
        date = $.format.date(date, mask);
    }

    return date;
};

function initEditor(element, type) {
    switch (type) {
        case "basic":
            tinyMCE.init({
                mode: "exact",
                elements: element,
                theme: "advanced",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                plugins: "safari,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

                theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,fontselect,fontsizeselect,|,forecolor,backcolor",
                theme_advanced_buttons2: "bullist,numlist,|,outdent,indent,|,link,unlink,image,|,table,|,hr,|,sub,sup,|,charmap,emotions,|,ltr,rtl",
                theme_advanced_buttons3: "",
                content_css: '/styles/tinymce.css',
                force_br_newlines: true,
                force_p_newlines: false
            });
            break;
        case "full":
            tinyMCE.init({
                mode: "exact",
                elements: element,
                theme: "advanced",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                plugins: "safari,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

                theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,fontselect,fontsizeselect,|,forecolor,backcolor",
                theme_advanced_buttons2: "pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,code,|,insertdate,inserttime,preview,spellchecker",
                theme_advanced_buttons3: "table,|,hr,removeformat,|,sub,sup,|,charmap,emotions,iespell,media,|,ltr,rtl,|,fullscreen",
                content_css: '/styles/tinymce.css',
                force_br_newlines: false,
                force_p_newlines: true
            });
            break;
    }
}

function removeSign(str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "-");
    str = str.replace(/-+-/g, "-"); //change -- become -
    str = str.replace(/^\-+|\-+$/g, ""); //remove - at begin and end of string
    return str;
}

function enableEmotion(str) {
    var path = '<img src="/images/emoticon/';
    var endClose = '.gif" />';
    str = str.replace(/\n/g, '<br />');
    str = str.replace(/\*:\)/g, path + 'devilish' + endClose);
    str = str.replace(/:\)\)/g, path + 'laughing' + endClose);
    str = str.replace(/:\)/g, path + 'smile' + endClose);
    str = str.replace(/o:-\)/g, path + 'angel' + endClose);
    str = str.replace(/x-\(/g, path + 'angry' + endClose);
    str = str.replace(/:D|:d/g, path + 'biggrin' + endClose);
    str = str.replace(/=\(\(/g, path + 'brokenheart' + endClose);
    str = str.replace(/:\(\(/g, path + 'crying' + endClose);

    str = str.replace(/=p~/g, path + 'drooling' + endClose);
    str = str.replace(/:\(/g, path + 'frown' + endClose);
    str = str.replace(/:-\*/g, path + 'kiss' + endClose);

    str = str.replace(/:x/g, path + 'lovestruck' + endClose);
    str = str.replace(/\*:p/g, path + 'phbbbt' + endClose);
    str = str.replace(/\[-O\*/g, path + 'praying' + endClose);
    str = str.replace(/:-&/g, path + 'puke' + endClose);
    str = str.replace(/=\)\)/g, path + 'rolling' + endClose);
    str = str.replace(/:-p/g, path + 'tongue' + endClose);
    str = str.replace(/#:-s/g, path + 'whew' + endClose);
    str = str.replace(/:-s/g, path + 'worried' + endClose);
    return str;
}

function getImage(text) {
    text = text.replace(/\)|\(|&quot;/g, "");
    var img = $(text).find("img:first").attr("src");
    if(img != null && img != undefined)
    {
        return "<img alt='' src='" + img + "' />";
    }
    else
    {
        return '';
    }
}

jQuery.cookie = function (key, value, options) {
    // key and at least value given, set cookie...
    if (arguments.length > 1 && String(value) !== "[object Object]") {
        options = jQuery.extend({}, options);

        if (value === null || value === undefined) {
            options.expires = -1;
        }

        if (typeof options.expires === 'number') {
            var days = options.expires, t = options.expires = new Date();
            t.setDate(t.getDate() + days);
        }

        value = String(value);

        return (document.cookie = [
            encodeURIComponent(key), '=',
            options.raw ? value : encodeURIComponent(value),
            options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
            options.path ? '; path=' + options.path : '',
            options.domain ? '; domain=' + options.domain : '',
            options.secure ? '; secure' : ''
        ].join(''));
    }

    // key and possibly options given, get cookie...
    options = value || {};
    var result, decode = options.raw ? function (s) { return s; } : decodeURIComponent;
    return (result = new RegExp('(?:^|; )' + encodeURIComponent(key) + '=([^;]*)').exec(document.cookie)) ? decode(result[1]) : null;
};

/*insert to text area*/
$.fn.insertAtCaret = function (myValue) {
    return this.each(function () {
        //IE support
        if (document.selection) {
            this.focus();
            sel = document.selection.createRange();
            sel.text = myValue;
            this.focus();
        }
        //MOZILLA/NETSCAPE support
        else if (this.selectionStart || this.selectionStart == '0') {
            var startPos = this.selectionStart;
            var endPos = this.selectionEnd;
            var scrollTop = this.scrollTop;
            this.value = this.value.substring(0, startPos)
                            + myValue
                            + this.value.substring(endPos, this.value.length);
            this.focus();
            this.selectionStart = startPos + myValue.length;
            this.selectionEnd = startPos + myValue.length;
            this.scrollTop = scrollTop;
        } else {
            this.value += myValue;
            this.focus();
        }
    });
};

function changeNumber(element, amount) {
    var text = $(element).text();
    var r = text.match(/[\d\.]+/g);
    if(text.length == 0 || r == null) return;

    var next = parseInt(r[0]) + amount;
    $(element).text(text.replace(r[0],next));
}

function  isInteger(str) {
    return /^ *[0-9]+ *$/.test(str);
}

function noInformation(col) {
    return '<tr><td colspan="'+col+'"></td></tr><tr><td colspan="'+col+'">No information available</td></tr>';
}

function autoIndex() {
    var index = 0;
    $('tr').each(function () {
        $(this).children("td").eq(0).html(index);
        index = index + 1;
    });
}

function zeroPrefix(value) {
    if(parseInt(value) < 10) return "0" + parseInt(value);
    return value;
}