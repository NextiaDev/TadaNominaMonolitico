$("#IdsPeriodos").dblclick(function () {
    var item = $('#IdsPeriodos option:selected').appendTo("#IdsPeriodosSelecionados");
});

$("#limpiarSeleccionados").click(function () {
    $("#IdsPeriodosSelecionados").empty();
});

$("#_cerrarOtros").click(function () {
    $("#modalEmpleadosOtros").modal('hide');
    window.location.reload();
});

$("#getConfigSDI").click(function () {
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();

    $.ajax({
        type: 'POST',
        url: 'getDatosConfSDI',
        data: { IdPeriodoNomina },
        dataType: 'json',
        success: function (result) {
            if (result.result == "Ok") {
                $('#IdsPeriodosSelecionados').empty();
                if (result.datos.CalcularLiquidacionSDI == "S") {
                    $("#LiquidacionSDIGral").prop("checked", true);
                }
                else {
                    $("#LiquidacionSDIGral").prop("checked", false);
                }

                $("#ConceptosSDI").val(result.datos.ConceptosSDILiquidacion);
                
                $.each(result.datos2, function (index, value) {
                    var newItem = $('<option/>').text(value.Periodo).val(value.IdPeriodoNomina);
                    newItem.attr('selected', 'true');
                    $('#IdsPeriodosSelecionados').append(newItem);                                      
                });

                $("#configuracionSDI").modal('show');

            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "fadeInDown", "fadeOutDown", 3500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "fadeInDown", "fadeOutDown", 3500);
        }
    });
});

$("#getConfiguracionAvanzada").click(function () {
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();

    $.ajax({
        type: 'POST',
        url: 'getDatosConceptosPagar',
        data: { IdPeriodoNomina },
        dataType: 'json',
        success: function (result) {
            if (result.result == "Ok") {
                if (result.datos.BanderaNoVacaciones == "S") {
                    $('#banderaVacGral').prop("checked", true);
                } else {
                    $('#banderaVacGral').prop("checked", false);
                }

                if (result.datos.BanderaNoPV == "S") {
                    $('#banderaPVGral').prop("checked", true);
                } else {
                    $('#banderaPVGral').prop("checked", false);
                }

                if (result.datos.BanderaNoAguinaldo == "S") {
                    $('#banderaAguiGral').prop("checked", true);
                } else {
                    $('#banderaAguiGral').prop("checked", false);
                }

                if (result.datos.BanderaNo90Dias == "S") {
                    $('#bandera90dGral').prop("checked", true);
                } else {
                    $('#bandera90dGral').prop("checked", false);
                }

                if (result.datos.BanderaNo20Dias == "S") {
                    $('#bandera20dGral').prop("checked", true);
                } else {
                    $('#bandera20dGral').prop("checked", false);
                }

                if (result.datos.BanderaNoPA == "S") {
                    $('#banderaPAGral').prop("checked", true);
                } else {
                    $('#banderaPAGral').prop("checked", false);
                }

                $("#configuracionAvanzada").modal('show');
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "fadeInDown", "fadeOutDown", 3500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "fadeInDown", "fadeOutDown", 3500);
        }
    })
});

$("#guardaConfSDI").click(function () {
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();
    var PeriodosSeleccionados = $("#IdsPeriodosSelecionados")
    var LiquidacionSDI = $("#LiquidacionSDIGral").is(":checked");
    var ConceptosIntegran = $("#ConceptosSDI").val();
    var selectedIndices_ = [];
    $.each(PeriodosSeleccionados, function () {
        selectedIndices_.push($(this).val());
    });

    var selectedIndices = selectedIndices_.toString();

    $.ajax({
        type: 'POST',
        url: 'guardaDatosConfSDI',
        data: { IdPeriodoNomina, selectedIndices, LiquidacionSDI, ConceptosIntegran },
        dataType: 'json',
        success: function (result) {
            if (result.result == "Ok") {
                mensajeAlerta("Atencion!", result.mensaje, "info", "fadeInDown", "fadeOutDown", 3500);
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "fadeInDown", "fadeOutDown", 3500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "fadeInDown", "fadeOutDown", 3500);
        }
    });
});

$("#guardaConf").click(function () {
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();
    var BanderaNoVacaciones = $('#banderaVacGral').is(":checked");
    var BanderaNoPV = $('#banderaPVGral').is(":checked");
    var BanderaNoAguinaldo = $('#banderaAguiGral').is(":checked");
    var BanderaNo90Dias = $('#bandera90dGral').is(":checked");
    var BanderaNo20Dias = $('#bandera20dGral').is(":checked");
    var BanderaNoPA = $('#banderaPAGral').is(":checked");

    $.ajax({
        type: 'POST',
        url: 'guardaDatosConceptosGral',
        data: { IdPeriodoNomina, BanderaNoVacaciones, BanderaNoPV, BanderaNoPV, BanderaNoAguinaldo, BanderaNo90Dias, BanderaNo20Dias, BanderaNoPA },
        dataType: 'json',
        success: function (result) {
            if (result.result == "Ok") {
                mensajeAlerta("Atencion!", result.mensaje, "info", "fadeInDown", "fadeOutDown", 3500);
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "fadeInDown", "fadeOutDown", 3500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "fadeInDown", "fadeOutDown", 3500);
        }
    });
});


