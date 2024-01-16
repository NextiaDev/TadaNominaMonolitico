let unidadenegocio = [];
let listaclientesagregados = [];

function addcliente(idcliente) {
    let indice = listaclientesagregados.indexOf(idcliente);
    if (indice < 0) {
        listaclientesagregados.push(idcliente);
    }
    else {
        listaclientesagregados.splice(indice, 1);
    }
    let clientesvalue = listaclientesagregados.join(",");
    document.getElementById("clientesselect").value = clientesvalue;

}

function addunidad(idunidad) {
    let indice = unidadenegocio.indexOf(idunidad);
    if (indice < 0) {
        unidadenegocio.push(idunidad);
    }
    else {
        unidadenegocio.splice(indice, 1);
    }
    let unidadvalue = listaclientesagregados.join(",");
    document.getElementById("unidadselect").value = unidadvalue;

}