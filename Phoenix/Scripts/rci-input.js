/* Add a div to the right component. This div will contain a :
    <p> element displaying a damage
    <input> hidden element that will be used when submitting the form
    */
function addDamage(componentID) {
    var pElement = "<p>" + $("#text-input-" + componentID).val() + " | <a onclick='deleteNewDamages(event,this);'>Delete</a></p>";
    var inputHiddenElement = "<input type='hidden' name=" + componentID + " value='" + $("#text-input-" + componentID).val() + "'/>";
    var divWrapper = "<div>" + pElement + inputHiddenElement + "</div>";
    $("#new-damages-" + componentID).append(divWrapper);
        
    
    $("#text-input-" + componentID).val(""); // Empty the textfield of adding damages
}

/* Delete the div wrapper for a damage entry that was created, but not yet saved to the database*/
function deleteNewDamages(event, element) {
    $(element).parent().parent().remove();
}

/* Delete the div wrapper for a damage that has already been saved to the database */
function deleteExistingDamages(event, element, id)
{
    $(element).parent().parent().remove();
    $.ajax({
        url: "/RCIInput/QueueDamageForDelete",
        data: { id: id },
        method: "POST"
    });
}




/* Register Handers */


// Handler to allow users to add damages by simply pressing the enter key
$(".adding-damages").on("keypress", function (e) {
    var key = e.keyCodd || e.which;
    if (key == 13) {
        e.preventDefault();
        $("#submit-" + $(this).attr("id")).click();
    }
});


// Handler to specify the behavior of the submit button
$("form").submit(function (event) {
    event.preventDefault(); // Stop the form from submitting normally
    // Make an ajax call instead
    $.ajax({
        url: "/RCIInput/SaveRCI",
        data: { rci: $(this).serializeArray() },
        method: "POST",
        success: function (result) {
            location.reload(); // if successfull, reload page.
        }
    });
    
});