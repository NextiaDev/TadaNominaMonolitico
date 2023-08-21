$("#IdsPeriodos").dblclick(function () {
    var item = $('#IdsPeriodos option:selected').appendTo("#IdsPeriodosSelecionados");    
});

$("#IdsPeriodosSelecionados").dblclick(function () {
    var item = $('#IdsPeriodosSelecionados option:selected').appendTo("#IdsPeriodos");
});



$("#seleccionPeriodo").change(function () {
    var IdUnidadNegocio = $("#seleccionPeriodo").val();

    $.ajax({
        type: 'POST',
        url: 'Costear/getPeriodos',
        data: { IdUnidadNegocio },
        dataType: 'json',
        success: function (periodos) {

            $("#IdsPeriodos").empty();
            $.each(periodos, function () {
                $("#IdsPeriodos").append($("<option></option>").val(this['Value']).html(this['Text']));
            });
        }
    });
});

$(function () {
    $(document).bind("contextmenu", function (e) {
        return false;
    });
});

$("#IdsPeriodosSelecionados").mousedown(function (e) {
    if (e.which == 3) {
        //$.ajax({
        //    type: 'POST',
        //    url: 'Costear/getCalculos'
        //});
        $("#modalFiniquitos").modal('show');
    }
    
});

$("#_facturasDif").click(function () {
    var total = $("#TOTAL").text();
    var montoFormat = total.replace(/[$,]/g, '');
    var nuevoMonto = parseFloat(montoFormat);
    $("#_totalFalt").text(nuevoMonto);
    $("#_total").text(total);
    $("#modalDividir").modal("show");
    $("#_guardarCosteo").attr('disabled', 'disabled');
});

$("#calcular_").click(function () {
    var subtotal = parseFloat($("#subtotal_").val());
    var iva = subtotal * 0.16;
    var total = subtotal + iva;
    $("#iva_").val(iva);
    $("#total_").val(total);   
});

$("#agregar_").click(function () {
    var subtotal = parseFloat($("#subtotal_").val());
    var iva = parseFloat($("#iva_").val());
    var total = parseFloat($("#total_").val());
    var totalDescontado = parseFloat($("#totDescontado").val());
    if (isNaN(totalDescontado)) {
        totalDescontado = 0;
    }    
    var html = "";

    if (subtotal != 0 && iva != 0 && total) {
        
        html += "<div class='row _nuevos'>";
        html += "<div class='col-md-2 text-right'> Subtotal:</div > <div class='col-md-2'><label name='subtotal'>" + subtotal + "</label></div>";
        html += "<div class='col-md-2 text-right'> IVA:</div > <div class='col-md-2' ><label name='iva'>" + iva + "</label></div>";
        html += "<div class='col-md-2 text-right'> TOTAL:</div > <div class='col-md-2' ><label name='total'>" + total + "</label></div>";
        html += "</div>";
                
        totalDescontado += total;
        var totalGral = $("#TOTAL").text();
        var montoFormat = totalGral.replace(/[$,]/g, '');
        var totGral = parseFloat(montoFormat);
        var nuevoMonto = totGral - totalDescontado;

        $("#_totalFalt").text(nuevoMonto);
        $("#subtotal_").val("");
        $("#iva_").val("");
        $("#total_").val("");
        $("#tablaDif").append(html);
        $("#totDescontado").val(totalDescontado);

        if (nuevoMonto < 1 && nuevoMonto > -1) {
            $("#_guardarCosteo").removeAttr('disabled');
        }

        if (nuevoMonto < -1) {
            mensajeAlerta("Atencion!", "El faltante no puede ser menor a 0.", "danger", "jelly", "fadeOut", 2500);
        }
    }    
});

$("#_cerrar").click(function () {
    $("#totDescontado").val(0);
    $("._nuevos").remove();
    $("#numFact").val("");
    $("#modalDividir").modal("hide");
});

$("#limpiar_").click(function () {
    var totalGral = $("#TOTAL").text();
    var montoFormat = totalGral.replace(/[$,]/g, '');
    var totGral = parseFloat(montoFormat);   

    $("#_totalFalt").text(totGral);
    $("#totDescontado").val(0);
    $("#numFact").val("");
    $("._nuevos").remove();
});

$("#calcularNumFact_").click(function () {
    var numFact = parseFloat($("#numFact").val());
   
    if (isNaN(numFact)) {
        numFact = 0;
    }

    var stotalGral = $("#SUBTOTAL").text();    
    var smontoFormat = stotalGral.replace(/[$,]/g, '');
    var stotGral = parseFloat(smontoFormat);

    var totalGral = $("#TOTAL").text();
    var montoFormat = totalGral.replace(/[$,]/g, '');
    var totGral = parseFloat(montoFormat);

    if (numFact > 0) {
        var subtotal = stotGral / numFact;
        var iva = subtotal * 0.16;
        var total = subtotal + iva;
        var totalDescontado = 0;
        var nuevoMonto = 0;

        for (x = 0; x < numFact; x++) {
            var html = "";
            html += "<div class='row _nuevos'>";
            html += "<div class='col-md-2 text-right'> Subtotal:</div > <div class='col-md-2'><label name='subtotal'>" + subtotal + "</label></div>";
            html += "<div class='col-md-2 text-right'> IVA:</div > <div class='col-md-2' ><label name='iva'>" + iva + "</label></div>";
            html += "<div class='col-md-2 text-right'> TOTAL:</div > <div class='col-md-2' ><label name='total'>" + total + "</label></div>";
            html += "</div>";

            totalDescontado += total;            

            $("#tablaDif").append(html);                        
        }

        if (nuevoMonto < 1 && nuevoMonto > -1) {
            $("#_guardarCosteo").removeAttr('disabled');
        }

        if (nuevoMonto < -1) {
            mensajeAlerta("Atencion!", "El faltante no puede ser menor a 0.", "danger", "jelly", "fadeOut", 2500);
        }

        nuevoMonto = totGral - totalDescontado;
        $("#_totalFalt").text(nuevoMonto);
    }
});

$("#_guardarCosteo").click(function () {
    var elementoshtml = document.getElementsByClassName("_nuevos");
    var IdsPeriodosSelecionados = $("#_guardarCosteo").attr("val");
    var IdCosteo = $("#_guardarCosteo").attr("val2");
    var datos = "";
    
    for (item = 0; item < elementoshtml.length; item++) {
        var subtotal = elementoshtml[item].children[1].childNodes[0].textContent;
        var iva = elementoshtml[item].children[3].childNodes[0].textContent;
        var total = elementoshtml[item].children[5].childNodes[0].textContent;

        datos += subtotal + "|" + iva + "|" + total + "&";
    }

    $.ajax({
        type: 'POST',
        url: 'Costear/guardaCosteosDif',
        data: { datos, IdsPeriodosSelecionados, IdCosteo },
        dataType: 'json',
        success: function (result) {
            if (result == "Exito") {
                mensajeAlerta("Atencion!", "Se generaron " + elementoshtml.length + " solicitudes de factura correctamente.", "success", "jelly", "fadeOut", 2500);
                $("#_guardarCosteo").attr('disabled', 'disabled');
            }
            else {
                mensajeAlerta("Atencion!", result, "danger", "jelly", "fadeOut", 2500);
            }
        }
    });
    
});
