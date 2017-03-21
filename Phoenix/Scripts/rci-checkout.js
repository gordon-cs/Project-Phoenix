﻿/* Add a fine
  * To differentiate between the common areas and individual views, we check whether fineOwnerName 
  * is defined. That refers to what is curretnly selected in the dropdown. 
  * Since there is no dropdown for indivudial views, it won't be defined 
  * It seems hacky, but I couldn't think of an easier way that wouldn't require two seperate js files
  */
function addFine(componentID) {
    let $finesArea = $("#fine-list-" + componentID);
    let fineText = $("#text-input-" + componentID).val();
    let fineAmount = Number($("#fine-amount-input-" + componentID).val());
    let fineOwnerName = $("#fine-owner-" + componentID + " option:selected").text(); // Will only be defined for common areas.
    let fineOwnerID = $("#fine-owner-" + componentID + " option:selected").val();

    if (fineOwnerName && fineOwnerID === "0") { // Split the charge among everyone.
        let numberOfPeople = $("#fine-owner-" + componentID).children().length - 1;
        $("#fine-owner-" + componentID).children().each(function (index, element) {

            let $element = $(element);
            if ($element.val() === "0") { return; }

            addFineToDb(componentID, fineText, (fineAmount / numberOfPeople).toFixed(2), $element.val())
            .then(function (data) {
                let div = $("<div />", { "class": "divAddOn" });
                div.append("<p class='divAddOn-field'>" + fineText + "</p>");
                div.append("<span class='divAddOn-item'>$</span>");
                div.append("<p class=\"divAddOn-item\">" + (fineAmount / numberOfPeople).toFixed(2) + "</p>");
                div.append("<p  class='divAddOn-item'>" + $element.text() + "</p>");
                div.append("<i class='divAddOn-item material-icons' onclick='deleteFine(event, this,"+data+");'>delete</i>");
                console.log(div);
                $finesArea.append(div);
            });
        });        
    }
    else if (fineOwnerName) { // A person was selected
        addFineToDb(componentID, fineText, fineAmount, fineOwnerID)
        .then(function (data) {
            let div = $("<div />", { "class": "divAddOn" });
            div.append("<p class='divAddOn-field'>" + fineText + "</p>");
            div.append("<span class='divAddOn-item'>$</span>");
            div.append("<p class=\"divAddOn-item\">" + fineAmount.toFixed(2) + "</p>");
            div.append("<p class='divAddOn-item'>" + fineOwnerName + "</p>")
            div.append("<i class='divAddOn-item material-icons' onclick='deleteFine(event, this, "+data+");'>delete</i>");
            $finesArea.append(div);
        });
    }
    else { // fineOwnerName is not defined, which means this is an individual checkout
        let gordonID = $(".view").attr("data");
        addFineToDb(componentID, fineText, fineAmount, gordonID )
        .then(function (data) {
            let div = $("<div />", { "class": "divAddOn" });
            div.append("<p class='divAddOn-field'>" + fineText + "</p>");
            div.append("<span class='divAddOn-item'>$</span>");
            div.append("<p class=\"divAddOn-item\">" + fineAmount.toFixed(2) + "</p>");
            div.append("<i class='divAddOn-item material-icons' onclick='deleteFine(event, this, "+data+");'>delete</i>");
            $finesArea.append(div);
        });
    }
    $("#text-input-" + componentID).val(""); // Empty the textfield of adding fines
    $("#fine-amount-input-" + componentID).val("");
}

/* Delete a fine */
function deleteFine(event, element, id) {
    removeFineFromDb(id)
    .then(function () {
        $(element).parent().remove();
    });   
}

/* Helper method to make the ajax call and return the promise */
function addFineToDb(componentId, reason, amount, owner) {
    return $.ajax({
        method: "POST",
        url: "/RciCheckout/AddFine",
        data: {componentId: componentId, fineReason: reason, fineAmount: amount, fineOwner: owner}
    });
}

/* Helper method to make the ajax call and return the promise */
function removeFineFromDb(fineId) {
    return $.ajax({
        method: "POST",
        url: "/RciCheckout/RemoveFine",
        data: {fineId: fineId}
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

// Open the signature overlay
$("#next-button").click(function () {
    $(".signature-overlay").show();
    $("#signature-container").toggle();
});

