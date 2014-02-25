$(document).ready(function () {
    var directionsDisplay;
    Initialize();
});

function Initialize() {
    directionsDisplay = new google.maps.DirectionsRenderer();
    // Google has tweaked their interface somewhat - this tells the api to use that new UI
    google.maps.visualRefresh = false;
    var SMSTOWER = new google.maps.LatLng(10.8550869, 106.63152500000001);

    var mapOptions = {
        center: SMSTOWER,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
        //polylineOptions: { strokeColor: 'rgb(240, 11, 46)' }
    };

    // Change color of direction
    //directionsDisplay = new google.maps.DirectionsRenderer(mapOptions);

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
        new google.maps.Size(40, 60)
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

    // Using the JQuery "each" selector to iterate through the JSON list and drop marker pins
    $.each(data, function (i, item) {
        var marker = new google.maps.Marker({
            'position': new google.maps.LatLng(item.GeoMapLatitude, item.GeoMapLongtitude),
            'map': map,
            'Name': item.NickName,
            'title': item.NickName,
            'draggable': false
        });

        var infowindow = new google.maps.InfoWindow({
            content: "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>"
            + item.FullName + "</span></div><div><span class='headerInfo'>Team:</span> "
            + item.Team.Name + "<div><span class='headerInfo'>Current Address: </span>"
            + item.CurrentAddress + "<div><span class='headerInfo'>Distance To Company: </span>"
            + item.DistanceToCompany + "</div></div></div></div></div>"
        });

        if (item.NickName == $("#currentUser").val()) {
            marker.setIcon('http://maps.google.com/mapfiles/ms/icons/yellow-dot.png');
            displayDirection(marker);
            directionsDisplay.setMap(map);
            infowindow.open(map, marker);
            
        }
        else {
            marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');
        }

        // finally hook up an "MouseOver" listener to the map so it pops up out info-window when the marker-pin is clicked!
        google.maps.event.addListener(marker, 'mouseover', function () {
            infowindow.open(map, marker);
        });

        google.maps.event.addListener(marker, 'mouseout', function () {
            infowindow.close();
        });

        google.maps.event.addListener(marker, 'click', function () {
            displayDirection(marker);
            directionsDisplay.setMap(map);
        });

    })
}

function displayDirection(marker) {
    var directionsService = new google.maps.DirectionsService();
    var curLatLng = marker.getPosition();
    var smsLatlng = new google.maps.LatLng(10.8550869, 106.63152500000001);

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




