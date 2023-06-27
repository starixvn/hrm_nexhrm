/// <reference path='jquery.min.js' />

function alert(message, callback) {
    var isChrome = !!window.chrome

    if (isChrome) {
        var defaults = {
            title: 'NexHRM',
            modal: true,
            resizable: false,
            buttons: [
                {
                    text: 'OK',
                    class: 'btn-ok waves-effect waves-light',
                    click: function () {
                        $(this).dialog('close');
                        if (callback != null) {
                            callback();
                        }
                        return true;
                    }
                }
            ],
            show: 'fade',
            hide: 'fade',
            minHeight: 50,
            dialogClass: 'modal-shadow'
        }

        $alert = $.getOrCreateDialog('alert');
        // set message
        $($alert).html(message);
        // init dialog
        $alert.dialog($.extend({}, defaults));
    }
};

$.extend({
    getOrCreateDialog: function (id) {
        $box = $('#' + id);
        if (!$box.length) {
            $box = $('<div id="' + id + '"><p></p></div>').hide().appendTo('body');
        }

        return $box;
    }
});

function confirm(message, callback, options) {
    var defaults = {
        title: 'NexHRM',
        modal: true,
        resizable: false,
        buttons: [
            {
                text: 'Cancel',
                class: 'btn-cancel waves-effect waves-light m-r--m',
                click: function () {
                    $(this).dialog('close');
                    return false;
                }
            },
            {
                text: 'OK',
                class: 'btn-ok waves-effect waves-light',
                click: function () {
                    $(this).dialog('close');
                    if (callback != null) {
                        callback();
                    }
                    return true;
                }
            }
        ],
        show: 'fade',
        hide: 'fade',
        minHeight: 50,
        dialogClass: 'modal-shadow'
    }

    $confirm = $.getOrCreateDialog('confirm');
    // set message
    $($confirm).html(message);
    // init dialog
    $confirm.dialog($.extend({}, defaults, options));
}

function initDatePicker() {
    $('.datepicker-button').each(function () {
        var $datepicker = $($(this).find('.datepicker')),
            $icon = $($(this).find('.calendar-icon'));

        $datepicker.datepicker({
            dateFormat: "dd M, yy",
            changeMonth: true,
            changeYear: true,
            nextText: '',
            prevText: '',
            showAnim: 'blind'
        });

        // get minDate, maxDate, disabledDates
        var mindate = $datepicker.attr('mindate'),
            maxdate = $datepicker.attr('maxdate'),
            disabledDates = $datepicker.attr('disabledDate');

        if (mindate !== 'null' && mindate !== undefined) {
            mindate = mindate.split('/');
            $datepicker.datepicker("option", 'minDate', new Date(mindate[0], mindate[1] - 1, mindate[2]));
        }

        if (maxdate !== 'null' && maxdate !== undefined) {
            maxdate = maxdate.split('/');
            $datepicker.datepicker("option", 'maxDate', new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
        }

        if (disabledDates !== 'null' && disabledDates.length > 0) {
            $datepicker.datepicker("option", 'beforeShowDay', function (mydate) {
                var isEnable = true,
                    badDates = disabledDates.split(","),
                    checkdate = $.format.date(new Date(mydate), 'dd MMM, yyyy');

                for (var i = 0; i < badDates.length; i++) {
                    var date = $.format.date(new Date(badDates[i]), 'dd MMM, yyyy');

                    if (date == checkdate) {
                        isEnable = false;
                    }
                }

                return [isEnable];
            });
        }

        $icon.on('click', function (event) {
            event.stopPropagation();
            event.preventDefault();

            if (!$datepicker.datepicker('widget').is(':visible')) {
                $datepicker.datepicker('show');
            }
        });
    });
}

function setDateTimeRange(selectedIndex) {
    var fromDate = new Date($('#FromDate').val()),
        toDate = new Date($('#ToDate').val()),
        now = $('#DateTime-Now').val();

    if (now === undefined) {
        var toDay = new Date();
        now = toDay.toString("dd MMM, yyyy");
    }

    fromDate = $.format.date(fromDate, 'dd MMM, yyyy');
    toDate = $.format.date(toDate, 'dd MMM, yyyy');

    switch (parseInt(selectedIndex)) {
        case 0: //default
            break;
        case 1: //today
            toDate = fromDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
        case 2: // yesterday
            toDate = fromDate = Date.parse(now).add(-1).days().toString('dd MMM, yyyy');
            break;
        case 3: //current week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
        case 4: //last week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-7).days().toString('dd MMM, yyyy');
            toDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-1).days().toString('dd MMM, yyyy');
            break;
        case 5: //current month
            fromDate = Date.parse(now).moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
        case 6: //last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).add(-1).months().moveToLastDayOfMonth().toString('dd MMM, yyyy');
            break;
        case 7: //since last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
    }

    $('#FromDate').val(fromDate);
    $('#ToDate').val(toDate);
}

