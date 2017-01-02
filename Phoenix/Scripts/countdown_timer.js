// End time
var deadline = "2017-03-20";

// Call update clock function when user first loads page
updateClock("clock-div", deadline);

// Then start calling it over and over in 1 second intervals
var interval = setInterval(function () {
    updateClock("clock-div", deadline);
}, 1000);


/* Function to update the DOM with the correct time values.
 id = HTML id of the clock.
 endDate = the time we are counting down towards
*/
function updateClock(id, endDate) {

    var clock = document.getElementById(id);

    var t = getTimeRemaining(endDate);

    var seconds = clock.querySelector(".seconds");
    var minutes = clock.querySelector(".minutes");
    var hours = clock.querySelector(".hours");
    var days = clock.querySelector(".days");

    seconds.innerHTML = ("0" + t.seconds).slice(-2);
    minutes.innerHTML = ("0" + t.minutes).slice(-2);
    hours.innerHTML = ("0" + t.hours).slice(-2);
    days.innerHTML = ("0" + t.days).slice(-2);

}


/*
Utility function to get the time remaining between now and 
a certain date.
*/
function getTimeRemaining(endDate) {
    // DateToCome - Now = Time in between in milliseconds
    var t = Date.parse(endDate) - Date.parse(new Date());

    // We always divide by milliseconds, because
    // t is in milliseconds, and we don't need that level
    // of accuracy.
    var seconds = Math.floor((t / 1000) % 60);
    var minutes = Math.floor((t / 1000 / 60) % 60);
    var hours = Math.floor((t / 1000 / 60 / 60) % 24);
    var days = Math.floor((t / 1000 / 60 / 60 / 24));

    var result = {
        "total": t,
        "seconds": seconds,
        "minutes": minutes,
        "hours": hours,
        "days": days
    };

    // return the result object
    return result;

}
