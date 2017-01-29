/* Logout functionality */

$(".logout-button").click(function () {
    $.ajax({
        url: document.location.origin + "/Logout",
        method: "GET",
        success: function () {
            window.location.href = document.location.origin + "/Login";
        },
        error: function () {
            console.log("Error loging out");
        }
    });
});