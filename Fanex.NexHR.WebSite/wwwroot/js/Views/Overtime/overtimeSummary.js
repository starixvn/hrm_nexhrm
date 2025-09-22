'use strict';
/*global _global, _messages, initDatePicker, floatThead*/

var overtimeSummary = (function ($) {
  var domElement = {
    //addLeaveConversion: '.addLeaveConversion',
    //insertformLeaveConversion: '#insertformLeaveConversion',
    //formLeaveConversion: '#insertformLeaveConversion form',
    createConversion: '.createConversion',
    dialogConversion: '#createConversion',
    formConversion: '#createConversion form'
  };

  function registerCreateConversionEvent() {
    $(domElement.createConversion).on('click', function (e) {
      e.preventDefault();
      var employeeId = $(this).attr('data-id');
      $(domElement.dialogConversion).dialog({
        modal: true,
        width: 'auto',
        height: 'auto',
        open: function () {
          $('#Deduct').val('0.5');
          $('#Description').val('');
          $('#EmpId').val(employeeId);
          initDatePicker();
        },
        show: {
          effect: 'blind',
          duration: 200
        },
        hide: {
          effect: 'blind',
          duration: 100
        }
      });
    });
  }

  function initModalCreateConversionEvent() {
    $(domElement.formConversion).on('submit', function (e) {
      e.preventDefault();
      $.ajax({
        url: _global.root + 'Overtime/CreateOvertimeConversion',
        method: 'POST',
        data: {
          EmpId: $('#EmpId').val(),
          Deduct: $('#Deduct').val(),
          DeductDate: $('#DeductDate').val(),
          Description: $('#Description').val()
        },
        success: function (res) {
          if (res.IsSuccessful) {
            $(domElement.dialogConversion).dialog('close');
            query(false);
          } else {
            notifyError(res.Message);
          }
        },
        error: function (jqXHR, textStatus, errorThrown) {
          notifyError(textStatus + ': ' + errorThrown);
        }
      });
    });

    $('#cancel-insert').on('click', function () {
      $(domElement.dialogConversion).dialog('close');
    });
  }

  function query(isFirstLoad) {
    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
      initDataFirstQuery(queryString);
    }
    var data = 'empId=' + $('#EmployeeId').val() + '&teamid=' + $('#TeamId').val() + '&year=' + $('#Year').val() + '&departmentId=' + $('#DepartmentId').val();
    var teamName = $('.team .select-dropdown').val();
    var empName = $('#Employee').val();
    var departmentName = $('.department .select-dropdown').val();

    var url = _global.root + 'Overtime/OvertimeSummary?' + data;
    if (!isFirstLoad) {
      var orgUrl = _global.root + 'overtime/overtimesummary?' + data + '&teamName=' + teamName + '&empName=' + empName + '&departmentName=' + departmentName;
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
      success: function (response) {
        var curYear = $('#Year').val();
        $('#oldYear').html(curYear - 1);
        $('#curYear').html(curYear);
        $('#listOvertimeSummary').html(response);
        //registerAddLeaveDeductEvent();
        registerCreateConversionEvent();
        floatThead('table#data');
      },

      error: function () {
        notifyError(_messages.errorTryAgain);
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
    var empId = urlParams.get('empId');
    var empName = urlParams.get('empName');
    var teamid = urlParams.get('teamid');
    var year = urlParams.get('year');
    var teamName = urlParams.get('teamName');
    var departmentId = urlParams.get('departmentId');
    var departmentName = urlParams.get('departmentName');

    $('#EmployeeId').val(empId);
    $('#Employee').val(empName);
    $('#TeamId').val(teamid);
    $('#Year').val(year);
    $('#DepartmentId').val(departmentId);

    $('.team .select-dropdown').val(teamName);
    $('.year .select-dropdown').val(year);
    $('.department .select-dropdown').val(departmentName);
  }

  //function registerAddLeaveDeductEvent{
  //    $(domElement.addLeaveConversion).on("click", function () {
  //        var employeeId = $(this).attr('data-id');
  //        $('#EmpId').val(employeeId);
  //        $(domElement.insertformLeaveConversion).dialog('open');
  //    });

  //    $(domElement.insertformLeaveConversion).dialog({
  //        modal: true,
  //        autoOpen: false,
  //        width: 'auto',
  //        height: 'auto',
  //        open: function () {
  //            $('#TotalDays').val('0.5');
  //            initDatePicker();
  //        },
  //        show: {
  //            effect: "blind",
  //            duration: 200
  //        },
  //        hide: {
  //            effect: "blind",
  //            duration: 100
  //        }
  //    });
  //}

  //function initAddLeaveConversion() {
  //    $(domElement.formLeaveConversion).on("submit", function (e) {
  //        e.preventDefault();
  //        $.post(_global.root + 'AnnualLeave/AddLeaveConversion',
  //            {
  //                empId: $('#EmpId').val(),
  //                totalDays: $('#TotalDays').val(),
  //                ondate: $('#OnDate').val(),
  //                description: $('#Description').val()
  //            }, function (response) {
  //                if (response.IsSuccessful) {
  //                    $('#insertform').dialog('close');
  //                    document.location = _global.root + 'overtime/OvertimeSummary';
  //                } else {
  //                    alert(response.Message);
  //                }
  //            });
  //    });

  //    $('#cancel').on("click", function () {
  //        $(domElement.insertformLeaveConversion).dialog('close');
  //    });
    //}

function eventExportExcel() {

    $('#exportExcel').on('click', function (evt) {
        evt.preventDefault();
        const data = 'empId=' + $('#EmployeeId').val() + '&teamid=' + $('#TeamId').val() + '&year=' + $('#Year').val() + '&departmentId=' + $('#DepartmentId').val();

        window.location.href =
            _global.root +
            'Export/OvertimeSummary?query=' + data;
    });
}

  function registerSubmitEvent() {
    $('#submit').on('click', function () {
      query(false);
    });
  }

  function init() {
    registerSubmitEvent();
    //initAddLeaveConversion();
    initModalCreateConversionEvent();
      initDepartmentChange();
      eventExportExcel();
  }

  return {
    initModule: init,
    query: query
  };
})(jQuery);

$(document).ready(function () {
  overtimeSummary.query(true);
  overtimeSummary.initModule();
});
