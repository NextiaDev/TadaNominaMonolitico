$(".cargaCSDPAC").click(function () {
    $("#hiddenIdEmpresaCSD").val($(this).attr('data-id'));
    $("#detalleEmpresa").text($(this).attr('data-text'));
    $("#modalCargaSCD").modal('show');
});

$(".cargaFIELPAC").click(function () {
    $("#hiddenIdEmpresaFIEL").val($(this).attr('data-id'));
    $("#detalleEmpresaF").text($(this).attr('data-text'));
    $("#modalCargaFIEL").modal('show');
});

$("#_enviaCSDPAC").click(function () {
    var IdRegistroPatronal = $("#hiddenIdEmpresaCSD").val();
    $("#modalCargaSCD").modal('hide');
    $.showLoading({ name: 'circle-fade'});
    $.ajax({
        type: 'POST',
        url: 'CargaCertificados/enviarCSDalPAC',
        data: { IdRegistroPatronal },
        dataType: 'json',
        success: function (result) {
            $.hideLoading();
            if (result.result == "Ok") {
                mensajeAlerta("Atencion!", "Se envio la información de forma correcta al PAC.", "mint", "jelly", "fadeOut", 2000);
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "warning", "jelly", "fadeOut", 2000);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "warning", "jelly", "fadeOut", 2000);
        }
    });
});

$("#_enviaFIELPAC").click(function () {
    var IdRegistroPatronal = $("#hiddenIdEmpresaFIEL").val();
    $("#modalCargaFIEL").modal('hide');
    $.showLoading({ name: 'circle-fade' });
    $.ajax({
        type: 'POST',
        url: 'CargaCertificados/enviarFIELalPAC',
        data: { IdRegistroPatronal },
        dataType: 'json',
        success: function (result) {
            $.hideLoading();
            if (result.result == "Ok") {
                mensajeAlerta("Atencion!", "Se envio la información de forma correcta al PAC.", "mint", "jelly", "fadeOut", 2000);
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "warning", "jelly", "fadeOut", 2000);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "warning", "jelly", "fadeOut", 2000);
        }
    });
});