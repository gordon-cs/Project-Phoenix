function deleteComponent(element) {
    var component = $(element).parent().parent();
    var componentID = $(".component").index(component);
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4),10);
    $.ajax({
        url:'/ManageRciComponent/DeleteComponent',
        data: {componentID: componentID, rciID: rciID},
        method: "POST"
    }).done(function () {
        component.remove();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to delete that furniture.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
};

function startEdit(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("span")[0]).hide();
    $($(parentElement).children("i")[0]).hide();
    $($(parentElement).children("input")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[1]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[2]).attr("style", "display: inline !important");
}

function saveEdit(element) {
    var parentElement = $(element).parent();
    var component = $(parentElement).parent().parent();
    var componentID = $(".component").index(component);
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4), 10);
    var newDescription = $($(parentElement).children("input")[0]).val();
    $.ajax({
        url: '/ManageRciComponent/EditComponentDescription',
        data: { componentID: componentID, rciID: rciID, newDescription: newDescription },
        method: "POST"
    }).done(function () {
        $($(parentElement).children("span")[0]).html(newDescription);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to edit that furniture's description.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
    $($(parentElement).children("span")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("input")[0]).hide();
    $($(parentElement).children("i")[1]).hide();
    $($(parentElement).children("i")[2]).hide();
}

function cancelEdit(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("input")[0]).val($($(parentElement).children("span")[0]).html());
    $($(parentElement).children("span")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("input")[0]).hide();
    $($(parentElement).children("i")[1]).hide();
    $($(parentElement).children("i")[2]).hide();
}

function startAddBuilding(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("i")[0]).hide();
    $($(parentElement).children("input")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[1]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[2]).attr("style", "display: inline !important");
}

function saveAddBuilding(element) {
    var parentElement = $(element).parent();
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4), 10);
    var newBuilding = $($(parentElement).children("input")[0]).val();
    $.ajax({
        url: '/ManageRciComponent/SaveAddBuilding',
        data: { rciID: rciID, newBuilding: newBuilding },
        method: "POST"
    }).done(function () {
        $('<span class="building-name">' + newBuilding.toUpperCase() + '<i class="material-icons">clear</i></span>').insertBefore("#start-add-building");
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to edit that furniture's description.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
    $($(parentElement).children("input")[0]).val("");
    $($(parentElement).children("i")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("input")[0]).hide();
    $($(parentElement).children("i")[1]).hide();
    $($(parentElement).children("i")[2]).hide();
}

function cancelAddBuilding(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("input")[0]).val("");
    $($(parentElement).children("i")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("input")[0]).hide();
    $($(parentElement).children("i")[1]).hide();
    $($(parentElement).children("i")[2]).hide();
}

function deleteBuilding(element) {
    var building = $(element).ignore("i").text();
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4), 10);
    $.ajax({
        url: '/ManageRciComponent/DeleteBuilding',
        data: { rciID: rciID, building: building },
        method: "POST"
    }).done(function () {
        $(element).remove();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to delete that furniture.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
}

$.fn.ignore = function (sel) {
    return this.clone().find(sel || ">*").remove().end();
}