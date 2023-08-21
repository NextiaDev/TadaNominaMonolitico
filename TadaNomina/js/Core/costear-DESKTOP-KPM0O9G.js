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
    console.log(total);
    $("#_total").text(total);
    $("#modalDividir").modal("show");
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
    var html = "";

    if (subtotal != 0 && iva != 0 && total) {

        html += "<div class='row'>";
        html += "<div class='col-md-2 text-right'> Subtotal</div > <div class='col-md-2'><strong>" + subtotal + "</strong></div>";
        html += "<div class='col-md-2 text-right'> IVA</div > <div class='col-md-2'><strong>" + iva + "</strong></div>";
        html += "<div class='col-md-2 text-right'> TOTAL</div > <div class='col-md-2'><strong>" + total + "</strong></div>";
        html += "</div>";

        var totalDescontado = parseFloat($("#totDescontado").val());
        var nuevoTotDescontado = totalDescontado + total;
        $("#totDescontado").val(nuevoTotDescontado);

        var totalGral = $("#TOTAL").text();
        var montoFormat = totalGral.replace(/[$,]/g, '');
        var nuevoMonto = parseFloat(montoFormat) - nuevoTotDescontado;

        $("#_totalFalt").text(nuevoMonto);
        $("#subtotal_").val("");
        $("#iva_").val("");
        $("#total_").val("");
        
        $("#tablaDif").append(html);
    }    
});
