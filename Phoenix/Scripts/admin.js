/* Send a search query to the database
*/
function sendSearch() {
    var sessionsSelected = [];
    var buildingsSelected = [];
    var sessionCode = $("#session-select option:selected");
    if (sessionCode.text() === "All Sessions")
    {
        $("#session-select").children().each(function (index, element) {
            if (index == 0) { return; }

            sessionsSelected.push($(element).attr("id"));
        });
    }
    else
    {
        sessionsSelected.push(sessionCode.attr("id"));
    }

    var buildingCode = $("#building-select option:selected");
    if (buildingCode.text() === "All Buildings") {
        $("#building-select").children().each(function (index, element) {
            if (index == 0) { return; }

            buildingsSelected.push($(element).attr("id"));
        });
    }
    else {
        buildingsSelected.push(buildingCode.attr("id"));
    }

    var keyword = $("#search-bar-input").val();

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


/**** Register event handlers ****/
$(window).load(sendSearch);
$("#search-button").click(sendSearch);

$("#search-bar-input").on("keypress", function (e) {
    var key = e.keyCode || e.which;
    if (key == 13) {
        e.preventDefault();
        sendSearch();
    }
});