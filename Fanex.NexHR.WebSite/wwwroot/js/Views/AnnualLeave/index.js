'use strict';
/*global _global, floatThead*/
/*exported title*/

var _checked = false,
  _totalDays = 0,
  _ids;

function eventAdjustment() {
  $('#adjustment').on('click', function () {
    _ids = [];
    $('.aba').each(function (index, item) {
      var checked = $(item).prop('checked');
      if (checked) {
        _ids.push($(item).attr('data-id'));
      }
    });

    $('#EmpIds').val(_ids);

    if (_ids.length > 0) {
      $('#insertform').dialog({
        title: 'Adjust Annual Leave Balance',
        modal: true,
        width: 'auto',
        dialogClass: 'add-adjustment',
        open: function () {
          $('#TotalDays').val('');
        }
      });
    } else {
      alert('Please choose at least one employee to continue!');
    }
  });

  $('#insertform form').submit(function (evt) {
    var $form = $(this);
    evt.preventDefault();
    confirm('Are you sure you want to add leave transaction?', function () {
      _totalDays = parseFloat($('#TotalDays').val());

      if ($form.valid()) {
        $.post(_global.root + 'AnnualLeave/AdjustAnnualLeaves', $('form').serialize(), function (response) {
          if (response.IsSuccessful) {
            $.each(_ids, function (index, value) {
              var currentAnnualLeaveBalance = parseFloat($('#alb' + value).text());
              $('#alb' + value).text(currentAnnualLeaveBalance + _totalDays);
            });
            const queryString = window.location.search;
            document.location = _global.root + 'annualleave/index' + queryString;
          } else {
            alert(response.Message);
          }
        });
      }
    });
  });

  $('#cancel').click(function () {
    $('#insertform').dialog('close');
  });
}

function eventAddBalance() {
  $('#addbalance').on('click', function () {
    confirm('Are you sure you add annual leave balance for new year?', function () {
      $.post(_global.root + 'AnnualLeave/OpenAnnualLeaves', function (response) {
        if (response.IsSuccessful) {
          document.location = _global.root + 'annualleave/index';
        } else {
          alert(response.Message);
        }
      });
    });
  });
}

function eventCloseBalance() {
  $('#resetblc').on('click', function () {
    var lastYear = new Date().getFullYear() - 1;

    $('#resetform').dialog({
      title: 'Close Balance For Year ' + lastYear,
      modal: true,
      width: 'auto',
      dialogClass: 'reset-balance',
      open: function () {
        $('#TotalDays').val('');
      }
    });
  });

  $('#resetform form').submit(function (evt) {
    evt.preventDefault();
    confirm('Are you sure you want to reset leave transaction?', function () {
      _totalDays = parseFloat($('#MinBalance').val());

      if (_totalDays >= 0) {
        $.post(_global.root + 'AnnualLeave/ResetAnnualLeaves', { minbalance: _totalDays, descriptionReset: $('#DescriptionReset').val() }, function (response) {
          if (response.IsSuccessful) {
            $('#resetform').dialog('close');
          } else {
            alert(response.Message);
          }
        });
      }
    });
  });

  $('#cancelReset').click(function () {
    $('#resetform').dialog('close');
  });
}

function query(isFirstLoad) {
  const queryString = window.location.search;
  if (queryString && isFirstLoad) {
    initDataFirstQuery(queryString);
  }
  var data = $('.card-container-base .form form').serialize();
  var selectedTeam = $('.select-teamIds .select-dropdown').val();
  var url = _global.root + 'annualleave/index?' + data;
  if (!isFirstLoad) {
    var orgUrl = _global.root + 'annualleave/index?' + data + '&Team=' + selectedTeam;
    window.history.replaceState(null, null, orgUrl);
  }

  $.ajax({
    url: url,
    type: 'POST',
    dataType: 'html',
    beforeSend: function () {
      if (!isFirstLoad) {
        $('.loading-process').fadeIn(300);
      }
    },
    success: function (data) {
      $('#div-content').html(data);

      $('.viewhistorybymonth').click(function () {
        var url = $(this).attr('id');
        var nickname = $(this).attr('data-name');

        $.post(url, function (data) {
          $('#dialoghistorycontent').html(data);

          $('#dialoghistorycontent').dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            title: nickname + "'s " + 'Annual Leave Balance History',
            show: {
              effect: 'fade',
              duration: 300
            },
            hide: {
              effect: 'fade',
              duration: 200
            }
          });
        });
      });

      $('.aba').each(function () {
        if ($(this).prop('checked')) {
          $(this).closest('tr').addClass('row-selected');
        }
      });

      $('.aba').on('click', function () {
        var tr = $(this).closest('tr');

        if (tr.hasClass('row-selected')) {
          tr.removeClass();
        } else {
          tr.addClass('row-selected');
        }
      });

      $('.checkuncheckall').on('click', function () {
        _checked = !_checked;
        $('.aba').prop('checked', _checked);
        $('.checkuncheckall').prop('checked', _checked);

        if (_checked) {
          $('tbody tr').not('.row-divider').addClass('row-selected');
        } else {
          $('tbody tr').not('.row-divider').removeClass('row-selected');
        }
      });

      floatThead('table#data');
    },
    error: function (jqXHR, textStatus, errorThrown) {
      alert(textStatus + ': ' + errorThrown);
    },
    complete: function () {
      if (!isFirstLoad) {
        $('.loading-process').fadeOut(300);
      }
    }
  });
}

function initDataFirstQuery(queryString) {
  var urlParams = new URLSearchParams(queryString);
  var yearParam = urlParams.get('Year');
  var employeeParam = urlParams.get('Employee');
  var employeeIdParam = urlParams.get('EmployeeId');
  var teamIdParam = urlParams.get('TeamId');
  var teamParam = urlParams.get('Team');
  var minBalanceParam = urlParams.get('MinAnnualBalance');
  var minAnnualBalanceLable = !minBalanceParam ? 'None' : minBalanceParam;

  $('#Employee').val(employeeParam);
  $('#Year').val(yearParam);
  $('#TeamId').val(teamIdParam);
  $('#EmployeeId').val(employeeIdParam);
  $('#MinAnnualBalance').val(minBalanceParam);
  $('.select--year .select-dropdown').val(yearParam);
  $('.select-teamIds .select-dropdown').val(teamParam);
  $('.select-min-balance .select-dropdown').val(minAnnualBalanceLable);
}

$(document).ready(function () {
  eventAdjustment();
  eventAddBalance();
  eventCloseBalance();

  initDepartmentChange();

  $('#submit').on('click', function () {
    query(false);
  });

  $('#exportExcel').on('click', function () {
    var data = $('.card-container-base .form form').serialize();
    var url = _global.root + 'annualleave/exportexcel?' + data;
    
    // Create a form and submit it to trigger file download
    var form = $('<form>', {
      'method': 'POST',
      'action': url
    });
    
    // Add all form data as hidden inputs
    var formData = $('.card-container-base .form form').serializeArray();
    $.each(formData, function(i, field) {
      form.append($('<input>', {
        'type': 'hidden',
        'name': field.name,
        'value': field.value
      }));
    });
    
    $('body').append(form);
    form.submit();
    form.remove();
  });

  query(true);
});
