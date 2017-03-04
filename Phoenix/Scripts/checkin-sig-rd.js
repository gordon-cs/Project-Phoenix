$(document).ready(function () {
    $("#submit-button").click(function () {
        submit();
    });

    $("#rci-sig-checkbox").click(function () {
        check();
    });
})


function check() {
    if ($("#rci-sig-checkbox").is(":checked")) {
        var sigCheck = 1;
    }
    else {
        var sigCheck = 0;
    }
    var id = $("h2[id^='rci-']").first().attr("id").substring(4);
    $.ajax({
        sync: false,
        url: "/RciInput/CheckSigRD",
        data: { sigCheck: sigCheck, id: id },
        method: "POST",
        /*success: function (data) {
            window.location.href = data;
        },*/
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
            console.log(errorThrown);
        }
    })
}

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