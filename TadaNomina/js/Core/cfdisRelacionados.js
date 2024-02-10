$("#_cancelaRelacionados").click(function () {
  var IdPeriodo = $("#IdPeriodoNomina").val();
  $.showLoading({ name: 'circle-fade' });
  $.ajax({
    type: 'POST',
    url: 'CancelaTimbradoTP/CancelaRelacionados',
    data: { IdPeriodo },
    dataType: 'json',
    success: function (data) {
      $.hideLoading();
      if (data.result == "Ok") {
        mensajeAlerta("Atencion!", data.mensaje, "success", "jelly", "fadeOut", 0);
      }
      else {
        mensajeAlerta("Atencion!", data.mensaje, "danger", "jelly", "fadeOut", 0);
      }
    },
    error: function (er) {
      mensajeAlerta("Atencion!", er.mensaje, "danger", "jelly", "fadeOut", 0);
      $.hideLoading();
    }
  });
});
