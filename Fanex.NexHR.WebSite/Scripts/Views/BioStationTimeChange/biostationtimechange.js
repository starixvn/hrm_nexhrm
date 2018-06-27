var biostationtimechange = {
    init: function() {
        biostationtimechange.handleFilter();
        biostationtimechange.loadDefault();
    },

    loadDefault: function() {
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
                    success: function(value) {
                        $("#table-list").html(value);
                    }
                });
            });
    }
}

$(document).ready(function() {
    biostationtimechange.init();
})