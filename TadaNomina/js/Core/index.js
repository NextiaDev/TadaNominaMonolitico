if (!localStorage.getItem('nominaSelecionada')) {
    var cli = $("#nomSelect").attr("val");
    var uni = $("#nomSelect").attr("val2");
    mensajeAlerta("Iniciaste Sesión en:", "Cliente: <b>" + cli + "</b><br/>Unidad de Negocio: <b>" + uni + "</b>.", "mint", "fadeIn", "fadeOut", 4500);
    localStorage.setItem('nominaSelecionada', 1); 
} 

$(document).ready(function () {

    localStorage.removeItem('_menuPantalla');
    localStorage.removeItem('_submenuPantalla');

    $('ul[data-menu1="Inicio"]').attr('class', 'collapse in _menu1');
    $('li[data-menu="Inicio"][data-text="Inicio"]').attr('class', 'active-link _subMenu');
    $('ul[data-menu1="Inicio"]').closest('.lista').attr('class', 'active-sub');
});