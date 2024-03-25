
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
        + '&status=' + $('#Status').val() + '&fromDate=' + $('#FromDate').val() + '&toDate=' + $('#ToDate').val();
    });
}

function pageSizeChanged() {
    query(false);
}

function showRowInfo() {
    const totalRow = document.getElementById('total-row').value;
    const paginationElement = document.querySelector('.pagination');
    const itemRangeStart = document.getElementById('item-range-start');
    const itemRangeEnd = document.getElementById('item-range-end');
    const totalItems = document.getElementById('total-items');
  
    if (totalRow != 0) {
        const pageNumber = document.getElementById('PageNumber').value;
        const pageSize = document.getElementById('PageSize').value;

        const startItem = (pageNumber - 1) * pageSize + 1;
        const endItem = pageNumber * pageSize;
    
        itemRangeStart.textContent = startItem;
        itemRangeEnd.textContent = Math.min(endItem, totalRow);
        totalItems.textContent = totalRow;
        paginationElement.style.display = '';
    } else {
        paginationElement.style.display = 'none';
    }
}

function disableBtnPreNext() {
    const totalRow = parseInt(document.getElementById('total-row').value, 10);
    const pageSize = parseInt(document.getElementById('PageSize').value, 10);
    const lastPageNumber = Math.ceil(totalRow / pageSize);
    const pageNumber = document.getElementById('PageNumber').value;

    const btnLeft = document.querySelector('.mdi-chevron-left');
    const btnDoubleLeft = document.querySelector('.mdi-chevron-double-left');
    const btnRight = document.querySelector('.mdi-chevron-right');
    const btnDoubleRight = document.querySelector('.mdi-chevron-double-right');

    btnLeft.classList.toggle('disabled', pageNumber == 1);
    btnDoubleLeft.classList.toggle('disabled', pageNumber == 1);
    btnRight.classList.toggle('disabled', pageNumber == lastPageNumber);
    btnDoubleRight.classList.toggle('disabled', pageNumber == lastPageNumber);
}

function clickBtnPrevNext() {
    const buttonIds = ['btnDoublePrev', 'btnPrev', 'btnDoubleNext', 'btnNext'];
    const pageNumberInput = document.getElementById('PageNumber');
    const totalRowInput = document.getElementById('total-row');
    const pageSizeInput = document.getElementById('PageSize');
  
    const lastPageNumber = calculateLastPageNumber();
  
    buttonIds.forEach(function(buttonId) {
      const button = document.getElementById(buttonId);
      button.addEventListener('click', function(evt) {
        evt.preventDefault();
        handleButtonClick(buttonId);
        query(false);
      });
    });
  
    function calculateLastPageNumber() {
      const totalRow = parseInt(totalRowInput.value, 10);
      const pageSize = parseInt(pageSizeInput.value, 10);
      return Math.ceil(totalRow / pageSize);
    }
  
    function handleButtonClick(buttonId) {
      switch (buttonId) {
        case 'btnDoublePrev':
          pageNumberInput.value = 1;
          break;
        case 'btnPrev':
          pageNumberInput.value = parseInt(pageNumberInput.value) - 1;
          break;
        case 'btnDoubleNext':
          pageNumberInput.value = lastPageNumber;
          break;
        case 'btnNext':
          pageNumberInput.value = parseInt(pageNumberInput.value) + 1;
          break;
        default:
          break;
      }
    }
  }


$(document).ready(function () {
    $('#FullName').keyup(function (event) {
        if (event.keyCode === 13) {
            query(false);
        }
    });

    $('#PageNumber').keyup(function (event) {
        if (event.keyCode === 13) {
            const totalRow = parseInt(document.getElementById('total-row').value, 10);
            const pageSize = parseInt(document.getElementById('PageSize').value, 10);
            const lastPageNumber = Math.ceil(totalRow / pageSize);
            const pageNumber = document.getElementById('PageNumber').value;
            if(lastPageNumber >= pageNumber){
                query(false);
            }
        }
    });

    $('#submit').on('click', function () {
        query(false);
    });

    query(true);
    eventExportExcel();
    pageSizeChanged();
    clickBtnPrevNext();
});