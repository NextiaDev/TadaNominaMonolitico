const BancoSelect = document.getElementById('IdBanco');

BancoSelect.addEventListener('change', validaBanco);

function validaBanco() {
    delRequeridos();
    ocultarAlerta();
    const botonEnv = document.getElementById('btn_G');
    botonEnv.setAttribute('disabled', 'disabled');
    const idBanco = document.getElementById('IdBanco').value;
    if (idBanco != '') {
        const peticion = fetch(`getValidacionBanco?IdBanco=${idBanco}`);
        peticion.then(function (response) {
            if (response.ok) {
                peticion
                    .then((resp) => resp.json())
                    .then((data) => {
                        validaPeticion(data);
                    });
            } else {
                console.log("entra else");
                const peticion2 = fetch(`ArchivotxtDispersion/getValidacionBanco?IdBanco=${idBanco}`);
                peticion2
                    .then((resp) => resp.json())
                    .then((data) => {
                        validaPeticion(data);
                    });
            }
        });
    };
};

function validaPeticion(respuesta) {
    if (respuesta === 'OK') {
        const botonEnv = document.getElementById('btn_G');
        botonEnv.removeAttribute('disabled');
        mostrarFormulario();
    }
    else {
        let alerta = document.getElementById('alert');
        document.getElementById('msj_ctnt').innerText = "Por el momento no se puede generar el archivo del banco seleccionado";
        alerta.removeAttribute('hidden');
        ocultarForms();
    }
};

function mostrarFormulario() {
    let idBancoSelect = document.getElementById('IdBanco').value;
    let formularioByS = document.getElementById('form_BBVA-STDR');
    let formularioBNX = document.getElementById('form_BNX');
    let formularioBnte = document.getElementById('form_BNTE');
    let formularioBajio = document.getElementById('form_BAJIO');
    switch (idBancoSelect) {
        case '-1':
            console.log('TODOS');
            ocultarForms();
            formularioByS.removeAttribute('hidden');
            formularioBNX.removeAttribute('hidden');
            formularioBajio.removeAttribute('hidden');
            addRequeridos();
            break;
        case '1':
            console.log('BANAMEX');
            ocultarBYSform();
            limpiarBNX();
            formularioBNX.removeAttribute('hidden');
            break;
        case '4':
            console.log('BBVA');
            ocultarBNXform();
            limpiarBYS();
            formularioByS.removeAttribute('hidden');
            break;
        case '5':
            console.log('SANTANDER');
            ocultarBNXform();
            limpiarBYS();
            formularioByS.removeAttribute('hidden');
            break;
        case '18':
            console.log('BANORTE');
            ocultarBNTEform();
            limpiarBNTE();
            formularioBnte.removeAttribute('hidden');
            break;
        case '8':
            console.log('BAJIO');
            ocultarBAJIOform();
            limpiarBAJIO();
            formularioBAJIO.removeAttribute('hidden');
            break;
        default:
            ocultarForms();
            break;
    }
}

function ocultarForms() {
    ocultarBYSform();
    ocultarBNXform();
    ocultarBNTEform();
    ocultarBAJIOform();
}

function ocultarBYSform() {
    let formByS = document.getElementById('form_BBVA-STDR');
    delRequeridoBYS();
    limpiarBYS();
    formByS.setAttribute('hidden', 'hidden');
}

function ocultarBNXform() {
    let formBNX = document.getElementById('form_BNX');
    delRequeridoBNX();
    limpiarBNX();
    formBNX.setAttribute('hidden', 'hidden');
}

function ocultarBNTEform() {
    let formBNTE = document.getElementById('form_BNTE');
    delRequeridoBNTE();
    limpiarBNTE();
    formBNTE.setAttribute('hidden', 'hidden');
}

function ocultarBAJIOform() {
    let formBAJIO = document.getElementById('form_BAJIO');
    delRequeridoBAJIO();
    limpiarBAJIO();
    formBAJIO.setAttribute('hidden', 'hidden');
}

function addRequeridoBYS() {
    let Tipoarchivo = document.getElementById('TA');
    Tipoarchivo.setAttribute('required', 'required');
}

function addRequeridoBNX() {
    let NumCliente = document.getElementById('NC');
    NumCliente.setAttribute('required', 'required');
    let CvlSuc = document.getElementById('CS');
    CvlSuc.setAttribute('required', 'required');
    let RefNum = document.getElementById('RN');
    RefNum.setAttribute('required', 'required');
    let RefAl = document.getElementById('RA');
    RefAl.setAttribute('required', 'required');
}

function addRequeridoBNTE() {
    let Empresa = document.getElementById('E');
    Empresa.setAttribute('required', 'required');
    let TipoArchivo = document.getElementById('TA');
    TipoArchivo.setAttribute('required', 'required');
}

function addRequeridoBAJIO() {
    let Tipoarchivo = document.getElementById('TA');
    Tipoarchivo.setAttribute('required', 'required');
}

function addRequeridos() {
    addRequeridoBNX();
    addRequeridoBYS();
    addRequeridoBNTE();
    addRequeridoBAJIO();
}

function delRequeridoBYS() {
    let Tipoarchivo = document.getElementById('TA');
    Tipoarchivo.removeAttribute('required');
}

function delRequeridoBNTE() {
    let TipoArchivo = document.getElementById('TA');
    TipoArchivo.removeAttribute('required');
}

function delRequeridoBNX() {
    let NumCliente = document.getElementById('NC');
    NumCliente.removeAttribute('required');
    let CvlSuc = document.getElementById('CS');
    CvlSuc.removeAttribute('required');
    let RefNum = document.getElementById('RN');
    RefNum.removeAttribute('required');
    let RefAl = document.getElementById('RA');
    RefAl.removeAttribute('required');
}

function delRequeridoBAJIO() {
    let Tipoarchivo = document.getElementById('TA');
    Tipoarchivo.removeAttribute('required');
}

function delRequeridos() {
    delRequeridoBYS();
    delRequeridoBNX();
    delRequeridoBNTE();
    delRequeridoBAJIO();
}

function limpiarBYS() {
    document.getElementById('TA').value = '';
}

function limpiarBNX() {
    document.getElementById('NC').value = '';
    document.getElementById('CS').value = '';
    document.getElementById('RN').value = '';
    document.getElementById('RA').value = '';
    document.getElementById('NE').value = '';
}

function limpiarBNTE() {
    document.getElementById('TA').value = '';
}

function limpiarBAJIO() {
    document.getElementById('TA').value = '';
}

function ocultarAlerta() {
    let alertaO = document.getElementById('alert');
    alertaO.setAttribute('hidden', 'hidden');
}