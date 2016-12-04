function addDamage(damageHtmlID) {
    // Add newly inputted damage from textfield to list of damages of the corresponding component
    $("#new-damages-" + damageHtmlID).append("<p>" + $("#" + damageHtmlID).val() + " | <a onclick='deleteNewDamages(event,this);'>Delete</a> </p>")
        .append("<input type='hidden' name=" + damageHtmlID + " value='" + $("#" + damageHtmlID).val() + "'/>");
    // Empty the textfield of adding damages
    $("#" + damageHtmlID).val("");
}

function deleteNewDamages(event, element) {
    event.preventDefault();
    $(element).parent().remove();
}

$(".adding-damages").on("keypress", function (e) {
    var key = e.keyCodd || e.which;
    if (key == 13) {
        e.preventDefault();
        $("#ok-" + $(this).attr("id")).click();
    }
});