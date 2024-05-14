
function CambiaStatus(IdCredito) {
    $.ajax({
        type: 'POST',
        url: 'Infonavit/CambiaStatusCredito',
        data: { IdCredito: IdCredito },
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
        $("#TituloModal").text('Activar créditos');
        $("#EtiquetaModal").text('¿Estás seguro que quieres activar todos los créditos?');
    } else {
        $("#TituloModal").text('Desactivar créditos');
        $("#EtiquetaModal").text('¿Estás seguro que quieres desactivar todos los créditos?');
    }
}

function MovimientoCreditos() {
    const tipoMov = $("#TipoMovimiento").val();
    $.ajax({
        type: 'POST',
        url: 'Infonavit/Desactivacreditos',
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