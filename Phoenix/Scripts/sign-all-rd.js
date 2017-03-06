$("#submit-button").click(function () {
    submit();
})

$("input:checkbox[id^='rci-sig-checkbox-']").click(function () {
    check(($(this).attr("id")).substring(17));
});

function check(id) {
    if ($("#rci-sig-checkbox-"+id).is(":checked")) {
        var sigCheck = 1;
    }
    else {
        var sigCheck = 0;
    }
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