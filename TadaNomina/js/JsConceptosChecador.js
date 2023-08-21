function eliminar(IdConceptoChecador) {
    document.getElementById("IdConcepto").value = IdConceptoChecador;
}

function Descripcion(text) {
    document.getElementById("DescripcionGV").value = text.options[text.selectedIndex].text;
    console.log(document.getElementById("DescripcionGV").value);
}