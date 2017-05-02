/* Send a search query to the database
*/
function sendSearch() {
    let sessionsSelected = [];
    let buildingsSelected = [];
    let sessionCode = $("#session-select option:selected");
    if (sessionCode.text() === "All Sessions")
    {
        $("#session-select").children().each(function (index, element) {
            if (index == 0)
            { return; }

            console.log($(element));
            sessionsSelected.push($(element).attr("id"));
        });
    }
    else
    {
        sessionsSelected.push(sessionCode.attr("id"));
    }
    console.log("Session code: " + sessionCode);
    let buildingCode = $("#building-select option:selected");
    if (buildingCode.text() === "All Buildings") {
        $("#building-select").children().each(function (index, element) {
            if (index == 0)
            { return; }

            console.log($(element));
            buildingsSelected.push($(element).text());
        });
    }
    else {
        buildingsSelected.push(buildingCode.text());
    }

    let keyword = $("#search-bar-input").val();

    $.ajax(
        {
            method: "POST",
            url: "/AdminDashboard/SearchRcis",
            data: { sessions: sessionsSelected, buildings: buildingsSelected, keyword: keyword }
        })
    .then(function (result) {
        // first clear out an old search results
        $("#search-results-section").empty();

        // searchResults will be a partial view, which is a chunk of HTML we now want to insert into the DOM
        $("#search-results-section").append(result);
    });

}

//Create a new type of RCI in the XML doc
function createNewType(buildingCode, roomType, buildingCopy) {
    console.log("About to create new type");

    $.ajax(
        {
            method: "POST",
            url: "/AdminDashboard/AddNewRciType",
            data: { buildingCode: buildingCode, roomType: roomType, copyOption: buildingCopy }
        })
    .then(function(result) {
        console.log("Success");
        window.location.href = result;
    })
    .fail(function () {
        console.log("fail");
        alert("It looks like there is no " + roomType + " RCI for the building " + copyOption + ". Are you sure you picked the right options?");
    });
}

// Delete a certain rci type
function deleteRciType(buildingCode, roomType) {
    console.log("About to delete " + buildingCode + roomType);

    $.ajax(
        {
            method: "DELETE",
            url: "/AdminDashboard/DeleteRciType",
            data: {buildingCode: buildingCode, roomType: roomType}
        })
    .then(function () {
        alert("Successfully delete: " + buildingCode + roomType);
    })
}


/**** Register event handlers ****/
$("#search-button").click(sendSearch);

$("#search-bar-input").on("keypress", function (e) {
    var key = e.keyCode || e.which;
    if (key == 13) {
        e.preventDefault();
        sendSearch();
    }
});


$("#add-type-button").click(function () {
    $(".signature-overlay").show();
    $("#signature-container").toggle();
});

$("#cancel-add-type").click(function () {
    $(".signature-overlay").hide();
    $("#signature-container").toggle();
});

$("#create-new-type").click(function () {
    let buildingCode = $("#new-building-code-input").val();
    console.log(buildingCode);

    let roomType = $("#room-type-select option:selected").text().toLocaleLowerCase();
    console.log("Room type: " + roomType);

    let buildingCopy = $("#copy-building-select option:selected").text();

    createNewType(buildingCode, roomType, buildingCopy);
});

// Handler for deleting photos
$(".rci-types-flex").on("click", ".delete", function () {
    console.log($(this));

    
    // The delete button has an id of the form "delete-WIL-common", so we split it up to get building code and room type
    let splitArray = $(this).attr("id").split("-");
    
    let buildingCode = splitArray[1];
    let roomType = splitArray[2];

    deleteRciType(buildingCode, roomType);
});