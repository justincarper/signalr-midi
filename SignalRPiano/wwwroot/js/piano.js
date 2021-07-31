"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/midiHub").build();

connection.on("ReceiveMessage", function (msg) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${msg.note} at ${msg.velocity}`;
    //document.getElementById("userInput").value = msg.userName;
    //document.getElementById("messageInput").value = msg.message;
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

$(".key").on("click", function () {
    debugger;
    var mykey = this;
    //alert("Key:" + mykey.dataset.key);

    //make the key look pressed
    var jQueryObj = $(".key [data-key=" + mykey.dataset.key + "]");
    jQueryObj.addClass("pressed");
    setTimeout(function () {
        jQueryObj.removeClass("pressed");
    }, 500);

    connection.invoke("SendMessage", { "Note": mykey.dataset.key, "Velocity": 64 }).catch(function (err) {
        return console.error(err.toString());
    });

});