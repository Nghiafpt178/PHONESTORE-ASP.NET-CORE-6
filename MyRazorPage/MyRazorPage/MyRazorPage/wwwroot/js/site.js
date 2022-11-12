
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();
connection.on("LoadProducts", (data) => {
    var tr = '';
    $.each(data, (k, v) => {
        tr +=
            `
            <tr>
                <td>${v.productId}</td>
                <td>${v.productName}</td> 
                <td>${v.category}</td>
            </tr>
            `
    })
    $("#tableBody").html(tr);
    console.log(data);
});

connection.start().then().catch(function (err) {
    return console.log(err.toString());
});
