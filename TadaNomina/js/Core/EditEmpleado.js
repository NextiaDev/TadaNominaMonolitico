$(document).ready(function () {
    if ($("#PremioP").val()) {
        $("#PremioP").prop("disabled", false);
    }
});

function ValidaDepartamentos(IdDepto) {
    if (IdDepto == "AAA") {
        if (model != null) {
            if (model.DepartamentoList) {
                var tamanolista = model.DepartamentoList.length;
                var ultimoElem = model.DepartamentoList[tamanolista - 1].Text;
                var busqueda = null;
                busqueda = ultimoElem.search("-Inactivo-");
                if (busqueda != -1) {
                    $("#lblDepartamento").show('true');
                }
            }
        }
    } else {
        if (model.IdDepartamento == IdDepto) {
            alert("El departamento que quieres seleccionar está inactivo, favor de seleccionar otro.")
            $("#IdDepartamento").val(null);
            $("#lblDepartamento").show('true');
        } else {
            $("#lblDepartamento").hide('true');
        }
    }
}

function ValidaPuesto(IdPuesto) {
    if (IdPuesto == "AAA") {
        if (model != null) {
            if (model.PuestosList) {
                var tamanolista = model.PuestosList.length;
                var ultimoElem = model.PuestosList[tamanolista - 1].Text;
                var busqueda = null;
                busqueda = ultimoElem.search("-Inactivo-");
                if (busqueda != -1) {
                    $("#lblPuesto").show('true');
                }
            }
        }
    } else {
        if (model.IdPuesto == IdPuesto) {
            alert("El Puesto que quieres seleccionar está inactivo, favor de seleccionar otro.")
            $("#IdPuesto").val(null);
            $("#lblPuesto").show('true');

        } else {
            $("#lblPuesto").hide('true');
        }
    }
}

function ValidaArea(IdArea) {
    if (IdArea == "AAA") {
        if (model != null) {
            if (model.AreaList) {
                var tamanolista = model.AreaList.length;
                var ultimoElem = model.AreaList[tamanolista - 1].Text;
                var busqueda = null;
                busqueda = ultimoElem.search("-Inactivo-");
                if (busqueda != -1) {
                    $("#lblArea").show('true');
                }
            }
        }
    } else {
        if (model.idArea == IdArea) {
            alert("El Área que quieres seleccionar está inactiva, favor de seleccionar otra.")
            $("#IdArea").val(null);
            $("#lblArea").show('true');

        } else {
            $("#lblArea").hide('true');
        }
    }
}

function ValidaCC(IdCC) {
    if (IdCC == "AAA") {
        if (model != null) {
            if (model.CentrosCostosList) {
                var tamanolista = model.CentrosCostosList.length;
                var ultimoElem = model.CentrosCostosList[tamanolista - 1].Text;
                var busqueda = null;
                busqueda = ultimoElem.search("-Inactivo-");
                if (busqueda != -1) {
                    $("#lblCC").show('true');
                }
            }
        }
    } else {
        if (model.IdCentroCostos == IdCC) {
            alert("El Centro de costos que quieres seleccionar está inactivo, favor de seleccionar otro.")
            $("#IdCC").val(null);
            $("#lblCC").show('true');

        } else {
            $("#lblCC").hide('true');
        }
    }
}

function ValidaSucursal(IdSucursal) {
    if (IdSucursal == "AAA") {
        if (model != null) {
            if (model.SucursalList) {
                var tamanolista = model.SucursalList.length;
                var ultimoElem = model.SucursalList[tamanolista - 1].Text;
                var busqueda = null;
                busqueda = ultimoElem.search("-Inactivo-");
                if (busqueda != -1) {
                    $("#lblSucursal").show('true');
                }
            }
        }
    } else {
        if (model.IdSucursal == IdSucursal) {
            alert("La sucursal que quieres seleccionar está inactiva, favor de seleccionar otra.");
            $("#IdSucursal").val(null);
            $("#lblSucursal").show('true');

        } else {
            $("#lblSucursal").hide('true');
        }
    }
}

$("#ApellidoPaterno").on("paste keyup", function (event) {
    if (idCliente == 172 || idCliente == 286 || idCliente == 285 || idCliente == 287 || idCliente == 284 || idCliente == 282 || idCliente == 283) {
        let apellidoPaterno = $(this).val();
        if (apellidoPaterno.length > 0) {
            generaClaveEmpleado();
        } else if (apellidoPaterno.length == 0) {
            $("#ClaveEmpleado").val(claveEmpleadoOriginal);
            $(this).val(apellidoPaternoOriginal);
        }
    }
});

function Premio(valor) {
    if (valor.checked == true) {
        document.getElementById('PremioP').disabled = false;
    } else {
        document.getElementById('PremioP').checked = false;
        document.getElementById('PremioP').disabled = true;
    }
}