$(document).ready(function () {
	// menu
	bindMenu('.Pending', 'pendingMenu');
	bindMenu('.Approved', 'approvedMenu');

	$(".viewhistory").click(function () {
		var url = $(this).attr("id");
		$.get(url, function (data) {
			$('#dialoghistorycontent').html(data);
			$("#dialoghistorycontent").dialog({
				modal: true,
				minHeight: 0,
				width: 1100,
				title: 'Request History'
			});
		});
	});

});

function processRequest(command, eventElement) {
	var requestId = $(eventElement).attr('requestId');
	showMessage('Processing');
	$.post(_global.root + 'ajax/processrequest',
	{ command: command, id: requestId },
		function (response) {
		    fadeOutMessage();
		    if (response != true) {
		        alert(response);
		        return;
		    }
		    else {
		        //                $(eventElement).removeClass().destroyContextMenu();
		        //                switch (command) {
		        //                    case 'Approve':
		        //                        $(eventElement).text('Approved').addClass('Approved');
		        //                        bindProcessedInfo(eventElement, 'Approve');
		        //                        bindMenu(eventElement, 'approvedMenu');
		        //                        break;
		        //                    case 'Reject':
		        //                        $(eventElement).text('Rejected').addClass('Rejected');
		        //                        bindProcessedInfo(eventElement, 'Reject');
		        //                        break;
		        //                    case 'Disapprove':
		        //                        $(eventElement).text('Pending').addClass('Pending');
		        //                        bindProcessedInfo(eventElement, 'Disapprove');
		        //                        bindMenu(eventElement, 'pendingMenu');
		        //                        break;
		        //                }
		        window.location.reload();
		    }
		});

}

function bindMenu(element, menu) {
	$(element).contextMenu({
		menu: menu
	},
	function (command, eventElement) {
		switch (command) {
			case 'Approve':
				confirm('Are you sure you want to Approve?', function () {
					processRequest(command, eventElement);
				});

				break;
			case 'Reject':
				confirm('Are you sure you want to Reject?', function () {
					processRequest(command, eventElement);
				});
				break;
			case 'Disapprove':
				confirm('Are you sure you want to Disapprove?', function () {
					processRequest(command, eventElement);
				});
				break;
		}
	});
}

function bindProcessedInfo(element, type) {
	var now = new Date();
	var msg;
	switch (type) {
		case 'Approve':
			msg = "Approved by " + _global.nickName + "( " + now.format('dd mmm yyyy- HH') + 'h' + now.format('MM\'') + ")";
			$(element).addClass("tooltip").attr('title', msg);
			tooltip(element, true);
			break;
		case 'Reject':
			msg = "Rejected by " + _global.nickName + "( " + now.format('dd mmm yyyy- HH') + 'h' + now.format('MM\'') + ")";
			$(element).addClass("tooltip").attr('title', msg);
			tooltip(element, true);
			break;
		case 'Disapprove':
			$(element).removeClass("tooltip").removeAttr('original-title');
			tooltip(element, false);
			break;
	}
}
