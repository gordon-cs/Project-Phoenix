var damagesToDelete = [];

$("#save-button").click(function () {
    location.reload(true); // No need to save, since save() is called in window.onbeforeunload
});
/* Save before the window unloads its resources e.g. reloading, closing browser etc... */
window.onbeforeunload = function (event) {
    save(); 
}

/* Save the damages in the RCI. Can be used in both regular save and auto save */
function save() {
    let rci = {}
    rci.newDamages = [];
    rci.damagesToDelete = damagesToDelete;
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
    // clear the array of delete elements
    damagesToDelete = [];
    $.ajax({
        url: "/RciInput/SaveRci",
        data: { rci: rci },
        method: "POST",
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
            console.log(errorThrown);
        }

    });
}

// Helpful link: https://developer.mozilla.org/en-US/docs/Using_files_from_web_applications
function uploadPhoto() {
    console.log("reached uploadPhoto");
    console.log(this.files);
    window.URL = window.URL || window.webkitURL;
    let photoList = this.files;
    let fileQuantity = photoList.length;
    let fileType = /^image\//;
    let previewArea = $(".img-preview");
    if (!fileQuantity) {
        previewArea.innerHTML = "<p>No pictures uploaded</p>";
    }
    else {
        for (let i = 0; i < fileQuantity; i++) {
            let file = photoList[i];
            if (!fileType.test(file.type)) {
                console.log(file.type);
                alert("Oops! Please select an image file of type .jpg or .png.")
            }
            else {
                console.log("Photo" + i + "size: " + file.size);
                let img = document.createElement("img");
                img.classList.add("uploaded-img");
                img.src = window.URL.createObjectURL(file); // I am not entirely sure how this works
                img.width = 60; // Make photo small for thumbnail
                img.onload = function () {
                    window.URL.revokeObjectURL(this.src);
                }
                img.alt = file.name;
                previewArea.append(img);

            }
        }
    }

}

/* Add a div to the right component. This div will contain a :
    <p> element displaying a damage
    <input> hidden element that will be used when submitting the form
    */
function addDamage(componentID) {
    var pElement = "<p class='divAddOn-field new-damage'>" + $("#text-input-" + componentID).val() + "</p><i class='divAddOn-item material-icons' onclick='deleteNewDamages(event, this);'>delete</i>";
    //var inputHiddenElement = "<input type='hidden' name=" + componentID + " value='" + $("#text-input-" + componentID).val() + "'/>";
    var divWrapper = "<div class='divAddOn'>" + pElement + "</div>";
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
    damagesToDelete.push(id);
}



/* Register Handers */


// Handler to allow users to add damages by simply pressing the enter key
$(".adding-damages").on("keypress", function (e) {
    var key = e.keyCodd || e.which;
    if (key == 13) {
        e.preventDefault();
        $("#add-" + $(this).attr("id").substring(11)).click();
    }
});

$("input[id^='dmg-input'").change(uploadPhoto
    //function () {
//    console.log(this);
//    console.log(this.files);
//    console.log(this.files[0].size);
//    //let photoFile = this.file[0];
//    console.log(photoFile.size);
//    uploadPhoto(photoFile);
//}
);

