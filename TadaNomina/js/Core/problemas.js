$("#EnviarProblema").click(function () {
    var mensaje = $("#mensajeProblema").val();

    alert(mensaje);
});

if (localStorage.getItem('_menu') != null) {
    $("#container").attr('class', localStorage.getItem('_menu'));
} else {
    $("#container").attr('class', 'effect aside-float aside-bright navbar-fixed footer-fixed mainnav-lg')
}

$("#menuCerrado").click(function () {
    var menuCerrado = 'effect aside-float aside-bright navbar-fixed footer-fixed mainnav-sm';
    var menuAbierto = 'effect aside-float aside-bright navbar-fixed footer-fixed mainnav-lg';
    var c_menu = $("#container").attr('class');

    if (c_menu == menuAbierto) {
        localStorage.setItem('_menu', menuCerrado);
    }

    if (c_menu == menuCerrado) {
        localStorage.setItem('_menu', menuAbierto);
    }
});

$("._noMenu").click(function () {
    localStorage.removeItem('_menuPantalla');
    localStorage.removeItem('_submenuPantalla');
});


$("._menu1").on("click", "._subMenu", function (event) {
    var menu = $(this).attr('data-menu');
    var submenu = $(this).attr('data-text');
    console.log(menu + ">" + submenu);    
   
    localStorage.setItem('_menuPantalla', menu);    
    localStorage.setItem('_submenuPantalla', submenu);    
});


if (localStorage.getItem('_menuPantalla') != null && localStorage.getItem('_submenuPantalla') != null) {
    var rutaHome = $("#rutaHome").attr('data-Id');
     var div = ` <ol class="breadcrumb" >
        <li><a class="_Load" href="` + rutaHome + `" onclick="borraLocalStorage()"><i class="demo-pli-home"></i></a></li>
        <li>` + localStorage.getItem('_menuPantalla') + `</li>
        <li class="active">` + localStorage.getItem('_submenuPantalla') + `</li>
    </ol>`;

    $("#rutaPrincipal").html(div);

    var lmenu = localStorage.getItem('_menuPantalla');
    var lsubmenu = localStorage.getItem('_submenuPantalla')

    $('ul[data-menu1="' + lmenu + '"]').attr('class', 'collapse in _menu1');
    $('li[data-menu="' + lmenu + '"][data-text="' + lsubmenu + '"]').attr('class', 'active-link _subMenu');
    $('ul[data-menu1="' + lmenu + '"]').closest('.lista').attr('class', 'active-sub');
} 

function borraLocalStorage() {
    localStorage.removeItem('_menuPantalla');
    localStorage.removeItem('_submenuPantalla');
}

$(document).ready(function () {
    // Set idle time
    $(document).idleTimer(300000);
});

$(document).on("idle.idleTimer", function (event, elem, obj) {
    
    
    $("#statusUsuario").removeClass('badge badge-success');
    $("#statusUsuario").addClass('badge badge-warning');

    mensajeAlertaSesion("fa fa-frown-o fa-2x", "Sesión", "¡No se detecto actividad dentro del sistema!", "warning", "fadeIn", "fadeOut", 2500);

   

});

$(document).on("active.idleTimer", function (event, elem, obj, triggerevent) {
   
    var rutaValida = $("#rutaValidaSesion").attr('data-id');

    $.ajax({
        type: 'POST',
        url: rutaValida,
        success: function (result) {
            if (result == "Ok") {
                $("#statusUsuario").removeClass('badge badge-warning');
                $("#statusUsuario").addClass('badge badge-success');

                mensajeAlertaSesion("fa fa-smile-o fa-2x", "Sesión", "¡Se ha detectado actividad dentro del sistema!", "success", "fadeIn", "fadeOut", 2500);
            }
            else {
                $("#statusUsuario").removeClass('badge badge-success');
                $("#statusUsuario").addClass('badge badge-warning');

                mensajeAlertaSesion("fa fa-frown-o fa-2x", "Sesión", "La sesión ha caducado, sera redireccionado a la pantalla de login.", "danger", "fadeIn", "fadeOut", 2500);
                setTimeout(redirigir, 2700);
            }
        },
        error: function (er) {            

            mensajeAlertaSesion("fa fa-frown-o fa-2x", "Sesión", "Error al tratar de validar los datos de sesión: " + er, "warning", "fadeIn", "fadeOut", 2500);
        }
    });

    
});

function mensajeAlertaSesion(icon, titulo, mensaje, tipo, animationIn, animationOut, time) {    
    $.niftyNoty({
        type: tipo,
        container: 'floating',
        title: titulo,
        message: mensaje,
        closeBtn: true,
        floating: {
            position: 'top-center',
            animationIn: animationIn,
            animationOut: animationOut
        },
        focus: true,
        timer: time,
        icon: icon
    });
}

function redirigir() {
    var rutaLogin = $("#rutaLogin").attr('data-id');
    window.location.href = rutaLogin;
}

function cerrarSesionesServer() {
  var rutaValida = $("#rutaCerrarSesion").attr('data-id');
  $.ajax({
    ype: 'POST',
    url: rutaValida,
    success: function () {

    }
  });
}
