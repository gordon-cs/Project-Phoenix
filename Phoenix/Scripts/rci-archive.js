

$(".rci-archive-button").click(function () {
    var rcisToArchive = [];

    $(".rci-card").each(function (index, element) {
        var $element = $(element);

        var $checkbox = $element.find("input[type=checkbox]").first();
        if ($checkbox.prop("checked"))
        {
            rcisToArchive.push( $checkbox.attr("value") );
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