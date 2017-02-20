$("#submit-button").click(function () {
    submit();
});

/* Submit signatures for both RCI and Life and Conduct Agreement */
function submit() {
    var rciSig = "";
    if ($("#rci-sig").attr("disabled") != "disabled") {
        rciSig = $("#rci-sig").val();
    }
    var id = $("h2[id^='rci-']").first().attr("id").substring(4);

    $.ajax({
        sync: false,
        url: "/RciInput/SaveSigRD",
        data: { rciSig: rciSig, id: id },
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