
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
                $("#Empleado").val(data.Nombre);
                $("#rfc").val(data.Rfc);
                $("#IdEmpleado").val(data.IdEmpleado);
            }
            else {
                mensajeAlerta("Atencion!", data, "danger", "fadeInDown", "fadeOutDown", 2500);
            }
        }
    });

});