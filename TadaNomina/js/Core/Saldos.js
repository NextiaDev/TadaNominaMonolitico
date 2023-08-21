$("#AplicaPor").on("change", function () {
    Saldos();
});


function Saldos() {
    var x = document.getElementById("AplicaPor").value;

    if (x == 'Saldos') {
        $("#Conceptos").show();
        $("#Observaciones").show();
        $("#Saldos").show();
        $("#Saldosdos").show();
        $("#fechas").hide();
        $("#Indefinido").hide();
        $("#Monto").hide();


    }
    if (x == 'Periodo de Tiempo') {
        $("#Conceptos").show();
        $("#fechas").show();
        $("#Indefinido").show();
        $("#Monto").show();
        $("#Observaciones").show();
        $("#Saldos").hide();
        $("#Saldosdos").hide();
      

    }
}


$(".editCredito").click(function (e) {
    var idSaldo = $("#_Id").val();
    $("#editTipo").modal("show");
    var tr = $(this).closest('tr');
    var id = tr.children('td:eq(0)').text(); //get the text from first col of current row
    /*console.log(id);*/ //you'll get the actual ids here


    $.ajax({
        type: 'POST',
        url: 'EditarSaldos',
        dataType: 'json',
        data: { idSaldo: id },
        success: function (data) {



            try {
                var dateString = data.FechaInicial.substr(6);
                var currentTime = new Date(parseInt(dateString));
                var dateStringd = data.FechaFinal.substr(6);
                var currentTimedos = new Date(parseInt(dateStringd));
            }
            catch (err) {
            }


            console.log(data);


            GetTipoInfoEditar(data.IdConcepto);
            $("#NombreE").val(data.Nombre + " " + data.ApellidoPaterno + " " + data.ApellidoMaterno);
            $("#RfcE").val(data.Rfc);
            $("#AplicaE").val(data.Tipo)
            $("#ConceptoE").val(data.ClaveConcepto);
            $("#saldoInicialE").val(data.SaldoInicial);
            $("#saldoActualE").val(data.SaldoActual);
            $("#descuentoPeriodoE").val(data.DescuentoPeriodo);
            $("#numeroPeriodosE").val(data.NumeroPeriodos);
            $("#observacionesE").val(data.Observaciones);
            $("#_Id").val(data.IdSaldo);
            $("#FechaInicialE").val(new Date(currentTime).toLocaleDateString('es', { day: '2-digit', month: '2-digit', year: 'numeric' }));
            $("#FechaFinalE").val(new Date(currentTimedos).toLocaleDateString('es', { day: '2-digit', month: '2-digit', year: 'numeric' }));
            $("#descuentoPeriodoTE").val(data.DescuentoPeriodo);
            $("#observacionesTE").val(data.Observaciones);

            if (data.Indefinido == 1) {
                $("#editSaldos_").prop('checked', true);
                $("#FechaInicialE").show();
                $("#desdeid").show();
                $("#idhas").hide();


            } else {
                $("#editSaldos_").prop('checked', false);
                $("#FechaInicialE").show();
                $("#desdeid").show();
                $("#idhas").show();
            }



            idSaldo = data.idSaldo;
            IdEmpleado = data.idEmpleado;
            /* console.log(data.Tipo);*/

            if (data.Tipo == "Saldos") {
                $("#SaldosE").show();
                $("#SaldosE1").show();
                $("#SaldosE2").show();
                $("#SaldosE3").show();
                $("#SaldosE4").show();
                $("#PeriodoTE").hide();
                $("#PeriodoTE1").hide();
                $("#PeriodoTE2").hide();

            }
            if (data.Tipo == 'Periodo de Tiempo') {
                $("#SaldosE").hide();
                $("#SaldosE1").hide();
                $("#SaldosE2").hide();
                $("#SaldosE3").hide();
                $("#SaldosE4").hide();
                $("#PeriodoTE").show();
                $("#PeriodoTE1").show();
                $("#PeriodoTE2").show();

            }
        }
    });
    // console.log(FirstCol);
});


