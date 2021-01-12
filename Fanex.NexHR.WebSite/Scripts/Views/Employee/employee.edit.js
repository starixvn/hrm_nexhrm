'use strict';
/*global _global, _messages, actingButton, google, reloadButtonText, formatDate, initDatePicker*/
/* exported query*/

var directionsDisplay;

function displayLatLong(marker, latitudeTextBox, longitudeTextBox) {
    var directionsService = new google.maps.DirectionsService(),
        curLatLng = marker.getPosition(),
        smsLatlng = new google.maps.LatLng(10.8553024280436, 106.63105838755791);

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
        longitudeTextBox = $('#GeoMapLongtitude'),
        myOption = { suppressMarkers: true, polylineOptions: { strokeColor: 'red'} };

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
    $.getJSON(_global.root + 'employee/getemployees', { query: keyword, groupId: null }, function (data) {
        $('#data tbody').children().remove();
        $('#itemTemplate').tmpl(data.Employees).appendTo('#data');
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
                text: 'Close',
                class: 'btn-cancel waves-effect waves-light m-r--m',
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: 'Submit',
                class: 'btn-submit waves-effect waves-light',
                click: function () {
                    $.ajax({
                        type: 'POST',
                        url: _global.root + 'toeic/updatetoeic',
                        dataType: 'json',
                        data: $('#frmUpdateTOEIC').serialize(),
                        success: function (data) {
                            var errorCont = $('#divErrorCont');
                            errorCont.removeClass();
                            errorCont.html('');

                            if (data.code === 0) {
                                errorCont.addClass('success validation-summary-success');
                                errorCont.append('Update TOEIC score for Employee successfully');
                                setTimeout(function () { $('#dialog-toeic').dialog("close"); query(); }, 3000);
                            }
                            else {
                                var errors = '<ul>';
                                $(data.error).each(function (i, value) {
                                    errors += '<li>' + value.ErrorMessage + '</li>';
                                });
                                errors += '</ul>';

                                errorCont.addClass('validation-summary-errors');
                                errorCont.append(errors);
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert(textStatus + ': ' + errorThrown);
                        }
                    });
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
    var numbericTextbox = $('#frmUpdateTOEIC input[type=number]');

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

function initBirthdayPicker() {
    $('#DOBPicker').birthdaypicker({
        futureDates: true,
        maxYear: new Date().getFullYear(),
        maxAge: 75,
        defaultDate: $('#DateOfBirth').val()
    });

    $('#month, #day, #year').on('change', function () {
        var year = $('#year').val(),
            month = $('#month').val(),
            day = $('#day').val();
        var date = new Date(year, month - 1, day);

        $('#DateOfBirth').val(formatDate(date, 'dd MMM, yyyy'));
    });
}

function initEventForSubGroupAndLeaderGroup() {
    $('input.SubGroupArray').on('change', function () {
        var id = $(this).val(),
            teamId = $('#TeamId').val();

        if (id === teamId) {
            $(this).prop('checked', false);
        }

        //if (!$(this).prop('checked')) {
        //    $('#LeaderGroupsArray' + id).prop('checked', false);
        //}
    });

    //$('input.LeaderGroupsArray').on('change', function () {
    //    var id = $(this).val(),
    //        teamId = $('#TeamId').val();

    //    if (id !== teamId && $('#SubGroupArray' + id).prop('checked') === false) {
    //        $(this).prop('checked', false);
    //        alert('Teams\' leaders receive request must be the main team or other team(s)');
    //    }
    //});
}

function initEventForUpdateToeic() {
    $('#toeic').on('click', function () {
        var id = parseInt($('#Id').val());
        if (id > 0) {
            $.ajax({
                type: 'GET',
                url: _global.root + 'toeic/updatetoeic?id=' + id + '&name=' + $('#NickName').val(),
                dataType: 'html',
                success: function (data) {
                    if (data) {
                        var divContent = $('#dialog-toeic');
                        divContent.html('');
                        divContent.append(data);
                        initNumberTextbox();
                        initDatePicker();
                        initDialog(divContent);
                        $('#Employee').prop('readonly', true);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        }
    });
}

function calculateToeicScore() {
    var listening = $('form').find('input[name=Listening]').val();
    if (!listening) listening = 0;

    var reading = $('form').find('input[name=Reading]').val();
    if (!reading) reading = 0;

    var total = parseInt(listening) + parseInt(reading);

    $('form').find('input[name=Total]').val(total);
}

function calculateOldToeicScore() {
    var oldListening = $('#frmUpdateTOEIC').find('input[name=OldListening]').val();
    if (!oldListening) oldListening = 0;

    var oldReading = $('#frmUpdateTOEIC').find('input[name=OldReading]').val();
    if (!oldReading) oldReading = 0;

    var oldTotal = parseInt(oldListening) + parseInt(oldReading);
    $('#frmUpdateTOEIC').find('input[name=OldTotal]').val(oldTotal);
}

function initEventForDelete() {
    $('#delete').on('click', function () {
        confirm(_messages.confirmDelete, function () {
            var resignedDate = $('#ResignedDate').val();
            if (resignedDate.length === 0) {
                alert(_messages.requireResignedDate);
                return;
            }

            $.post(_global.root + 'employee/Delete',
            { employeeId: $('#Id').val(), resignedDate: resignedDate },
            function (response) {
                if (response.IsSuccessful) {
                    document.location = _global.root + 'employee/usermanagement';
                } else {
                    alert(response.Message);
                }
            }).error(function () { alert(_messages.errorTryAgain); });
        });
    });
}

$(document).ready(function () {
    // Birthday picker.
    initBirthdayPicker();

    $('#SubGroupArray' + $('#TeamId').val()).prop('checked', false);
    $('#TeamId').on('change', function () {
        var id = $(this).val();
        $('#SubGroupArray' + id).prop('checked', false);
    });

    initEventForSubGroupAndLeaderGroup();

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

    initEventForDelete();

    /// TOEIC
    initEventForUpdateToeic();

    /// Geo map
    $('#geolocationMap').hide();
    $('#showHideMap').on('click', function () {
        if ($('#geolocationMap').is(':visible')) {
            $('#geolocationMap').hide();
        } else {
            $('#geolocationMap').show();

            $('#GeoMapLatitude, #GeoMapLongtitude, #DistanceToCompany').attr('readonly', true);
            initialize();
            $('#searchmap').on('click', function () {
                dynamicSearchMap();
            });
        }
    });

    var successed = $('#successed').val();
    if (successed === 'true') {
        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + 'Employee/Usermanagement';
                }
            );
    }
});