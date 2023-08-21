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
            { "data": "SD" },
            { "data": "SDIMSS" },
            { "data": "SDI" },
            {
                "render": function (data, type, row) {
                    return '<a href="#" onclick="EditSueldos(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i></a>'
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
            { "data": "SD" },
            { "data": "SDIMSS" },
            { "data": "SDI" },
            {
                "render": function (data, type, row) {
                    return '<a href="#" onclick="EditSueldos(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i></a>'
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
            { "data": "SD" },
            { "data": "SDIMSS" },
            { "data": "SDI" },
            {
                "render": function (data, type, row) {
                    return '<a href="#" onclick="EditSueldos(' + row.IdEmpleado + ')"><i class="fa fa-pencil-square-o"></i></a>'
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

function EditSueldos(id) {
    $.ajax({
        type: 'POST',
        url: 'EncodeParam',
        dataType: 'text',
        data: { param: id },
        success: function (data) {
            var url = "EditSueldos?data=" + data;
            $(location).attr('href', url);
        }
    });
}


