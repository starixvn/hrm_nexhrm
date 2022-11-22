'use strict';
/*global _global, _messages, actingButton, google, reloadButtonText, formatDate, initDatePicker*/
/* exported query*/

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
    $.getJSON(_global.root + 'employee/getemployees', { query: keyword, isSpecial: false, groupId: null }, function (data) {
        $('#data tbody').children().remove();
        $('#itemTemplate').tmpl(data.Employees).appendTo('#data');
        if (keyword.length > 0) {
            $('#data tbody').highlight(keyword);
        }
    });
}

function uploadComplete(evt) {
    if (evt.target.status === 200) {
        console.log("ok done")
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
/* HERE MAP API */
var platform = new H.service.Platform({
    apikey: 'J7GXuxkgZ23QX19IcXY9gZd677O9gb0WQLXcsAp__Mw'
});
var defaultLayers = platform.createDefaultLayers();
var address = "";

var SMSTOWER = { lat: 10.8550869, lng: 106.63152500000001 };
var map = new H.Map(
    document.getElementById('map-canvas'),
    defaultLayers.vector.normal.map,
    {
        zoom: 15,
        center: SMSTOWER,
        pixelRatio: window.devicePixelRatio || 1
    });
window.addEventListener('resize', () => map.getViewPort().resize());
var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
var ui = H.ui.UI.createDefault(map, defaultLayers);



function changeScreenMedia() {
    window.addEventListener('resize', function (event) {
        var width = document.documentElement.clientWidth;
        if (width > 0 && width <= 993)
            $('#map-canvas').css({ "width": "100%", "height": "50vh", "margin-bottom": "20rem" });
        if (width > 993 && width <= 1627)
            $('#map-canvas').css({ "width": "100%", "height": "50vh", "margin-bottom": "10rem" });
        if (width > 1627)
            $('#map-canvas').css({ "width": "100%", "height": "50vh", "margin-bottom": "5rem" });
    });
}

function addNexcelMarker(map) {
    var nexcelMarkup = '<svg  width="34" height="34" xmlns="http://www.w3.org/2000/svg">' +
        '<rect stroke="black" fill="white" x="1" y="1" width="32" height="32" />' +
        '<text x="17" y="28" font-size="22pt" font-family="Arial" font-weight="bold" ' +
        'text-anchor="middle" fill="orange" >N</text></svg>';
    var nexcelIcon = new H.map.Icon(nexcelMarkup);
    var marker = new H.map.Marker({ lat: 10.8550869, lng: 106.63152500000001 }, { icon: nexcelIcon });

    map.addObject(marker);
}

function searchHereMapLocation(platform, keyword) {
    var geocoder = platform.getSearchService(),
        geocodingParameters = {
            q: keyword
        };

    geocoder.geocode(
        geocodingParameters,
        onSuccess,
        onError
    );
}

var bubble;
function openBubble(position, text) {
    if (!bubble) {
        bubble = new H.ui.InfoBubble(
            position,
            { content: text });
        ui.addBubble(bubble);
    } else {
        bubble.setPosition(position);
        bubble.setContent(text);
        bubble.open();
    }
}

function showRouteToCompany(platform, lat, lng) {
    //draw route on map
    var onResult = function (result) {
        if (result.routes.length) {
            result.routes[0].sections.forEach((section) => {

                let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);
                var routeOutline = new H.map.Polyline(linestring, {
                    style: {
                        lineWidth: 8,
                        strokeColor: 'rgba(0, 128, 255, 0.7)',
                        lineTailCap: 'arrow-tail',
                        lineHeadCap: 'arrow-head'
                    }
                });
                var routeArrows = new H.map.Polyline(linestring, {
                    style: {
                        lineWidth: 8,
                        fillColor: 'white',
                        strokeColor: 'white',
                        lineDash: [0, 2],
                        lineTailCap: 'arrow-tail',
                        lineHeadCap: 'arrow-head'
                    }
                }
                );
                var routeLine = new H.map.Group();
                routeLine.addObjects([routeOutline, routeArrows]);

                var marker = new H.map.Marker({ lat, lng });
                var infoBubble = new H.ui.InfoBubble({ lat, lng }, {
                    content: address
                });
                marker.addEventListener('pointermove', function () {
                    ui.addBubble(infoBubble);
                    infoBubble.open();
                }, false);

                marker.addEventListener('pointerleave', function () {
                    infoBubble.close();
                }, false);
                routeLine.addObject(marker);

                var obj = map.getObjects();
                obj.forEach((x, index) => {
                    if (index > 0) {
                        map.removeObject(x);
                    }
                });
                map.addObjects([routeLine]);
                map.getViewModel().setLookAtData({ bounds: routeLine.getBoundingBox() });
            });
        }
    };
    var routingParameters = {
        'routingMode': 'fast',
        'transportMode': 'car',
        'origin': lat.toString() + ',' + lng.toString(),
        'destination': '10.8550869,106.63152500000001',
        'return': 'polyline'
    };
    var router = platform.getRoutingService(null, 8);
    router.calculateRoute(routingParameters, onResult,
        function (error) {
            alert(error.message);
        });
}

function distanceToCompany(platform, lat, lng) {
    //calculate distance
    const rService = platform.getRoutingService();
    const params = {
        mode: "fastest;car;traffic:enabled",
        waypoint0: lat.toString() + ',' + lng.toString(),
        waypoint1: "10.8550869,106.63152500000001",
        representation: "display",
        routeAttributes: "summary"
    };
    rService.calculateRoute(params, success => {
        var meters = success.response.route[0].summary.distance / 1000;
        $('#DistanceToCompany').val(meters.toFixed(1) + " km");
    }, error => {
        console.log(error);
    });
}

function onSuccess(result) {
    var locations = result.items;
    //if dont have any address
    if (locations.length <= 0)
        alert("Please enter the correct address (do not abbreviate)");
    else {
        let location = locations[0];
        $('#GeoMapLatitude').val(location.position.lat);
        $('#GeoMapLongtitude').val(location.position.lng);
        address = location.address.label;
        showRouteToCompany(platform, location.position.lat, location.position.lng);
        distanceToCompany(platform, location.position.lat, location.position.lng);
    }
}

function onError(error) {
    alert('Can\'t reach the remote server');
}

/* HERE MAP API */

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

            var lat = $('#GeoMapLatitude').val();
            var lng = $('#GeoMapLongtitude').val();
            if (lat !== "" && lng !== "") {
                address = $('#CurrentAddress').val();
                showRouteToCompany(platform, lat, lng);
            }

            $('#GeoMapLatitude, #GeoMapLongtitude, #DistanceToCompany').attr('readonly', true);
            $('#searchmap').on('click', function () {
                searchHereMapLocation(platform, $('#geoMapAddress').val());
            });
        }
    });
    addNexcelMarker(map);
    changeScreenMedia();

    var successed = $('#successed').val();
    if (successed === 'true') {

        confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfully!</p><p>Press OK to continue</p>',
            function () {
                document.location = _global.root + $('#LastUrl').val();
                //document.location = _global.root + 'Employee/Usermanagement';
            }
        );
    }
});