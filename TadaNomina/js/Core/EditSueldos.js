function myFunction() {
    var IdEmpleado = $('#IdEmpleado').val();    
    var SDIMSS = $("#SDIMSSSueldos").val();
    var fechaReconocimientoAntiguedad = $("#FechaReconocimientoAntiguedad").val();

    console.log(IdEmpleado, SDIMSS, fechaReconocimientoAntiguedad);

    $.ajax({
        type: "POST",
        url: "CalcularSdI",
        data: JSON.stringify(
            {
                'IdEmpleado': IdEmpleado,
                'SDIMSS': SDIMSS,
                'FechaReconocimientoAntiguedad': fechaReconocimientoAntiguedad
            }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (datos) {
            $('#SDINuevo').val(datos);
        },
        error: function (error) {
        }
    });
}