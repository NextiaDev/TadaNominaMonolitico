$(".deletePeriodoNomina").click(function () {
    console.log("hola");

    var idHonorario = $(this).attr('data-id');
    $("#hiddenDeleteId").val(idHonorario);
    $("#deletePeriodo").modal("show");
    console.log(idHonorario);




});

$("#_eliminarHonorario").click(function () {
    var idHonorario = $("#hiddenDeleteId").val();
    var Observaciones = $("#newObservaciones_").val();

    console.log(idHonorario);

    $.ajax({
        type: 'POST',
        url: 'Honorarios/DeleteHonorarios',
        data: { idHonorario, Observaciones },
        dataType: 'json',
        async: false,
        success: function (result) {
            if (result == "OK") {
                $("#deletePeriodo").modal('hide');
                $("#_eliminarPeriodoNomina").prop("disabled", true);
                mensajeAlerta("Atencion!", "Se elimino la informacion del Honorario.", "mint", "bounce", "fadeOut", 2000);
                setTimeout(redirigir, 2500);
            } else {
                mensajeAlerta("Atencion!", result, "danger", "bounce", "fadeOut", 2100);
            }
        }
    });
   
});

$("#HonorariosN").change(function () {
    var honorariosN = $("#HonorariosN").val();

    console.log(honorariosN);
 
    $.ajax({
        type: "POST",
        url: "CalcularHonorariosN",
        data: JSON.stringify(
            {
                'CalcularHonorariosN': honorariosN,
   
            }
        ),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (datos) {


            console.log("Hola");
        },
        error: function (error) {
        }
    });
});

function redirigir() {
    window.location.reload();

}