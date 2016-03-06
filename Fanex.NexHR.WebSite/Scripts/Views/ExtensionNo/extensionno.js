'use strict';
/*global _global:false, _messages:false*/

$(document).ready(function () {
    $(".deleteExt").on("click", function () {
        var id = $(this).closest('tr').attr('id');
        confirm('Are you sure you want to delete?', function () {
            $.post(_global.root + 'extensionno/DeleteExtensionNumber',
            { extId: id },
            function (response) {
                if (response.IsSuccessful) {
                    document.location = _global.root + 'extensionno/index';
                }
                else {
                    alert(response.Message);
                }
            }).error(function () {
                alert(_messages.errorTryAgain);
            });
        });
    });

    $(".deleteExternal").on("click", function () {
        var id = $(this).closest('tr').attr('id');
        confirm('Are you sure you want to delete?', function () {
            $.post(_global.root + 'extensionno/DeleteExternalGroup',
            { extId: id },
            function (response) {
                if (response.IsSuccessful) {
                    document.location = _global.root + 'extensionno/ManageExternalGroup';
                }
                else {
                    alert(response.Message);
                }
            }).error(function () {
                alert(_messages.errorTryAgain);
            });
        });
    });

    floatThead('table#data');
});

$(function () {
    var name = $("#name"),
            allFields = $([]).add(name),
            tips = $(".validateTips");

    function updateTips(t) {
        tips.text(t);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            o.addClass("ui-state-error");
            updateTips("Length of " + n + " must be between " +
                    min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }

    $("#dialog-form").dialog({
        autoOpen: false,
        height: 180,
        width: 400,
        modal: true,
        buttons: [{
            text: "Cancel",
            class: "btn-cancel waves-effect waves-light m-r--m",
            click: function () {
                $(this).dialog("close");
            }
        },
        {
            text: "Create",
            class: "btn-ok waves-effect waves-light",
            click: function () {
                var bValid = true;
                allFields.removeClass("ui-state-error");
                bValid = bValid && checkLength(name, "job title", 3, 100);

                if (bValid) {
                    $.post(_global.root + 'extensionno/AddNewExternalGroup',
                        { groupName: name.val() },
                        function (response) {
                            if (response.IsSuccessful) {
                                document.location = _global.root + 'extensionno/ManageExternalGroup';
                            }
                            else {
                                alert(response.Message);
                            }
                        }).error(function () {
                            alert(_messages.errorTryAgain);
                        });
                }
            }
        }],
        close: function () {
            updateTips('');
            allFields.val("").removeClass("ui-state-error");
        }
    });
    $("#btnNewExternalGroup").click(function () {
        $("#dialog-form").dialog("open");
    });
});

//function update GroupName
$(function () {
    var name = $("#name_edit"),
            allFields = $([]).add(name),
            tips = $(".validateTips");

    function updateTips(t) {
        tips.text(t);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            o.addClass("ui-state-error");
            updateTips("Length of " + n + " must be between " +
                    min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }

    $(".editExternal").on("click", function () {
        var id = $(this).closest('tr').attr('id');
        var currentName = $(this).closest('tr').find('.groupName').text().trim();
        $("#name_edit").val(currentName);

        $("#dialog-form_edit").dialog({
            autoOpen: false,
            height: 180,
            width: 450,
            modal: true,
            class: "btn-ok waves-effect waves-light",
            buttons: [{
                text: "Cancel",
                class: "btn-cancel waves-effect waves-light m-r--m",
                click: function () {
                    $(this).dialog("close");
                }
            },
            {
                text: "Update",
                class: "btn-ok waves-effect waves-light",
                click: function () {
                    var bValid = true;
                    allFields.removeClass("ui-state-error");
                    bValid = bValid && checkLength(name, "job title", 3, 100);

                    if (bValid) {
                        $.post(_global.root + 'extensionno/EditExternalGroup',
                        { groupName: name.val(), extId: id },
                        function (response) {
                            if (response.IsSuccessful) {
                                document.location = _global.root + 'extensionno/ManageExternalGroup';
                            }
                            else {
                                alert(response.Message);
                            }
                        }).error(function () {
                            alert(_messages.errorTryAgain);
                        });
                    }
                }
            }],
            close: function () {
                updateTips('');
                allFields.val("").removeClass("ui-state-error");
            }
        });

        $("#dialog-form_edit").dialog("open");
    });
});