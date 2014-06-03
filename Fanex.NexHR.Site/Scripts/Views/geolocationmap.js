'use strict';
/*global _global:false, google:false*/

var directionsDisplay;
var countDupe = 0;

function displayDirection(marker) {
	var directionsService = new google.maps.DirectionsService();
	var smsLatlng = new google.maps.LatLng(10.8553024280436, 106.63105838755791);
	var curLatLng = marker.getPosition();

	if (curLatLng.lat() === 0 || curLatLng.lng() === 0) {
		curLatLng = smsLatlng;
	}

	var request = {
		origin: curLatLng,
		destination: smsLatlng,
		travelMode: google.maps.DirectionsTravelMode.DRIVING
	};

	directionsService.route(request, function (response, status) {
		if (status === google.maps.DirectionsStatus.OK) {
			directionsDisplay.setDirections(response);
		}
	});
}

function searchEmployeeDirection(lat, lng) {
	var directionsService = new google.maps.DirectionsService();
	var smsLatlng = new google.maps.LatLng(10.8553024280436, 106.63105838755791);
	var employeeLatLng = new google.maps.LatLng(lat, lng);

	var request = {
		origin: employeeLatLng,
		destination: smsLatlng,
		travelMode: google.maps.DirectionsTravelMode.DRIVING
	};

	directionsService.route(request, function (response, status) {
		if (status === google.maps.DirectionsStatus.OK) {
			directionsDisplay.setDirections(response);
		}
	});
}

function isDupeMarker(lat, lng, arrMarkers) {
	var isDupe = false;
	if (arrMarkers.length > 0) {
		for (var i in arrMarkers) {
			var curLatLng = arrMarkers[i].getPosition();
			if (curLatLng.lat() === lat && curLatLng.lng() === lng) {
				countDupe++;
				isDupe = true;
			}
		}
	}

	return isDupe;
}

//check duplicate lat, long
function MapMarker(NickName, lat, lng, arrMarkers) {
	var increment = 0.000025;
	var isDupe = isDupeMarker(lat, lng, arrMarkers);

	while (isDupe) {
		if (countDupe > 1) {
			increment += 0.000025;
		}
		//var parseLat = parseFloat(lat);
		var parseLng = parseFloat(lng);
		//lat = parseLat + increment;
		lng = parseLng + increment;
		isDupe = false;
	}
	this.lat = lat;
	this.lng = lng;

	return [lat, lng];
}

function getHighestZIndex(markers, highestZIndex) {
	// if we haven't previously got the highest zIndex  
	// save it as no need to do it multiple times  
	if (highestZIndex === 0) {
		if (markers.length > 0) {
			for (var i = 0; i < markers.length; i++) {
				var tempZIndex = markers[i].getZIndex();
				// if this zIndex is the highest so far  
				if (tempZIndex > highestZIndex) {
					highestZIndex = tempZIndex;
				}
			}
		}
	}

	return highestZIndex;
}

