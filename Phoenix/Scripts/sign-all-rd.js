$("#submit-button").click(function () {
    submit();
})

function submit() {
    var rciSig = "";
    if ($("#rci-sig").attr("disabled") != "disabled") {
        rciSig = $("#rci-sig").val();
    }

    $.ajax({
        sync: false,
        url: "/RciInput/SubmitSignAllRD",
        data: { rciSig: rciSig},
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