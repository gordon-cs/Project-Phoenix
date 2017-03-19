var damagesToDelete = [];

$("#save-button").click(function () {
    save();
    location.reload(true); 
});

$("#next-button").click(function () {
    save();
});

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

/****** Modal functions ******/

///*
// * Open the picture modal
// */
//function openModal(componentID, slideNum) {
//    $("#modal-" + componentID).show();
//    showSlides(slideNum, "modal-" + componentID);
//}

///*
// * Close the picture modal
// */
//function closeModal(modalID) {
//    $("#" + modalID).hide();
//}

///*
// * Show the slides in the modal
// */
//var slideIndex = 0;
//// This function increments the slide index (a global variable) by a given value
//function plusSlides(n, modalId) {
//    showSlides(slideIndex += n, modalId);
//}
//// Display the slide, based on the selected image
//function showSlides(slideNumber, modalId) {
//    let slides = $("#" + modalId).find(".img-slide");
//    if (slideNumber >= slides.length)
//    {
//        slideIndex = 0;
//    }
//    else if (slideNumber < 0) {
//        slideIndex = slides.length - 1;
//    }
//    else {
//        slideIndex = slideNumber;
//    }
//    for (var i = 0; i < slides.length; i++) {
//        slides[i].style.display = "none";
//    }
//    slides[slideIndex].style.display = "block";
//}


/* Add a div to the right component. This div will contain a :
    <p> element displaying a damage
    <input> hidden element that will be used when submitting the form
    */
function addDamage(componentID) {
    var damageDescription = $("#text-input-" + componentID).val();
    console.log(componentID);
    console.log(damageDescription);

    $.ajax({
        url: '/RciInput/SaveDamage',
        data: { componentID: componentID, damageDescription: damageDescription },
        method: "POST"
    }).done(function (data) {
        // the ajax call returns the damage id
        console.log(data);
        var pElement = "<p class='divAddOn-field new-damage'>" + $("#text-input-" + componentID).val() + "</p><i class='divAddOn-item material-icons' " + "id='" + data + "' onclick='deleteDamage(event, this);'>delete</i>";
        //var inputHiddenElement = "<input type='hidden' name=" + componentID + " value='" + $("#text-input-" + componentID).val() + "'/>";
        var divWrapper = "<div class='divAddOn'>" + pElement + "</div>";
        $("#div-list-" + componentID).append(divWrapper);
        console.log(divWrapper);
        console.log($("#text-input-" + componentID).val());
        $("#text-input-" + componentID).val(""); // Empty the textfield of adding damages
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to save that damage to the database.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
}

function deleteDamage(event, element)
{
    var id = $(element).attr("id");
    $(element).parent().remove();
    console.log(id);
    $.ajax({
        url: '/RciInput/DeleteDamage',
        data: { damageID: id },
        method: "POST"
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to save that damage to the database.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
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

// Attach modal handlers (reference: https://www.w3schools.com/howto/howto_js_lightbox.asp)

// For all the thumbnail areas, attach the modal opener to each of its thumbnail images

//$(".img-thumbnails").on("click", ".thumbnail", function () {
//    let componentID = $(this).closest(".img-thumbnails").attr("id").substring(12);
//    // Count up all the previous thumbnail images to know what to set the slide index to for the modal
//    let newIndex = $(this).parent().prevAll().length;
//    openModal(componentID, newIndex);
// });


//$(".material-icons.clear").click(function () {
//    let modalID = $(this).closest(".img-modal").attr("id");
//    console.log(modalID);
//    closeModal(modalID);
//});

//$(".forward").click(function () {
//    let modalID = $(this).closest(".img-modal").attr("id");
//    plusSlides(1, modalID);
//});
//$(".backward").click(function () {
//    let modalID = $(this).closest(".img-modal").attr("id");
//    plusSlides(-1, modalID);
//});


