using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TadaNomina.Models.ClassCore.PDF_CFDI
{
    public class nodosOrdenados
    {
        public string Clave { get; set; }
        public string TipoConcepto { get; set; }
        public string Concepto { get; set; }
        public string ImporteExento { get; set; }
        public string ImporteGravado { get; set; }
        public string Importe { get; set; }

        public List<nodosOrdenados> ordenaNodosXML(XmlNodeList lista, string TipoLista)
        {
            List<nodosOrdenados> nuevaLista = new List<nodosOrdenados>();
            foreach (XmlNode iper in lista)
            {
                nodosOrdenados nodo = new nodosOrdenados();

                nodo.Clave = iper.Attributes.Item(1).Value;
                nodo.TipoConcepto = iper.Attributes.Item(0).Value;
                nodo.Concepto = iper.Attributes.Item(2).Value;

                if (TipoLista == "Percepciones")
                {
                    nodo.ImporteGravado = iper.Attributes.Item(3).Value;
                    nodo.ImporteExento = iper.Attributes.Item(4).Value;
                }

                if (TipoLista == "Deducciones" || TipoLista == "OtrosPagos")
                {
                    nodo.Importe = iper.Attributes.Item(3).Value;
                }

                nuevaLista.Add(nodo);
            }

            return nuevaLista.OrderBy(x=> x.Clave).ToList();
        }
    }
}
