$(document).ready(function () {
    var mensaje = $("#msj").val();
    console.log(mensaje);    
    if (mensaje.length>0) {
        $("#ModalError").modal("show");
    } else {
        $("#ModalError").modal("hide");
    }
});

$("#searchEmp").click(function () {
  $("#errorSearch").text("");

  var imss = $("#imss").val();

  if (imss != "") {
    EmpleadosByNSS(imss);
  } else {
    $("#alerta1").removeAttr("hidden");
    $("#tableSearch").fadeOut();
  }
});


//Funcion para obtener empledos por NSS
function EmpleadosByNSS(imss) {
  var rowSelection = $("#dataEmp").DataTable({
    destroy: true,
    responsive: true,
    ajax: {
      data: { imss: imss },
      url: "GetEmpledoByIMSS",
      type: "POST",
      dataType: "json",
      },
    
    columns: [
      { data: "Lote" },
      { data: "NombrePatrona" },
      { data: "TipoMovimiento" },
      { data: "ApellidoPaterno" },
      { data: "ApellidoMaterno" },
      { data: "Nombre" },
      { data: "Imss" },
      { data: "FechaMovimiento" },
      { data: "FechaEnvio" },
      {
        render: function (data, type, row) {
              return (
                  '<a href="GetAfilIndividual?lote=' +
                  row.Lote +
                  "&Imss=" +
                  row.Imss +
                  '"class="btn btn-sm btn-danger" title="Descarga Acuse Individual"><i class="fa fa-file-pdf-o"></i></a>' +
                  '<a href="DescargaPDFRespuestaGeneral?IdRegistroPatronal=' +
                  row.IdRegistroPatronal +
                  "&Lote=" +
                  row.Lote +
                  '"class="btn btn-sm btn-primary" title = "Descarga Acuse del Lote" > <i class="fa fa-file-pdf-o"></i></a >' +
                  '<a href="DescargaMasivaAcuses?IMSS=' +
                  row.Imss +
                  '"class="btn btn-sm btn-info" title = "Descarga Emisiones"><i class="fa fa-file"></i></a>' +
                  '<a href="DescargaAfilesIndividuales?IMSS=' +
                  row.Imss +
                  '"class="btn btn-sm btn-success" title = "Descarga Acuses Individuales"><i class="fa fa-file"></i></a>'
              );
        },
        orderable: false,
      },
    ],
    language: {
      paginate: {
        previous: '<i class="demo-psi-arrow-left"></i>',
        next: '<i class="demo-psi-arrow-right"></i>',
      },
    },
  });

  $("#dataEmp").on("click", "tr", function () {
    if ($(this).hasClass("selected")) {
      $(this).removeClass("selected");
    } else {
      rowSelection.$("tr.selected").removeClass("selected");
      $(this).addClass("selected");
    }
  });

  $("#tableSearch").fadeIn();
}

$("#btn_cls_alert1").click(function () {
  $("#alerta1").attr("hidden", "hidden");
});
