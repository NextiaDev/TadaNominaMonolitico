
$("._mover").click(function () {
    var IdEmpleado = $(this).attr("val");
    var IdPeriodoNomina = $(this).attr("val2");

    $("#_Aceptar").attr('val', IdEmpleado);
    $("#_Aceptar").attr('val2', IdPeriodoNomina);

    $("#modalMover").modal('show');
});

$("#_Aceptar").click(function () {
    var IdEmpleado = $(this).attr("val");
    var IdPeriodo = $(this).attr("val2");
    var nuevoIdPeriodo = $("#IdPeriodo").val();

    $.ajax({
        type: 'POST',
        url: 'FiniquitoNoAcumulados/moverFiniquito',
        data: { IdEmpleado, IdPeriodo, nuevoIdPeriodo },
        dataType: 'json',
        async: false,
        success: function (result) {
            if (result == "Ok") {
                mensajeAlerta("Atencion!", "Se movio el calculo.", "success", "jelly", "fadeOut", 2500);
                recargar();
            }
            else {
                mensajeAlerta("Atencion!", data, "danger", "jelly", "fadeOut", 2500);
            }
            $("#modalMover").modal('hide');
        }
    });
});

function recargar() {
    setTimeout(function () {
        window.location.reload();
    }, 2600);
}