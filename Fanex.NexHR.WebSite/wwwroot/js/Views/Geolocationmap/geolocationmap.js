'use strict';
/*global _global, google*/
var countDupe = 0;

function autoCompleteEmployee() {
    $("#Employee").autocomplete({
        source: function (request, response) {
            var term = request.term;
            var index = term.indexOf(' (');

            if (index > 0) {
                term = term.substr(0, index);
            }

            $.ajax({
                url: _global.root + 'employee/GetGeoMapEmployeeSuggestion',
                data: {
                    query: term,
                    isSpecial: false,
                    $top: 10
                },
                success: function (data) {
                    if (data && data.length > 0) {
                        response($.map(data, function (item) {
                            return {
                                label: item.NickName + ' (' + item.FullName + ')',
                                lat: item.GeoMapLatitude,
                                lng: item.GeoMapLongtitude
                            };
                        }));
                    }
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            $('#Employee').val(ui.item.label);
            $('#Latitude').val(ui.item.lat);
            $('#Longitude').val(ui.item.lng);
        },
        focus: function (event, ui) {
            $('#Employee').val(ui.item.label);
            $('#Latitude').val(ui.item.lat);
            $('#Longitude').val(ui.item.lng);
            event.preventDefault();
        },
        change: function (event, ui) {
            if (ui && ui.item) {
                $('#Employee').val(ui.item.label);
                $('#Latitude').val(ui.item.lat);
                $('#Longitude').val(ui.item.lng);
            }
        }
    });
}

//check duplicate lat, long
function MapMarker(lat, lng, arrMarkers) {
    var increment = 0.00005,
        isDupe = false;

    if (arrMarkers.length > 0) {
        for (let i = 0; i < arrMarkers.length; i++) {
            if (arrMarkers[i].lat.toString() === lat.toString() && arrMarkers[i].lng.toString() === lng.toString()) {
                countDupe++;
                isDupe = true;
            }
        }
    }

    var parseLat = 0;
    var parseLng = 0;

    if (!isNaN(parseFloat(lat)) && !isNaN(parseFloat(lng))) {
        parseLat = parseFloat(lat);
        parseLng = parseFloat(lng);
    }
    while (isDupe) {
        if (countDupe > 1) {
            increment += 0.00005;
        }
        parseLat = parseLat + increment;
        parseLng = parseLng + increment;
        isDupe = false;
    }
    return [parseLat, parseLng];
}

function addMarkerToGroup(group, coordinate, html, icon, zindex) {
    var marker = new H.map.Marker(coordinate, icon);
    marker.setData(html);
    marker.setZIndex(zindex);


    var infoBubble = new H.ui.InfoBubble(coordinate, {
        content: html
    });

    marker.addEventListener('pointermove', function () {
        ui.addBubble(infoBubble);
        infoBubble.open();
    }, false);

    marker.addEventListener('pointerleave', function () {
        infoBubble.close();
    }, false);

    group.addObject(marker);
}

function searchEmployee(markers) {
    var lat = 0,
        lng = 0,
        lable = $('#Employee').val(),
        html = '';
    for (let i = 1; i < markers.length; i++) {
        var index = lable.indexOf('(');
        if (index > 0) {
            lable = lable.substring(0, index);
        }
        if (lable.toLowerCase().indexOf(markers[i].nick.toLowerCase()) !== -1) {
            html = markers[i].html;
            lat = markers[i].lat;
            lng = markers[i].lng;
        }
    }
    if (lat === 0 || lng === 0) {
        alert("No information about this employee");
        return false;
    } else {
        showRouteToCompany(lat, lng);
        //var bubble = new H.ui.InfoBubble({ lat, lng }, {
        //    content: html,
        //});
        //ui.addBubble(bubble);
    }
}

function showRouteToCompany(lat, lng) {
    var onResult = function (result) {
        if (result.routes.length) {
            result.routes[0].sections.forEach((section) => {

                let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);
                var routeOutline = new H.map.Polyline(linestring, {
                    style: {
                        lineWidth: 8,
                        strokeColor: 'blue',
                        lineTailCap: 'arrow-tail',
                        lineHeadCap: 'arrow-head'
                    }
                });
                var routeArrows = new H.map.Polyline(linestring, {
                    style: {
                        lineWidth: 8,
                        fillColor: 'white',
                        strokeColor: 'rgba(255, 255, 255, 1)',
                        lineDash: [0, 2],
                        lineTailCap: 'arrow-tail',
                        lineHeadCap: 'arrow-head'
                    }
                }
                );
                var routeLine = new H.map.Group();
                routeLine.addObjects([routeOutline, routeArrows]);

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

function addMarkersToMap(map) {
    var markers = [];
    var group = new H.map.Group();
    map.addObject(group);

    //create icon of nexcel
    var nexcelMarkup = '<svg  width="34" height="34" xmlns="http://www.w3.org/2000/svg">' +
        '<rect stroke="black" fill="white" x="1" y="1" width="32" height="32" />' +
        '<text x="17" y="28" font-size="22pt" font-family="Arial" font-weight="bold" ' +
        'text-anchor="middle" fill="orange" >N</text></svg>';
    var nexcelIcon = new H.map.Icon(nexcelMarkup);

    //add nexcel's marker to group
    addMarkerToGroup(group, { lat: 10.8550869, lng: 106.63152500000001 },
        "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>" +
        "NEXCEL SOLUTIONS COMPANY" + "</span></div><div><span class='headerInfo'>Address: </span> " +
        "SMS Tower, Lot 40, Quang Trung Software City, Tan Chanh Hiep Ward, District 12, HCMC" +
        "<div><span class='headerInfo'>Tel: </span>" + "+848 3 7154 278" + "<div><span class='headerInfo'>Fax: </span>" +
        "+848 3 7154 278" + "<div><span class='headerInfo'>Website: </span>" +
        "<a href='http://www.nexcel.vn/'>" + "www.nexcel.vn" + "</a>" + "</div></div></div></div></div></div>", { icon: nexcelIcon }, 2);
    // add lat, lng to markers []
    markers.push({ lat: 10.8550869, lng: 106.63152500000001 });

    //Nexcel Employee Data
    var data = [];
    $.ajax({
        url: _global.root + 'employee/getemployees',
        data: {
            query: '',
            isSpecial: false,
            $top: 10
        },
        async: false,
        success: function (employeeInfo) {
            data = employeeInfo.Employees;
        }
    });

    $.each(data, function (i, item) {
        var checkDuplicateData = new MapMarker(item.GeoMapLatitude, item.GeoMapLongtitude, markers);

        var icon = new H.map.Icon('/employee/avatar/' + item.Code, { size: { w: 45, h: 60 } });
        var html;
        //Check for role HR to display corresponding InfoWindow
        if ($("#roleHR").val() === "True") {
            html = "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>" +
                item.DisplayName + "</span></div><div><span class='headerInfo'>Team:</span> " +
                item.Team.Name + "<div><span class='headerInfo'>Current Address: </span>" +
                item.CurrentAddress + "<div><span class='headerInfo'>Distance To Company: </span>" +
                item.DistanceToCompany + "</div></div></div></div></div>";
        } else {
            html = "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>" +
                item.DisplayName + "</span></div><div><span class='headerInfo'>Team:</span> " +
                item.Team.Name + "<div><span class='headerInfo'>Distance To Company: </span>" +
                item.DistanceToCompany + "</div></div></div></div>";
        }
        addMarkerToGroup(group, { lat: checkDuplicateData[0], lng: checkDuplicateData[1] }, html, { icon: icon }, 1);
        // add lat, lng to markers []
        markers.push({ lat: checkDuplicateData[0], lng: checkDuplicateData[1], html: html, nick: item.NickName });

        if (item.NickName === $("#currentUser").val()) {
            map.setCenter({ lat: checkDuplicateData[0], lng: checkDuplicateData[1] });
            map.setZoom(13);
            var bubble = new H.ui.InfoBubble({ lat: checkDuplicateData[0], lng: checkDuplicateData[1] }, {
                content: html,
            });
            ui.addBubble(bubble);
        }
    });

    autoCompleteEmployee();

    $('#searchmap').on("click", function () {
        searchEmployee(markers);
    });
    $('#Employee').keypress(function (event) {
        if (event.keyCode === 13) {
            searchEmployee(markers);
        }
    });
}

//init map
var platform = new H.service.Platform({
    apikey: "J7GXuxkgZ23QX19IcXY9gZd677O9gb0WQLXcsAp__Mw"
});
var defaultLayers = platform.createDefaultLayers();
var SMSTOWER = { lat: 10.8550869, lng: 106.63152500000001 };
var map = new H.Map(document.getElementById('map_canvas'),
    defaultLayers.vector.normal.map, {
    center: SMSTOWER,
    zoom: 14,
    pixelRatio: window.devicePixelRatio || 1
});
window.addEventListener('resize', () => map.getViewPort().resize());
var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
var ui = H.ui.UI.createDefault(map, defaultLayers);

$(document).ready(function () {
    addMarkersToMap(map);
});