
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'position/index',
        dataType: 'html',
        method: "POST",
        data: {
            keyword: $('#Keyword').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#position-detail').html(data);

            floatThead('table#data');
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

function deletePosition(id) {
    confirm('Are you sure you want to delete this position?', function () {
        $.post(_global.root + 'position/Delete', { id: id },
            function (response) {
                if (response.IsSuccessful) {
                    $('#row_' + id).remove();
                    autoIndex();
                }
                else {
                    alert(response.Message);
                }
            });
    });
}

function init() {
    $('#Keyword').keyup(function (event) {
        if (event.keyCode === 13) {
            query(false);
        }
    });

    $('#submit').on('click', function () {
        query(false);
    });

    query(true);
}

function showErrorMessage(message) {
    $('<div>' + '<p>' + message + '</p>').dialog({
        title: 'NexHRM',
        modal: true,
        resizable: false,
        buttons: [
            {
                text: 'OK',
                class: 'btn-ok waves-effect waves-light',
                click: function () {
                    $(this).dialog('close');
                    return true;
                }
            }
        ],
        show: 'fade',
        hide: 'fade',
        minHeight: 50,
        dialogClass: 'modal-shadow'
    });
}

function initDialog(title, form) {
    // $.validator.addMethod("validateSpace", function (value, element, field) {
    //     return value.trim().length > 0; 
    // }, "The {0} field is required.");
   
    $('#dialog').dialog({
        modal: true,
        width: 'auto',
        title: title,
        buttons: [
            {
                text: 'Cancel',
                class: 'btn-cancel waves-effect waves-light m-r--m btn-dialog',
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: 'Save',
                class: 'btn-submit waves-effect waves-light btn-dialog',
                click: function () {
                    var formData = new FormData(form[0]);
                    $.ajax({
                        url: form.attr('action'),
                        type: form.attr('method'),
                        data: formData,
                        processData: false,
                        contentType: false,
                        success: function (data) {
                            onSuccess(data);
                        },
                        error: function () {
                            alert(_messages.errorTryAgain);
                        }
                    });
                }
            }
        ],
        show: {
            effect: 'fade',
            duration: 300
        },
        hide: {
            effect: 'fade',
            duration: 200
        }
    });
}

function onSuccess(response) {
    if (response.IsSuccessful) {
        $('#dialog').dialog('close');
        query(false);
    } else {
        showErrorMessage(response.Message);
    }
}

$(document).ready(function () {
    init();

    $('#newPosition').click(function(e) {
        var title="Create New Position";
        var form = $('#create-form');
        e.preventDefault();
        $.ajax({
            url: _global.root + 'position/create',
            type: 'GET',
            data: initDialog(title, form),
            success: function (data) {
                $('#dialog').html(data);
            },
            error: function () {
                alert(_messages.errorTryAgain);
            },
        });
    });

    $('#editPosition').click(function(e) {
        var title="Edit Position";
        var form = $('#edit-form');
        var Id = parseInt($('#positionId').val());
        e.preventDefault();
        $.ajax({
            url: _global.root + 'position/edit' + Id,
            type: 'GET',
            data: initDialog(title, form),
            success: function (data) {
                $('#dialog').html(data);
            },
            error: function () {
                alert(_messages.errorTryAgain);
            },
        });
    });
});
