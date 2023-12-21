$("#guardaFactor").click(function () {
    $.showLoading();
    var IdConcepto = $(this).attr('valor');
    var limInferior = $("#_limInf").val();
    var limSuperior = $("#_limSup").val();
    var tipoDato = $("#_tipoDato").val();
    var valor = $("#_valor").val();
    var fIniVig = $("#fIniVig").val();

    $.ajax({
        type: 'POST',
        url: 'guardaFactor',
        data: { IdConcepto, limInferior, limSuperior, tipoDato, valor, fIniVig },
        dataType: 'json',
        success: function (data) {
            if (data.result == "Ok") {
                mensajeAlerta("Atencion!", data.mensaje, "success", "fadeInDown", "fadeOutDown", 3500);
                setTimeout(redirigirHideLoading, 3600);
            } else {
                mensajeAlerta("Atencion!", data.mensaje, "danger", "fadeInDown", "fadeOutDown", 3500);
            }
        },
        error(er) {
            mensajeAlerta("Atencion!", er, "danger", "fadeInDown", "fadeOutDown", 3500);
        }
    });

    $.hideLoading();
});

$("#_cerrar").click(function () {

    $("#modalCargarFactor").modal('hide');
    redirigir();
});

$("#_cerrarE").click(function () {

    $("#modalEditarFactor").modal('hide');
    redirigir();
});

$("#_cerrarD").click(function () {

    $("#modalDeleteFactor").modal('hide');
    redirigir();
});

function redirigir() {
    window.location.reload();
}

function redirigirHideLoading() {
    $.hideLoading();
    window.location.reload();
}

$("._editFactor").click(function () {
    var IdConceptoFactor = $(this).attr('data-id');
    $("#_IdFactorEdit").val(IdConceptoFactor);
    $.ajax({
        type: 'POST',
        url: 'getFactor',
        data: { IdConceptoFactor },
        dataType: 'json',
        success: function (data) {
            if (data.result == "Ok") {
                $("#_limInfE").val(data.factor.Limite_Inferior);
                $("#_limSupE").val(data.factor.Limite_Superior);
                $("#_tipoDatoE").val(data.factor.TipoDato);
                $("#_valorE").val(data.factor.Valor);
                $("#fIniVigE").val(data.fecha);
                $("#modalEditarFactor").modal('show');
            } else {
                mensajeAlerta("Atencion!", data.mensaje, "danger", "fadeInDown", "fadeOutDown", 3500);
            }
        },
        error(er) {
            mensajeAlerta("Atencion!", er, "danger", "fadeInDown", "fadeOutDown", 3500);
        }
    });    
});

$("#editFactor").click(function () {
    $.showLoading();
    var IdConceptoFactor = $("#_IdFactorEdit").val();
    var limInferior = $("#_limInfE").val();
    var limSuperior = $("#_limSupE").val();
    var tipoDato = $("#_tipoDatoE").val();
    var valor = $("#_valorE").val();
    var fIniVig = $("#fIniVigE").val();

    $.ajax({
        type: 'POST',
        url: 'editFactor',
        data: { IdConceptoFactor, limInferior, limSuperior, tipoDato, valor, fIniVig },
        dataType: 'json',
        success: function (data) {
            if (data.result == "Ok") {
                mensajeAlerta("Atencion!", data.mensaje, "success", "fadeInDown", "fadeOutDown", 3500);
                setTimeout(redirigirHideLoading, 3600);
            }
            else {
                mensajeAlerta("Atencion!", data.mensaje, "danger", "fadeInDown", "fadeOutDown", 3500);
            }
            
        },
        error(er) {
            mensajeAlerta("Atencion!", er, "danger", "fadeInDown", "fadeOutDown", 3500);
        }
    });
});

$("._deletefactor").click(function () {
    $("#_IdFactorDelete").val($(this).attr('data-id'));
    $("#_infoEliminar").text($(this).attr('data-text'));
    $("#modalDeleteFactor").modal('show');
});

$("#deleteFactor").click(function () {
    var IdConceptoFactor = $("#_IdFactorDelete").val();
    $.showLoading();
    $.ajax({
        type: 'POST',
        url: 'deleteFactor',
        data: { IdConceptoFactor },
        dataType: 'json',
        success: function (data) {
            if (data.result == "Ok") {
                mensajeAlerta("Atencion!", data.mensaje, "success", "fadeInDown", "fadeOutDown", 3500);
                setTimeout(redirigirHideLoading, 3600);
                 
            } else {
                mensajeAlerta("Atencion!", data.mensaje, "danger", "fadeInDown", "fadeOutDown", 3500);
            }
        },
        error(er) {
            mensajeAlerta("Atencion!", er, "danger", "fadeInDown", "fadeOutDown", 3500);
        }
    });

});