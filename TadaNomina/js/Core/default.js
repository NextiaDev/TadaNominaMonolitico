$(document).ready(function () {

    $("#IdCliente").change(function () {
        $.showLoading({
            name: 'jump-pulse'
        });
        $("form").submit();
        async: true;
    }); 

    localStorage.removeItem('_menuPantalla');
    localStorage.removeItem('_submenuPantalla');    

    $('ul[data-menu1="Inicio"]').attr('class', 'collapse in _menu1');
    $('li[data-menu="Inicio"][data-text="Cambiar de Nómina"]').attr('class', 'active-link _subMenu');
    $('ul[data-menu1="Inicio"]').closest('.lista').attr('class', 'active-sub');
});

localStorage.removeItem('nominaSelecionada');

if (!localStorage.getItem('ingreso')) {
    var user = $("#userName").attr("val");
    mensajeAlerta("Hola! " + user, "Bienvenido al Sistema Integral TADA!", "pink", "fadeIn", "fadeOut", 3500);  
    localStorage.setItem('ingreso', 1);
} 





