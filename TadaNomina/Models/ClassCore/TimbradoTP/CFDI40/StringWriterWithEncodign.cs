using System.IO;
using System.Text;

namespace API_Nomors.Core.CFDI40
{
    public class StringWriterWithEncodign: StringWriter
    {
        private readonly Encoding m_encoding;

        public StringWriterWithEncodign(Encoding encoding) : base()
        { 
            this.m_encoding = encoding;
        }

        public override Encoding Encoding
        { 
            get { return m_encoding; }
        }
    }
}
