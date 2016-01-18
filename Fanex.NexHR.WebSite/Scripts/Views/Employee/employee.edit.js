'use strict';
/*global _global:false, _messages:false, actingButton:false, initDatePicker:false,
google:false, reloadButtonText:false, formatDate:false*/
/* exported query */

var directionsDisplay;

function displayLatLong(marker, latitudeTextBox, longitudeTextBox) {
    var directionsService = new google.maps.DirectionsService();
    var curLatLng = marker.getPosition();
    var smsLatlng = new google.maps.LatLng(10.8553024280436, 106.63105838755791);

    latitudeTextBox.val(curLatLng.lat());
    longitudeTextBox.val(curLatLng.lng());
    var request = {
        origin: curLatLng,
        destination: smsLatlng,
        travelMode: google.maps.DirectionsTravelMode.DRIVING
    };
    directionsService.route(request, function (response, status) {
        if (status === google.maps.DirectionsStatus.OK) {
            var distance = response.routes[0].legs[0].distance.text;
            $('#DistanceToCompany').val(distance);
            directionsDisplay.setDirections(response);
        }
    });
}

function InitializeGoogleMap(map, latlng) {
    var myOptions = {
        zoom: 16,
        center: latlng,
        scrollwheel: true
    };
    map = new google.maps.Map(document.getElementById('map-canvas'), myOptions);
    return map;
}

function initialize() {
    var marker, map, latlng,
        checkInitLatlng = false,
        latitudeTextBox = $('#GeoMapLatitude'),
        longitudeTextBox = $('#GeoMapLongtitude');

    var myOption = { suppressMarkers: true, polylineOptions: { strokeColor: 'red'} };
    directionsDisplay = new google.maps.DirectionsRenderer(myOption);

    if (latitudeTextBox.val() === '' && longitudeTextBox.val() === '') {
        latlng = new google.maps.LatLng(10.8550869, 106.63152500000001);
        checkInitLatlng = true;
    } else {
        latlng = new google.maps.LatLng(latitudeTextBox.val(), longitudeTextBox.val());
    }

    map = new InitializeGoogleMap(map, latlng);

    if (checkInitLatlng) {
        marker = new google.maps.Marker({
            draggable: false,
            map: map,
            position: latlng
        });
    } else {
        marker = new google.maps.Marker({
            draggable: true,
            map: map,
            position: latlng
        });
        displayLatLong(marker, latitudeTextBox, longitudeTextBox);
        google.maps.event.addListener(marker, 'dragend', function () {
            displayLatLong(marker, latitudeTextBox, longitudeTextBox);
        });
    }
    directionsDisplay.setMap(map);
}

function dynamicSearchMap() {
    var marker, map, latlng,
        latitudeTextBox = $('#GeoMapLatitude'),
        longitudeTextBox = $('#GeoMapLongtitude'),
        address = $('#geoMapAddress').val();

    if (latitudeTextBox.val() === '' && longitudeTextBox.val() === '') {
        latlng = new google.maps.LatLng(10.8550869, 106.63152500000001);
        latitudeTextBox.val('10.8550869');
        longitudeTextBox.val('106.63152500000001');
    } else {
        latlng = new google.maps.LatLng(latitudeTextBox.val(), longitudeTextBox.val());
    }

    map = new InitializeGoogleMap(map, latlng);

    if (address === null || address === '') {
        latlng = new google.maps.LatLng(latitudeTextBox.val(), longitudeTextBox.val());
        marker = new google.maps.Marker({
            draggable: true,
            map: map,
            position: latlng
        });
        displayLatLong(marker, latitudeTextBox, longitudeTextBox);
        google.maps.event.addListener(marker, 'dragend', function () {
            displayLatLong(marker, latitudeTextBox, longitudeTextBox);
        });
        alert('Please input an location!');
    } else {
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location,
                    draggable: true
                });
                displayLatLong(marker, latitudeTextBox, longitudeTextBox);

                google.maps.event.addListener(marker, 'dragend', function () {
                    displayLatLong(marker, latitudeTextBox, longitudeTextBox);
                });
            } else {
                alert('Geocode was not successful for the following reason: ' + status);
            }
        });
    }
    directionsDisplay.setMap(map);
}

