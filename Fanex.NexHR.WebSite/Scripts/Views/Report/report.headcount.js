"use strict";

var headCount = (function ($) {
    var domElement = {
        Year: "#Year",
        Submit: "#submit",
        Grid: "#head-count-grid",
        Export: "#exportExcel"
    };

    function getHeadCount() {
        $(domElement.Submit).on("click", function (e) {
            e.preventDefault();
            var year = $(domElement.Year).val();
            $.ajax({
                url: _global.root + "Report/GetHeadCount?Year=" + year,
                type: "GET",
                success: function (data) {
                    $(domElement.Grid).html(data);
                },
                complete: function () {
                    $('.loading-process').fadeOut(300);
                }
            });
        });
    }

    function exportExcel() {
        $(domElement.Export).on("click", function () {
            var url = _global.root + "Export/HeadCountExport?Year=" + $(domElement.Year).val();
            console.log(url);

            window.location.href = url;
        });
    }

    function init() {
        getHeadCount();
        exportExcel();
    }

    return {
        init
    }
})(jQuery);

$(document).ready(function () {
    headCount.init();
    $("#submit").trigger("click");
});