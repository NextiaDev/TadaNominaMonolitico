$("#_select").change(function(){
    
    var idPeriodo = $(this).val();
    var periodo = $(this).find("option:selected").text();
    var texto = $("#_periodos").val();
    var textoIds = $("#IdsPeriodo").val();
    var periodos = "";
    var idsPeriodos = "";

    if (texto.length > 0) {
        periodos = texto + "," + periodo;        
    }
    else {
        periodos = periodo;
    }

    if (textoIds.length > 0) {
        idsPeriodos = textoIds + "," + idPeriodo;
    }
    else {
        idsPeriodos = idPeriodo;
    }
    
    $("#_periodos").val(periodos);
    $("#IdsPeriodo").val(idsPeriodos);
});

function eliminar(_id) {
    console.log(_id);
    $("#idCosteo").val(_id);
}

function eliminarConceptos(_id, _id2) {
    console.log(_id);
    console.log(_id2);
    $("#idCosteosConcepto").val(_id);
    $("#idCosteo").val(_id2);
}

function eliminarConceptosconfig(_id) {
    console.log(_id);
    $("#idCosteoConceptoConfiguracion").val(_id);
}
