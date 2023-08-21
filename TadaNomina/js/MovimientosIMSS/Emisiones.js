function onSuccessNuevo(title, message) {
  $.niftyNoty({
    type: "success",
    container: "floating",
    title: title,
    message: message,
    closeBtn: true,
    timer: 5000,
  });
}

function onDangerNuevo(title, message) {
  $.niftyNoty({
    type: "danger",
    container: "floating",
    title: title,
    message: message,
    closeBtn: true,
    timer: 5000,
  });
}

function Valida() {
  document.getElementById("TE").disabled = true;
  document.getElementById("Validar").disabled = true;

  var idregistro = $("#idregistro").val();

  var tipoemision = $("#TE").val();

  $.ajax({
    type: "POST",
    url: "Validar",
    data: JSON.stringify({
      IdRegistro: idregistro,
      _tipoEmision: tipoemision,
    }),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (datos) {
      if (datos.indexOf("ERROR:") !== -1) {
        onDangerNuevo("ERROR", datos);
        document.getElementById("TE").disabled = false;
        document.getElementById("Validar").disabled = false;
        $("#Descargar").hide();
      } else {
        onSuccessNuevo("CORRECTO", datos);
        $("#Descargar").show();
        document.getElementById("TE").disabled = false;
        document.getElementById("Validar").disabled = false;
      }
    },
    error: function (error) {
      onDangerNuevo("ERROR", error.message);
    },
  });
}

$(document).ready(function () {
  $("#TE").change(function () {
    $("#Descargar").hide();
  });
});