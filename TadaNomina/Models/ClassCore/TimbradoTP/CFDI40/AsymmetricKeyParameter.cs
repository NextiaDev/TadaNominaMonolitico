using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ClassCore.TimbradoTP.CFDI40
{
    public abstract class AsymmetricKeyParameter
    {
        private readonly bool privateKey;

        public bool IsPrivate => privateKey;

        protected AsymmetricKeyParameter(bool privateKey)
        {
            this.privateKey = privateKey;
        }

        public override bool Equals(object obj)
        {
            AsymmetricKeyParameter asymmetricKeyParameter = obj as AsymmetricKeyParameter;
            if (asymmetricKeyParameter == null)
            {
                return false;
            }

            return Equals(asymmetricKeyParameter);
        }

        protected bool Equals(AsymmetricKeyParameter other)
        {
            return privateKey == other.privateKey;
        }

        public override int GetHashCode()
        {
            return privateKey.GetHashCode();
        }
    }
}