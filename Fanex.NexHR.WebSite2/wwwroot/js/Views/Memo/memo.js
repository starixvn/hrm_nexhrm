"use strict";
/*global _global*/
/*exported deleteMemoFile, index*/

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function uploadFailed() {
}

function deleteComplete(evt) {
    if (evt.target.status === 200) {
        $('#attach').hide();
        $("#AttachFile").val("");
        $("#Contents").val("");
    }
}

function deleteMemoFile(fileName) {
    var fd = new FormData();
    fd.append('fileName', fileName);
    var xhr = new XMLHttpRequest();
    xhr.addEventListener("load", deleteComplete, false);
    xhr.addEventListener("error", uploadFailed, false);
    xhr.open("POST", _global.root + "file/Delete");
    xhr.send(fd);
}