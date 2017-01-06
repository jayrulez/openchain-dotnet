using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Openchain.CLI
{
    public class Wallet
    {
        public ExtKey Key { get; private set; }
        public ExtKey DerivedKey { get; private set; }
        public string RootAccount { get; private set; }

        public bool Initialized { get; private set; }

        public Wallet(ExtKey key)
        {
            Key = key;
            DerivedKey = Key.Derive(44, true).Derive(64, true).Derive(0, true).Derive(0).Derive(0);

            var address = DerivedKey.PrivateKey.PubKey.GetAddress(Network.Main).ToString();

            RootAccount = $"/p2pkh/{address}/";

            Initialized = true;
        }

        public ExtKey GetAssetKey(uint index)
        {
            return Key.Derive(44, true).Derive(64, true).Derive(1, true).Derive(0).Derive(index);
        }

        public string GetAssetPath(uint index)
        {
            var assetKey = GetAssetKey(index);

            return $"/asset/{assetKey.PrivateKey.PubKey.GetAddress(Network.Main).ToString()}/";
        }
    }
}
