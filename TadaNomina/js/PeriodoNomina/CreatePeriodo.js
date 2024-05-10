$(function () {
    $("#TipoNomina").change(function () {
        elementdos = document.getElementById("fechasPtu");
        elementdosds = document.getElementById("PtuRegP");
        elementosCalculoPtu = document.getElementById("tipocalculoPtu");

        var Tipo = $("#TipoNomina").val();
        if (Tipo == "PTU") {
            elementdos.style.display = 'block';
            elementdosds.style.display = 'block';
            elementosCalculoPtu.style.display = 'block';
        }
        else {
            elementdos.style.display = 'none';
            elementdosds.style.display = 'none';
            tipocalculoPtu.style.display = 'none';
        }




        console.log(PTU);
    });
});


$(function () {
    $("#AjusteImpuestos").change(function () {
        var Ajuste = $("#AjusteImpuestos").val();
        element = document.getElementById("AjusteAnualc");
        tabla = document.getElementById("TablaDiaria");


        if (Ajuste == "SI") {
            element.style.display = 'block';
            tabla.style.display = 'none';


        }
        else {
            element.style.display = 'none';
            tabla.style.display = 'block';


        }


        console.log(Ajuste);
    });
});



$(function () {
    $("#PeriodoAjuste").change(function () {
        var Periodos = $("#PeriodoAjuste").val() + ",";
        var valorActual = $("#IdsPeriodosAjuste").val();
        var valorFinal = valorActual + Periodos;

        var PeriodosDesc = $("#PeriodoAjuste option:selected").text() + ",";
        var valorActualDesc = $("#PeriodosAjuste").val();
        var valorFinalDesc = PeriodosDesc + valorActualDesc;

        $("#IdsPeriodosAjuste").val(valorFinal);
        $("#PeriodosAjuste").val(valorFinalDesc);
    });
});

$(function () {
    $("#btnLimpiar").click(function () {
        $("#IdsPeriodosAjuste").val("");
        $("#PeriodosAjuste").val("");
    });
});

$('#TipoNomina').change(function () {
    if ($('#TipoNomina').val() == "Nomina") {
        $('#rc').removeAttr('hidden');
        $('#rc').show();
    }
    else {
        $('#FechaInicioC').val('');
        $('#FechaFinC').val('');
        $('#rc').hide();
        $('#rc').attr('hidden', 'hidden');
    }
});

function ValidaFechas() {
    var inicio = $("#FechaInicioC").val();
    var fin = $("#FechaFinC").val();
    var r = 0;
    //Fecha fin individual por D,M,A
    var diaF = fin.substring(0, 2);
    var mesF = fin.substring(3, 5);
    var anioF = fin.substring(6, 10);

    //Fecha inicio individual por D,M,A
    var diaI = inicio.substring(0, 2);
    var mesI = inicio.substring(3, 5);
    var anioI = inicio.substring(6, 10);

    //Valida años
    if (anioF < anioI) {
        alert("Fecha de inicio debe ser menor a la fecha final");
        r = 1;
    }
    else if (anioF > anioI) {
    }
    else if (anioF = anioI) {
        //Valida meses
        if (mesF < mesI) {
            alert("Fecha de inicio debe ser menor a la fecha final");
            r = 1;
        }
        //Valida dias
        if (mesF == mesI && diaF < diaI) {
            alert("Fecha de inicio debe ser menor a la fecha final");
            r = 1;
        }
        //Valor a regresar 1 = inconsistencia, 0 = ok
    }

    return r;
}

$('#FechaFinC').change(function () {
    var r = ValidaFechas();
    console.log(r)
    if (r == 1) {
        $('#FechaFinC').val("");
    }
})

$('#FechaInicioC').change(function () {
    var r = ValidaFechas();
    console.log(r)
    if (r == 1) {
        $('#FechaInicioC').val("");
    }
})