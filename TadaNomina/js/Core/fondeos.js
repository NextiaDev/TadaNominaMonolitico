$("#IdCosteo").change(function () {
    var IdCosteo = $("#IdCosteo").val();
    $.ajax({
        type: 'POST',
        url: 'getListas',
        data: { IdCosteo },
        dataType: 'json',
        success: function (result) {

            $("#IdPatrona").empty();
            $.each(result.lPatrona, function () {
                $("#IdPatrona").append($("<option></option>").val(this['Value']).html(this['Text']));
            });

            $("#IdDivision").empty();
            $.each(result.lDivision, function () {
                $("#IdDivision").append($("<option></option>").val(this['Value']).html(this['Text']));
            });
        }
    });
});

$("#IdsConcetos").dblclick(function () {
    var item = $('#IdsConcetos option:selected').appendTo("#IdsConceptosSelec");
});

$("#IdsConceptosSelec").dblclick(function () {
    var item = $('#IdsConceptosSelec option:selected').appendTo("#IdsConcetos");
});

$("#IdPeriodoNomina").change(function () {    
    $("form").submit();
});

