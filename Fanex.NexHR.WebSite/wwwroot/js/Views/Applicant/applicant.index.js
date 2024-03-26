
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'applicants/index',
        dataType: 'html',
        method: "POST",
        data: {
            FullName: $('#FullName').val(),
            PositionId: $('#PositionId').val(),
            Status: $('#Status').val(),
            FromDate: $('#FromDate').val(),
            ToDate: $('#ToDate').val(),
            PageNumber: $('#PageNumber').val(),
            PageSize: $('#PageSize').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#applicants-detail').html(data);
            floatThead('table#data');
            showRowInfo();
            disableBtnPreNext();
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
}

function deleteApplicant(id, fileName) {
    confirm('Are you sure you want to delete this applicant?', function () {
        $.post(_global.root + 'applicants/Delete', { id: id, fileName: fileName},
            function (response) {
                if (response.IsSuccessful) {
                    $('#row' + id).remove();
                    autoIndex();
                }
                else {
                    alert(response.Message);
                }
            });
    });
}

function eventExportExcel() {
    $('#exportExcel').on('click', function (evt) {
        evt.preventDefault();

        window.location.href = _global.root + 'Export/ApplicantsReport?fullName=' + $('#FullName').val() + '&positionId=' + $('#PositionId').val()
        + '&status=' + $('#Status').val() + '&fromDate=' + $('#FromDate').val() + '&toDate=' + $('#ToDate').val()
        + '&pageNumber=' + $('#PageNumber').val() + '&pageSize=' + $('#PageSize').val();
    });
}

function pageSizeChanged() {
    const totalRow = parseInt(document.getElementById('total-row')?.value, 10) || 0;
    const pageNumber = document.getElementById('PageNumber');
    const pageSize = document.getElementById('PageSize');
  
    if ((pageNumber.value - 1) * pageSize.value + 1 > totalRow) {
      pageNumber.value = 1;
    }
    query(false);
  }

function showRowInfo() {
    const totalRow = parseInt($('#total-row').val(), 10);
    const paginationElement = $('.pagination');
    const itemRangeStart = $('#item-range-start');
    const itemRangeEnd = $('#item-range-end');
    const totalItems = $('#total-items');

    if (totalRow !== 0) {
      const pageNumber = parseInt($('#PageNumber').val(), 10);
      const pageSize = parseInt($('#PageSize').val(), 10);
  
      const startItem = (pageNumber - 1) * pageSize + 1;
      const endItem = Math.min(pageNumber * pageSize, totalRow);

      itemRangeStart.text(startItem);
      itemRangeEnd.text(endItem);
      totalItems.text(totalRow);
      paginationElement.show();
    } else {
      paginationElement.hide();
    }
  }

function disableBtnPreNext() {
    const totalRow = parseInt($('#total-row').val(), 10);
    const pageSize = parseInt($('#PageSize').val(), 10);
    const lastPageNumber = Math.ceil(totalRow / pageSize);
    const pageNumber = parseInt($('#PageNumber').val(), 10);
  
    $('.mdi-chevron-left, .mdi-chevron-double-left').toggleClass('disabled', pageNumber === 1);
    $('.mdi-chevron-right, .mdi-chevron-double-right').toggleClass('disabled', pageNumber === lastPageNumber);
  }

function clickBtnPrevNext() {
    const buttonIds = ['btnNext', 'btnPrev', 'btnDoubleNext', 'btnDoublePrev'];
    buttonIds.forEach(buttonId => {
      const button = document.getElementById(buttonId);
  
      button.addEventListener('click', function(event) {
        if (!this.classList.contains('disabled')) {
            handleButtonClick(getUpdatedPageNumber(buttonId));
        }
      });
    });

    function getUpdatedPageNumber(buttonId) {
        const currentPageNumber = getPageNumber();
        switch (buttonId) {
            case 'btnNext':
                return currentPageNumber + 1;
            case 'btnPrev':
                return currentPageNumber - 1;
            case 'btnDoubleNext':
                return getLastPageNumber();
            case 'btnDoublePrev':
                return 1;
            default:
                return currentPageNumber;
            }
        }

    function handleButtonClick(pageNumber) {
      setPageNumber(pageNumber);
      query(false);
    }
  
    function getPageNumber() {
      return parseInt(document.getElementById('PageNumber').value);
    }
  
    function setPageNumber(value) {
      document.getElementById('PageNumber').value = value;
    }
  
    function getLastPageNumber() {
      const totalRowElement = document.getElementById('total-row');
      const totalRow = totalRowElement != null ? parseInt(totalRowElement.value, 10) : 0;
      const pageSize = parseInt(document.getElementById('PageSize').value, 10);
      return Math.ceil(totalRow / pageSize);
    }
}

function enterPageNumber() {
    $('#PageNumber').keyup(event => {
      const ENTER_KEYCODE = 13;
      const totalRow = parseInt($('#total-row').val(), 10);
      const pageSize = parseInt($('#PageSize').val(), 10);
      const lastPageNumber = Math.ceil(totalRow / pageSize);
      const pageNumber = parseInt($(event.currentTarget).val(), 10);
  
      if (event.keyCode === ENTER_KEYCODE && pageNumber > 0 && pageNumber <= lastPageNumber) {
        query(false);
      }
    });
  }

$(document).ready(function () {
    $('#FullName').keyup(function (event) {
        if (event.keyCode === 13) {
            query(false);
        }
    });

    $('#submit').on('click', function () {
        query(false);
    });

    query(true);
    eventExportExcel();
    clickBtnPrevNext();
    enterPageNumber();
});