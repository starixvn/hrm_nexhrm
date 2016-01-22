"use strict"

var memberList = [];
function createRow(nick, id) {
    var hour = $('#Hour').val();
    memberList.push({ EmployeeId: id, Hour: parseInt(hour) });
    var row = '<tr class="sortedIndex" id="row' + id + '">' +
                '<td>' + memberList.length + '</td>' +
                '<td>' + nick + '</td>' +
                '<td><input type="number" value="' + hour + '" step = "0.5" /> </td>' +
                '<td><a href="javascript:;" onclick="deleteRow(' + id + ');"><i class="mdi mdi-delete mdi-dark"></i></a></td>' +
                '</tr>';
    $('#memberList').append(row);
};

function deleteRow(id) {
    $('#row' + id).remove();
    $(memberList).each(function (index, element) {
        if (element.EmployeeId == id) {
            memberList.splice(index, 1);
        }
    });

    generatedIndex();
}

function generatedIndex() {
    var rowIndex = 1;
    $('.sortedIndex').each(function () {
        $(this).children('td').eq(0).html(rowIndex);
        rowIndex++;
    });
}

$().ready(function () {

    initDatePicker(".datepicker");

    $("#form").submit(function () {
        $("#RecordData").val(JSON.stringify(memberList));
    })
    
    initAutocomplete("#Employee", "#EmployeeId")    
});