function validateFromDateAndToDate() {
    var fromDate = new Date($("#FromDate").val()),
        toDate = new Date($("#ToDate").val()),
        maxDate = new Date(fromDate.getFullYear() + 1, fromDate.getMonth(), fromDate.getDate()),
        isValid = true;

    if (toDate.getTime() < fromDate.getTime()) {
        $('#toast-container').children().remove();
        Materialize.toast('<i class="mdi mdi-close-circle"></i><span class="m-l--m">From Date must be less than or equal To Date<span>', 2000);

        isValid = false;
    } else if (toDate > maxDate) {
        $('#toast-container').children().remove();
        Materialize.toast('<i class="mdi mdi-close-circle"></i><span class="m-l--m">From Date and To Date must not exceed 365 days<span>', 2000);

        isValid = false;
    }

    return isValid;
}

$(document).ready(function () {
    var currentUrl = $('#controller').val() + '_' + $('#action').val(),
        selectNavItem = $('#' + currentUrl);
    console.log(selectNavItem);
    if (selectNavItem !== undefined) {
        selectNavItem.addClass('active');

        var section = selectNavItem.closest('div.collapsible-body').closest('li.section');
        if (!$(section).hasClass('active')) {
            $(section).find('> a.collapsible-header').trigger('click');
        }
    }

    $('.tooltip').tipsy({
        live: true,
        gravity: $.fn.tipsy.autoNS
    });

    $('.slider').slider({ full_width: true });
    $('.parallax').parallax();
    $('.modal-trigger').leanModal();
    $('.scrollspy').scrollSpy();
    $('.button-collapse').sideNav({ 'edge': 'left' });
    $('select').not('.disabled').material_select();

    $('.flash').animate({ opacity: 0.25 }, 3500, function () {
        $(this).slideUp().remove();
    });

    $('#searchbox').keyup(function (event) {
        if (event.keyCode === 13) {
            document.location = _global.root + 'employee/index?query=' + $(this).val();
        }
    });

    $('#label-search').click(function () {
        $(this).parent().addClass('focused');
        $("#searchbox").focus();
    });

    $("#searchbox").blur(function () {
        if (!$(this).val()) {
            $(this).parent().removeClass('focused');
        }
    });

    //Handle event when click search icon
    $('#searchIcon').click(function () {
        document.location = _global.root + 'employee/index?query=' + $('#searchbox').val();
    });

    $('input[type=number]').keydown(function (event) {
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
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
            }
        }
    });

    if ($('.datepicker-button').length > 0) {
        initDatePicker();
    }

    $('.processing-submit').click(function (evt) {
        $('.loading-process').fadeIn(300);
    });
});

function totalHoursToTime(value) {
    var display = '',
        hour = Math.floor(value);

    if (hour > 0) {
        display = hour < 10 ? '0' + hour + 'h' : hour + 'h';
    }

    var minute = Math.round((value - hour) * 60);
    return minute < 10 ? display + '0' + minute + '\'' : display + minute + '\'';
}

function displayMinute(minutes) {
    var hour = Math.floor(minutes / 60),
        minute = minutes % 60;

    if (hour == 0) {
        return minute > 9 ? '00:' + minute : '00:0' + minute;
    }
    return (hour > 9 ? hour : '0' + hour) + ':' + (minute > 9 ? minute : '0' + minute);
}

function displayMinuteForTotalWorking(minutes) {
    var hour = Math.floor(minutes / 60),
        minute = minutes % 60;

    if (hour == 0) {
        if (minute == 0) return ""
        return minute > 9 ? minute + '\'' : '0' + minute + '\'';
    }
    return (hour > 9 ? hour : '0' + hour) + 'h' + (minute > 9 ? minute : '0' + minute) + '\'';
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
        $(element).tipsy('hide');
    }
}

function getParam(name) {
    name = name.replace(/[\[]/, '\\\[').replace(/[\]]/, '\\\]');
    var regexS = '[\\?&]' + name + '=([^&#]*)';
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return null;
    else
        return results[1];
}

function stringToBool(value) {
    switch (value) {
        case 'null':
            return null;
        case 'false':
            return false;
        case 'true':
            return true;
    }
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

function actingButton(element, disabled) {
    $(element).attr('origin-value', $(element).attr('value'));
    $(element).attr('value', 'Processing');
    $(element).addClass('disabled');
}

function reloadButtonText(element) {
    $(element).attr('value', $(element).attr('origin-value'));
    $(element).removeClass('disabled');
}

function getDomain(url) {
    return url.match(/:\/\/(www\.)?(.[^/:]+)/)[2];
}

function getPlainText(text) {
    if (text == null || typeof (text) != 'string') return null;
    return text.replace(/<[^>]*>/g, '');
}

function getEncodePlainText(text) {
    if (text == null || typeof (text) != 'string') return null;
    return encodeURIComponent(text.replace(/<[^>]*>/g, ''));
}

function formatJsonDate(jsonDate, mask) {
    //var date = new Date(parseInt(jsonDate.substr(6)));

    if (mask) {
        jsonDate = $.format.date(jsonDate, mask);
    }
    return jsonDate;
};

function formatJsonDateToShortFormatedDate(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6))),
        h = date.getHours(),
        m = date.getMinutes();

    if (h < 10) {
        h = '0' + h;
    }

    if (m < 10) {
        m = '0' + m;
    }

    date = $.format.date(date, 'dd MMM, yyyy');

    return date + ' - ' + h + 'h' + m + '\'';
};

