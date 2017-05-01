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

function startEditCost(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("b")[0]).hide();
    $($(parentElement).children("span")[0]).hide();
    $($(parentElement).children("i")[0]).hide();
    $($(parentElement).children("i")[1]).hide();
    $($(parentElement).children("input")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("input")[1]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[2]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[3]).attr("style", "display: inline !important");
}

function saveEditCost(element) {
    var parentElement = $(element).parent();
    var component = $(parentElement).parent().parent().parent();
    var componentID = $(".component").index(component);
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4), 10);
    var oldCostName = $($(parentElement).children("b")[0]).html();
    var oldCostApproxCost = $($(parentElement).children("span")[0]).html();
    var newCostName = $($(parentElement).children("input")[0]).val();
    var newCostApproxCost = $($(parentElement).children("input")[1]).val();
    $.ajax({
        url: '/ManageRciComponent/EditCost',
        data: {
            componentID: componentID,
            rciID: rciID,
            oldCostName: oldCostName,
            oldCostApproxCost: oldCostApproxCost,
            newCostName: newCostName,
            newCostApproxCost: newCostApproxCost
        },
        method: "POST"
    }).done(function () {
        $($(parentElement).children("b")[0]).html(newCostName);
        $($(parentElement).children("span")[0]).html(newCostApproxCost);
        $($(parentElement).children("b")[0]).attr("style", "display: inline !important");
        $($(parentElement).children("span")[0]).attr("style", "display: inline !important");
        $($(parentElement).children("i")[0]).attr("style", "display: inline !important");
        $($(parentElement).children("i")[1]).attr("style", "display: inline !important");
        $($(parentElement).children("input")[0]).hide();
        $($(parentElement).children("input")[1]).hide();
        $($(parentElement).children("i")[2]).hide();
        $($(parentElement).children("i")[3]).hide();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to edit that cost.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
}

function cancelEditCost(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("input")[0]).val($($(parentElement).children("b")[0]).html());
    $($(parentElement).children("input")[1]).val($($(parentElement).children("span")[0]).html());
    $($(parentElement).children("b")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("span")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[1]).attr("style", "display: inline !important");
    $($(parentElement).children("input")[0]).hide();
    $($(parentElement).children("input")[1]).hide();
    $($(parentElement).children("i")[2]).hide();
    $($(parentElement).children("i")[3]).hide();
}

function deleteCost(element) {
    var parentElement = $(element).parent();
    var component = $(parentElement).parent().parent().parent();
    var componentID = $(".component").index(component);
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4), 10);
    var name = $($(parentElement).children("b")[0]).html();
    var approxCost = $($(parentElement).children("span")[0]).html();
    $.ajax({
        url: '/ManageRciComponent/DeleteCost',
        data: {
            componentID: componentID,
            rciID: rciID,
            name: name,
            approxCost: approxCost
        },
        method: "POST"
    }).done(function () {
        parentElement.remove();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to delete that cost.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
}

function startAddCost(element) {
    console.log(element);
    var parentElement = $(element).parent().parent();
    console.log(parentElement);
    var addCostElement = $(parentElement).children("p")[0];
    console.log(addCostElement);
    console.log($(addCostElement).children("input")[0]);
    $($(addCostElement).children("input")[0]).attr("style", "display: inline !important");
    $($(addCostElement).children("input")[1]).attr("style", "display: inline !important");
    $($(addCostElement).children("i")[0]).attr("style", "display: inline !important");
    $($(addCostElement).children("i")[1]).attr("style", "display: inline !important");
}

function saveAddCost(element) {
    var parentElement = $(element).parent();
    var component = $(parentElement).parent().parent().parent();
    var componentID = $(".component").index(component);
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4), 10);
    var newCostName = $($(parentElement).children("input")[0]).val();
    var newCostApproxCost = $($(parentElement).children("input")[1]).val();
    if (newCostName != "" && newCostApproxCost != "") {
        $.ajax({
            url: '/ManageRciComponent/AddCost',
            data: {
                componentID: componentID,
                rciID: rciID,
                newCostName: newCostName,
                newCostApproxCost: newCostApproxCost
            },
            method: "POST"
        }).done(function () {
            let $wrapperP = $("<p></p>");
            $wrapperP.append("<b>" + newCostName + "</b>");
            $wrapperP.append("<span>" + newCostApproxCost + "</span>");
            $wrapperP.append('<i class="material-icons start-edit-cost" onclick="startEditCost(this)">create</i>');
            $wrapperP.append('<i class="material-icons delete-cost" onclick="deleteCost(this)">delete</i>');
            $wrapperP.append('<input class="input-edit-name" type="text" value="' + newCostName + '" />');
            $wrapperP.append('<input class="input-edit-approxCost" type="text" value="' + newCostApproxCost + '" />');
            $wrapperP.append('<i class="material-icons save-edit-cost" onclick="saveEditCost(this)">done</i>');
            $wrapperP.append('<i class="material-icons cancel-edit-cost" onclick="cancelEditCost(this)">clear</i>');
            $(parentElement).parent().append($wrapperP);
            $($(parentElement).children("input")[0]).html("");
            $($(parentElement).children("input")[1]).html("");
            $($(parentElement).children("input")[0]).hide();
            $($(parentElement).children("input")[1]).hide();
            $($(parentElement).children("i")[0]).hide();
            $($(parentElement).children("i")[1]).hide();
        }).fail(function (jqXHR, textStatus, errorThrown) {
            alert("Oops! We were unable to edit that cost.");
            console.log("Status: " + jqXHR.status);
            console.log("Response Text: " + jqXHR.responseText);
            console.log(textStatus);
            console.log(errorThrown);
        });
    }
    else if (newCostName == "") {
        alert("Please enter name of the new cost!");
    }
    else {
        alert("Please enter the approximate cost of the new cost!");
    }
}

