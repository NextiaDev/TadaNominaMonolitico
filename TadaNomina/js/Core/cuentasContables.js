$("#IdTipoNomina").change(function () {
    var valor = $("#IdTipoNomina").val();

    if (valor.length > 0) {
        $.ajax({
            type: 'POST',
            url: 'getLitConceptos',
            data: { IdTipoNomina: valor },
            dataType: 'json',
            success: function (data) {
                eval(data);
                $("#Concepto").empty();
                $("#Concepto").append('<option value="">Elegir...</option>');

                if (valor == 1) {
                    $.each(data, function (i, data) {
                        $("#Concepto").append('<option value="' + data.idCatTipoNomina + '">' + data.nombre + '</option>');
                    });
                }

                if (valor == 2) {
                    $.each(data, function (i, data) {
                        $("#Concepto").append('<option value="' + data.IdConcepto + '">' + data.ClaveConcepto + " - " + data.DescripcionConcepto + '</option>');
                    });
                }
            }
        });
    }
});


function eliminar(_id, IdCuenta, Nivel, _Descripcion) {
    document.getElementById("_id").value = _id;
    document.getElementById("IdCuenta_").value = IdCuenta;
    document.getElementById("Nivel_").value = Nivel;
    document.getElementById("_Descripcion_").value = _Descripcion;
}
