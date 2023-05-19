'use strict';
/*global _global:false */

$(document).ready(function () {
    var successed = $("#successed").val();
    if (successed === "true") {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + "extensionno/index";
                }
            );
    }

    var bindingDataToDropdown = function () {
        $.post(_global.root + 'extensionNo/GetGroupExtension', { isInternal: $("#IsInternal").is(":checked"), groupId: null },
        function (respone) {
            if (respone[0] != null) {
                $("#TeamId").empty();
                var options = "";
                for (var i = 0; i < respone.length; i++) {
                    options += "<option value='" + respone[i].Value + "'>" + respone[i].Text + "</option>";
                }
                $("#TeamId").append(options);
                $("#TeamId").material_select('update');     // jshint ignore:line
            }
        });
    };

    var checkForChecked = function () {
        if ($("#IsInternal").is(":checked")) {

            if ($("#EmpId").val() > 0) {
                var FullName = $("#NickName").val() + ' (' + $("#FullName").val() + ')';
                $("#Employee").val(FullName);
            }

            $("#FullName, #NickName").attr('readonly', true);
            $("#EmployeeField").show();
        }
        else {
            $("#FullName, #NickName").attr('readonly', false);
            $("#EmployeeField").hide();
        }
    };

    var IsAddForm = $("#IsAddForm").val();
    if (IsAddForm === "True") {
        bindingDataToDropdown();
    }

    checkForChecked();

    $("#IsInternal").change(function () {
        bindingDataToDropdown();
        checkForChecked();
    });

    $("#Employee").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _global.root + 'employee/GetEmployeeSuggestion',
                data: {
                    query: request.term,
                    isSpecial: false
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')',
                            value: item.Id,
                            fullname: item.FullName,
                            nickname: item.NickName
                        };
                    }));
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            var nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);

            $('#FullName').val(ui.item.fullname);
            $('#NickName').val(ui.item.nickname);
        },
        focus: function (event, ui) {
            var nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item !== null ? ui.item.value : "");
            $('#FullName').val(ui.item.fullname);
            $('#NickName').val(ui.item.nickname);
            event.preventDefault();
        },
        change: function (event, ui) {
            var nickName = ui.item !== null ? ui.item.label : "";
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item !== null ? ui.item.value : "");
            $('#FullName').val(ui.item.fullname);
            $('#NickName').val(ui.item.nickname);
        }
        //close: function (event, ui) {
        //    var nickName = ui.item != null ? ui.item.label : "";
        //    $('#Employee').val(nickName);
        //}
    });    
});