$("#btnBUscar").click(function () {

    $.showLoading({
        name: 'circle-fade'
    });

    var date = $("#_date").val();

    $.ajax({
        type: 'POST',
        url: 'Logs/Logs',
        data: { date },
        dataType: 'json',
        success: function (result) {
            $.hideLoading();
            if (result.result == "Ok") {

                $("#_logs").html(result.log);
            }
            else {
                mensajeAlerta("Atencion!", result.Mensaje, "danger", "jelly", "fadeOut", 2100);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2100);
        }
    });
});

$("#btnBUscar2").click(function () {

    $.showLoading({
        name: 'circle-fade'
    });

    var date = $("#_date").val();

    $.ajax({
        type: 'POST',
        url: 'Logs/Logs',
        data: { date },
        dataType: 'json',
        success: function (result) {
            $.hideLoading();
            if (result.result == "Ok") {

                $("#_logs").html(result.log);
            }
            else {
                mensajeAlerta("Atencion!", result.Mensaje, "danger", "jelly", "fadeOut", 2100);
            }
        },
        error: function (er) {
            mensajeAlerta("Atencion!", er, "danger", "jelly", "fadeOut", 2100);
        }
    });
});