﻿$("#save-button").click(save);

/* Save the damages in the RCI. Can be used in both regular save and auto save */
function save() {
    let rci = {}
    rci.newDamages = [];
    $(".component").each(function (index, element) {
        let componentId = $(element).attr("id");
        $(element).find(".new-damage").each(function (index, element) {
            let damageText = $(element).text();
            let newDamageDescriptions = {};
            newDamageDescriptions["componentId"] = componentId;
            newDamageDescriptions["damage"] = damageText;
            rci.newDamages.push(newDamageDescriptions);
        });
    });

    $.ajax({
        url: "/RCIInput/SaveRCI",
        data: { rci: rci },
        method: "POST",
        success: function (result) {
            location.reload(); // if successfull, reload page.
        }
    });
}

/* Add a div to the right component. This div will contain a :
    <p> element displaying a damage
    <input> hidden element that will be used when submitting the form
    */
function addDamage(componentID) {
    var pElement = "<p class='divAddOn-field new-damage'>" + $("#text-input-" + componentID).val() + "</p><i class='divAddOn-item material-icons' onclick='deleteNewDamages(event, this);'>delete</i>";
    //var inputHiddenElement = "<input type='hidden' name=" + componentID + " value='" + $("#text-input-" + componentID).val() + "'/>";
    var divWrapper = "<div class='divAddOn'></i>" + pElement + "</div>";
    $("#div-list-" + componentID).append(divWrapper);
    console.log(divWrapper);
    console.log($("#text-input-" + componentID).val());
    $("#text-input-" + componentID).val(""); // Empty the textfield of adding damages
}

/* Delete the div wrapper for a damage entry that was created, but not yet saved to the database*/
function deleteNewDamages(event, element) {
    $(element).parent().remove();
}

/* Delete the div wrapper for a damage that has already been saved to the database */
function deleteExistingDamages(event, element, id)
{
    $(element).parent().remove();
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
$(".rci-form").submit(function (event) {
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

