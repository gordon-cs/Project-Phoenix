﻿var damagesToDelete = [];

$("#save-button").click(function () {
    save();
    location.reload(true); 
});

$("#next-button").click(function () {
    save();
});
/* Save before the window unloads its resources e.g. reloading, closing browser etc... */
//window.onbeforeunload = function (event) {
//    save(); 
//}

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

/* Receive a photo from the <input> element, add it to the DOM, and save it to the db
/* Helpful link: https://developer.mozilla.org/en-US/docs/Using_files_from_web_applications */
function uploadPhoto() {
    console.log("reached uploadPhoto");
    console.log(this.files);
    window.URL = window.URL || window.webkitURL;
    let photoList = this.files;
    let rciComponentId = this.id.slice(10);
    let fileQuantity = photoList.length;
    let fileType = /^image\//;
    let previewArea = $("#img-preview-" + rciComponentId);
    let modalArea = $("#modal-" + rciComponentId).find(".modal-content");
    if (!fileQuantity) {
        previewArea.innerHTML = "<p>No pictures uploaded</p>";
    }
    else {
        for (let i = 0; i < fileQuantity; i++) {
            let file = photoList[i];
    // Make sure the file is a picture
            if (!fileType.test(file.type)) {
                console.log(file.type);
                alert("Oops! Please select an image file of type .jpg or .png.")
            }
            else if (file.size > 10000000) { // Photo size > 10 MB
                alert("Aw man, we're sorry! That photo is too big. Please use one that is 10 MB or smaller.")
            }
            else {
                console.log("Photo" + i + "size: " + file.size);
                let img = document.createElement("img");
                img.classList.add("uploaded-img");
                img.classList.add("thumbnail");
                img.src = window.URL.createObjectURL(file); // I am not entirely sure how this works
                img.onload = function () {
                    window.URL.revokeObjectURL(this.src);
                }
                img.alt = "Damage Image Thumbnail";
                
                // Add the delete icon and wrap it in div for css purposs
                let $deleteIcon = $("<i>close</i>");
                $deleteIcon.addClass("material-icons");
                $deleteIcon.addClass("delete");
                let $deleteWrapper = $("<div></div>");
                $deleteWrapper.addClass("delete-icon");
                $deleteWrapper.append($deleteIcon);
                
                // Wrap everything in another div for css purposes
                let $wrapperDiv = $("<div></div>");
                $wrapperDiv.addClass("thumbnail-container");
                $wrapperDiv.append(img);
                $wrapperDiv.append($deleteWrapper);
                previewArea.append($wrapperDiv);

                // Now create and add the image for the modal
                let slideImg = document.createElement("img");
                slideImg.classList.add("uploaded-img");
                slideImg.src = window.URL.createObjectURL(file);
                slideImg.onload = function () {
                    window.URL.revokeObjectURL(this.src);
                }
                let $newWrapperDiv = $("<div class='img-slide'></div>");
                $newWrapperDiv.append(slideImg);
                modalArea.append($newWrapperDiv);

                var response = savePhoto(file, rciComponentId, $deleteIcon, img, $newWrapperDiv);
                console.log(response);
            }
        }
    }
}

// Send the uploaded photo to the server via AJAX
function savePhoto(photoFile, fileName, deleteIcon, imgElement, slideDiv) {
    let formData = new FormData();
    formData.append('file', photoFile, fileName);

    $.ajax({
        url: '/RciInput/SavePhoto',
        data: formData,
        method: "POST",
        processData: false,
        contentType: false,
    }).done(function (data) {
        // the ajax call returns the damage id, so here we set the image's id to be its db DamageId
        console.log(data);
        deleteIcon.attr("id", data);
        imgElement.id = "thumbnail-img-" + data; // This looks different b/c imgElement is not passed as a jQuery obj
        slideDiv.attr("id", "img-slide-" + data);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to save that image to the database.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
}

/* Delete a selected photo from the db */
function deletePhoto(damageId) {
    $.ajax({
        url: '/RciInput/DeletePhoto',
        data: { damageId: damageId },
        method: "POST"
    }).done(function (data) {
        console.log("Successfully deleted.");
        // Remove the image from the DOM
        console.log("Damage img: " + $("#thumbnail-img-" + damageId));
        $("#thumbnail-img-" + damageId).closest(".thumbnail-container").remove(); // Remove the thumbnail image, which was the previous element

        $("#img-slide-" + damageId).remove(); // Remove the slide with this image
        //$("#" + damageId).remove();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to delete that image from the database.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });

}

/* Different signature submission methods, distinguished by role */

function ResSigSubmit() {
    var rciSig = "";
    var lacSig = "";
    if ($("#rci-sig").attr("disabled") != "disabled") {
        rciSig = $("#rci-sig").val();
    }
    if ($("#lac-sig").attr("disabled") != "disabled") {
        lacSig = $("#lac-sig").val();
    }
    var id = $("h2[id^='rci-']").first().attr("id").substring(4);

    $.ajax({
        sync: false,
        url: "/RciInput/SaveSigRes",
        data: { rciSig: rciSig, lacSig: lacSig, id: id },
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

function RASigSubmit() {
    var rciSig = "";
    var rciSigRes = "";
    var lacSig = "";
    if ($("#rci-sig").attr("disabled") != "disabled") {
        rciSig = $("#rci-sig").val();
    }
    if ($("#rci-sig-res").attr("disabled") != "disabled") {
        rciSigRes = $("#rci-sig-res").val();
    }
    if ($("#lac-sig").attr("disabled") != "disabled") {
        lacSig = $("#lac-sig").val();
    }
    var id = $("h2[id^='rci-']").first().attr("id").substring(4);

    $.ajax({
        sync: false,
        url: "/RciInput/SaveSigRA",
        data: { rciSig: rciSig, rciSigRes: rciSigRes, lacSig: lacSig, id: id },
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

function RDSigSubmit() {
    var rciSig = "";
    if ($("#rci-sig").attr("disabled") != "disabled") {
        rciSig = $("#rci-sig").val();
    }
    var id = $("h2[id^='rci-']").first().attr("id").substring(4);

    $.ajax({
        sync: false,
        url: "/RciInput/SaveSigRD",
        data: { rciSig: rciSig, id: id },
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


$("#rci-sig-checkbox").click(function () {
    check();
});

function check() {
    if ($("#rci-sig-checkbox").is(":checked")) {
        var sigCheck = 1;
    }
    else {
        var sigCheck = 0;
    }
    var id = $("h2[id^='rci-']").first().attr("id").substring(4);
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


/* Add a div to the right component. This div will contain a :
    <p> element displaying a damage
    <input> hidden element that will be used when submitting the form
    */
function addDamage(componentID) {
    var pElement = "<p class='damage-element new-damage'>" + $("#text-input-" + componentID).val() + "</p><i class='material-icons' onclick='deleteNewDamages(event, this);'>close</i>";
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
// Attach upload photo handler
$("input[id^='dmg-input']").change(uploadPhoto);

// Handler for deleting photos
$(".img-thumbnails").on("click", ".delete", function () {
    console.log($(this));
    deletePhoto($(this).attr("id"));
});


