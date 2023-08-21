function mensajeAlerta(titulo, mensaje, tipo, animationIn, animationOut, time) {
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
        timer: time
    });
}