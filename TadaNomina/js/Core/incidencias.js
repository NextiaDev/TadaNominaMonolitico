
$("#guardaConf").click(function () {
    var IdConfiguracionFiniquito = $("#guardaConf").attr("val");
    var banderaVac = $('#banderaVac').is(":checked");
    var banderaPV = $('#banderaPV').is(":checked");
    var banderaAgui = $('#banderaAgui').is(":checked");
    
    var bandera90d = $('#bandera90d').is(":checked");
    var bandera20d = $('#bandera20d').is(":checked");
    var banderaPA = $('#banderaPA').is(":checked");
    var LiquidacionSDI = $("#LiquidacionSDI").is(":checked");

    $.ajax({
        type: 'POST',
        url: 'GuardarConfAvanzada',
        dataType: 'json',
        data: { IdConfiguracionFiniquito, banderaVac, banderaPV, banderaAgui, bandera90d, bandera20d, banderaPA, LiquidacionSDI },
        success: function (data) {
            $('#configuracionAvanzada').modal('hide');
            if (data == "Exito") {
                mensajeAlerta("Atencion!", "La configuración del calculo fue modificada", "success", "jelly", "fadeOut", 2500);
            }
            else {
                mensajeAlerta("Atencion!", data, "danger", "jelly", "fadeOut", 2500);
            }
        }
    });
});

$("#Liquidacion").change(function () {
    var Liquidacion = false;
    var IdEmpleado = $("#IdEmpleado").val();
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();

    if (this.checked) {
        Liquidacion = true;
    }

    $.ajax({
        type: "Post",
        url: "CambiaLiquidacion",
        dataType: "json",
        data: { Liquidacion, IdEmpleado, IdPeriodoNomina },
        async: false,
        success: function (json) {
            if (json == "Exito") {
                mensajeAlerta("Atencion!", "Se cambio la configuración del finiquito, para calculo de liquidación", "success", "jelly", "fadeOut", 2500);
            }
        },
        failure: function (response) {
            mensajeAlerta(response, "danger");
        }

    });
});

$("#_actualizaFechaBaja").click(function () {
    var fechaBaja = $("#_fechaBaja").val();
    var IdEmpleado = $("#IdEmpleado").val();
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();

    $.ajax({
        type: 'POST',
        url: 'ActualizaFechaBaja',
        data: { fechaBaja, IdEmpleado, IdPeriodoNomina },
        dataType: 'json',
        success: function (data) {
            if (data == "Exito") {
                mensajeAlerta("Atencion!", "Se actualizo la fecha de baja", "success", "jelly", "fadeOut", 2500);
            }
            else {
                mensajeAlerta("Atencion!", data, "danger", "jelly", "fadeOut", 2500);
            }
        }
    });
});

$("#_cambiarSueldo").click(function () {

    var IdEmpleado = $("#IdEmpleado").val();
    var nuevoSueldo = $("#_nuevoSueldo").val();

    $.ajax({
        type: 'POST',
        url: 'ActualizaSueldoReal',
        data: { IdEmpleado, nuevoSueldo },
        dataType: 'json',
        success: function (result) {
            if (result == "Exito") {
                
                $("#SueldoDiarioReal").text(nuevoSueldo);
                mensajeAlerta("Atencion!", "Se actualizo el sueldo real correctamente", "success", "jelly", "fadeOut", 2500);
            }
            else {
                mensajeAlerta("Atencion!", result, "danger", "jelly", "fadeOut", 2500);
            }
        }
    });
});
