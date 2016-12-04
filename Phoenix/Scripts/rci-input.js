function addDamage(damageHtmlID) {
    // Add newly inputted damage from textfield to list of damages of the corresponding component
    $("#existing-damages-" + damageHtmlID).append("<li>" + $("#" + damageHtmlID).val() + "<input type='hidden' name=" + damageHtmlID + " value=" + $("#" + damageHtmlID).val() + "/></li>");
    // Empty the textfield of adding damages
    $("#" + damageHtmlID).val("");
}

$(".adding-damages").on("keypress", function (e) {
    var key = e.keyCodd || e.which;
    if (key == 13) {
        e.preventDefault();
        $("#ok-" + $(this).attr("id")).click();
    }
});