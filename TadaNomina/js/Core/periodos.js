$(".modalAcumular").click(function () {
    var periodo = $(this).attr('val')
    var idperiodo = $(this).attr('data-id')
    $("#idper").text(idperiodo);
    $("#info").text(periodo);
    $('#modalAcumularPeriodo').modal('show');
});

$("#acumularPeriodo").click(function () {
    var FechaDispersion = $("#FechaDispersion").val();
    var IdPeriodoNomina = $("#idper").text();

    $.ajax({
        type: 'POST',
        url: 'Periodos/AcumularNomina',
        dataType: 'json',
        data: { IdPeriodoNomina, FechaDispersion },
        success: function (data) {
            $('#modalAcumularPeriodo').modal('hide');
            if (data == "Exito") {
                document.getElementById("acumular" + IdPeriodoNomina).disabled = true;
                document.getElementById("delete" + IdPeriodoNomina).disabled = true;
                $("#edit" + IdPeriodoNomina).attr("disabled", true);
                mensajeAlerta("Atencion!", "El periodo de nómina se acumulo de forma correcta!", "success", "jelly", "fadeOut", 2500);
            }
            else {
                mensajeAlerta("Atencion!", data, "danger", "jelly", "fadeOut", 2500);
            }
        }
    });
});



$(".anchorDetail").click(function () {     
    var $buttonClicked = $(this);
    var id = $buttonClicked.attr('data-id');    
    $.ajax({
        type: "GET",
        url: 'Periodos/Detail',
        contentType: "application/json; charset=utf-8",
        data: { "Id": id },
        datatype: "json",
        success: function (data) {
            eval(data)            
            $("#_lblUnidad").text(data.UnidaNegocio);
            $("#_lblPeriodo").text(data.Periodo);
            $("#_fechaInicial").text(data.FechaInicio);
            $("#_fechaFinal").text(data.FechaFin);
            $("#_tipoNomina").text(data.TipoNomina);
            $("#_AjusteImp").text(data.AjusteImpuestos);
            $("#_pAjuste").text(data.PeriodosAjuste);
            $("#_omitirDesc").text(data.OmitirDescuentosFijos);
            $("#_fCreacion").text("");
            $("#_Observaciones").text(data.Observaciones);
            $('#modalDetail').modal('show');
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
});

$(".anchorDetail1").click(function () {
    var $buttonClicked = $(this);
    var id = $buttonClicked.attr('data-id');
    $.ajax({
        type: "GET",
        url: 'AcumularPeriodos/Detail',
        contentType: "application/json; charset=utf-8",
        data: { "Id": id },
        datatype: "json",
        success: function (data) {
            eval(data)
            $("#_lblUnidad").text(data.UnidaNegocio);
            $("#_lblPeriodo").text(data.Periodo);
            $("#_fechaInicial").text(data.FechaInicio);
            $("#_fechaFinal").text(data.FechaFin);
            $("#_tipoNomina").text(data.TipoNomina);
            $("#_AjusteImp").text(data.AjusteImpuestos);
            $("#_pAjuste").text(data.PeriodosAjuste);
            $("#_omitirDesc").text(data.OmitirDescuentosFijos);
            $("#_fCreacion").text("");
            $("#_Observaciones").text(data.Observaciones);
            $("#_totalObrero").text(data.CargaObrera);
            $("#_totalPatron").text(data.CargaPatronal);
            $("#_totalISR").text(data.ISR);
            $("#_totalPercepciones").text(data.TotalPercepciones);
            $("#_totalDeducciones").text(data.TotalDeducciones);
            $("#_totalNeto").text(data.NetoPagar);
            $('#modalDetail1').modal('show');
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
});

$(".anchorDelete").click(function () {

    var periodo = $(this).attr('val')
    var idperiodo = $(this).attr('data-id')
    $("#idperEl").text(idperiodo);
    $("#infoEl").text(periodo);
    $('#modalDeletePeriodo').modal('show');
});

$("#eliminarPeriodo").click(function () {   
    var IdPeriodoNomina = $("#idperEl").text();

    $.ajax({
        type: 'POST',
        url: 'Periodos/Delete',
        dataType: 'json',
        data: { IdPeriodoNomina },
        success: function (data) {
            $('#modalDeletePeriodo').modal('hide');
            if (data == "Exito") {
                document.getElementById("acumular" + IdPeriodoNomina).disabled = true;
                document.getElementById("delete" + IdPeriodoNomina).disabled = true;
                document.getElementById("detail" + IdPeriodoNomina).disabled = true;
                $("#edit" + IdPeriodoNomina).attr("disabled", true);
                document.getElementById("detail" + IdPeriodoNomina).disabled = true;
                mensajeAlerta("Atencion!", "El periodo de nómina se elimino de forma correcta!", "success", "jelly", "fadeOut", 2500);
            }
            else {
                mensajeAlerta("Atencion!", data, "danger", "jelly", "fadeOut", 2500);
            }
        }
    });
});

