$("#guardarTodo").click(function () {
    var data = $("#form");
    $.ajax({
        type: 'POST',
        url: '/PagoHoras/GuardaTodo',
        dataType: 'json',
        cache: false,
        data: data.serialize(),
        //data: { m: data },
        async: false,
        success: function (data) {
            if (data == "Exito") {
                mensajeAlerta("La informacion se guardo de forma correcta.", "success");
            } else {
                mensajeAlerta("Error", "danger");
            }
            
        }
    });
});

$("#guardaDatosPrincipales").click(function () {
    var CuotaFija = $("#CuotaFija").val();
    var CostoxHora = $("#CostoxHora").val();
    var MetaHoras = $("#MetaHoras").val();
    var TipoBono = $("#TipoBono").val();
    var Bono = $("#Bono").val();

    $.ajax({
        type: 'POST',
        url: '/PagoHoras/GuardaInformacionPrincipal',
        dataType: 'json',
        data: { cuotaFija: CuotaFija, cobroHora: CostoxHora, meta: MetaHoras, tipo: TipoBono, bono: Bono },
        async: false,
        success: function (data) {
            $.hideLoading();
            if (data == "Exito") {
                mensajeAlerta("La informacion se guardo de forma correcta.", "success");
            } else {
                mensajeAlerta("Error", "danger");
            }

        }
    });
});

$("#guardaMaterias").click(function () {
    
    var valores = "{[";
    $("._contenido").parent("tr").find("td").children("input:checkbox:checked").each(function () {        
        
        valores += "{id:" + $(this).attr('id') + ",";
        valores += "costo:" + $("._contenido").parent("tr").find("._costo").val();
        valores += "},";
    });

    valores += "null]}";
        
    $.ajax({
        type: 'POST',
        url: '/PagoHoras/GuardaMaterias',
        dataType: 'json',
        data: { materias: valores },
        async: false,
        success: function (data) {
            $.hideLoading();
            if (data == "Exito") {
                mensajeAlerta("La informacion se guardo de forma correcta.", "success");
            } else {
                mensajeAlerta("Error", "danger");
            }

        }
    });
});

$("#guardaPersonal").click(function () { 
    var valores = "";
    $("._idEmpleado").parent("tr").find("td").children("input:checkbox:checked").each(function () {
        valores += $(this).attr('id') + ",";
    });
   
    $.ajax({
        type: 'POST',
        url: '/PagoHoras/GuardaPersonalACargo',
        dataType: 'json',
        data: { idsPersonal: valores },
        async: false,
        success: function (data) {
            $.hideLoading();
            if (data == "Exito") {
                
                mensajeAlerta("La informacion se guardo de forma correcta.", "success");
            } else {
                mensajeAlerta("Error", "danger");
            }

        }
    });
});

function mensajeAlerta(mensaje, tipo) {
    $.niftyNoty({
        type: tipo,
        container: 'floating',
        message: mensaje,
        closeBtn: false,
        floating: {
            position: 'top-right',
            animationIn: "jellyIn",
            animationOut: "fadeOutDownBig"
        },
        focus: true,
        timer: 2500
    });
}