$("#_GuardarSaldosE").click(function () {
    var x = document.getElementById("AplicaE").value;
    var Tipo = $("#AplicaE").val();
    var idSaldo = $("#_Id").val();
    var FechaInicio = $("#FechaInicialE").val();
    var FechaFin = $("#FechaFinalE").val();
    var indefinidon = $("#editSaldos_").prop("checked")

    var obj = {
        idSaldo: idSaldo,
        idEmpleado: IdEmpleado,
        Tipo: Tipo,
    }

    if (x == 'Saldos') {
        /*console.log(x);*/
        /*console.log(obj);*/

        $.ajax({
            type: 'POST',
            url: 'EditarSaldoU',
            dataType: 'json',
            data: { idSaldo, Tipo: Tipo, idConcepto: $("#TipoE").val(), saldoInicial: $("#saldoInicialE").val(), saldoActual: $("#saldoActualE").val(), descuentoPeriodo: $("#descuentoPeriodoE").val(), numeroPeriodos: $("#numeroPeriodosE").val(), observaciones: $("#observacionesE").val() },
            success: function (data) {

                if (data == "OK") {
                    $("#editTipo").modal('hide');
                    mensajeAlerta("Atencion!", "Se edito correctamente la información del saldo.", "mint", "bounce", "fadeOut", 2000);

                    setTimeout(redirigir, 2500);
                }
                else {
                    mensajeAlerta("Atencion!", data, "danger", "bounce", "fadeOut", 2100);

                }
            },
            error(er) {
            }
        });
    }
    else {
        /*console.log(x);*/

        $.ajax({
            type: 'POST',
            url: 'EditarSaldoUPeriodoTiempo',
            dataType: 'json',
            data: { idSaldo, Tipo: Tipo, idConcepto: $("#TipoE").val(), saldoInicial: $("#saldoInicialE").val(), saldoActual: $("#saldoActualE").val(), descuentoPeriodo: $("#descuentoPeriodoTE").val(), observaciones: $("#observacionesTE").val(), FechaInicial: FechaInicio, FechaFinal: FechaFin, indefinidon: indefinidon },
            success: function (data) {
                /*console.log(data);*/
                if (data == "Ok") {
                    $("#editTipo").modal('hide');
                    mensajeAlerta("Atencion!", "Se edito correctamente la información del saldo.", "mint", "bounce", "fadeOut", 2000);

                    setTimeout(redirigir, 2500);
                }
                else {
                }
            },
            error(er) {
                mensajeAlerta("Atencion!", data, "danger", "bounce", "fadeOut", 2100);

            }
        });
    }
    /*console.log(data);*/
});

function GetTipoInfoEditar(valueSelected) {

    $.ajax({
        type: 'GET',
        url: 'getConceptos',
        dataType: 'json',
        /* data: { clave },*/
        success: function (data) {

            /*$("#Tipo").append('<option value="0">Selecione una opción.</option>');*/
            $('#TipoE').html('');
            $("#TipoE").append('<option value="0">Selecione una opción.</option>');
            $.each(data, function (key, registro) {
                // console.log(registro);
                $("#TipoE").append('<option value=' + registro.IdConcepto + '>' + registro.ClaveConcepto + ' - ' + registro.Concepto + '</option>');
            });

            $('#TipoE option[value="' + valueSelected + '"]').attr('selected', 'selected')
        }
    });
}

