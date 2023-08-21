$("#descarga").click(function () {
    $.ajax({
        type: 'POST',
        url: 'DescargaWS',
        dataType: 'json',
        success: function (data) {
            var d = JSON.parse(data);
            console.log(d)

            $("#dvjson").excelexportjs({
                containerid: "dvjson",
                datatype: 'json',
                dataset: d,
                columns: getColumns(d),
                worksheetName: "Poliza"
            });
        }
    });
});


$(function () {
    $("#btnSubmit").click(function () {
        
        const gridQueQuiero = $("#Grid").html();
        console.log(gridQueQuiero);


        $("input[name='GridHtml']").val(gridQueQuiero.trim());
    });
});

$("#_actualizarInfo").click(function () {
    var IdPeriodoNomina = $("#IdPeriodoNomina").val();
    if (IdPeriodoNomina.length > 0) {
        $.ajax({
            type: 'POST',
            url: 'ActualizarInfo',
            data: { IdPeriodoNomina },
            dataType: 'json',
            async: true,
            success: function (result) {
                if (result == "Exito") {
                    mensajeAlerta("Atencion!", "Se elimino la informacion de la poliza.", "mint", "jelly", "fadeOut", 3500);
                }
                else {
                    mensajeAlerta("Atencion!", data, "danger", "jelly", "fadeOut", 3500);
                }
            }
        });
    }
    
});