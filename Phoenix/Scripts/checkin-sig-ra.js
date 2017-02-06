$("#submit-button").click(function () {
    submit();
    location.reload();
});

/* Submit signatures for both RCI and Life and Conduct Agreement */
function submit() {
    var rciSig = $("#rci-sig").val();
    var id = $("h2").first().attr("id");

    $.ajax({
        url: "/RCIInput/SaveSigRA",
        data: { rciSig: rciSig, id: id },
        method: "POST",
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
            console.log(errorThrown);
        }

    });
}