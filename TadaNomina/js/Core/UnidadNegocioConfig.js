var datos = [];
function redirigir() {
    window.location.reload();
}


$("#_GuardarDatos").click(function () {

    var CargaSocial = $("#id_cobroCarga").prop("checked");
    var RetencionISR = $("#id_retencionisr").prop("checked");
    var RetencionISRAguinaldos = $("#id_isr74").prop("checked");
    var SubSidioSN = $("#id_PeriodosEx").prop("checked");
    var NCargaObrera = $("#id_noCObrera").prop("checked");
    var NCPatronal = $("#id_noCOpatronal").prop("checked");
    var FaltasImporte = $("#id_faltasImporte").prop("checked");
    var DiasMas = $("#id_faltasImporte").prop("checked");
    var DiasMenos = $("#id_DiasAltasme").prop("checked");
    var GoCEdeVacas = $("#id_PvGoce").prop("checked");
    var SeptimoDia = $("#id_septimo").prop("checked");
    var IdSeptimoDIa = $("#idConceptos").val();
        $.ajax({
            type: 'POST',
            url: 'UnidadesNegocio/Gurdar',
            data: { CargaSocial, RetencionISR, RetencionISRAguinaldos, SubSidioSN, NCargaObrera, NCPatronal, FaltasImporte, DiasMas, DiasMenos, GoCEdeVacas, SeptimoDia, IdSeptimoDIa },
            dataType: 'json',
            async: false,
            success: function (result) {
                if (result == "Exito") {
                    $("#newPeriodo").modal('hide');
                    $("#_GuardarPeriodoNomina").prop("disabled", true);
                    mensajeAlerta("Atencion!", "Se guardo la informacion del periodo de Nómina.", "mint", "bounce", "fadeOut", 2000);
                    setTimeout(redirigir, 2500);
                } else {

                    $.hideLoading();
                    mensajeAlerta("Atencion!", result, "danger", "bounce", "fadeOut", 2100);
                }
            }
        });
    
});




$("#Limpiar").click(function () {
   
    var a = document.getElementById("eltexto").value = "";
    datos = [];
});

function actualizar() {

    var select = document.querySelector("#idConceptosd").value;
    var eltexto = document.querySelector("#eltexto");
    datos.push(select);
    eltexto.value = datos;
}


function actualizardos() {

    var select = document.querySelector("#idEmpleados").value;
    var eltexto = document.querySelector("#eltextodos");
    datos.push(select);
    eltexto.value = datos;
}

function GetTiposPeriodos() {
    $.ajax({
        type: 'GET',
        url: '../GetConceptos',
        dataType: 'json',
        success: function (data) {

            $('#idConceptos').html('');
            $("#idConceptos").append('<option value="0">Selecione un Concepto.</option>');
            $.each(data, function (key, registro) {
                $("#idConceptos").append('<option value=' + registro.IdConcepto + '>' + registro.Concepto + '</option>');
            });
        }
    });
}

function GetEmpleados() {
    $.ajax({
        type: 'GET',
        url: '../getEmpleados',
        dataType: 'json',
        success: function (data) {
            $('#idEmpleados').html('');
            $("#idEmpleados").append('<option value="0">Selecione un Usuario.</option>');
            $.each(data, function (key, registro) {
                $("#idEmpleados").append('<option value=' + registro.IdUsuario + '>' + registro.Usuario + '</option>');

            });
        }
    });
}

function GetTiposPeriodosd() {
    $.ajax({
        type: 'GET',
        url: '../GetConceptosd',
        dataType: 'json',
        success: function (data) {
            $('#idConceptosd').html('');
            $("#idConceptosd").append('<option value="0">Selecione un Concepto.</option>');
            $.each(data, function (key, registro) {
                $("#idConceptosd").append('<option value=' + registro.IdConcepto + '>' + registro.Concepto + '</option>');
            });
        }
    });
}


$('#id_validarN').change(function () {
    if ($(this).prop('checked') == true) {
        $("#id_Emple").show();
        GetEmpleados();
    }
    else {
        $("#id_Emple").hide();
    }


});



$('#id_septimo').change(function () {
    if ($(this).prop('checked') == true) {
        $("#idConceptos").prop("disabled", false);
        GetTiposPeriodos();
    }
    else {
        $("#idConceptos").prop("disabled", true);

    }
});


$('#id_DiasFr').change(function () {
    if ($(this).prop('checked') == true) {
        $("#idConceptosd").prop("disabled", false);
        $("#id_Factors").show();
        $("#id_DiasFrd").show();

        GetTiposPeriodosd();

    }
    else {
        $("#idConceptosd").prop("disabled", true);
        $("#id_Factors").hide();
        $("#id_DiasFrd").hide();


    }


});


$('#idArchivH').change(function () {
    if ($(this).prop('checked') == true) {
        $("#PeriodoTE").show();
        console.log("Holaverdad");

    }
    else {
        $("#PeriodoTE").hide();

        console.log("Holafalso");

    }


});

$('#id_correosN').change(function () {
    if ($(this).prop('checked') == true) {
        $("#idCorreosc").show();
       

    }
    else {
        $("#idCorreosc").hide();

      

    }


});