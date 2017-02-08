/***********************************************/
/* TABLE OF CONTENTS:
 *
 * 1.................. DivListDropDown
 * 2.................. Drawer Menu
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
$(".view-drawer").click(function () {
    $(".drawer-menu").toggle(200);   
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

$(".search-bar").keyup(function () {
    let textToMatch = $(this).find("input[type='text']").val();
    textToMatch = textToMatch.toLowerCase();
    let content = "";

    $(".search-field").parent().show();

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
    }).parent().hide()
});
