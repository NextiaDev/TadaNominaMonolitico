using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelAusentismos
    {
        [Key]
        public int IdAusentismo { get; set; }
        [Display(Name = "Tipo de Ausentismo")]
        public int? IdTipoAusentismo { get; set; }
        [Display(Name = "Tipo de Incapacidad")]
        public int? IdTipoIncapacidad { get; set; }
    
        public string IdSubsecuente { get; set; }
        public List<SelectListItem> idIncidencia { get; set; }
        public int IdFolio { get; set; }


        [Display(Name = "Incapacidad:")]
        public string IdIncapacidad { get; set; }

        [Display(Name = "Ausentismos:")]
        public string idAusentismos { get; set; }

        [Display(Name = "Ausentismos:")]
        public List<SelectListItem> LAusentismos { get; set; }

        [Display(Name = "Tipo de Incidencia:")]
        public List<SelectListItem> TipodeIncidencia { get; set; }
        public List<SelectListItem> Subsecuente { get; set; }
        [Display(Name = "Incapacidad:")]

        public List<SelectListItem> Incapacidad { get; set; }

        [Display(Name = "Aplicar Subsidio:")]

        public List<SelectListItem> Subsidio { get; set; }


        [Display(Name = "Tipo Incapacidad:")]

        [DataType(DataType.MultilineText)]
        public int? IdEmpleado { get; set; }
        [Display(Name = "Cve. Emp.")]
        public string ClaveEmp { get; set; }
        public string Ausentismo { get; set; }
        [Display(Name = "Nombre.")]
        public string Empleado { get; set; }
        [Display(Name = "R.F.C.")]
        public string rfc { get; set; }
        [Display(Name = "fecha. Inicial")]
        public string FechaInicial { get; set; }

        [Display(Name = "fecha. Aplicacion")]
        public string FechaInicialAplicacion { get; set; }

        [Display(Name = "fecha. Final")]
        public string FechaFinal { get; set; }

        [RegularExpression(@"^[A-Za-z]{2}[0-9]{6}$", ErrorMessage = "El formato del Folio no es correcto(2 letras y dos 6 numeros)")]
        
        [Display(Name = "Folio:")]

        public string Folio { get; set; }

        [RegularExpression(@"^[A-Za-z]{2}[0-9]{6}$", ErrorMessage = "El formato del Folio no es correcto(2 letras y dos 6 numeros)")]
        [Display(Name = "Folio Subsecuente.")]
        public string FolioSub { get; set; }

        public int unidadnegocio { get; set; }

        public int ClaveConcepto { get; set; }

        public string Concepto { get; set; }

        public string Incapaci { get; set; }

        [Display(Name = "Subir Archivo")]
        public string Imagen { get; set; }
        public int Dias { get; set; }

        [Display(Name = "Dias aplicados")]

        public int DiasApli { get; set; }

        [Display(Name = "Dias Subsidio Inicial")]

        public int DiasSubidioInicial { get; set; }

        [Display(Name = "Porcentaje Subsidio Inicial")]

        public decimal PorcentajeSubsidio { get; set; }

        [Display(Name = "Porcentaje Dias  Restante")]

        public decimal PorcentajeSubsidioRestante { get; set; }


        public string Observaciones { get; set; }

        public string Mensaje { get; set; }

        public string Apaterno { get; set; }
        public string Nombre { get; set; }

        public string Amaterno { get; set; }

        public string idcaptura { get; set; }

        public int idCliente { get; set; }
        public bool Validacion { get; set; }
        public string TipoIncidencia { get; set; }
    }
}