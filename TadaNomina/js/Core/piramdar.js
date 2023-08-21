$("#btnBuscarEmp").click(function () {
    var ClaveEmpleado = $("#_ClaveEmpleado").val();
    $.ajax({
        type: 'POST',
        url: 'Piramidados/getEmpleado',
        data: { ClaveEmpleado },
        dataType: 'json',
        success: function (result) {
            
            if (result.result) {
                $("#IdEmpleado_").val(result.emp.IdEmpleado);
                $("#_sd").val(result.emp.SDIMSS);
                $("#_datosEmp").val(result.emp.ClaveEmpleado + " - " + result.emp.ApellidoPaterno + " " + result.emp.ApellidoMaterno + " " + result.emp.Nombre + " - " + result.emp.Rfc);
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 2500);
                $("#_ClaveEmpleado").val('')
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });    
});

$("#btnAgregar").click(function () {
    Cargando();
    var IdEmpleado = $("#IdEmpleado_").val();
    var Importe = $("#_importe").val();
    var ConsideraSMO = $("#check-SMO").is(':checked');
    var datosEmp = $("#_datosEmp").val();
    var SD = $("#_sd").val();
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();
    var IdConcepto = $("#IdConcepto").val();
        
    $.ajax({
        type: 'POST',
        url: 'Piramidados/piramida',
        data: { IdEmpleado, datosEmp, SD, ConsideraSMO, Importe },
        dataType: 'json',
        success: function (result) {
            
            var table = `<table class='table table-hover table-bordered'>
                <tr>  <th>D.Pago</th> <th>SD</th> <th>SMB</th> <th>ISRSM</th> <th>SMN</th> <th>SNI</th> <th>ISRT</th> <th>ISRR</th> <th>TP</th> <th>Acción</th> </tr><tr>
                 <td>` + result.diasPago + `</td> <td>` + result.SD_F + `</td> <td>` + result.SM_F + `</td> <td>` +
                result.ISRSM_F + `</td> <td>` + result.NetoSueldo_F + `</td> <td>` + result.ImporteConConcepto_F + `</td> <td>` + result.ISRT_F + `</td> <td>` +
                result.ISRR_F + `</td> <td>` + result.TP_F + `</td> <td><button class='btn btn-primary btn-sm _Aceptar' onClick='Insert(` +
                IdPeriodoNomina + `,` + IdEmpleado + `,` + IdConcepto + `,` + result.diasPago + `,` + result.SD + `,` + result.SM + `,` + result.ISRSM + `,` + result.NetoSueldo + `,` +
                Importe + `,` + result.ImporteConConcepto + `,` + result.ISRT + `,` + result.ISRR + `,` + result.TP + `,` + ConsideraSMO + `)' ><span class='fa fa-plus'></span> Agregar</button></td</tr>
                </table`;
            $("#result").html(table);

            QuitarCargando();

            mensajeAlerta("Atencion!", "Se genero el calculo de forma correcta.", "success", "jelly", "fadeOut", 2500);
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });
});


$("#btnAgregar_cap").click(function () {
    Cargando();
    var IdEmpleado = $("#IdEmpleado_").val();    
    var ConsideraSMO = $("#check-SMO").is(':checked');
    var datosEmp = $("#_datosEmp").val();
    var SD = $("#_sd").val();
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();
    var IdConcepto = $("#IdConcepto").val();
    var TotalPercepcionesCap = $("#_tp_cap").val();
    var ISRCap = $("#_isr_cap").val();
    var Importe = $("#_importe").val();
    var NetoCap = Importe;    
        
    $.ajax({
        type: 'POST',
        url: 'Piramidados/AgregaPersonalizado',
        data: { IdEmpleado, datosEmp, SD, ConsideraSMO, Importe, TotalPercepcionesCap, ISRCap, NetoCap },
        dataType: 'json',
        success: function (result) {

            var table = `<table class='table table-hover table-bordered'>
                <tr>  <th>D.Pago</th> <th>SD</th> <th>SMB</th> <th>ISRSM</th> <th>SMN</th> <th>SNI</th> <th>ISRT</th> <th>ISRR</th> <th>TP</th> <th>Acción</th> </tr><tr>
                 <td>` + result.diasPago + `</td> <td>` + result.SD_F + `</td> <td>` + result.SM_F + `</td> <td>` +
                result.ISRSM_F + `</td> <td>` + result.NetoSueldo_F + `</td> <td>` + result.ImporteConConcepto_F + `</td> <td>` + result.ISRT_F + `</td> <td>` +
                result.ISRR_F + `</td> <td>` + result.TP_F + `</td> <td><button class='btn btn-primary btn-sm _Aceptar' onClick='Insert(` +
                IdPeriodoNomina + `,` + IdEmpleado + `,` + IdConcepto + `,` + result.diasPago + `,` + result.SD + `,` + result.SM + `,` + result.ISRSM + `,` + result.NetoSueldo + `,` +
                Importe + `,` + result.ImporteConConcepto + `,` + result.ISRT + `,` + result.ISRR + `,` + result.TP + `,` + ConsideraSMO + `)' ><span class='fa fa-plus'></span> Agregar</button></td</tr>
                </table`;
            $("#result").html(table);

            QuitarCargando();

            mensajeAlerta("Atencion!", "Se genero el calculo de forma correcta.", "success", "jelly", "fadeOut", 2500);
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });
});

function Insert(IdPeriodoNomina, IdEmpleado, IdConcepto, diasPago, SD, SM, ISRSM, NetoSueldo, Importe, ImporteConConcepto, ISRT, ISRR, TP, ConsideraSMO) {
    $.ajax({
        type: 'POST',
        url: 'Piramidados/AddPiramidado',
        data: { IdPeriodoNomina, IdEmpleado, IdConcepto, diasPago, SD, SM, ISRSM, NetoSueldo, Importe, ImporteConConcepto, ISRT, ISRR, TP, ConsideraSMO },
        dataType: 'json',
        success: function (result) {
            if (result == "OK") {
                redirigir();
            }
            else {
                mensajeAlerta("Atencion!", result, "danger", "jelly", "fadeOut", 2500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });
    
}

function Cargando() {
    $.showLoading({
        name: 'circle-fade'
    });
}

function QuitarCargando() {
    $.hideLoading({
        name: 'circle-fade'
    });
}

function redirigir() {
    window.location.reload();
}

$(".btnEliminar").click(function () {
    $("#_IdEliminar").val($(this).attr('data-id'));
    $("#lblCalculoElim").text($(this).attr('data-text') + "-" + $(this).attr('data-decimal'));
    $("#modalElimiar").modal('show');
});

$(".btnDetalle").click(function () {
    var IdConceptoConfigurado = $(this).attr("data-id");
    $.ajax({
        type: 'POST',
        url: 'Piramidados/getConceptoById',
        data: { IdConceptoConfigurado },
        dataType: 'json',
        success: function (result) {
            console.log(result.concepto);
            if (result.result) {
                $("#lblCveEmp").text(result.concepto.ClaveEmpleado);
                $("#lblNombre").text(result.concepto.ApellidoPaterno + ' ' + result.concepto.ApellidoMaterno + ' ' + result.concepto.Nombre);
                $("#lbldPago").text(result.concepto.DPago);
                $("#lblSD").text(result.concepto.SD);
                $("#lblSMB").text(result.concepto.SMO);
                $("#lblISRSM").text(result.concepto.ISR_SMO);
                $("#lblSMN").text(result.concepto.SMN);
                $("#lblsmni").text(result.concepto.SMN_Imp);
                $("#lblIsrT").text(result.concepto.ISR_Total);
                $("#lblIsrRet").text(result.concepto.ISR_Cobrar);
                $("#lblTP").text(result.concepto.ImporteBruto);
                $("#lblTC").text(result.concepto.TipoCalculo);
                $("#lblImporte").text(result.concepto.Importe);
                $("#lblConcepto").text(result.concepto.ClaveConcepto + " - " + result.concepto.Concepto);

                $("#modalDetalle").modal('show');
            }
            else {
                mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });    
});

$("#eliminar").click(function () {
    var IdConceptoConfigurado = $("#_IdEliminar").val();
    $.ajax({
        type: 'POST',
        url: 'Piramidados/DeleteCalculo',
        data: { IdConceptoConfigurado },
        dataType: 'json',
        success: function (result) {
            if (result == "Ok") {
                window.location.reload();
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });
});

$("#eliminarTodo").click(function () {
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();
    $.ajax({
        type: 'POST',
        url: 'Piramidados/DeleteAllCalculos',
        data: { IdPeriodoNomina  },
        dataType: 'json',
        success: function (result) {
            if (result == "Ok") {
                window.location.reload();
            }
            else {
                mensajeAlerta("Atencion!", result, "danger", "jelly", "fadeOut", 2500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });
});

$("#cargarLayout").click(function () {
    var fileUpload = $("#filePiramidados").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();
    var ConsideraSMO = $("#check-SMO").is(':checked');

    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }

    fileData.append("IdPeriodoNomina", IdPeriodoNomina);
    fileData.append("ConsideraSMO", ConsideraSMO);

    $.ajax({
        type: "POST",
        url: 'Piramidados/UploadFiles',
        processData: false,
        contentType: false,
        data: fileData,
        success: function (result) {
            if (result.result) {
                window.location.reload();
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 2500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });
});