function searchEmployeeInCompany() {
	$("#Employee").autocomplete({
		source: function (request, response) {
			var term = request.term;
			var index = term.indexOf(' (');

			if (index > 0) {
				term = term.substr(0, index);
			}

			$.ajax({
				url: _global.root + 'api/employee/getemployees',
				data: {
					query: term,
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
			var nickName = ui.item.label;
			$('#Employee').val(nickName);
			$('#Latitude').val(ui.item.lat);
			$('#Longitude').val(ui.item.lng);
		},
		focus: function (event, ui) {
			var nickName = ui.item.label;
			$('#Employee').val(nickName);
			$('#Latitude').val(ui.item.lat);
			$('#Longitude').val(ui.item.lng);
			event.preventDefault();
		},
		change: function (event, ui) {
			if (ui && ui.item) {
				var nickName = ui.item.label;
				$('#Employee').val(nickName);
				$('#Latitude').val(ui.item.lat);
				$('#Longitude').val(ui.item.lng);
			}
		}
	});
}

function initialize() {

	// Basic information to declare and display a google map.
	var SMSTOWER = new google.maps.LatLng(10.8550869, 106.63152500000001);
	var mapOptions = {
		zoom: 1,
		center: SMSTOWER,
		mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
	};
	var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

	//SMS Tower
	var marker = new google.maps.Marker({
		position: SMSTOWER,
		map: map,
		title: 'SMS TOWER'
	});

	var SMSTowerIcon = new google.maps.MarkerImage(
        "/Styles/Images/Office_building_icon.png",
        null, /* size is determined at runtime */
        null, /* origin is 0,0 */
        new google.maps.Point(14, 30), /* anchor is bottom center of the scaled image */
        new google.maps.Size(40, 40)
        );
	marker.setIcon(SMSTowerIcon);

	var SMSInfoWindow = new google.maps.InfoWindow({
		content: "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>" +
            "NEXCEL SOLUTIONS COMPANY" + "</span></div><div><span class='headerInfo'>Address: </span> " +
            "SMS Tower, Lot 40, Quang Trung Software City, Tan Chanh Hiep Ward, District 12, HCMC" +
            "<div><span class='headerInfo'>Tel: </span>" + "+848 3 7154 278" + "<div><span class='headerInfo'>Fax: </span>" +
            "+848 3 7154 278" + "<div><span class='headerInfo'>Website: </span>" +
            "<a href='http://www.nexcel.vn/'>" + "www.nexcel.vn" + "</a>" + "</div></div></div></div></div></div>"
	});

	google.maps.event.addListener(marker, 'click', function () {
		SMSInfoWindow.open(map, marker);
	});

	//Nexcel Employee Data
	var data = [];
	var markers = [];
	var ZIndex = 1;
	var highestZIndex = 0;

	markers.push(marker);

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

	// Direction render options.
	var rendOpts = { preserveViewport: true, suppressMarkers: true, polylineOptions: { strokeColor: 'red'} };
	directionsDisplay = new google.maps.DirectionsRenderer();

	$.each(data, function (i, item) {
		var infowindow;
		var checkDuplicateData = new MapMarker(item.NickName, item.GeoMapLatitude, item.GeoMapLongtitude, markers);
		var marker = new google.maps.Marker({
			'position': new google.maps.LatLng(checkDuplicateData[0], checkDuplicateData[1]),
			'map': map,
			'Name': item.NickName,
			'title': item.NickName,
			'draggable': false,
			'zIndex': ZIndex
		});
		marker.set("myZIndex", marker.getZIndex());

		//store maker to array
		markers.push(marker);

		//Check for role HR to display corresponding InfoWindow
		if ($("#roleHR").val() === "True") {
			infowindow = new google.maps.InfoWindow({
				content: "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>" +
            item.DisplayName + "</span></div><div><span class='headerInfo'>Team:</span> " +
            item.Team.Name + "<div><span class='headerInfo'>Current Address: </span>" +
            item.CurrentAddress + "<div><span class='headerInfo'>Distance To Company: </span>" +
            item.DistanceToCompany + "</div></div></div></div></div>"
			});
		}
		else {
			infowindow = new google.maps.InfoWindow({
				content: "<div class='infoDiv'>" + "<div class='titleHeader'>" + "<span class='displayName'>" +
            item.DisplayName + "</span></div><div><span class='headerInfo'>Team:</span> " +
            item.Team.Name + "<div><span class='headerInfo'>Distance To Company: </span>" +
            item.DistanceToCompany + "</div></div></div></div>"
			});
		}

		//Check current user.
		if (item.NickName === $("#currentUser").val()) {
			displayDirection(marker);
			var myOption = { suppressMarkers: true, polylineOptions: { strokeColor: 'red'} };
			directionsDisplay = new google.maps.DirectionsRenderer(myOption);
			directionsDisplay.setMap(map);

			var listener = google.maps.event.addListener(map, "idle", function () {
				if (map.getZoom() > 16) {
					map.setZoom(16);
				}

				google.maps.event.removeListener(listener);
			});
			infowindow.open(map, marker);
		}

		//Change default icon of marker to employee's avatar
		var empIcon = new google.maps.MarkerImage(
            '/employee/avatar/' + item.Code,
            new google.maps.Size(71, 71),
            new google.maps.Point(0, 0),
            new google.maps.Point(25, 25),
            new google.maps.Size(40, 40)
        );
		marker.setIcon(empIcon);

		// Google map Events
		google.maps.event.addListener(marker, 'mouseover', function () {
			infowindow.open(map, marker);
			getHighestZIndex(markers, highestZIndex);
			this.setOptions({ zIndex: highestZIndex + (ZIndex - 1) });
		});

		google.maps.event.addListener(marker, 'mouseout', function () {
			infowindow.close();
			this.setOptions({ zIndex: this.get("myZIndex") });
		});

		google.maps.event.addListener(marker, 'click', function () {
			directionsDisplay.setMap(null);
			displayDirection(marker);
			directionsDisplay = new google.maps.DirectionsRenderer(rendOpts);
			directionsDisplay.setMap(map);
		});
		ZIndex++;
	});

	searchEmployeeInCompany();

	$('#searchmap').on("click", function () {
		var lat = $('#Latitude').val();
		var lng = $('#Longitude').val();
		if (lat === "" && lng === "") {
			return false;
		}
		else {
			directionsDisplay.setMap(null);
			searchEmployeeDirection(lat, lng);
			directionsDisplay = new google.maps.DirectionsRenderer(rendOpts);
			directionsDisplay.setMap(map);
		}
	});

	$('#Employee').keypress(function (event) {
		if (event.keyCode === 13) {
			var lat = $('#Latitude').val();
			var lng = $('#Longitude').val();
			if (lat === "" && lng === "") {
				return false;
			}
			else {
				directionsDisplay.setMap(null);
				searchEmployeeDirection(lat, lng);
				directionsDisplay = new google.maps.DirectionsRenderer(rendOpts);
				directionsDisplay.setMap(map);
			}
		}
	});
}

$(document).ready(function () {
    initialize();
});