function bindSARole(onload) {
    if ($('#RoleArray4').prop('checked')) {
        $('#RoleArray3').prop('checked', true).prop('disabled', true);
        $('#RoleArray2').prop('checked', true).prop('disabled', true);
        $('#RoleArray1').prop('checked', true).prop('disabled', true);
    } else if (!onload) {
        $('#RoleArray3').prop('checked', false).prop('disabled', false);
        $('#RoleArray2').prop('checked', false).prop('disabled', false);
        $('#RoleArray1').prop('checked', false).prop('disabled', false);
    }
}

function bindLeaderRole(onload) {
    if ($('#RoleArray2').prop('checked')) {
        $('#RoleArray1').prop('checked', true).prop('disabled', true);
    } else if (!onload) {
        $('#RoleArray1').prop('checked', false).prop('disabled', false);
    }
}

function query() {
    var keyword = $('#keyword').val();
    $.getJSON(_global.root + 'api/employee/getemployees', { query: keyword, groupId: null }, function (data) {
        $('#data tbody').children().remove();
        $('#itemTemplate').tmpl(data).appendTo('#data');
        if (keyword.length > 0) {
            $('#data tbody').highlight(keyword);
        }
    });
}

function uploadComplete(evt) {
    if (evt.target.status === 200) {
        var url = $('#avatar').attr('src'),
            parts = url.split('?f=');

        $('#avatar').attr('src', parts[0] + '?f=' + Math.random());
    }
    reloadButtonText('#upload');
}

function uploadFailed() {
    $('#m').text('Upload fail, please try again!');
    reloadButtonText('#upload');
}

function upload() {
    var file = document.getElementById('file').files[0];
    if (file === undefined) {
        return;
    }

    if (file.size / 1024 > 5120) {
        alert('File is to big (' + file.size / 1024 + 'KB) to upload. Maximum = 5120KB');
        return;
    }

    var pass = /\.(gif|jpe?g|png)$/i.test(file.name);
    if (!pass) {
        alert('Please choose an image!');
        return;
    }

    //notice
    actingButton('#upload', true);
    $('#m').text('');

    var fd = new FormData();
    fd.append('code', $('#Code').val());
    fd.append('file', file);

    var xhr = new XMLHttpRequest();
    xhr.addEventListener('load', uploadComplete, false);
    xhr.addEventListener('error', uploadFailed, false);
    xhr.open('POST', _global.root + 'employee/uploadavatar');
    xhr.send(fd);
}

