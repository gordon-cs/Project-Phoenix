$("#submit-button").click(function () {
    submit();
});

$("input:checkbox[id^='rci-sig-checkbox-']").click(function () {
    check(($(this).attr("id")).substring(17));
});

function check(rciId) {
    var flag = 0;

    if ($("#rci-sig-checkbox-" + rciId).is(":checked")) {
        flag = 1;
    }

    $.ajax({
        sync: false,
        url: "/RciInput/CheckSigRD",
        data: { queueRciFlag: flag, rciId: rciId },
        method: "POST",
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
            console.log(errorThrown);
        }
    });
}

function submit() {
    var rciSig = "";
    if ($("#rci-sig").attr("disabled") !== "disabled") {
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