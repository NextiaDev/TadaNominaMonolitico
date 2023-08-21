$("#calcula").click(function () {
    var importe = $("#importe").val();
    var pHonorario = parseFloat($("#pHon").val());     
    var honorario = 0;
    var subtotal = importe;

    if (!isNaN(pHonorario)) {
        honorario = parseFloat(importe) * (parseFloat(pHonorario) * 0.01);
        subtotal = parseFloat(subtotal) + parseFloat(honorario);
    }

    var iva = subtotal * 0.16;
    var total = parseFloat(subtotal) + parseFloat(iva);

    $("#honorario").val(honorario);
    $("#subtotal").val(subtotal);
    $("#iva").val(iva);
    $("#total").val(total);
});