function formatDate(date, mask) {
    if (mask) {
        date = $.format.date(date, mask);
    }

    return date;
};

function initEditor(element, type) {
    tinyMCE.baseURL = RootUrlTinyMCE;
    switch (type) {
        case 'basic':
            tinyMCE.init({
                selector: element,
                plugins: [
                    'advlist autolink link image lists charmap print preview hr anchor pagebreak',
                    'searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking',
                    'table emoticons template paste help'
                ],
                toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | print preview media fullpage | forecolor backcolor emoticons | help',
                menubar: ''
            });
            break;
        case 'full':
            tinyMCE.init({
                selector: element,
                plugins: [
                    'advlist autolink link image lists charmap print preview hr anchor pagebreak',
                    'searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking',
                    'table emoticons template paste help'
                ],
                toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | ' +
                    'bullist numlist outdent indent | link image | print preview media fullpage | ' +
                    'forecolor backcolor emoticons | help'
            });
            break;
    }
}

function removeSign(str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, 'a');
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, 'e');
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, 'i');
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, 'o');
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, 'u');
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, 'y');
    str = str.replace(/đ/g, 'd');
    str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\'|\&|\#|\[|\]|~/g, '-');
    str = str.replace(/-+-/g, '-'); //change -- become -
    str = str.replace(/^\-+|\-+$/g, ''); //remove - at begin and end of string
    return str;
}

function enableEmotion(str) {
    var path = "<img src='/images/emoticon/",
        endClose = ".gif' />";

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
    text = text.replace(/\)|\(|&quot;/g, '');
    var img = $(text).find('img:first').attr('src');
    if (img != null && img != undefined) {
        return "<img alt='' src='" + img + "' />";
    }
    else {
        return '';
    }
}

jQuery.cookie = function (key, value, options) {
    // key and at least value given, set cookie...
    if (arguments.length > 1 && String(value) !== '[object Object]') {
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
    if (text.length == 0 || r == null) return;

    var next = parseInt(r[0]) + amount;
    $(element).text(text.replace(r[0], next));
}

function isInteger(str) {
    return /^ *[0-9]+ *$/.test(str);
}

function noInformation(col) {
    return '<tr><td colspan="' + col + '"></td></tr><tr><td colspan="' + col + '">No information available</td></tr>';
}

function autoIndex() {
    var index = 0;
    $('tr').each(function () {
        $(this).children('td').eq(0).html(index);
        index = index + 1;
    });
}

function zeroPrefix(value) {
    if (parseInt(value) < 10) return '0' + parseInt(value);
    return value;
}

function floatThead(el) {
    var heightHeader = $('header nav.right').outerHeight(false);
    $(el).floatThead({
        top: heightHeader,
        zIndex: 888,
        autoReflow: true,
        responsiveContainer: function ($table) {
            return $table.parent();
        },
    });

    //$('table#data').floatThead('reflow');
}

function ajaxLoadingProcess() {
    $(document).ajaxStart(function () {
        $('.loading-process').fadeIn(300);
    }).ajaxStop(function () {
        $('.loading-process').fadeOut(300);
    });
}

function initAutocomplete(selectorName, selectorId, isSpecial) {
    $(selectorName).autocomplete({
        source: function (request, response) {
            var term = request.term;
            var index = term.indexOf(' (');

            if (index > 0) {
                term = term.substr(0, index);
            }
            $.ajax({
                url: _global.root + 'employee/GetEmployeeSuggestion',
                dataType: 'json',
                data: {
                    query: term,
                    isSpecial: isSpecial,
                    $top: 10
                },
                success: function (data) {
                    var listEmployee = data;
                    if (listEmployee && listEmployee.length > 0) {
                        response($.map(listEmployee, function (item) {
                            console.log(item)
                            return {
                                label: item.NickName + ' (' + item.FullName + ')',
                                value: item.Id
                            };
                        }));
                    }
                    else {
                        $(selectorId).val('0');
                    }
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            event.preventDefault();
            nickName = ui.item.label;
            $(selectorName).val(nickName);
            $(selectorId).val(ui.item.value);
        },
        focus: function (event, ui) {
            event.preventDefault();
            nickName = ui.item.label;
            $(selectorName).val(nickName);
            $(selectorId).val(ui.item.value);
        },
        change: function (event, ui) {
            event.preventDefault();
            if (ui && ui.item) {
                nickName = ui.item.label;
                $(selectorName).val(nickName);
                $(selectorId).val(ui.item.value);
            }
            else {
                if ($(selectorName).val() === "") {
                    $(selectorId).val('');
                } else {
                    $(selectorId).val('0');
                }
            }
        }
    });
}

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

function notifyError(errorMessage) {
    $('#toast-container').children().remove();
    Materialize.toast('<i class="mdi mdi-close-circle"></i><span class="m-l--m">' + errorMessage + '<span>', 2000);
}