function generaReporte(IdPeriodo) {
    $.ajax({
        type: 'POST',
        url: '@Url.Action("Descargar")',
        data: { "Id": IdPeriodo },
        dataType: 'json',
        success: function (data) {

            var d = JSON.parse(data);
            console.log(d)

            $("#dvjson").excelexportjs({
                containerid: "dvjson",
                datatype: 'json',
                dataset: d,
                columns: getColumns(d),
                worksheetName: "Nomina"
            });
        }
    });
}

$("#actualizarEmp").click(function () {
    var ids = $("#Ids").val();
    var IdPeriodo = $("#actualizarEmp").attr('val');
    $.ajax({
        type: 'POST',
        url: 'addEmpleado',
        data: { ids, IdPeriodo },
        dataType: 'json',
        success: function (result) {
            $("#modalBuscar").modal('hide');
            if (result == "Exito") {
                window.location.reload();
            }
            else if (result.includes('|')) {
                $("#modalEmpleadosOtros").modal('show');
                $("#_empleadosNoAgregados").html(result);
            }
            else {
                mensajeAlerta("Atencion!", result, "warning", "fadeInDown", "fadeOutDown", 3500);
            }
        }
    });
});

$("#buscarEmp").click(function () {
    var clave = $("#_clave").val();
    var nombre = $("#_nombre").val();
    var rfc = $("#_rfc").val();
    var claves = $("#_claves").val();

    $.ajax({
        type: "POST",
        url: "BuscarEmpleados",
        data: { clave, nombre, rfc, claves },
        dataType: "json",
        success: function (result) {
            if (result.length > 0) {

                var table = "<div class='text-right'>";
                table += "</div><br />";
                table += "<table class='table table-hover table-sm'>";
                table += "<tr style='background-color:#CCCCCC'><th>Clave</th><th>Nombre</th><th>RFC</th><th>Patrona</th><th>Estatus</th><th>Elegir</th></tr>";

                for (var i = 0; i < result.length; i++) {
                    var cve = result[i].ClaveEmpleado;
                    var cls = "";
                    if (result[i].Estatus == "ACTIVO") { cls = "success" } else { cls = "danger" }
                    table += "<tr>";
                    table += "<td>" + result[i].ClaveEmpleado + "</<td>";
                    table += "<td>" + result[i].NombreCompleto + "</td>";
                    table += "<td>" + result[i].Rfc + "</td>";
                    table += "<td>" + result[i].NombrePatrona + "</td>";
                    table += "<td><label class='label label-" + cls + " label-sm'>" + result[i].Estatus + "</label></td>";
                    table += "<td><button class='btn btn-dark btn-sm' onClick='Insert(" + result[i].IdEmpleado + ", `" + cve + "`)'><span class='fa fa-download'></span></button></td>";
                    table += "</tr>"
                }

                table += "</table>";

                $("#_resultados").html(table);
            }
            else {
                $("#_resultados").html("<div class='alert alert-info'><span class='fa fa-info'></span> No hay resultados con los datos ingresados...</div>");
            }
        }
    });
});

function Insert(id, clave) {
    var valor = $("#Claves").val();
    var valorId = $("#Ids").val();
    var nuevoValor = "";
    var nuevoValorId = "";
    var elementos = valorId.split(",");
    var validacion = elementos.includes(id.toString());

    if (!validacion) {
        if (valor.length > 0) {
            nuevoValor = valor + "," + clave;
        }
        else {
            nuevoValor = clave;
        }

        if (valorId.length > 0) {
            nuevoValorId = valorId + "," + id;
        }
        else {
            nuevoValorId = id;
        }

        $("#Claves").val(nuevoValor);
        $("#Ids").val(nuevoValorId);

        mensajeAlerta("Atencion!", "Se agrego el empleado: " + clave, "info", "fadeInDown", "fadeOutDown", 3500);
    }
    else {
        mensajeAlerta("Atencion!", "Ya se agrego el empleado: " + clave, "warning", "fadeInDown", "fadeOutDown", 3500);
    }
}

function Limpiar() {
    $("#_resultados").empty();
    $("#_clave").val('');
    $("#_nombre").val('');
    $("#_rfc").val('');
}

$("#actualizaFechasBajaGral").click(function () {
    $.showLoading();
    var FechaBaja = $("#_fechaBajaGral").val();
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();

    $.ajax({
        type: 'POST',
        url: 'actualizaFechaBajaGral',
        data: { IdPeriodoNomina, FechaBaja },
        dataType: 'json',
        success: function (result) {
            if (result.result == "Ok") {
                mensajeAlerta("Atencion!", "Se actualizaron las fechas de baja para todos los empleados.", "info", "fadeInDown", "fadeOutDown", 3500);
                window.location.reload();
                $.hideLoading();
            }
            else {
                $.hideLoading();
                mensajeAlerta("Atencion!", result.mensaje, "warning", "fadeInDown", "fadeOutDown", 3500);                
            }
        },
        error: function (er) {
            $.hideLoading();
            mensajeAlerta("Atencion!", er, "warning", "fadeInDown", "fadeOutDown", 3500);            
        }
    });
});

