
$(function () {
    $('.calendar').datepicker({
        format: 'dd/mm/yyyy'
    });
});

/****************************************/
//Metodos para view Edit
//Data table
$("#searchEmp").click(function () {

    $("#errorSearch").text("");

    var clave = $("#clave").val();
    var name = $("#name").val();

    if ((clave == null || clave == "") && (name == null || name == "")) {
        AllEmpleados();
    }
    else if ((clave != null || clave != "") && (name == null || name == "")) {
        EmpleadosByClave(clave);
    }
    else if ((clave == null || clave == "") && (name != null || name != "")) {
        EmpleadosByName(name);
    }
    else {
        $("#errorSearch").text("Selecciona sola una opción");
        $("#tableSearch").fadeOut();
    }
});

//Funcion para obtener empledos general


//Funcion para obtener empledos pos clave
function EmpleadosByClave(clave) {
    var rowSelection = $('#dataEmp').DataTable({
        destroy: true,
        responsive: true,
        "ajax": {
            "data": { clave: clave },
            "url": "GetEmpleadosByClaveBaja",
            "type": "POST",
            "dataType": "json"
           
        },
        
        "columns": [
            { "data": "ClaveEmpleado" },
            { "data": "ApellidoPaterno" },
            { "data": "ApellidoMaterno" },
            { "data": "Nombre" },
            { "data": "Rfc" },
            { "data": "Curp" },
            { "data": "Imss" },
            { "data": "FechaBaja" },
            {
                "render": function (data, type, row) {
                    return '<a href="#" class="btn btn-warning add-tooltip" title="Reactivar Empleado" onclick="Edit(' + row.IdEmpleado + ')"><i class="fa fa-history"></i> Reactivar</a> <a href="#" class="btn btn-primary add-tooltip" title="Actualizar Info" onclick="Edit1(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i> Actualizar</a> '
                },
                "orderable": false
            }
        ],
        "language": {
            "paginate": {
                "previous": '<i class="demo-psi-arrow-left"></i>',
                "next": '<i class="demo-psi-arrow-right"></i>'
            }
        }
    });

 

    $('#dataEmp').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            rowSelection.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });

    $("#tableSearch").fadeIn();
}

//Funcion para obtener empledos pos clave
function EmpleadosByName(name) {
    var rowSelection = $('#dataEmp').DataTable({
        destroy: true,
        responsive: true,
        "ajax": {
            "data": { name: name },
            "url": "GetEmpleadosByNombreBaja",
            "type": "POST",
            "dataType": "json"
        },
        "columns": [
            { "data": "ClaveEmpleado" },
            { "data": "ApellidoPaterno" },
            { "data": "ApellidoMaterno" },
            { "data": "Nombre" },
            { "data": "Rfc" },
            { "data": "Curp" },
            { "data": "Imss" },
            { "data": "FechaBaja" },
            {
                "render": function (data, type, row) {
                    return '<a href="#" class="btn btn-warning" onclick="Edit(' + row.IdEmpleado + ')"><i class="fa fa-history add-tooltip" title="Reactivar Empleado"></i> Reactivar</a> <a href="#" class="btn btn-primary add-tooltip" title="Actualizar Info" onclick="Edit1(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i> Actualizar</a> '
                },
                "orderable": false
            }
        ],
        "language": {
            "paginate": {
                "previous": '<i class="demo-psi-arrow-left"></i>',
                "next": '<i class="demo-psi-arrow-right"></i>'
            }
        }
    });


    $('#dataEmp').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            rowSelection.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });

    $("#tableSearch").fadeIn();
}

//Redireccionamiento con parametro cifrado
function Edit(id) {
    $.ajax({
        type: 'POST',
        url: 'EncodeParam',
        dataType: 'text',
        data: { param: id },
        success: function (data) {
            var url = "EmpleadosReactivacion?data=" + data;
            $(location).attr('href', url);
        }
    });
}

function Edit1(id) {
    $.ajax({
        type: 'POST',
        url: 'EncodeParam',
        dataType: 'text',
        data: { param: id },
        success: function (data) {
            var url = "EmpleadosEditarInfoBaja?data=" + data;
            $(location).attr('href', url);
        }
    });
}

/*Metodos para view registra empleado*/
$("#CodigoPostal").keyup(function () {

    $("#errorCode").text("");
    $("#errorCode").hide();
    var cp = $("#CodigoPostal").val().trim();

    if (cp.length > 0) {
        $.ajax({
            type: 'POST',
            url: 'GetCodigosPostales',
            dataType: 'json',
            data: { cp: cp },
            success: function (data) {
                eval(data);
                var cps = new Array();

                $.each(data, function (i, data) {
                    cps.push(data.Codigo);
                });

                $("#CodigoPostal").autocomplete({
                    source: cps
                });

            }
        });
    }
});

