function MoAttBook(modelo) {
    var Table = document.getElementById("Registros");
    Table.innerHTML = ``;
    Table.innerHTML = `<tr>
                            <th align="center" width="30%">Ubicación registro</th>
                            <th align = "center" width = "30%">Tipo</th>
                            <th align= "center" width= "30%">Hora</th>
                       </tr>`;

    var count = modelo.length;
    for (var i = 0; i < count; i++) {
        document.getElementById("Registros").insertRow().innerHTML = "<td>" + modelo[i].GroupDescription + "</td><td>Registro</td><td>" + modelo[i].Date.substring(8, 10) + ":" + modelo[i].Date.substring(10, 12) + "</td>";
    }
}

function MoTimeOff(modelo) {
    var Table = document.getElementById("TimeOffs");
    Table.innerHTML = ``;
    Table.innerHTML = `<tr>
                            <th align="center">Tipo Permiso</th>
                            <th align="center" width="26%">Fecha Inicio</th>
                            <th align="center" width="26%">Fecha Fin</th>
                       </tr>`;
    var count = modelo.length;
    for (var i = 0; i < count; i++) {
        document.getElementById("TimeOffs").insertRow().innerHTML = "<td>" + modelo[i].TimeOffTypeDescription + "</td><td>" + modelo[i].Starts.substring(6, 8) + "/" + modelo[i].Starts.substring(4, 6) + "/" + modelo[i].Starts.substring(0, 4) + "</td><td>" + modelo[i].Ends.substring(6, 8) + "/" + modelo[i].Ends.substring(4, 6) + "/" + modelo[i].Ends.substring(0, 4) + "</td>";
    }
}