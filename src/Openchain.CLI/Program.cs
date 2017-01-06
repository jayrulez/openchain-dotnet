using NBitcoin;
using Newtonsoft.Json;
using Openchain.Infrastructure;
using Openchain.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openchain.CLI
{
    public class Subject
    {

        [JsonProperty("addresses")]
        public IList<string> Addresses { get; set; }

        [JsonProperty("required")]
        public int Required { get; set; }
    }

    public class Permissions
    {

        [JsonProperty("account_modify")]
        public string AccountModify { get; set; }

        [JsonProperty("account_create")]
        public string AccountCreate { get; set; }

        [JsonProperty("account_spend")]
        public string AccountSpend { get; set; }
    }

    public class AclRecord
    {

        [JsonProperty("subjects")]
        public IList<Subject> Subjects { get; set; }

        [JsonProperty("permissions")]
        public Permissions Permissions { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var networkBuilder = new NetworkBuilder();
            networkBuilder.CopyFrom(Network.Main);

            networkBuilder.SetName("openchain");
            networkBuilder.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new[] { (byte)76 });
            networkBuilder.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new[] { (byte)78 });
            networkBuilder.SetMagic(0);
            var network = networkBuilder.BuildAndRegister();
            
            var wallet = new Wallet("protect morning submit wrap wrestle warfare amateur disease sun right broccoli curtain", network);

            Console.WriteLine($"Root Account: {wallet.RootAccount}");

            for (uint i = 0; i < 20; i++)
            {
                Console.WriteLine($"Asset {i}: {wallet.GetAssetPath(i)}");
            }


            var client = new ApiClient("http://localhost:5000");
            client.Initialize().Wait();

            var dataRecordTask = client.GetDataRecord(wallet.GetAssetPath(1), "asdef");

            dataRecordTask.Wait();

            var dataRecord = dataRecordTask.Result;

            var assetDefinition = new
            {
                name = "Jamaican Dollar",
                name_short = "JMD",
                icon_url = ""
            };

            var builder = new SDK.TransactionBuilder(client);

            builder.AddRecord(dataRecord.Key, new ByteString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(assetDefinition))), dataRecord.Version);

            builder.AddSigningKey(new MutationSigner(wallet.GetAssetKey(1)));

            var submitTask = builder.Submit();

            submitTask.Wait();

            var mutation = builder.Build();

            var eckey = new Openchain.Infrastructure.ECKey(wallet.GetAssetKey(1).PrivateKey.PubKey.ToBytes());

            Console.WriteLine(eckey.VerifySignature(MessageSerializer.ComputeHash(mutation.ToByteArray()), new MutationSigner(wallet.GetAssetKey(1)).Sign(mutation).ToByteArray()));

            Console.ReadLine();
        }
    }
}
