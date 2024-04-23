using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.Nominas
{
    public class ModelFactorIntegracion
    {
        public int IdFactorIntegracion { get; set; }
        public Nullable<int> IdPrestaciones { get; set; }
        public Nullable<decimal> Limite_Inferior { get; set; }
        public Nullable<decimal> Limite_Superior { get; set; }
        public Nullable<decimal> Aguinaldo { get; set; }
        public Nullable<decimal> Vacaciones { get; set; }
        public Nullable<decimal> PrimaVacacional { get; set; }
        public Nullable<decimal> PremioPuntualidad { get; set; }
        public Nullable<decimal> PremioAsistencia { get; set; }
        public Nullable<decimal> ValesDespensa { get; set; }
        public Nullable<decimal> FondoAhorro { get; set; }
        public Nullable<decimal> PrimaVacacionalSDI { get; set; }
        public Nullable<decimal> FactorIntegracion { get; set; }
        public Nullable<System.DateTime> FechaInicioVigencia { get; set; }
        public Nullable<int> IdEstatus { get; set; }
        public Nullable<int> IdCaptura { get; set; }
        public Nullable<System.DateTime> FechaCaptura { get; set; }
    }
}
