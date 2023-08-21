$("#guardaConfAvanzada").click(function () {
    var IdPeriodoNomina = $("#guardaConfAvanzada").attr("val");
    var IdEmpleado = $("#guardaConfAvanzada").attr("val2");
    var IdEstatus = $("#guardaConfAvanzada").attr("val3");
    var suspenderTradicional = $('#suspTrad').is(":checked");
    var suspenderEsquema = $('#suspEsq').is(":checked");
    var suspenderCS = $('#suspCS').is(":checked");
    var incidenciasAut = $('#incidenciasAut').is(":checked");
    var pagSueldos = $('#pagSueldos').val();
    var cobCargas = $('#cobCargas').val();

    //if ((pagSueldos.length > 0 && cobCargas.length > 0)) {

    //    mensajeAlerta("Atencion!", "No se pueden Cobrar cargas y Pagar sueldos, elija solo una opción.", "danger", "jelly", "fadeOut", 3500);
    //}
    if ((pagSueldos.length > 0 && IdEstatus == 1) || (cobCargas.length > 0 && IdEstatus == 1)) {
        mensajeAlerta("Atencion!", "Opcion valida solo para empleados que son baja.", "danger", "jelly", "fadeOut", 3500);
    }
    else {
        $.ajax({
            type: 'POST',
            url: 'GuardarConfAvanzadaNomina',
            dataType: 'json',
            data: { IdPeriodoNomina, IdEmpleado, suspenderTradicional, suspenderEsquema, suspenderCS, pagSueldos, cobCargas, incidenciasAut },
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
    }
});

$("#abrirConf").click(function () {
    
    var IdPeriodoNomina = $("#_IdPeriodoNomina").val();
    var IdEmpleado = $("#_IdEmpleado").val();

    $.ajax({
        type: "POST",
        url: "GetConfiguracionAvanzada",
        dataType: "json",
        data: { IdPeriodoNomina, IdEmpleado },
        async: true,
        success: function (data) {
            eval(data);
            var supTrad = document.getElementById("suspTrad");
            var supEsq = document.getElementById("suspEsq");
            var supCarg = document.getElementById("suspCS");
            var incidenciasAut = document.getElementById("incidenciasAut");
            
            if (data.SupenderSueldoTradicional == "1") {
                supTrad.checked = true;
            }
            else {
                supTrad.checked = false;
            }
            
            if (data.SuspenderSueldoEsquema == "1") {
                supEsq.checked = true;
            }
            else {
                supEsq.checked = false;
            }

            if (data.SuspenderCargasSociales == "1") {
                supCarg.checked = true;
            }
            else {
                supCarg.checked = false;
            }

            if (data.IncidenciasAutomaticas == "1") {
                incidenciasAut.checked = true;
            }
            else {
                incidenciasAut.checked = false;
            }

            $("#pagSueldos").val(data.DiasSueldo);
            $("#cobCargas").val(data.DiasCargaSocial);
        }        
    });
    $("#confAvanzada").modal('show');
    
});