function cancelAddCost(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("input")[0]).hide();
    $($(parentElement).children("input")[1]).hide();
    $($(parentElement).children("i")[0]).hide();
    $($(parentElement).children("i")[1]).hide();
}

function startEditComponentName(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("span")[0]).hide();
    $($(parentElement).children("i")[0]).hide();
    $($(parentElement).children("i")[3]).hide();
    $($(parentElement).children("input")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("input")[1]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[1]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[2]).attr("style", "display: inline !important");
}

function saveEditComponentName(element) {
    var parentElement = $(element).parent();
    var component = $(parentElement).parent();
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4), 10);
    var oldComponentName = $($(parentElement).children("span")[0]).html();
    var oldComponentDescription = $($($($(component).children("div")[0]).children("p")[0]).children("span")[0]).html();
    var newComponentName = $($(parentElement).children("input")[0]).val();
    $.ajax({
        url: '/ManageRciComponent/EditComponentName',
        data: {
            rciID: rciID,
            oldComponentName: oldComponentName,
            oldComponentDescription: oldComponentDescription,
            newComponentName: newComponentName
        },
        method: "POST"
    }).done(function () {
        $($(parentElement).children("span")[0]).html(newComponentName);
        $($(parentElement).children("span")[0]).attr("style", "display: inline !important");
        $($(parentElement).children("i")[0]).attr("style", "display: inline !important");
        $($(parentElement).children("i")[3]).attr("style", "display: inline !important");
        $($(parentElement).children("input")[0]).hide();
        $($(parentElement).children("input")[1]).hide();
        $($(parentElement).children("i")[1]).hide();
        $($(parentElement).children("i")[2]).hide();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to edit that component's name.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
}

function cancelEditComponentName(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("span")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[3]).attr("style", "display: inline !important");
    $($(parentElement).children("input")[0]).hide();
    $($(parentElement).children("input")[1]).hide();
    $($(parentElement).children("i")[1]).hide();
    $($(parentElement).children("i")[2]).hide();
}

function startAddComponent(element) {
    var parentElement = $(element).parent().parent().children("p")[0];
    $($(parentElement).children("input")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("input")[1]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[0]).attr("style", "display: inline !important");
    $($(parentElement).children("i")[1]).attr("style", "display: inline !important");
}

