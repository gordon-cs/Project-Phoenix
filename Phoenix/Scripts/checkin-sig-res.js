$("#submit-button").click(function () {
    submit();
});

/* Submit signatures for both RCI and Life and Conduct Agreement */
function submit() {
    var rciSig = "";
    var lacSig = "";
    if ($("#rci-sig").attr("disabled") != "disabled") {
        rciSig = $("#rci-sig").val();
    }
    if ($("#lac-sig").attr("disabled") != "disabled") {
        lacSig = $("#lac-sig").val();
    }
    var id = $("h2[id^='rci-']").first().attr("id").substring(4);

    $.ajax({
        sync: false,
        url: "/RciInput/SaveSigRes",
        data: { rciSig: rciSig, lacSig: lacSig, id: id },
        method: "POST",
        success: function(data) {
            window.location.href = data;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
            console.log(errorThrown);
        }

    });
}