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
            {
                "render": function (data, type, row) {
                    return '<a href="#" onclick="AddAusen(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i></a>'
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
            "data": { clave: clave },
            "url": "GetEmpleadosByClave",
            "type": "POST",
            "dataType": "json"
        },
        "columns": [
            { "data": "ClaveEmpleado" },
            { "data": "ApellidoPaterno" },
            { "data": "ApellidoMaterno" },
            { "data": "Nombre" },

            {
                "render": function (data, type, row) {
                    return '<a href="#" onclick="AddAusen(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i></a>'
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
            "url": "GetEmpleadosByNombre",
            "type": "POST",
            "dataType": "json"
        },
        "columns": [
            { "data": "ClaveEmpleado" },
            { "data": "ApellidoPaterno" },
            { "data": "ApellidoMaterno" },
            { "data": "Nombre" },
            {
                "render": function (data, type, row) {
                    return '<a href="#" onclick="AddAusen(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i></a>'
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

function AddAusen(id) {
    $.ajax({
        type: 'POST',
        url: 'EncodeParam',
        dataType: 'text',
        data: { param: id },
        success: function (data) {
            var url = "AddAusen?data=" + data;
            $(location).attr('href', url);
        }
    });
}


function myFunctioncuatro() {
    var x = document.getElementById("_Ausen").value;

    if (x != null) {
        $('#_idFolio').css("display", "none");
        $('#fechaapli').css("display", "block");
        $('#fechaInicial').css("display", "block");
        $('#iddias_').css("display", "block");
        $('#idsubsidiosgrup').css("display", "none");
        $('#idSubsidio').css("display", "block");

        $('#txtIncapacidad').css("display", "none");
        $('#formatoid').css("display", "block");
        $('#iddiasApli_').css("display", "block");
    }
    else {
        $('#Folio').css("display", "none");
        $('#idsubsidiosgrup').css("display", "none");

    }
}

function myFunctiontres() {
    var x = document.getElementById("_incapacidadid").value;

    if (x != null) {
        $('#idSubsecuente').css("display", "block");


    }
    else {
        $('#idSubsecuente').css("display", "none");

    }  
}

function myFunctiondos() {
    var x = document.getElementById("mySelectdos").value;
    if (x == "Incapacidad") {
        $('#_incapacidad option').prop('selected', function () {
            return this.defaultSelected;
        })
        $('#_incapacidad').css("display", "block");
        $('#_ausen').css("display", "none");
        $('#_idFolio').css("display", "none");
        $('#fechaapli').css("display", "none");
        $('#fechaInicial').css("display", "none");
        $('#iddias_').css("display", "none");
        $('#idsubsidiosgrup').css("display", "none");
        $('#idSubsidio').css("display", "none");

        $('#txtIncapacidad').css("display", "none");
        $('#formatoid').css("display", "none");





    }
    else {
        $('#_ausen option').prop('selected', function () {
            return this.defaultSelected;
        })
        $('#idSubsecuente').css("display", "none");
        $('#_ausen').css("display", "block");
        $('#_incapacidad').css("display", "none");
        $('#divOtros').css("display", "none");
        $('#_idFolio').css("display", "none");
        $('#fechaapli').css("display", "none");
        $('#fechaInicial').css("display", "none");
        $('#iddias_').css("display", "none");
        $('#idsubsidiosgrup').css("display", "none");
        $('#idSubsidio').css("display", "none");

        $('#txtIncapacidad').css("display", "none");
        $('#formatoid').css("display", "none");
        $('#iddiasApli_').css("display", "none");

      

    }
}

function myFunction() {
    var x = document.getElementById("idSubsecuented").value;
    if (x == "Si") {
        $('#divOtros').css("display", "block");
        $('#_idFolio').css("display", "block");
        $('#fechaapli').css("display", "block");
        $('#fechaInicial').css("display", "block");
        $('#iddias_').css("display", "block");
        $('#txtIncapacidad').css("display", "none");
        $('#formatoid').css("display", "block");
        $('#iddiasApli_').css("display", "block");      
        $('#idSubsidio').css("display", "none");
        $('#idsubsidiosgrup').css("display", "none");

   
    }
    else {
        $('#divOtros').css("display", "none");
        $('#_idFolio').css("display", "block");
        $('#txtIncapacidad').css("display", "block");
        $('#fechaapli').css("display", "block");
        $('#fechaInicial').css("display", "block");
        $('#iddias_').css("display", "block");

        $('#formatoid').css("display", "block");
        $('#iddiasApli_').css("display", "block");
        $('#idSubsidio').css("display", "block");

        $('#txtIncapacidad').css("display", "none");
    }
}


function myFunctionSubsidio() {
    var x = document.getElementById("idSubsidioo").value;
    if (x == "Si") {
        $('#idsubsidiosgrup').css("display", "block");

       


    }
    else {
        $('#idsubsidiosgrup').css("display", "none");

       
    }
}


$(function () {
    $(".anchorDetail").click(function () {
        debugger;
        var $buttonClicked = $(this);
        var id = $buttonClicked.attr('data-id');
        var options = { "backdrop": "static", keyboard: true };
        var href = $(this).attr('href');
        $.ajax({
            type: "GET",
            url: 'Ausen/Details',
            contentType: "application/json; charset=utf-8",
            data: { "Id": id },
            datatype: "json",
            success: function (data) {
                debugger;
                $('#myModalContent').html(data);
                $('#myModal').modal(options);
                $('#myModal').modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });

    $("#closbtn").click(function () {
        $('#myModal').modal('hide');
    });
});


$(function () {
    $(".anchorDelete").click(function () {
        debugger;
        var $buttonClicked = $(this);
        var id = $buttonClicked.attr('data-id');
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: 'Ausen/Delete',
            contentType: "application/json; charset=utf-8",
            data: { "Id": id },
            datatype: "json",
            success: function (data) {
                debugger;
                $('#myModalContent').html(data);
                $('#myModal').modal(options);
                $('#myModal').modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });

    $("#closbtn").click(function () {
        $('#myModal').modal('hide');
    });
});