function saveAddComponent(element) {
    var parentElement = $(element).parent();
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4), 10);
    var newComponentName = $($(parentElement).children("input")[0]).val();
    var newComponentDescription = $($(parentElement).children("input")[1]).val();
    if (newComponentName != "" && newComponentDescription != "") {
        $.ajax({
            url: '/ManageRciComponent/AddComponent',
            data: {
                rciID: rciID,
                newComponentName: newComponentName,
                newComponentDescription: newComponentDescription
            },
            method: "POST"
        }).done(function () {
            let $wrapperDiv = $('<div class="pane component"></div>');
            let $wrapperH2 = $('<h2 class="pane-title"></h2>');
            $wrapperH2.append('<span>' + newComponentName + '</span>');
            $wrapperH2.append('<i class="material-icons start-edit-component" onclick="startEditComponentName(this)">create</i>');
            $wrapperH2.append('<input class="input-edit-component" type="text" value="' + newComponentName + '"/>');
            $wrapperH2.append('<i class="material-icons save-edit-component" onclick="saveEditComponentName(this)">done</i>');
            $wrapperH2.append('<i class="material-icons cancel-edit-component" onclick="cancelEditComponentName(this)">clear</i>');
            $wrapperH2.append('<i class="material-icons delete-component" onclick="deleteComponent(this)">delete</i>');
            $wrapperDiv.append($wrapperH2);
            let $wrapperDivPaneContent = $('<div class="input pane-content"></div>');
            let $wrapperP = $('<p></p>');
            $wrapperP.append('<b>Description:</b>');
            $wrapperP.append('<span>' + newComponentDescription + '</span>');
            $wrapperP.append('<i class="material-icons start-edit-description" onclick="startEdit(this)">create</i>');
            $wrapperP.append('<input class="input-edit-description" type="text" value="' + newComponentDescription + '" />');
            $wrapperP.append('<i class="material-icons save-edit-description" onclick="saveEdit(this)">done</i>');
            $wrapperP.append('<i class="material-icons cancel-edit-description" onclick="cancelEdit(this)">clear</i>');
            $wrapperDivPaneContent.append($wrapperP);
            let $wrapperDivPossibleCosts = $('<div class="possible-costs"></div>');
            $wrapperDivPossibleCosts.append('<div><h3>Possible Costs:</h3> <i class="material-icons" onclick="startAddCost(this)">add</i></div>');
            $wrapperDivPossibleCosts.append('<p><input class="input-edit-name" type="text" placeholder="Name of Cost"/><input class="input-edit-approxCost" type="text" placeholder="Approximate Cost" /><i class="material-icons save-edit-cost" onclick="saveAddCost(this)">done</i><i class="material-icons cancel-edit-cost" onclick="cancelAddCost(this)">clear</i></p>');
            $wrapperDivPaneContent.append($wrapperDivPossibleCosts);
            $wrapperDiv.append($wrapperDivPaneContent);
            $(parentElement).parent().parent().append($wrapperDiv);
            $($(parentElement).children("input")[0]).hide();
            $($(parentElement).children("input")[1]).hide();
            $($(parentElement).children("i")[0]).hide();
            $($(parentElement).children("i")[1]).hide();
            alert("The new component is added to the end among all components.");
        }).fail(function (jqXHR, textStatus, errorThrown) {
            alert("Oops! We were unable to edit that cost.");
            console.log("Status: " + jqXHR.status);
            console.log("Response Text: " + jqXHR.responseText);
            console.log(textStatus);
            console.log(errorThrown);
        });
    }
    else if (newComponentName == "") {
        alert("Please enter name of the new component!");
    }
    else {
        alert("Please enter description of the new component!");
    }
}

function cancelAddComponent(element) {
    var parentElement = $(element).parent();
    $($(parentElement).children("input")[0]).hide();
    $($(parentElement).children("input")[1]).hide();
    $($(parentElement).children("i")[0]).hide();
    $($(parentElement).children("i")[1]).hide();
}

function deleteComponent(element) {
    var parentElement = $(element).parent();
    var component = $(parentElement).parent();
    var rciID = parseInt($("h2[id^='rci-']").attr("id").substring(4), 10);
    var componentName = $($(parentElement).children("span")[0]).html();
    var componentDescription = $($($($(component).children("div")[0]).children("p")[0]).children("span")[0]).html();
    $.ajax({
        url: '/ManageRciComponent/DeleteComponent',
        data: {
            rciID: rciID,
            componentName: componentName,
            componentDescription: componentDescription
        },
        method: "POST"
    }).done(function () {
        component.remove();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("Oops! We were unable to edit that component's name.");
        console.log("Status: " + jqXHR.status);
        console.log("Response Text: " + jqXHR.responseText);
        console.log(textStatus);
        console.log(errorThrown);
    });
}

$.fn.ignore = function (sel) {
    return this.clone().find(sel || ">*").remove().end();
}