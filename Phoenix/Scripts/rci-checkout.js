var finesToDelete = [];

$("#save-button").click(function () {
    save().then(function () {
        location.reload(true);
    });
});

$("#next-button").click(function () {
    let $this = $(this);
    save().then(function () {
        location.href = $this.attr("data");
    });
});

/* Save the fines */
function save() {
    let rci = {}
    rci.newFines = [];
    rci.finesToDelete = finesToDelete;
    $(".component").each(function (index, element) {
        let componentId = $(element).attr("id");
        $(element).find(".new-fine").each(function (index, element) {
            let fineText = $(element).text();
            let fineAmount = $(element).siblings(".new-fine-amount").first().text();
            let fineOwner = $(element).siblings(".new-fine-owner").attr("data");
            let newFine = {};
            newFine["componentId"] = componentId;
            newFine["fineReason"] = fineText;
            newFine["fineAmount"] = fineAmount;
            newFine["fineOwner"] = $(".view").attr("data") || fineOwner;
            rci.newFines.push(newFine);
        });
    });

    finesToDelete = [];

    return $.ajax({
        url: "/RciCheckout/SaveRci",
        data: { rci: rci },
        method: "POST"
    });
}

function addFine(componentID) {
    let $finesArea = $("#fine-list-" + componentID);
    let fineText = $("#text-input-" + componentID).val();
    let fineAmount = Number($("#fine-amount-input-" + componentID).val());
    let fineOwnerName = $("#fine-owner-" + componentID + " option:selected").text(); // Will only be defined for common areas.
    let fineOwnerID = $("#fine-owner-" + componentID + " option:selected").val();

    if (fineOwnerID === "0") { // Split the charge among everyone.
        let numberOfPeople = $("#fine-owner-" + componentID).children().length - 1;
        $("#fine-owner-" + componentID).children().each(function (index, element) {
            let $element = $(element);
            if ($element.val() === "0") { return; }

            let div = $("<div />", { "class": "divAddOn" });
            div.append("<p class='divAddOn-field new-fine'>" + fineText + "</p>");
            div.append("<span class='divAddOn-item'>$</span>");
            div.append("<p class=\"divAddOn-item new-fine-amount\">" + (fineAmount / numberOfPeople).toFixed(2) + "</p>");
            div.append("<span data='" + $element.val() + "' class='divAddOn-item new-fine-owner'>" + $element.text() + "</span>");
            div.append("<i class='divAddOn-item material-icons' onclick='deleteNewFines(event, this);'>delete</i>");
            console.log(div);
            $finesArea.append(div);
        });        
    }
    else if (fineOwnerName) { // A person was selected
        let div = $("<div />", { "class": "divAddOn" });
        div.append("<p class='divAddOn-field new-fine'>" + fineText + "</p>");
        div.append("<span class='divAddOn-item'>$</span>");
        div.append("<p class=\"divAddOn-item new-fine-amount\">" + fineAmount.toFixed(2) + "</p>");
        div.append("<span data='" + fineOwnerID + "' class='divAddOn-item new-fine-owner'>" + fineOwnerName + "</span>")
        div.append("<i class='divAddOn-item material-icons' onclick='deleteNewFines(event, this);'>delete</i>");
        $finesArea.append(div);
    }
    else { // fineOwnerName is not defined, which means this is an individual checkout
        let div = $("<div />", { "class": "divAddOn" });
        div.append("<p class='divAddOn-field new-fine'>" + fineText + "</p>");
        div.append("<span class='divAddOn-item'>$</span>");
        div.append("<p class=\"divAddOn-item new-fine-amount\">" + fineAmount.toFixed(2) + "</p>");
        div.append("<i class='divAddOn-item material-icons' onclick='deleteNewFines(event, this);'>delete</i>");
        $finesArea.append(div);
    }
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
    finesToDelete.push(id);
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

