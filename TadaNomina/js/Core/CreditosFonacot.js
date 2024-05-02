$("#btnBuscar").click(function () {
    var clave = $("#ClaveEmp").val();
    $("#Empleado").val('');
    $("#rfc").val('');
    $.ajax({
        type: 'GET',
        url: 'BuscaEmpleado',
        dataType: 'json',
        data: { clave },
        success: function (data) {
            if (data != 'El Empleado con la clave que ingreso no existe!') {
                $("#IdEmpleado").val(data.IdEmpleado);
                $("#NombreEmpleado").val(data.Nombre);
                $("#RFC").val(data.Rfc);
            }
            else {
                mensajeAlerta("Atencion!", data, "danger", "fadeInDown", "fadeOutDown", 2500);
            }
        }
    });

});

$(function () {
    $(".anchorDetail").click(function () {
        debugger;
        var $buttonClicked = $(this);
        var id = $buttonClicked.attr('data-id');
        var options = { "backdrop": "static", keyboard: true };

        $.ajax({
            type: "GET",
            url: 'Fonacot/Details',
            contentType: "application/json; charset=utf-8",
            data: { id },
            datatype: "json",
            success: function (data) {
                debugger;
                $('#myModalContent').html(data);
                $('#myModal').modal(options);
                $('#myModal').modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });

    $("#closbtn").click(function () {
        $('#myModal').modal('hide');
    });
});

$(function () {
    $(".anchorDelete").click(function () {
        debugger;
        var $buttonClicked = $(this);
        var id = $buttonClicked.attr('data-id');
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: 'Fonacot/Delete',
            contentType: "application/json; charset=utf-8",
            data: { id },
            datatype: "json",
            success: function (data) {
                debugger;
                $('#myModalContent').html(data);
                $('#myModal').modal(options);
                $('#myModal').modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });

    $("#closbtn").click(function () {
        $('#myModal').modal('hide');
    });
});

function CambiaStatus(IdCredito) {
    $.ajax({
        type: 'POST',
        url: 'Fonacot/CambiaStatusCredito',
        data: { IdCredito: IdCredito },
        datatype: 'json',
        success: function (data) {
            if (data == "OK") {
                alert("Se realizó el cambio.");
            } else {
                alert("No se pudo realizar el cambio, favor de actualizar la ventara e intentarlo nuevamente");
            }
        }

    });
}