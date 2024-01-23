var inptregfis = document.getElementById('reginpt');

inptregfis.addEventListener('change', validatregimen)

function validatregimen() {
    var valorregimen = inptregfis.value;
    if (valorregimen == 602) {
        document.getElementById('curpinpt').setAttribute('required', true);
    }
    else {
        document.getElementById('curpinpt').removeAttribute('required');
    }
}