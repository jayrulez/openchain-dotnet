using NBitcoin;
using Org.BouncyCastle.Crypto.Signers;
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

        private readonly ECDsaSigner _signer;
        private readonly Key _key;

        public MutationSigner(Key key)
        {
            _key = key;

            PublicKey = new ByteString(_key.PubKey.ToBytes());
        }

        public ByteString Sign(ByteString mutation)
        {
            var transactionBuffer = new ByteString(mutation.ToByteArray());
            
            var hash = MessageSerializer.ComputeHash(transactionBuffer.ToByteArray());
            
            var signature = _key.Sign(new NBitcoin.uint256(hash));
            
            var signatureBuffer = new ByteString(signature.ToDER());
            
            return signatureBuffer;
        }
    }
}
