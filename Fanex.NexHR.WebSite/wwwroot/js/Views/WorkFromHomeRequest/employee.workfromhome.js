'use strict';
/*global _global, floatThead*/
/* exported deleteRequest */

function query(isFirstLoad) {
    

    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    const data = $('.card-container-base form').serialize();
    var teamParam = $(".team .select-dropdown").val();
    var statusParam = $(".status .select-dropdown").val();

    var url = _global.root + 'WorkFromHomeRequest/GetEmployeeWorkFromHomeGrid?' + data;
    if (!isFirstLoad) {
        var orgUrl = _global.root + 'WorkFromHomeRequest/HRView?' + data + "&TeamParam=" + teamParam + "&StatusParam=" + statusParam;
        window.history.replaceState(null, null, orgUrl);
    }

    $.ajax({
        url: url,
        dataType: 'html',
        method: "GET",
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#list-workfromhome').html(data);
            if ($('table#data').length > 0) {
                bindMenuPending();
                bindMenuApproved();
                $('table#data').addClass('table-success'); // class to style mobile

                $('#checkuncheckall').on('click', function () {
                    var isCheckAll = $('#checkuncheckall').prop('checked');

                    $('.chk-request').prop('checked', isCheckAll);
                });
            }
           
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
    var TeamId = urlParams.get('TeamId');
    var Employee = urlParams.get('Employee');
    var EmployeeId = urlParams.get('EmployeeId');
    var statusId = urlParams.get('statusId');
    var FromDate = urlParams.get('FromDate');
    var ToDate = urlParams.get('ToDate');
    var teamParam = urlParams.get('TeamParam');
    var statusParam = urlParams.get('StatusParam');

    $("#TeamId").val(TeamId);
    $("#Employee").val(Employee);
    $("#EmployeeId").val(EmployeeId);
    $("#statusId").val(statusId);
    $("#FromDate").val(FromDate);
    $("#ToDate").val(ToDate);

    $(".team .select-dropdown").val(teamParam);
    $(".status .select-dropdown").val(statusParam);
}

function initApproveMultiEvent() {
    $('#btn-approve').on('click', function () {
        var requestIds = [];
        var command = "Approve";
        $('.chk-request').each(function (index, item) {
            var checked = $(item).prop('checked');
            if (checked) {
                requestIds.push($(item).attr('data-id'));
            }
        });

        if (requestIds.length > 0) {
            confirm('Are you sure you want to approve checked request?', function () {
                approveMultiRequest(command, requestIds);
            });
        } else {
            alert('Please choose at least one request to continue!');
        }
    });
}

function initRejectMultiEvent() {
    $('#btn-reject').on('click', function () {
        var requestIds = [];
        var command = "Reject";
        $('.chk-request').each(function (index, item) {
            var checked = $(item).prop('checked');
            if (checked) {
                requestIds.push($(item).attr('data-id'));
            }
        });

        if (requestIds.length > 0) {
            confirm('Are you sure you want to reject checked request?', function () {
                approveMultiRequest(command, requestIds);
            });


        } else {
            alert('Please choose at least one request to continue!');
        }
    });
}

function approveMultiRequest(command, requestIds) {
    $.post(_global.root + 'WorkFromHomeRequest/ProcessRequestMultiple',
        { command: command, requestIds: requestIds },
        function (response) {
            if (response.IsSuccessful) {
                query(false);
            } else {
                alert(response.Message);
                return;
            }
        }
    );
}

function initCreateUpdatePage() {
    var successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
            function () {
                document.location = _global.root + 'WorkFromHomeRequest/HRView';
            }
        );
    }
}

function deleteRequest(id) {
    confirm('Are you sure you want to remove this employee work from home?', function () {
        $.post(_global.root + 'WorkFromHomeRequest/Delete', { id }, function (response) {
            if (response.IsSuccessful) {
                query(false);
            }
            else {
                alert(response.Message);
            }
        });
    });
}

function bindMenuPending() {
    $('table#data tbody td a.Pending').on('mouseup', function () {
        $.contextMenu({
            selector: '.Pending',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key) {
                        switch (key) {
                            case 'Approve':
                                confirm('Are you sure you want to Approve?', function () {
                                    processRequest(key, e.currentTarget);
                                });
                                break;
                            case 'Reject':
                                confirm('Are you sure you want to Reject?', function () {
                                    processRequest(key, e.currentTarget);
                                });
                                break;
                        }
                    },
                    items: {
                        'Approve': { name: 'Approve', icon: 'approve' },
                        'Reject': { name: 'Reject', icon: 'reject' }
                    }
                };
            }
        });
    });
}

function processRequest(command, eventElement) {
    var url = command === "Disapprove" ? 'WorkFromHomeRequest/ProcessDisapproveRequest' : 'WorkFromHomeRequest/ProcessRequest';
    $.post(_global.root + url,
        { command: command, id: $(eventElement).attr('workFromHomeRequestId') },
        function (response) {
            if (response.IsSuccessful) {
                query(false);
            } else {
                alert(response.Message);
                return;
            }
        }
    );
}


function bindMenuApproved() {
    $('table#data tbody td a.Approved').on('mouseup', function () {
        $.contextMenu({
            selector: '.Approved',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key) {
                        confirm('Are you sure you want to Disapprove?', function () {
                            processRequest(key, e.currentTarget);
                        });
                    },
                    items: {
                        'Disapprove': { name: 'Disapprove', icon: 'reject' }
                    }
                };
            }
        });
    });
}

$(document).ready(function () {
    if ($('.form-create-update').length > 0) {
        initCreateUpdatePage();
    } else {
        query(true);

        $('#submit').on('click', function () {
            query(false);
        });
    }

    initApproveMultiEvent();
    initRejectMultiEvent();
});