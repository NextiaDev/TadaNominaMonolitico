$(document).ready(function () {

    $('#IdCliente').chosen({
        width: '100%'
    });

  

});

$('#IdCliente').change(function () {
    if ($('#IdCliente').val() != null && $('#IdCliente').val().trim() != "") {
        $("#idUnidad").find('option').not(':first').remove();
        var slt = document.getElementById('idUnidad');  
        $("#idUnidad").trigger("liszt:updated");
        var x = document.getElementById("myDIV");
        x.style.display = "block"
        var idcliente = $("#IdCliente").val();
        $('#Seleccionar').attr('disabled', false);
        console.log(idcliente);
        $.ajax({
            type: 'POST',
            url: 'Default/Index',
            data: { idcliente },
            dataType: 'json',
            async: false,
            success: function (resul) {
                $.each(resul.unidadNegocio, function (key, registro) {
                    console.log(registro)
                    $("#idUnidad").append('<option value=' + registro.Value + '>' + registro.Text + '</option>');               
                });
            }
        });
        $('#idUnidad').chosen({
            width: '100%'
        });
        $("#idUnidad").trigger("chosen:updated");

    }
    else {
        $('#Seleccionar').attr('disabled', true);
    }
});

$("#Seleccionar").click(function () {
    var idCliente = $("#IdCliente").val();
    var slt = document.getElementById('idUnidad');  
    var IdunidadNegocio = slt.options[slt.selectedIndex].value;
    $.ajax({
        type: 'POST',
        url: 'Default/Index',
        dataType: 'json',
        data: { idCliente, IdunidadNegocio },
        async: false,
        success: function (data)
        {
            console.log(data);

            if (data == "ok") {
                $(location).attr('href', '../Index/Index');

            }
            else {
                mensajeAlerta("Alerta!", "Seleccione una Unidad!!", "pink", "fadeIn", "fadeOut", 3500);  

            }
           
        
        },     
    });


});
localStorage.removeItem('nominaSelecionada');
if (!localStorage.getItem('ingreso')) {
    var user = $("#userName").attr("val");
    mensajeAlerta("Hola! " + user, "Bienvenido al Sistema Integral TADA!", "pink", "fadeIn", "fadeOut", 3500);  
    localStorage.setItem('ingreso', 1);
} 