function initDialog(divContent) {
    divContent.dialog({
        modal: true,
        width: 'auto',
        buttons: [
            {
                text: 'Submit',
                class: 'btn-submit waves-effect waves-light m-r--m',
                click: function () {
                    $.ajax({
                        type: 'POST',
                        url: _global.root + 'toeic/updatefulltoeic',
                        dataType: 'json',
                        data: $('#frmUpdFullTOEIC').serialize(),
                        success: function (data) {
                            if (data) {
                                var errorCont = $('#divErrorCont');
                                errorCont.removeClass();
                                errorCont.html('');

                                if (data.code === 1) {
                                    errorCont.addClass('validation-summary-success');
                                    errorCont.append('Update TOEIC score for Employee successfully.');
                                    setTimeout(function () { $('#dialog-toeic').dialog('close'); location.reload(); }, 3000);
                                } else {
                                    var errors = '<ul>';
                                    $(data.error).each(function (i, value) {
                                        errors += '<li>' + value.ErrorMessage + '</li>';
                                    });
                                    errors += '</ul>';

                                    errorCont.addClass('validation-summary-errors');
                                    errorCont.append(errors);
                                }
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert(textStatus + ': ' + errorThrown);
                        }
                    });
                }
            },
            {
                text: 'Close',
                class: 'btn-cancel waves-effect waves-light',
                click: function () {
                    $(this).dialog('close');
                }
            }
        ],
        show: {
            effect: "blind",
            duration: 200
        },
        hide: {
            effect: "blind",
            duration: 100
        }
    });
}

function initNumberTextbox() {
    var numbericTextbox = $('#frmUpdFullTOEIC input[type=number]');

    numbericTextbox.attr('maxlength', '3');
    numbericTextbox.keydown(function (event) {
        // Allow: backspace, delete, tab, escape, and enter
        if (event.keyCode === 46 || event.keyCode === 8 || event.keyCode === 9 || event.keyCode === 27 || event.keyCode === 13 ||
            // Allow: Ctrl+A
            (event.keyCode === 65 && event.ctrlKey === true) ||
            // Allow: home, end, left, right
            (event.keyCode >= 35 && event.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        } else {
            // Ensure that it is a number and stop the keypress
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
            }
        }
    });
}

$(document).ready(function () {
    //Birthday picker.
    $('#DOBPicker').birthdaypicker({
        futureDates: true,
        maxYear: new Date().getFullYear(),
        maxAge: 75,
        defaultDate: $('#DateOfBirth').val()
    });

    initDatePicker('#DateOfBirth');
    initDatePicker('#IssuedDate');
    initDatePicker('#JoinedDate');
    initDatePicker('#ConfirmedDate');
    initDatePicker('#ResignedDate');
    initDatePicker('#PassportExp');

    $('#month, #day, #year').on('change', function () {
        var year = $('#year').val(),
            month = $('#month').val(),
            day = $('#day').val();
        var date = new Date(year, month - 1, day);

        $('#DateOfBirth').val(formatDate(date, 'dd MMM, yyyy'));
    });

    $('#SubGroupArray' + $('#Team_Id').val()).prop('checked', false);
    $('#Team_Id').on('change', function () {
        var id = $(this).val();
        $('#SubGroupArray' + id).prop('checked', false);
    });

    $('input[name=SubGroupArray]').on('change', function () {
        var id = $(this).val();
        var teamId = $('#Team_Id').val();
        if (id === teamId) {
            $(this).prop('checked', false);
        }

        if (!$(this).prop('checked')) {
            $('#LeaderGroupsArray' + id).prop('checked', false);
        }
    });

    $('input[name=LeaderGroupsArray]').on('change', function () {
        var id = $(this).val();
        var teamId = $('#Team_Id').val();

        if (id !== teamId && $('#SubGroupArray' + id).prop('checked') === false) {
            $(this).prop('checked', false);
            alert('Teams\' leaders receive request must be the main team or other team(s)');
        }
    });

    bindSARole(true);
    bindLeaderRole(true);
    $('#RoleArray4').on('click', function () {
        bindSARole(false);
    });

    $('#RoleArray2').on('click', function () {
        bindLeaderRole(false);
    });

    $('#upload').on('click', function () {
        upload();
    });

    $('#delete').on('click', function () {
        confirm(_messages.confirmDelete, function () {
            var resignedDate = $('#ResignedDate').val();
            if (resignedDate.length === 0) {
                alert(_messages.requireResignedDate);
                $('#ResignedDate').focus();
                return;
            }

            $.post(_global.root + 'ajax/DeleteEmployee',
            { id: $('#Id').val(), resignedDate: resignedDate },
            function (response) {
                if (response) {
                    document.location = '/employee/usermanagement';
                } else {
                    alert(_messages.errorTryAgain);
                }
            }).error(function () { alert(_messages.errorTryAgain); });
        });
    });

    // TOEIC
    $('#toeic').on('click', function () {
        var id = parseInt($('#Id').val());
        if (id > 0) {
            $.ajax({
                type: 'GET',
                url: _global.root + 'toeic/updatefulltoeic?id=' + id + '&name=' + $('#NickName').val(),
                dataType: 'html',
                success: function (data) {
                    if (data) {
                        var divContent = $('#dialog-toeic');
                        divContent.html('');
                        divContent.append(data);

                        initNumberTextbox();
                        initDatePicker('#frmUpdFullTOEIC #InputDate');
                        initDatePicker('#frmUpdFullTOEIC #LRExamDate');
                        initDatePicker('#frmUpdFullTOEIC #SWExamDate');
                        initDialog(divContent);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        }
    });

    //Geo map
    $('#geolocationMap').hide();
    $('#showHideMap').on('click', function () {
        if ($('#geolocationMap').is(':visible')) {
            $('#geolocationMap').hide();
        }
        else {
            $('#geolocationMap').show();

            $('#GeoMapLatitude, #GeoMapLongtitude, #DistanceToCompany').attr('readonly', true);
            initialize();
            $('#searchmap').on('click', function () {
                dynamicSearchMap();
            });
        }
    });
});