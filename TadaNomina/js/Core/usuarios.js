
$("#cargarFoto").click(function () {
    var fileUpload = $("#fileFoto").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();

    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }

    $.ajax({
        type: "POST",
        url: 'UploadFoto',
        processData: false,
        contentType: false,
        data: fileData,
        success: function (result) {
            if (result.result) {
                window.location.reload();
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 2500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });
});

$("#quitarFoto").click(function () {

    $.ajax({
        type: "POST",
        url: 'quitarFoto',              
        success: function (result) {
            if (result.result) {
                window.location.reload();
            }
            else {
                mensajeAlerta("Atencion!", result.mensaje, "danger", "jelly", "fadeOut", 2500);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2500);
        }
    });
});