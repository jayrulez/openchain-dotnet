using Openchain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openchain.SDK
{
    public class MutationSigner
    {
        public ByteString PublicKey { get; }

        public MutationSigner(ECKey key)
        {
            //PublicKey = new ByteString(key.ToString());
        }

        public ByteString Sign(ByteString mutation)
        {
            var transactionBuffer = new ByteString(mutation.ToByteArray());
            
            var hash = MessageSerializer.ComputeHash(transactionBuffer.ToByteArray());

            var signatureBuffer = new ByteString(Encoding.UTF8.GetBytes(""));

            return new ByteString(signatureBuffer.ToByteArray());
        }
    }
}
