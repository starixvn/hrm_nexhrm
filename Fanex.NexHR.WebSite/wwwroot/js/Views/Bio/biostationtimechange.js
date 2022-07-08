var biostationtimechange = {
    init: function () {
        biostationtimechange.handleFilter();
        biostationtimechange.loadDefault();
    },

    loadDefault: function () {
        $(".form-filter").trigger("submit");
    },

    handleFilter: function () {
        var form = $(".form-filter");
        $(form).on("submit",
            function (e) {
                e.preventDefault();
                $.ajax({
                    url: form.attr("action"),
                    method: form.attr("method"),
                    data: form.serialize(),
                    beforeSend: function () {
                        $(".loading-process").fadeIn(300);
                    },
                    success: function (value) {
                        $("#table-list").html(value);
                        floatThead("table#data");
                    },
                    complete: function () {
                        $(".loading-process").fadeOut(300);
                    }
                });
            });
    }
}

$(document).ready(function () {
    biostationtimechange.init();
})