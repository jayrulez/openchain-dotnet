using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Openchain.CLI
{
    public class Wallet
    {
        public ExtKey RootKey { get; private set; }
        public ExtKey AccountKey { get; private set; }
        public string RootAccount { get; private set; }

        public Network Network { get; private set; }

        public bool Initialized { get; private set; }

        public Wallet(string passphrase, Network network)
        {
            Network = network;
            var mnemonic = new Mnemonic(passphrase);
            AccountKey = mnemonic.DeriveExtKey();

            RootKey = AccountKey.Derive(44, true).Derive(64, true).Derive(0, true).Derive(0).Derive(0);

            var address = RootKey.PrivateKey.PubKey.GetAddress(Network);

            RootAccount = $"/p2pkh/{address}/";

            Initialized = true;
        }

        public ExtKey GetAssetKey(uint index)
        {
            return AccountKey.Derive(44, true).Derive(64, true).Derive(1, true).Derive(0).Derive(index);
        }

        public string GetAssetPath(uint index)
        {
            var assetKey = GetAssetKey(index);

            return $"/asset/p2pkh/{assetKey.PrivateKey.PubKey.GetAddress(Network)}/";
        }
    }
}
