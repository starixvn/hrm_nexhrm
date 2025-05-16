/*global _global, _messages, getParam, floatThead, noInformation, formatJsonDate*/
/* exported index*/
'use strict';

var rowIndex = 1,
  sorts = [];

function index() {
  return rowIndex++;
}

function query(isFirstLoad) {
  var keyword = $('#Keyword').val();

  const queryString = window.location.search;
  if (queryString && isFirstLoad) {
    initDataFirstQuery(queryString);
  }
  var data = 'query=' + keyword + '&teamId=' + $('#TeamId').val() + '&departmentId=' + $('#DepartmentId').val() + '&sort=' + JSON.stringify(sorts);
  var teamName = $('.team .select-dropdown').val();

  var url = _global.root + 'employee/getstaffinfo?' + data;
  if (!isFirstLoad) {
    var orgUrl = _global.root + 'employee?' + data + '&teamName=' + teamName;
    window.history.replaceState(null, null, orgUrl);
  }

  $.ajax({
    url: url,
    beforeSend: function () {
      if (!isFirstLoad) {
        $('.loading-process').fadeIn(300);
      }
    },
    success: function (data) {
      $('table#data tbody').children().remove();

      if (data.length > 0) {
        rowIndex = 1;

        var tmpl = $.templates('#template'),
          helpers = {
            index: index,
            formatDate: formatJsonDate
          },
          html = tmpl.render(data, helpers);

        $(html).appendTo('table#data');

        $('.nick').tipsy({
          className: 'tipsy-avatar',
          gravity: 'n',
          html: true,
          title: function () {
            var src = _global.root + 'employee/avatar/' + $(this).attr('data-code');

            return '<img src="' + src + '" />';
          }
        });
        $('table#data').addClass('responsive-table');
        floatThead('table#data');
      } else {
        $('table#data').removeClass('responsive-table');
        $('table#data').floatThead('destroy');
        $(noInformation(11)).appendTo('table#data');
      }
    },
    error: function () {
      alert(_messages.errorTryAgain);
    },
    complete: function () {
      if (!isFirstLoad) {
        $('.loading-process').fadeOut(300);
      }
    }
  });

  $('#Keyword').focus().select();
}

function initDataFirstQuery(queryString) {
  var urlParams = new URLSearchParams(queryString);
  var query = urlParams.get('query');
  var teamId = urlParams.get('teamId');
  var teamName = urlParams.get('teamName');

  $('#Keyword').val(query);
  $('#TeamId').val(teamId);

  $('.team .select-dropdown').val(teamName);
}

function eventSorting() {
  $('th.col-sorter').on('click', function () {
    var $this = $(this).find('div.sorter'),
      property = $this.attr('data-property'),
      direction,
      index;

    index = sorts
      .map(function (e) {
        return e.Property;
      })
      .indexOf(property);

    if ($this.hasClass('desc')) {
      $this.removeClass('desc').addClass('asc');
      direction = 'asc';
    } else if ($this.hasClass('asc')) {
      $this.removeClass('asc').addClass('desc');
      direction = 'desc';
    } else {
      refreshSorting();
      $this.addClass('desc');
      direction = 'desc';
    }

    if (index !== -1) {
      sorts[index].Direction = direction;
    } else {
      sorts.shift();
      sorts.push({ Property: property, Direction: direction });
    }

    query(false);
  });
}

function refreshSorting() {
  sorts = [];

  $('th.col-sorter').each(function () {
    $(this).find('div.sorter').removeClass('asc').removeClass('desc');
  });
}

function eventExportExcel() {
  $('#exportExcel').on('click', function (evt) {
    evt.preventDefault();

    var isDisable = null,
      isDelete = null,
      jobTitleId = null;

    window.location.href =
      _global.root +
      'Export/EmployeeProfile?query=' +
      $('#Keyword').val() +
      '&teamId=' +
      $('#TeamId').val() +
      '&isDisable=' +
      isDisable +
      '&isDelete=' +
      isDelete +
      '&jobTitleId=' +
      jobTitleId +
      '&sort=' +
      JSON.stringify(sorts);
  });
}

$(document).ready(function () {
  $('#Keyword').keyup(function (event) {
    if (event.keyCode === 13) {
      query(false);
    }
  });

  initDepartmentChange();

  var q = getParam('query');
  if (q !== null) {
    var decode = decodeURIComponent(q);
    $('#Keyword').val(decode);
  }

  eventSorting();
  eventExportExcel();

  //search onload
  query(true);

  $('#query').click(function (evt) {
    evt.preventDefault();
    refreshSorting();
    query(false);
  });
});
