
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
function AllEmpleados() {
    var rowSelection = $('#dataEmp').DataTable({
        destroy: true,
        responsive: true,
        "ajax": {            
            "url": "GetEmpleados",
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
            {
                "render": function (data, type, row) {
                    return '<a href="#" onclick="Edit(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i></a>'
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
function EmpleadosByClave(clave) {
    var rowSelection = $('#dataEmp').DataTable({
        destroy: true,
        responsive: true,
        "ajax": {
            "data": {clave: clave},
            "url": "GetEmpleadosByClave",
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
            {
                "render": function (data, type, row) {
                    return '<a href="#" onclick="Edit(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i></a>'
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
            "data": {name: name},
            "url": "GetEmpleadosByNombre",
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
            {
                "render": function (data, type, row) {
                    return '<a href="#" onclick="Edit(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i></a>'
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
            var url = "Edit?data=" + data;            
            $(location).attr('href', url); 
        }
    });   
}

/*Metodos para view registra empleado*/
$("#CodigoPostal").keyup(function () {

    $("#errorCode").text("");
    $("#errorCode").hide();
    var cp = $("#CodigoPostal").val().trim();

    if( cp.length > 0) {
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
    var fileName = $("#postedFile").val().replace(/D:\TadaNomina\fakepath\\/i, '');
    $("#nameFile").text("Archivo seleccionado: " + fileName);    
    $('#upload').prop('disabled', false);
});

$("#estatus").change(function () {
        
    var idEstatus = $("#estatus").val();
    if (idEstatus == 2 || idEstatus == 3) {
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
                worksheetName: "Catalogos"
            });
        }
    });
});

$("#SDIMSS").blur(function () {
    var sdimss = $("#SDIMSS").val();
    var idEmpleado = $("#IdEmpleado").val();
    var idprestacion = $("#IdPrestaciones").val();
    var fechaReconocimientoAntiguedad = $("#FechaReconocimientoAntiguedad").val();

    if (idEmpleado==undefined) {
        idEmpleado = 0;
    }   

    $.ajax({
        type: "POST",
        url: "CalcularSdI",
        data: JSON.stringify(
            {
                'IdEmpleado': idEmpleado,
                'SDIMSS': sdimss,
                'FechaReconocimientoAntiguedad': fechaReconocimientoAntiguedad,
                'IdPrestaciones': idprestacion
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


$("#searchCodeFiscalNuevo").click(function () {
    console.log("EntraFiscal");
    var cp = $("#CodigoPostalFiscal").val();

    //reinitialize components
    $("#errorCodeFiscal").text("");
    $("#errorCodeFiscal").hide();
    $("#IdCodigoPostalFiscal").empty();
    $("#IdCodigoPostalFiscal").prop("disabled", true);
    $("#MunicipioFiscal").val("");
    $("#EntidadFiscal").val("");


    if (isNaN(cp)) {
        $("#errorCodeFiscal").show();
        $("#errorCodeFiscal").text("Inserta un código postal valido");
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
                    $("#errorCodeFiscal").show();
                    $("#errorCodeFiscal").text("No se encontró el código postal");
                }
                else if (data.length == 1) {
                    $.each(data, function (i, data) {
                        $("#IdCodigoPostalFiscal").append('<option value="' + data.Id + '">' + data.Colonia + '</option>');
                        $("#MunicipioFiscal").val(data.Municipio);
                        $("#EntidadFiscal").val(data.Entidad);
                    });

                    $("#IdCodigoPostalFiscal").prop("disabled", false);
                }
                else if (data.length > 1) {
                    $.each(data, function (i, data) {
                        $("#IdCodigoPostalFiscal").append('<option value="' + data.Id + '">' + data.Colonia + '</option>');
                    });

                    $("#IdCodigoPostalFiscal").prop("disabled", false);
                }
            }
        });
    }
    else {
        $("#errorCodeFiscal").show();
        $("#errorCodeFiscal").text("Inserta un código postal valido");
    }
});

//metodo para listas de codigos postales

$("#IdCodigoPostalFiscal").change(function () {

    var data = $("#IdCodigoPostalFiscal").val();

    $.ajax({
        type: 'POST',
        url: 'GetCPByID',
        dataType: 'json',
        data: { data: data },
        success: function (data) {
            eval(data);
            console.log(data);
            $("#MunicipioFiscal").val(data.Municipio);
            $("#EntidadFiscal").val(data.Entidad);
        }
    });
});