$(".detalleCredito").click(function () {
    var idSaldo = $("#_Id").val();

    $("#detalleTipo").modal("show");
    var tr = $(this).closest('tr');
    var id = tr.children('td:eq(0)').text(); //get the text from first col of current row

    $.ajax({
        type: 'POST',
        url: 'Saldos/EditarSaldos',
        dataType: 'json',
        data: { idSaldo: id },
        success: function (data) {
            $("#NombreED").val(data.nombre + " " + data.apellidoPaterno + " " + data.apellidoMaterno);
            $("#RfcED").val(data.rfc);
            $("#TipoED").val(data.Tipo);
            $("#ConceptoED").val(data.concepto);
            $("#saldoInicialED").val(data.saldoInicial);
            $("#saldoActualED").val(data.saldoActual);
            $("#descuentoPeriodoED").val(data.descuentoPeriodo);
            $("#numeroPeriodosED").val(data.numeroPeriodos);
            $("#observacionesED").val(data.observaciones);
            $("#_Id").val(data.idSaldo);
            $("#FechaInicialED").val(new Date(data.FechaInicial).toLocaleDateString('es', { day: '2-digit', month: '2-digit', year: 'numeric' }));
            $("#FechaFinalED").val(new Date(data.FechaFinal).toLocaleDateString('es', { day: '2-digit', month: '2-digit', year: 'numeric' }));
            $("#descuentoPeriodoTED").val(data.descuentoPeriodo);
            $("#observacionesTED").val(data.observaciones);

            if (data.indefinido == 1) {
                $("#vertSaldos_").prop('checked', true);
                $("#desdev").show();
                $("#hastav").hide();


            } else {
                $("#vertSaldos_").prop('checked', false);
                $("#desdev").show();
                $("#hastav").show();
            }




            idSaldo = data.idSaldo;
            IdEmpleado = data.idEmpleado;
            /*console.log(data);*/

            if (data.Tipo == "Saldos") {
                $("#SaldosED").show();
                $("#SaldosED1").show();
                $("#SaldosED2").show();
                $("#SaldosED3").show();
                $("#SaldosED4").show();
                $("#PeriodoTED").hide();
                $("#PeriodoTED1").hide();
                $("#PeriodoTED2").hide();
            }
            if (data.Tipo == 'Periodo de Tiempo') {
                $("#SaldosED").hide();
                $("#SaldosED1").hide();
                $("#SaldosED2").hide();
                $("#SaldosED3").hide();
                $("#SaldosED4").hide();
                $("#PeriodoTED").show();
                $("#PeriodoTED1").show();
                $("#PeriodoTED2").show();
            }
        }
    });
});


function redirigir() {
    window.location.reload();

}


function ActivarSaldo(id, saldo) {
    if (!$('#' + id).is(":checked")) {
        $.ajax({
            type: 'POST',
            url: 'SuspenderSaldo',
            dataType: 'json',
            data: { idSaldo: saldo },
            success: function (data) {

                if (data == "OK") {
                    mensajeAlerta("Atencion!", "Se suspendio correctamente el saldo.", "mint", "bounce", "fadeOut", 2000);
                    setTimeout(redirigir, 2500);

                }
                else {

                    mensajeAlerta("Atencion!", data, "danger", "bounce", "fadeOut", 2100);
                }
            }
        });
    }
    else {
        $.ajax({
            type: 'POST',

            url: 'ActivarSaldo',
            dataType: 'json',
            data: { idSaldo: saldo },
            success: function (data) {

                if (data == "OK") {
                    mensajeAlerta("Atencion!", "Se activo correctamente el saldo.", "mint", "bounce", "fadeOut", 2000);
                    setTimeout(redirigir, 2500);

                }
                else {
                    mensajeAlerta("Atencion!", data, "danger", "bounce", "fadeOut", 2100);

                }
            }
        });
    }
}


$(".deleteCredito").click(function () {
    var idSaldo = $("#_Id").val();

    $("#deleteTipo").modal("show");
    var tr = $(this).closest('tr');
    var id = tr.children('td:eq(0)').text(); //get the text from first col of current row

    $.ajax({
        type: 'POST',
        url: 'EditarSaldos',
        dataType: 'json',
        data: { idSaldo: id },
        success: function (data) {
            $("#nombreTipoCreditoE").text(data.Concepto);

            $("#_Id").val(data.IdSaldo);

            idSaldo = data.idSaldo;
            IdEmpleado = data.idEmpleado;
        }
    });
});




$("#_eliminarS").click(function () {
    var IdSaldo = $("#_Id").val();

    $.ajax({
        type: 'POST',
        url: 'DeleteSaldos',
        dataType: 'json',
        data: { IdSaldo: IdSaldo },
        async: false,
        success: function (data) {
            if (data == "OK") {
                $("#deleteTipo").modal('hide');
                mensajeAlerta("Atencion!", "Se elimino el registro.", "mint", "bounce", "fadeOut", 2000);
                setTimeout(redirigir, 2500);
            }
            else {

                mensajeAlerta("Atencion!", data, "danger", "bounce", "fadeOut", 2100);
            }
        },

    });
});