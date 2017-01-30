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
