$(document).ready(function () {
    Initialize();
});

function Initialize() {
    var currentUserNickName = $("#currentUser").val();
    // Google has tweaked their interface somewhat - this tells the api to use that new UI
    google.maps.visualRefresh = true;
    var SMSTOWER = new google.maps.LatLng(10.8550869, 106.63152500000001);

    // These are options that set initial zoom level, where the map is centered globally to start, and the type of map to show
    var mapOptions = {
        zoom: 14,
        center: SMSTOWER,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };

    // This makes the div with id "map_canvas" a google map
    var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

    // This shows adding a simple pin "marker" - this happens to be the Tate Gallery in SMS Tower!
    var myLatlng = new google.maps.LatLng(10.8550869, 106.63152500000001);

    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        title: 'SMS TOWER'
    });

    marker.setIcon('http://maps.google.com/mapfiles/ms/icons/red-dot.png')

    //  custome Marker's icon
    //    var pinIcon = new google.maps.MarkerImage(
    //    "/Styles/Images/Office_building_icon.png",
    //    null, /* size is determined at runtime */
    //    null, /* origin is 0,0 */
    //    null, /* anchor is bottom center of the scaled image */
    //    new google.maps.Size(40, 60)
    //    );
    //    marker.setIcon(pinIcon)

    var data = [];
    $.ajax({
        url: _global.root + 'api/employee/getemployees',
        data: {
            query: '',
            $top: 10
        },
        async: false,
        success: function (dataLatLng) {
            data = dataLatLng;
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

        //flying distance
        //var distance = google.maps.geometry.spherical.computeDistanceBetween(center, markerLatLng)/1000;

        // put in some information about each json object - in this case, the opening hours.
        var infowindow = new google.maps.InfoWindow({
            content: "<div class='infoDiv'>" + "<span id='headerInfo'>"
            + item.DisplayName + "</span><div><span class='headerInfo'>Team:</span> "
            + item.Team.Name + "<div><span class='headerInfo'>Current Address: </span>"
            + item.CurrentAddress + "<div><span class='headerInfo'>Distance To Company: </span>" 
            + item.DistanceToCompany + " km" + "</div></div></div></div></div>"
        });

        if (item.NickName == currentUserNickName) {
            marker.setIcon('http://maps.google.com/mapfiles/ms/icons/yellow-dot.png')
            infowindow.open(map, marker);
        }
        else {
            marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png')
        }

        // finally hook up an "MouseOver" listener to the map so it pops up out info-window when the marker-pin is clicked!
        google.maps.event.addListener(marker, 'mouseover', function () {
            infowindow.open(map, marker);
            //mouseover
        });

        google.maps.event.addListener(marker, 'mouseout', function () {
            infowindow.close();
        });

    })
}




