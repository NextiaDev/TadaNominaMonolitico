
///////////////////////////////////////////////////////////////////////////////////////////
/// Reloj checador
function Premio(valor) {
    if (valor.checked == true) {
        document.getElementById('PremioP').disabled = false;
    } else {
        document.getElementById('PremioP').checked = false;
        document.getElementById('PremioP').disabled = true;
    }
}
$(document).ready(function () {
    document.getElementById('PremioP').disabled = true;
});
/////////////////////////////////////////////////////////////////////////////////////////