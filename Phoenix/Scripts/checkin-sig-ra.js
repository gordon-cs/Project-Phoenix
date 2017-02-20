$("#submit-button").click(function () {
    submit();
});

/* Submit signatures for RCI  */
function submit() {
    var rciSig = "";
    var rciSigRes = "";
    if ($("#rci-sig").attr("disabled") != "disabled") {
        rciSig = $("#rci-sig").val();
    }
    if ($("#rci-sig-res").attr("disabled") != "disabled") {
        rciSigRes = $("#rci-sig-res").val();
    }
    var id = $("h2[id^='rci-']").first().attr("id").substring(4);

    $.ajax({
        sync: false,
        url: "/RciInput/SaveSigRA",
        data: { rciSig: rciSig, rciSigRes: rciSigRes, id: id },
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