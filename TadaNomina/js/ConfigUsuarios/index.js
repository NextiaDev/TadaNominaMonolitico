var inpiduser = document.getElementById('idUser');
var inputnombre = document.getElementById('inpt_nom');
var inputappat = document.getElementById('inpt_appat');
var inputapmat = document.getElementById('inpt_apmat');
var inputcorreo = document.getElementById('inpt_corr');
var inputusuario = document.getElementById('inpt_usu');
var inputpass = document.getElementById('inpt_pass');

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
    let unidadvalue = unidadenegocio.join(",");
    document.getElementById("unidadselect").value = unidadvalue;
}

function setvaluetextbox(idinput) {
    var inpclick = document.getElementById(idinput).value;
    var valorinp = inpclick == 'false' ? 'true' : 'false'
    document.getElementById(idinput).value = valorinp;
}


function setvaluetextboxTipo(idinput) {
    var inpclickd = document.getElementById(idinput).value;
    var valorinpd = inpclickd == 'false' ? 'true' : 'false'
    document.getElementById(idinput).value = valorinpd;
}

function EditarUser(idUsuario, Nombre, apellidoPat, apellidoMat, correo, usuario, clientes, unidades, accesnomina, rhcloud, imss, contabilidad, tesoreria) {
    inpiduser.value = idUsuario;
    inputnombre.value = Nombre;
    inputappat.value = apellidoPat;
    inputapmat.value = apellidoMat;
    inputcorreo.value = correo;
    inputusuario.value = usuario;

    var passdiv = document.getElementById('div_inpt_pass');
    passdiv.className = "form-group";
    passdiv.hidden = true;

    if (!!clientes) {
        document.getElementById('clientesselect').value = clientes;
        var listaclientestoedit = clientes.split(',');
        if (listaclientestoedit.length > 0) {
            for (var i = 0; i < listaclientestoedit.length; i++) {
                var idclietelist = listaclientestoedit[i];
                listaclientesagregados.push(parseInt(idclietelist));
                try { document.getElementById(`inpcl-${idclietelist}`).setAttribute('checked', 'true'); } catch { }
            }
        }
    }

    if (!!unidades) {
        document.getElementById('unidadselect').value = unidades;
        var listaunidadestoedit = unidades.split(',');
        if (listaunidadestoedit.length > 0) {
            for (var i = 0; i < listaunidadestoedit.length; i++) {
                var idunidad = listaunidadestoedit[i];
                unidadenegocio.push(parseInt(idunidad));
                document.getElementById(`inpun-${idunidad}`).setAttribute('checked', 'true');
            }
        }
    }

    var inptmodnom = document.getElementById('mod-Nom');
    if (!!accesnomina) {
        inptmodnom.setAttribute('checked', 'true');
        inptmodnom.value = true;
    }

    //var inptmodnrh = document.getElementById('mod-RH');
    //if (!!rhcloud) {
    //    inptmodnrh.setAttribute('checked', 'true');
    //    inptmodnrh.value = true;

    //}

    //var inptmodnimss = document.getElementById('mod-IMSS');
    //if (!!imss) {
    //    inptmodnimss.setAttribute('checked', 'true');
    //    inptmodnimss.value = true;
    //}

    //var inptmodconta = document.getElementById('mod-Conta');
    //if (!!contabilidad) {
    //    inptmodconta.setAttribute('checked', 'true');
    //    inptmodconta.value = true;
    //}

    //var inptmodteso = document.getElementById('mod-Teso');
    //if (!!tesoreria) {
    //    inptmodteso.setAttribute('checked', 'true');
    //    inptmodteso.value = true;
    //}

    $('#addUser').modal('show');
}

function OpenPanel(identificador) {
    var collapse = document.getElementById(`${identificador}`);
    var info_class = collapse.className;
    if (info_class == 'collapse') {
        collapse.className = 'collapse in';
        collapse.setAttribute("aria-expanded", "true");
        collapse.removeAttribute("style");
    }
    else {
        collapse.className = "collapse";
        collapse.removeAttribute("aria-expanded")
        collapse.setAttribute("style", "height: 0px;");
    }
}

function EliminarUsuario(idusuario) {
    document.getElementById('idusuariodel').value = idusuario;
    $('#ModalEliminar').modal('show');
}