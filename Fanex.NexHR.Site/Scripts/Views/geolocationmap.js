$(document).ready(function () {
    var directionsDisplay;
    Initialize();
});

function Initialize() {
    // Google has tweaked their interface somewhat - this tells the api to use that new UI
    google.maps.visualRefresh = false;
    var SMSTOWER = new google.maps.LatLng(10.8550869, 106.63152500000001);

    var mapOptions = {
        zoom: 1,
        center: SMSTOWER,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };

    //marker.setIcon('http://maps.google.com/mapfiles/ms/icons/yellow-dot.png');
    // This makes the div with id "map_canvas" a google map
    var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

    var marker = new google.maps.Marker({
        position: SMSTOWER,
        map: map,
        title: 'SMS TOWER'
    });

    //  custome Marker's icon
    var pinIcon = new google.maps.MarkerImage(
        "/Styles/Images/Office_building_icon.png",
        null, /* size is determined at runtime */
        null, /* origin is 0,0 */
        new google.maps.Point(14, 30), /* anchor is bottom center of the scaled image */
        new google.maps.Size(40, 40)
        );
    marker.setIcon(pinIcon);

    var SMSInfoWindow = new google.maps.InfoWindow({
        content: "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>"
            + "NEXCEL SOLUTIONS COMPANY" + "</span></div><div><span class='headerInfo'>Address: </span> "
            + "SMS Tower, Lot 40, Quang Trung Software City, Tan Chanh Hiep Ward, District 12, HCMC"
            + "<div><span class='headerInfo'>Tel: </span>" + "+848 3 7154 278" + "<div><span class='headerInfo'>Fax: </span>"
            + "+848 3 7154 278" + "<div><span class='headerInfo'>Website: </span>"
            + "<a href='http://www.nexcel.vn/'>" + "www.nexcel.vn" + "</a>" + "</div></div></div></div></div></div>"
    });

    google.maps.event.addListener(marker, 'click', function () {
        SMSInfoWindow.open(map, marker);
    });

    var data = [];
    $.ajax({
        url: _global.root + 'api/employee/getemployees',
        data: {
            query: '',
            $top: 10
        },
        async: false,
        success: function (employeeInfo) {
            data = employeeInfo;
        }
    });

    var rendOpts = { preserveViewport: true, suppressMarkers: true };
    directionsDisplay = new google.maps.DirectionsRenderer();

    // Using the JQuery "each" selector to iterate through the JSON list and drop marker pins
    $.each(data, function (i, item) {
        var marker = new google.maps.Marker({
            'position': new google.maps.LatLng(item.GeoMapLatitude, item.GeoMapLongtitude),
            'map': map,
            'Name': item.NickName,
            'title': item.NickName,
            'draggable': false
        });

        var infowindow;

        if ($("#roleHR").val() == "True") {
            infowindow = new google.maps.InfoWindow({
                content: "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>"
            + item.FullName + "</span></div><div><span class='headerInfo'>Team:</span> "
            + item.Team.Name + "<div><span class='headerInfo'>Current Address: </span>"
            + item.CurrentAddress + "<div><span class='headerInfo'>Distance To Company: </span>"
            + item.DistanceToCompany + "</div></div></div></div></div>"
            });
        }
        else {
            infowindow = new google.maps.InfoWindow({
                content: "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>"
            + item.FullName + "</span></div><div><span class='headerInfo'>Team:</span> "
            + item.Team.Name + "<div><span class='headerInfo'>Distance To Company: </span>"
            + item.DistanceToCompany + "</div></div></div></div>"
            });
        }

        if (item.NickName == $("#currentUser").val()) {
            displayDirection(marker);
            var myOption = { suppressMarkers: true };
            directionsDisplay = new google.maps.DirectionsRenderer(myOption);

            directionsDisplay.setMap(map);
            var listener = google.maps.event.addListener(map, "idle", function () {
                if (map.getZoom() > 16) map.setZoom(16);
                google.maps.event.removeListener(listener);
            });
            infowindow.open(map, marker);
        }

        var empIcon = new google.maps.MarkerImage(
            '/employee/avatar/' + item.Code,
            new google.maps.Size(71, 71), /* size is determined at runtime */
            new google.maps.Point(0, 0), /* origin is 0,0 */
            new google.maps.Point(25, 25), /* anchor is bottom center of the scaled image */
            new google.maps.Size(40, 40)
        );
        marker.setIcon(empIcon);

        // finally hook up an "MouseOver" listener to the map so it pops up out info-window when the marker-pin is clicked!
        google.maps.event.addListener(marker, 'mouseover', function () {
            infowindow.open(map, marker);
        });

        google.maps.event.addListener(marker, 'mouseout', function () {
            infowindow.close();
        });

        google.maps.event.addListener(marker, 'click', function () {
            directionsDisplay.setMap(null);
            displayDirection(marker);
            directionsDisplay = new google.maps.DirectionsRenderer(rendOpts);
            directionsDisplay.setMap(map);
        });

    })
}

function displayDirection(marker) {
    var directionsService = new google.maps.DirectionsService();
    var smsLatlng = new google.maps.LatLng(10.8553024280436, 106.63105838755791);
    //var smsLatlng = new google.maps.LatLng(10.855531609229358, 106.63109062644196);
    var curLatLng = marker.getPosition();
    if (curLatLng.lat() == 0 || curLatLng.lng() == 0) {
        curLatLng = smsLatlng;
    }

    var request = {
        origin: curLatLng,
        destination: smsLatlng,
        travelMode: google.maps.DirectionsTravelMode.DRIVING
    };
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
        }
    });
}




