"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();
connection.on("ReloadProduct", function () {
    location.reload();
});

connection.start().then().catch(function (err) {
    return console.log(err.toString());
});
