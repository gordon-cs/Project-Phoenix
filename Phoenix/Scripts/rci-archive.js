$(".select-completed-rcis").find("button").click(function () {
    var $this = $(this);

    if ($this.hasClass("unselected")) {
        $(".rci-card").each(function (index, element) {
            var $element = $(element);

            //If it is a green card and has three signature blocks
            if ($element.find(".rci-card-checkout").length > 0 && $element.find(".signature-alert").length === 3) {
                $element.find("input[type=checkbox]").first().prop("checked", true);
                $this.removeClass("unselected");
                $this.addClass("selected");
                $this.text("Unselect All Completed Rcis");
            }
        });

    }
    else {
        $(".rci-card").each(function (index, element) {
            var $element = $(element);

            if ($element.find(".rci-card-checkout").length > 0 && $element.find(".signature-alert").length === 3) {
                $element.find("input[type=checkbox]").first().prop("checked", false);
                $this.removeClass("selected");
                $this.addClass("unselected");
                $this.text("Select All Completed Rcis");
            }
        });
    }
});

$(".rci-archive-button").click(function () {
    var rcisToArchive = [];

    $(".rci-card").each(function (index, element) {
        var $element = $(element);

        var $checkbox = $element.find("input[type=checkbox]").first();
        if ($checkbox.prop("checked")) {
            rcisToArchive.push($checkbox.attr("value"));
        }
    });

    $.ajax({
        method: "POST",
        url: "/Dashboard/ArchiveRcis",
        data: { rciID: rcisToArchive }
    })
    .then(function (data) {
        window.location.href = "/Dashboard";
    });

});