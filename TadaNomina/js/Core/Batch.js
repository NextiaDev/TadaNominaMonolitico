///Configuracion datatable para view Batch
$("#postedFile").change(function (e) {
    var fileName = $("#postedFile").val().replace(/D:\TadaNomina\fakepath\\/i, '');
    $("#nameFile").text("Archivo seleccionado: " + fileName);
    $('#upload').prop('disabled', false);
});

$('.btn-success').on('click', function () {
    var btn = $(this).button('loading')

    var doSomething = setTimeout(function () {
        clearTimeout(doSomething);
        btn.button('reset')
    }, 2000);

});

$('.btn-primary').on('click', function () {
    var btn = $(this).button('loading')

    var doSomething = setTimeout(function () {
        clearTimeout(doSomething);
        btn.button('reset')
    }, 2000);

});

$(".descargaCat").click(function () {
    var tipo = $(this).attr("id");
    $.ajax({
        type: 'POST',
        url: 'DescargarCC',
        dataType: 'json',
        data: { tipo },
        success: function (data) {
            var d = JSON.parse(data);
            $("#dvjson").excelexportjs({
                containerid: "dvjson",
                datatype: 'json',
                dataset: d,
                columns: getColumns(d),
                worksheetName: "Catalogos"
            });
        }
    });
});