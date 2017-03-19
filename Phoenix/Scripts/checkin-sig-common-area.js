$("#submit-button").click(function () {
    submit();
});


function submit() {
    var signature = "";
    var rciId = $("div[id^='rci-']").first().attr("id").substring(4);

    $(".rci-sig").each(function (index, element) {
        var $element = $(element);
        if ($element.val() && $element.val().trim() !== "")
        {
            signature = $element.val();
        }
    });

    $.ajax({
        sync: false,
        url: "/RciInput/SaveSigCommonArea",
        data: { rciSig: signature, rciId: rciId },
        method: "POST",
        success: function (data) {
            window.location.href = data;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
            console.log(errorThrown);
        }

    });
}