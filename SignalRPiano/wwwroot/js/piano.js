"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/midiHub").build();

connection.on("ReceiveMessage", function (msg) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);

    li.textContent = `${msg.note} at ${msg.velocity}`;

    debugger;
   //make the key look pressed
    $(".key[data-key=" + msg.note + "]").addClass("pressed");
    setTimeout(function () {
        $(".key[data-key=" + msg.note + "]").removeClass("pressed");
    }, 500);

});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

$(".key").on("click", function () {
    var mykey = this;
    //alert("Key:" + mykey.dataset.key);

    connection.invoke("SendMessage", { "Note": mykey.dataset.key, "Velocity": 64 }).catch(function (err) {
        return console.error(err.toString());
    });

});

$("#playkey").on("click", function () {
    $(".key[data-key=0]").addClass("pressed");
});