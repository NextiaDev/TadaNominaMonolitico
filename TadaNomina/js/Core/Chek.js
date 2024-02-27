

$("#IncidenciasAguinaldoAutomaticas").change(function () {
    var sino = false;
    if (this.checked)
        sino = true;

    $.ajax({
        type: "Post",
        url: "getAguinaldoAutomatico",
        dataType: "json",
        data: { AguinaldoSINO: sino },
        async: false,        
        success: function (json) {
            if (json == "Exito") {
                mensajeAlerta_("Se cambio la configuración de aguinaldos", "pink");
            }
        },
        failure: function (response) {
            mensajeAlerta_(response, "danger");
        }

    });
    
});

function mensajeAlerta_(mensaje, tipo) {
    $.niftyNoty({
        type: tipo,
        container: 'floating',
        message: mensaje,
        closeBtn: false,
        floating: {
            position: 'top-right',
            animationIn: "jellyIn",
            animationOut: "shake"
        },
        focus: true,
        timer: 4500
    });
}

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
        url: 'UploadFiles',
        processData: false,
        contentType: false,
        data: fileData,
        success: function (result) {
            QuitarCargando();
            if (result.result) {
                mensajeAlerta("Atencion!", "se actualizaron los datos de proyección mediante el layout.", "success", "jelly", "fadeOut", 2500);
                $("#modalCargarSueldosProy").modal('hide');
                
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 2500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
            QuitarCargando();
        }
    });
});

$("#quitarDatosProy").click(function () {
       

    $.ajax({
        type: "POST",
        url: 'LimpiarDatos',        
        success: function (result) {
            QuitarCargando();
            if (result.result) {
                mensajeAlerta("Atencion!", "se actualizaron los datos de proyección.", "success", "jelly", "fadeOut", 2500);
                $("#modalQuitarDatos").modal('hide');
                
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 2500);                
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
            QuitarCargando();
        }
    });
});


function QuitarCargando() {
    $.hideLoading({
        name: 'circle-fade'
    });
}


$("#configurarAjuste").click(function () {
    var IdPeriodoNomina = $(this).attr('data-id');
    $.ajax({
        type: 'POST',
        url: 'getDatosConfAjuste',
        data: { IdPeriodoNomina },
        dataType: 'json',
        success: function (result) {
            if (result.result == "Ok") {
                console.log(result.datos);
                $("#modalConfAjuste").modal('show');
                
                $("#_selectPeriodo").empty();
                $("#_selectPeriodo").append($("<option></option>").val('0').html('Elegir...'));
                $.each(result.datos.lSPeriodos, function () {
                    $("#_selectPeriodo").append($("<option></option>").val(this['Value']).html(this['Text']));
                });

                $("#_empleadosSinAjuste").val(result.datos.EmpleadosSinAjuste);
                $("#_idsPeriodsAjuste").val(result.datos.PeriodosAjusteSecundario);
                $("#_peridoosAjuste").val(result.datos.descPeriodos);
            } else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 2500);
            }    
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });    
});

$("#btnAdd").click(function () {
    var nuevo = $("#_selectPeriodo").val();
    var nuevoDesc = $("#_selectPeriodo option:selected").text();

    if (nuevo != '0') {
        var actual = $("#_idsPeriodsAjuste").val();
        var nuevo_ = "";
        if (actual.length > 0) {
            nuevo_ = actual + "," + nuevo;
        } else {
            nuevo_ = nuevo;
        }

        $("#_idsPeriodsAjuste").val(nuevo_);

        var actualDesc = $("#_peridoosAjuste").val();
        var nuevoDesc_ = "";
        if (actualDesc.length > 0) {
            nuevoDesc_ = actualDesc + "," + nuevoDesc;
        } else {
            nuevoDesc_ = nuevoDesc;
        }

        $("#_peridoosAjuste").val(nuevoDesc_);
    }
    else {
        mensajeAlerta("Atencion!", "Debe elegir un periodo valido", "danger", "jelly", "fadeOut", 2500);
    }
    
});

$("#btnLimpiar").click(function () {
    $("#_idsPeriodsAjuste").val('');
    $("#_peridoosAjuste").val('');
});

$("#_guardarConfAjuste").click(function () {
    var empleados = $("#_empleadosSinAjuste").val();
    var IdPeriodoNomina = $(this).attr('data-id');
    var IdsPeriodosAjuste = $("#_idsPeriodsAjuste").val();
    
    $.ajax({
        type: 'POST',
        url: 'guardaConfiguracionAjuste',
        data: { IdPeriodoNomina, empleados, IdsPeriodosAjuste },
        dataType: 'json',
        success: function (result) {
            if (result.result == "Ok") {
                mensajeAlerta("Atencion!", result.mensaje, "mint", "jelly", "fadeOut", 2500);
                $("#modalConfAjuste").modal('hide');
                QuitarCargando();
            } else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 2500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });    
});

$("#_pagarNetos").click(function () {
  $("#modalCargarSueldosNetos").modal('show');
});

$("#ActualizaNetos").click(function () {
  var valores = "";
  $.showLoading();
  $("#modalCargarSueldosNetos").modal('hide');
  $("._body").find("tr").each(function () {
    $(this).find("td").each(function () {
      if ($(this).attr("class") == "_idEmp") {
        valores += $(this).html().trim() + ":";
      }

      if ($(this).attr("class") == "_netoEmps") {
        valores += $(this).children("._neto").val() + ",";
      }
    });    
  });

  $.ajax({
    type: 'POST',
    url: 'ActualizaNetos',
    data: { valores },
    dataType: 'json',
    success: function (data) {
      $.hideLoading();
      if (data.result == "Ok") {
        mensajeAlerta("Atencion!", data.mensaje, "mint", "jelly", "fadeOut", 2500);
      }
      else {
        mensajeAlerta("Atencion!", data.mensaje, "danger", "jelly", "fadeOut", 2500);
      }
    },
    error(er) {
      $.hideLoading();
      mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
    }
  });
});
