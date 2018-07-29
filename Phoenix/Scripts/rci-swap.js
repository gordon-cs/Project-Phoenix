$("#rci-swap-button").click(swapRcis);


function swapRcis() {
    var firstRciId = 0;
    var secondRciId = 0;

    firstRciId = $("#first-rci").val();
    secondRciId = $("#second-rci").val();

    $.ajax(
        {
            method: "POST",
            url: "/Dashboard/SwapRcis",
            data: { firstRciId: firstRciId, secondRciId: secondRciId }
        })
    .then(function (result) {
        // first clear out an old search results
        $("#search-results-section").empty();

        // searchResults will be a partial view, which is a chunk of HTML we now want to insert into the DOM
        $("#search-results-section").append(result);
    });

}