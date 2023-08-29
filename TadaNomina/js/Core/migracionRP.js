$("#_validar").click(function () {
    var claves = $("#Claves").val();

    $.ajax({
        type: 'POST',
        url: 'MigracionRP/getEmpleadosMigrar',
        data: { claves },
        dataType: 'json',
        success: function (data) {
            if (data.length > 0) {
                var table = "<table class='table table-bordered table-hover'>";
                table += "<tr><th>Clave</th><th>Nombre</th><th>RFC</th></tr>"

                for (var i = 0; i < data.length; i++) {
                    table += "<tr>";
                    table += "<td>" + data[i].Clave + "</<td>";
                    table += "<td>" + data[i].Nombre + "</td>";
                    table += "<td>" + data[i].Rfc + "</td>";
                    table += "</tr>"
                }

                table += "</table>";
                $("#myModalContent").html(table);
            }
            else {
                $("#myModalContent").html("<div class='alert alert-warning'>No se encontro ningun empleado con las claves proporcionadas.</div>");
            }
            $('#myModal').modal('show');
        }
    });
});