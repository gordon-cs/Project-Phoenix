/* Send a search query to the database
*/
function sendSearch() {
    let sessionCode = $("#session-select option:selected").text();
    if (sessionCode === "Session")
        sessionCode = null; // attempt to handle no option selected. This passes an empty string
    let buildingCode = $("#building-select option:selected").text();
    if (buildingCode === "Building")
        buildingCode = null; // attempt to handle no option selected. This passes an empty string
    let keyword = $("#search-bar-input").val();

    let searchResults = $.ajax(
        {
            method: "GET",
            url: "/AdminDashboard/SearchRcis",
            data: { building: buildingCode, session: sessionCode, keyword: keyword}
        });

    // first clear out an old search results
    $("#search-results-section").empty();

    // searchResults will be a partial view, which is a chunk of HTML we now want to insert into the DOM
    $("#search-results-section").append(searchResults);

}

/**** Register event handlers ****/
$("#search-button").click(sendSearch);
