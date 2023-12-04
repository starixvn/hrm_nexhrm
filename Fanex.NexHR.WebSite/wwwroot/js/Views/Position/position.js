
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'position/index',
        dataType: 'html',
        method: "POST",
        data: {
            FullName: $('#keyword').val()
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
    $('#Name').keyup(function (event) {
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

var position = (function ($) {
    var $dialog = $('#dialog');
    var $createForm;
    var $editForm;

    function initCreateDialog() {
        $createForm = $('#create-form');
        $.validator.unobtrusive.parse($createForm);
        $dialog.dialog({
            modal: true,
            width: 'auto',
            title: 'Create New Position',
            buttons: [
                {
                    text: 'Save',
                    class: 'btn-submit waves-effect waves-light btn-dialog',
                    click: function () {
                        $('#create-form').submit();
                    }
                },
                {
                    text: 'Cancel',
                    class: 'btn-cancel waves-effect waves-light m-r--m btn-dialog',
                    click: function () {
                        $(this).dialog('close');
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

    function initEditDialog() {
        $editForm = $('#edit-form');
        $.validator.unobtrusive.parse($editForm);

        $dialog.dialog({
            modal: true,
            width: 'auto',
            title: 'Edit Position',
            buttons: [
                {
                    text: 'Save',
                    class: 'btn-submit waves-effect waves-light btn-dialog',
                    click: function () {
                        $editForm.submit();
                    }
                },
                {
                    text: 'Cancel',
                    class: 'btn-cancel waves-effect waves-light m-r--m btn-dialog',
                    click: function () {
                        $(this).dialog('close');
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

    function onCreateEditSuccess(response) {
        if (response.IsSuccessful) {
            $dialog.dialog('close');
            query(false);
        } else {
            showErrorMessage(response.Message);
        }
    }

    return {
        init: init,
        onLoadingCreateFormSuccess: initCreateDialog,
        onLoadingEditFormSuccess: initEditDialog,
        onCreateSuccess: onCreateEditSuccess,
        onEditSuccess: onCreateEditSuccess
    }

}(jQuery));

$(document).ready(function () {
    position.init();
});