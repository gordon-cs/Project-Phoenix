/***********************************************/
/* TABLE OF CONTENTS:
 *
 * 1.................. DivListDropDown
 * 2.................. Drawer Menu
 * 3.................. Search Bar
 * 4.................. Damage Image Slides Modal
 * 5.................. Signature overlay
 * 6.................. Select all
 *
/***********************************************/




/* DivListDropDown
 *
 * To use this, give any DOM element a "div-list-toggle" class. This will be
 * the trigger.
 * Any sibling divs to the trigger element will have their visibility toggled
 * when the trigger is clicked.
 */


// The trigger element
var trigger = ".div-list-toggle";

// Constructor function
function DivListDropDown() {
    // DivListDropDown objects will all use this function
    this.toggle = function() {
        this.elementsToToggle.toggle(200);
    };

}

$(trigger).click( function() {
    // create a new DivListDropDown objects
    let divlist = new DivListDropDown();
    // set elementsToToggle to the div siblings of the trigger.
    divlist.elementsToToggle = $(this).siblings("div");
    divlist.toggle();
});


/*
 * Menu drawer
 * Menu that is toggled by clicking on the view-drawer div in the header.
 * Menu is originally hidden. The toggle jquery function toggles an element's visiblity.
 */
$(".dropdown").click(function () {
    $(".dropdown-menu").toggle(200);   
});


/* Search bar
 * Search filter functionality
 * Explanation:
 * We use the "keyup" event handler to listen for anytime a new character is entered.
 * When a new character is entered, we put the new string we want to match in textToMatch.
 * We start by showing everything, then we filter.
 * For each search-field, the filter finds all the <p> tags inside the search-field and puts 
 * them together. This usually results in strings like "Jane        Smith" with multiple whitespace
 * inside. We use a regular expression to remove that whitespace.
 */

$(window).load(function () {
    // If there is a search bar, apply whatever is in it
    var searchBar = document.querySelector(".search-bar");
    if (searchBar) {
        search(searchBar);
    }
})

$(".search-bar").keyup(function () {
    search(this);
});

function search(searchBarElement) {
    let textToMatch = $(searchBarElement).find("input[type='text']").val();
    textToMatch = textToMatch.toLowerCase().trim();
    let content = "";

    $(".search-field").parent().parent().show();

    $(".search-field").filter(function () {

        // For a quick intro to regular expressions in javascript, see
        /* https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_Expressions */
        let regExpMatchWhitespace = /\s+/g;
        content = $(this).find("p").text().replace(regExpMatchWhitespace, " ").toLowerCase();

        let result = content.indexOf(textToMatch);
        if (result == -1) {
            return true;
        }
        else {
            return false;
        }
    }).parent().parent().hide()
}

/*** Admin dashboard DOM manipulation **/

// Find RCI button
$("#find-rci-button").click(function () {
    $("#update-furniture-view").hide();
    $("#find-rci-view").toggle()
});

// Update furniture button
$("#update-furniture-button").click(function () {
    $("#find-rci-view").hide();
    $("#update-furniture-view").toggle();
});


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


/* Signature overlay handlers
*/
$("#next-button").click(function () {
    $(".signature-overlay").show();
    $("#signature-container").toggle();
});

$(".signature-overlay").click(function () {
    $(".signature-overlay").hide();
    $("#signature-container").toggle();
});

$(".signature-overlay-cancel").click(function () {
    $(".signature-overlay").hide();
    $("#signature-container").toggle();
});


/********* Register handlers **********/

$(".img-thumbnails").on("click", ".thumbnail", function () {
    let componentID = $(this).closest(".img-thumbnails").attr("id").substring(12);
    // Count up all the previous thumbnail images to know what to set the slide index to for the modal
    let newIndex = $(this).parent().prevAll().length;
    openModal(componentID, newIndex);
});

$(".forward").click(function () {
    let modalID = $(this).closest(".img-modal").attr("id");
    plusSlides(1, modalID);
});
$(".backward").click(function () {
    let modalID = $(this).closest(".img-modal").attr("id");
    plusSlides(-1, modalID);
});
$(".material-icons.clear").click(function () {
    let modalID = $(this).closest(".img-modal").attr("id");
    console.log(modalID);
    closeModal(modalID);
});


/* Select all checkboxes */

$(".select-all").click(function () {
    $this = $(this);
    if ($this.text() === "Select all") {
        $this.text("Unselect all");
        $("input[type=checkbox]").prop("checked", true);
    }
    else {
        $this.text("Select all");
        $("input[type=checkbox]").prop("checked", false);
    }
});