$(function () {
    var chat = $.connection.chatHub;
    
    chat.client.sendChat = function (name, message, userId, userProp) {
        var usrSend = $("#displayUser").val();
        
        if (userId == usrSend || usrSend == userProp) {
            var divName = $("<div />").text(name).html();
            var divMessage = $("<div />").text(message).html();

            $("#discusion").append("<li><strong>" + divName + ": </strong>" + divMessage + "</li>");
        }
    };

    var nameOwner = $("#userName").attr("val"); //prompt("Escribe tu nombre: ", "");
    $("#displayname").val(nameOwner);

    var userId = $("#userName").attr("val2");
    $("#displayUser").val(userId);
    
    $("#message").focus();

    $.connection.hub.start().done(function () {
        $("#sendMessage").click(function () {
            var nameOwner = $("#displayname").val();
            var messageText = $("#message").val();
            var userId = $("#displayUsersend").val();
            
            var userIdProp = $("#displayUser").val();
            
            chat.server.send(nameOwner, messageText, userId, userIdProp);

            $("#message").val("").focus();
        });
    });
});

//$(function () {
$("body").on('click', "#itemUso a", function () {
    var userIdSend = $(this).attr('id');
    //alert(userIdSend);
    $("#displayUsersend").val(userIdSend);
});
//}
