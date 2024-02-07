$("#IdPeriodoNomina").change(function () {
    var IdPeriodo = $(this).val();    
    $.showLoading({ name: 'circle-fade' });
    $("form").submit();
});

$(document).ready(function () {
    var mensaje = $("#mensajeError").attr("data-text");

    if (mensaje.length > 0) {
        mensajeAlerta("Atencion!", mensaje, "danger", "jelly", "fadeOut", 0);
    }
});

$("#generarXML").click(function () {
    $.showLoading({ name: 'circle-fade' });
    var IdPeriodoNomina = $(this).attr('data-id');
    var tipoTimbrado = $("#_tipo").val();
  var claves = $("#_claves").val();

    $.ajax({
        type: 'POST',
      url: 'GeneraXML/GenerarArchivos',
      data: { IdPeriodoNomina, tipoTimbrado, claves },
        dataType: 'json',
        async: true,
        success: function (result) {
            $.hideLoading();
            if (result.estatus == "Ok") {

                $("#MensajeContador").text("XML que hay actualmente en este periodo :" + result.cantidad);
                mensajeAlerta("Atencion!", result.mensaje, "success", "jelly", "fadeOut", 0);

                if (result.errores.length > 0) {
                    mensajeAlerta("Atencion!", "Se generaron los siguientes errores: " + result.errores, "warning", "jelly", "fadeOut", 0);
                }
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 0);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 0);
        }
    });
});

$("#_Eliminar").click(function () {
    $.showLoading({ name: 'circle-fade' });
    $("#myModalEliminarArchivos").modal('hide');
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();
    if (IdPeriodoNomina != 0) {
        $.ajax({
            type: 'POST',
            url: 'GeneraXML/EliminarXML',
            data: { IdPeriodoNomina },
            dataType: 'json',            
            success: function (result) {
                
                if (result.estatus == "Ok") {
                    
                    $("#MensajeContador").text("XML que hay actualmente en este periodo :" + result.cantidad);
                    mensajeAlerta("Atencion!", result.mensaje, "success", "jelly", "fadeOut", 0);                    
                }
                else {
                    mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 0);
                }

                $.hideLoading();
            },
            error: function (er) {
                mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 0);
            }
        });
    }

    
});

function recargar() {
    setTimeout(function () {
        window.location.href = "GeneraXML";
    }, 2600);
}
