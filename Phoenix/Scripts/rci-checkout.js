$("#save-button").click(function () {
    location.reload(true);
});

/* Save before the window unloads its resources e.g. reloading, closing browser etc... */
window.onbeforeunload = function (event) {
    save();
    event.returnValue = null; // Don't open a popup window. On some browsers, this might be the default behavior
    return null;
}


/* Save the fines */
function save() {
    let rci = {}
    rci.newFines = [];
    $(".component").each(function (index, element) {
        let componentId = $(element).attr("id");
        $(element).find(".new-fine").each(function (index, element) {
            let fineText = $(element).text();
            let fineAmount = $(element).siblings(".new-fine-amount").first().text();
            let newFine = {};
            newFine["componentId"] = componentId;
            newFine["fineReason"] = fineText;
            newFine["fineAmount"] = fineAmount;
            rci.newFines.push(newFine);
        });
    });

    $.ajax({
        url: "/RCICheckout/SaveRCI",
        data: { rci: rci },
        method: "POST",
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
            console.log(errorThrown);
        }

    });
}

function addFine(componentID) {
    let fineText = $("#text-input-" + componentID).val();
    let fineAmount = $("#fine-amount-input-" + componentID).val();
    let fineTextElement = "<p class='divAddOn-field new-fine'>" + fineText + "</p>";
    let fineAmountElement = "<p class=\"divAddOn-item new-fine-amount\">" + fineAmount + "</p>";
    let fineIcon = "<i class='divAddOn-item material-icons' onclick='deleteNewFines(event, this);'>delete</i>";
    let divWrapper = "<div class='divAddOn'>" + fineTextElement + fineAmountElement + fineIcon + "</div>";
    $("#div-list-" + componentID).append(divWrapper);
    $("#text-input-" + componentID).val(""); // Empty the textfield of adding fines
    $("#fine-amount-input-" + componentID).val("");
}

/* Delete the div wrapper for a fine entry that was created, but not yet saved to the database*/
function deleteNewFines(event, element) {
    $(element).parent().remove();
}

/* Delete the div wrapper for a fine that has already been saved to the database */
function deleteExistingFines(event, element, id) {
    $(element).parent().remove();
    $.ajax({
        url: "/RCICheckout/QueueFineForDelete",
        data: { id: id },
        method: "POST"
    });
}




/* Register Handers */


// Handler to allow users to add damages by simply pressing the enter key
$(".adding-fines").on("keypress", function (e) {
    var key = e.keyCodd || e.which;
    if (key == 13) {
        e.preventDefault();
        $("#add-" + $(this).attr("id").substring(11)).click();
    }
});

