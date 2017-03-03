/****** Modal functions ******/

/*
 * Open the picture modal
 */
function openModal(componentID, slideNum) {
    $("#modal-" + componentID).show();
    showSlides(slideNum, "modal-" + componentID);
}

/*
 * Close the picture modal
 */
function closeModal(modalID) {
    $("#" + modalID).hide();
}

/*
 * Show the slides in the modal
 */
var slideIndex = 0;
// This function increments the slide index (a global variable) by a given value
function plusSlides(n, modalId) {
    showSlides(slideIndex += n, modalId);
}
// Display the slide, based on the selected image
function showSlides(slideNumber, modalId) {
    let slides = $("#" + modalId).find(".img-slide");
    if (slideNumber >= slides.length) {
        slideIndex = 0;
    }
    else if (slideNumber < 0) {
        slideIndex = slides.length - 1;
    }
    else {
        slideIndex = slideNumber;
    }
    for (var i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    slides[slideIndex].style.display = "block";
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



/* Register Handers */


// Attach modal handlers (reference: https://www.w3schools.com/howto/howto_js_lightbox.asp)

// For all the thumbnail areas, attach the modal opener to each of its thumbnail images
$(".img-thumbnails").each(function (index, element) {
    let componentID = $(this).attr("id").substring(12);
    $(this).find(".thumbnail").each(function (index, element) {
        $(this).click(function () {
            openModal(componentID, index)
        });
    });
});

$(".material-icons.clear").click(function () {
    let modalID = $(this).closest(".img-modal").attr("id");
    console.log(modalID);
    closeModal(modalID);
});

$(".forward").click(function () {
    let modalID = $(this).closest(".img-modal").attr("id");
    plusSlides(1, modalID);
});
$(".backward").click(function () {
    let modalID = $(this).closest(".img-modal").attr("id");
    plusSlides(-1, modalID);
});

