$(function () {
    $(".anchorDetail").click(function () {
        debugger;
        var $buttonClicked = $(this);
        var id = $buttonClicked.attr('data-id');
        var options = { "backdrop": "static", keyboard: true };

        $.ajax({
            type: "GET",
            url: 'PensionAlimenticia/Details',
            contentType: "application/json; charset=utf-8",
            data: { id },
            datatype: "json",
            success: function (data) {
                debugger;
                $('#myModalContent').html(data);
                $('#myModal').modal(options);
                $('#myModal').modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });

    $("#closbtn").click(function () {
        $('#myModal').modal('hide');
    });
});

$(function () {
    $(".anchorDelete").click(function () {
        debugger;
        var $buttonClicked = $(this);
        var id = $buttonClicked.attr('data-id');
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: 'PensionAlimenticia/Delete',
            contentType: "application/json; charset=utf-8",
            data: { id },
            datatype: "json",
            success: function (data) {
                debugger;
                $('#myModalContent').html(data);
                $('#myModal').modal(options);
                $('#myModal').modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });

    $("#closbtn").click(function () {
        $('#myModal').modal('hide');
    });
});

function CambiaStatus(IdPension) {
    $.ajax({
        type: 'POST',
        url: 'PensionAlimenticia/CambiaStatusCredito',
        data: { IdPension: IdPension },
        datatype: 'json',
        success: function (data) {
            if (data == "OK") {
                mensajeAlerta("Atencion!", "Movimiento realizado correctamente.", "mint", "bounce", "fadeOut", 2000);
            }
            else {
                mensajeAlerta("Atencion!", data, "danger", "bounce", "fadeOut", 2100);
            }
        }
    });
}

    
function ModalDesactivarCreditos(tipoMovimiento) {
    $("#modDesactivaCreditos").modal('show');
    $("#TipoMovimiento").val(tipoMovimiento);

    if (tipoMovimiento == 1) {
        $("#TituloModal").text('Activar pensiones');
        $("#EtiquetaModal").text('¿Estás seguro que quieres activar todas las pensiones?');
    } else {
        $("#TituloModal").text('Desactivar pensiones');
        $("#EtiquetaModal").text('¿Estás seguro que quieres desactivar todas las pensiones?');
    }
}

function MovimientoCreditos() {
    const tipoMov = $("#TipoMovimiento").val();
    $.ajax({
        type: 'POST',
        url: 'PensionAlimenticia/DesactivaPension',
        data: { tipoMov: tipoMov },
        datatype: 'json',
        success: function (data) {
            if (data == "OK") {
                mensajeAlerta("Atencion!", "Movimiento realizado correctamente.", "mint", "bounce", "fadeOut", 2000);
                setTimeout(function () {
                    window.location.reload();
                }, 2500);
            }
            else {
                mensajeAlerta("Atencion!", data, "danger", "bounce", "fadeOut", 2100);
                setTimeout(function () {
                    window.location.reload();
                }, 2500);
            }
        }
    });
}

function CierraModal() {
    $("#modDesactivaCreditos").modal('hide');
    $("#TipoMovimiento").val(null);
    $("#TituloModal").text('');
    $("#EtiquetaModal").text('');
}