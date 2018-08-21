var skypeAccount = (function ($) {
    var domElement = {
        filterForm: "#skype-account-filter",
        table: "#list-account",
        editForm: "#skype-account-edit",
        dialog: "#skype-account-dialog",
        editIcon: ".account-edit"
    }

    function getSkypeAccountList() {
        var $form = $(domElement.filterForm);
        $.ajax({
            method: $form.attr("method"),
            url: $form.attr("action"),
            data: $form.serialize(),
            dataType: "html",
            beforeSend: function () {
                $('.loading-process').fadeIn(300);
            },
            success: function (response) {
                $(domElement.table).html(response);
                registerEditEvent();
            },
            error: function () {
                alert(_messages.errorTryAgain);
            },
            complete: function () {
                if ($('.loading-process').length) {
                    $('.loading-process').fadeOut(300);
                }
            }
        })
    }

    function initDialog() {
        $(domElement.dialog).dialog({
            modal: true,
            width: 'auto',
            title: '',
            buttons: [
                {
                    text: 'Cancel',
                    class: 'btn-cancel waves-effect waves-light m-r--m',
                    click: function () {
                        $(this).dialog('close');
                    }
                },
                {
                    text: 'Submit',
                    class: 'btn-submit waves-effect waves-light',
                    click: function () {
                        editAccount();
                    }
                }
            ],
            show: {
                effect: 'blin',
                duration: 300
            },
            hide: {
                effect: 'blin',
                duration: 200
            }
        });
    }

    function editAccount() {
        var $form = $(domElement.editForm);
        console.log($form.serialize());
        $.ajax({
            url: _global.root + 'SkypeAccount/EditAccount',
            method: "POST",
            data: $form.serialize(),
            success: function (res) {
                if (res.isSuccessful) {
                    $(domElement.dialog).dialog("close");
                    getSkypeAccountList();
                } else {
                    if (res.errors) {
                        showErrorMessage(domElement.editForm, res.errors);
                    }

                    if (res.message) {
                        alert(res.message);
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        })
    }

    function showErrorMessage(form, errors) {
        errors.forEach(function (error) {
            $(form).find("input[name='" + error.Key + "']").addClass("input-validation-error");
            $(form).find('[data-valmsg-for="' + error.Key + '"]:last').text(error.ErrorMessage);
        });
    }

    function registerEditEvent() {
        $(domElement.editIcon).on("click", function () {
            var fullName = $(this).attr("data-fullName");
            var nickName = $(this).attr("data-nickName");
            $.ajax({
                method: 'GET',
                url: _global.root + 'SkypeAccount/EditAccount',
                data: {
                    fullName,
                    nickName
                },
                dataType: 'html',
                success: function (data) {
                    $(domElement.dialog).html(data);
                    initDialog();
                    initSelectMaterial();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        });
    }

    function initSelectMaterial() {
        var select = $(domElement.editForm).find("select");
        $(select).material_select();
    }

    function registerEventSubmit() {
        $(domElement.filterForm).on("submit", function (e) {
            e.preventDefault();
            getSkypeAccountList();
        })
    }

    function init() {
        registerEventSubmit();
        getSkypeAccountList();
    }

    return {
        initModule: init
    }
})(jQuery);

$(document).ready(function () {
    skypeAccount.initModule();
});