//metodo para obtener codigo postal
$("#searchCode").click(function () {
    var cp = $("#CodigoPostal").val();

    //reinitialize components
    $("#errorCode").text("");
    $("#errorCode").hide();
    $("#IdCodigoPostal").empty();
    $("#IdCodigoPostal").prop("disabled", true);
    $("#Municipio").val("");
    $("#Entidad").val("");


    if (isNaN(cp)) {
        $("#errorCode").show();
        $("#errorCode").text("Inserta un código postal valido");
        return;
    }

    if (cp.length > 0) {
        $.ajax({
            type: 'POST',
            url: 'GetDetailCP',
            dataType: 'json',
            data: { cp: cp },
            success: function (data) {
                eval(data);
                if (data.length == 0) {
                    $("#errorCode").show();
                    $("#errorCode").text("No se encontró el código postal");
                }
                else if (data.length == 1) {
                    $.each(data, function (i, data) {
                        $("#IdCodigoPostal").append('<option value="' + data.Id + '">' + data.Colonia + '</option>');
                        $("#Municipio").val(data.Municipio);
                        $("#Entidad").val(data.Entidad);
                    });

                    $("#IdCodigoPostal").prop("disabled", false);
                }
                else if (data.length > 1) {
                    $.each(data, function (i, data) {
                        $("#IdCodigoPostal").append('<option value="' + data.Id + '">' + data.Colonia + '</option>');
                    });

                    $("#IdCodigoPostal").prop("disabled", false);
                }
            }
        });
    }
    else {
        $("#errorCode").show();
        $("#errorCode").text("Inserta un código postal valido");
    }
});

//metodo para listas de codigos postales
$("#IdCodigoPostal").change(function () {

    var data = $("#IdCodigoPostal").val();

    $.ajax({
        type: 'POST',
        url: 'GetCPByID',
        dataType: 'json',
        data: { data: data },
        success: function (data) {
            eval(data);
            console.log(data);
            $("#Municipio").val(data.Municipio);
            $("#Entidad").val(data.Entidad);
        }
    });
});

///Configuracion datatable para view Batch
$("#postedFile").change(function (e) {
    var fileName = $("#postedFile").val().replace(/C:\\fakepath\\/i, '');
    $("#nameFile").text("Archivo seleccionado: " + fileName);
    $('#upload').prop('disabled', false);
});

$("#estatus").change(function () {

    var idEstatus = $("#estatus").val();
    if (idEstatus == 2) {
        $("#fechaBaja").prop('disabled', false);
        $("#mBaja").prop('disabled', false);
        $("#recontratable").prop('disabled', false);
        //$("#tablaPer").show();
    }
    else {
        $("#fechaBaja").prop('disabled', true);
        $("#mBaja").prop('disabled', true);
        $("#recontratable").prop('disabled', true);
        //$("#tablaPer").hide();
    }

    //if (idEstatus != 1) {
    //    var idEmp = $("#_idEmp").val();
    //    $.ajax({
    //        type: 'POST',
    //        url: 'getPeriodosProcesados',
    //        dataType: 'json',
    //        data: { IdEmpleado: idEmp },
    //        success: function (data) {
    //            console.log(data);
    //        }
    //    });
    //}
});

$('.btn-success').on('click', function () {
    var btn = $(this).button('loading')

    var doSomething = setTimeout(function () {
        clearTimeout(doSomething);
        btn.button('reset')
    }, 2000);

});

$('.btn-primary').on('click', function () {
    var btn = $(this).button('loading')

    var doSomething = setTimeout(function () {
        clearTimeout(doSomething);
        btn.button('reset')
    }, 2000);

});

$(".descargaCat").click(function () {
    var tipo = $(this).attr("id");
    $.ajax({
        type: 'POST',
        url: 'DescargarCC',
        dataType: 'json',
        data: { tipo },
        success: function (data) {

            var d = JSON.parse(data);
            console.log(d)

            $("#dvjson").excelexportjs({
                containerid: "dvjson",
                datatype: 'json',
                dataset: d,
                columns: getColumns(d),
                worksheetName: "CentrosCostos"
            });
        }
    });
});

$("#SDIMSS").change(function () {
    var sdimss = $("#SDIMSS").val();
    var idEmpleado = $("#IdEmpleado").val();
    var fechaReconocimientoAntiguedad = $("#FechaReconocimientoAntiguedad").val();

    if (idEmpleado == undefined) {
        idEmpleado = 0;
    }

    $.ajax({
        type: "POST",
        url: "CalcularSdI",
        data: JSON.stringify(
            {
                'IdEmpleado': idEmpleado,
                'SDIMSS': sdimss,
                'FechaReconocimientoAntiguedad': fechaReconocimientoAntiguedad
            }
        ),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (datos) {
            $("#SDI").val(datos);
        },
        error: function (error) {
        }
    });
});

