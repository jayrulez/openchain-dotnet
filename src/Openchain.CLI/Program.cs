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
            //var client = new ApiClient("http://openchain-server.beta.caricoin.com");

            //client.Initialize();

            /*Console.WriteLine(client.Namespace.ToString());

            var task = client.GetSubAccounts("/");

            task.Wait();

            task.Result.ForEach(account =>
            {
                //Console.WriteLine($"Key: {account.Key}");
                //Console.WriteLine($"Balance: {account.Value}");
                //Console.WriteLine($"Version: {account.Version}");
            });

            var transactionTask = client.GetTransaction("2d80d11496540bf88cf0ebbdeba8bd718df32f9fcc71cac1d8cf519047372b86");

            transactionTask.Wait();

            Console.WriteLine($"Transaction Hash: {transactionTask.Result.TransactionHash}");

            var mutationTasks = client.GetRecordMutations("2f7032706b682f587263795a587144414775726d6f3357736f616a64696238633366684167767432782f3a4143433a2f61737365742f7032706b682f5875396267586459363462735754476a4c744c6d3651314e396531573874437769702f");

            mutationTasks.Wait();

            mutationTasks.Result.ForEach(mutation =>
            {
                Console.WriteLine($"mutation: {mutation.ToString()}");
            });

            var recordTask = client.GetRecord("2f61737365742f7032706b682f5875396267586459363462735754476a4c744c6d3651314e396531573874437769702f3a444154413a6173646566", null);

            recordTask.Wait();

            Console.WriteLine($"Record key: {recordTask.Result.Key}");

            var accountRecordTask = client.GetAccountRecord("/p2pkh/XyAPCn2x682MVdxFhzRw98H5Ryiyhc4W24/", "/asset/p2pkh/Xu9bgXdY64bsWTGjLtLm6Q1N9e1W8tCwip/", null);

            accountRecordTask.Wait();

            Console.WriteLine($"Balance: {accountRecordTask.Result.Balance}");

            var dataRecordTask = client.GetDataRecord("/p2pkh/XyAPCn2x682MVdxFhzRw98H5Ryiyhc4W24/", "day", null);

            dataRecordTask.Wait();

            Console.WriteLine($"Data: {dataRecordTask.Result.Data}");*/

            /*var key = new NBitcoin.Key();

            var publicKey = key.PubKey;

            Console.WriteLine(publicKey.GetAddress(Network.Main).ToString());
            Console.WriteLine(publicKey.GetAddress(Network.TestNet).ToString());

            var mutationSigner = new MutationSigner(key);

            var hash = ByteString.Parse("2d80d11496540bf88cf0ebbdeba8bd718df32f9fcc71cac1d8cf519047372b86");

            var signature = mutationSigner.Sign(hash);

            Console.WriteLine(signature.ToString());

            var ecKey = new ECKey(publicKey.ToBytes());

            var isVerified = ecKey.VerifySignature(MessageSerializer.ComputeHash(hash.ToByteArray()), signature.ToByteArray());

            Console.WriteLine(isVerified);
            */

            var seed = "protect morning submit wrap wrestle warfare amateur disease sun right broccoli curtain";

            var extKey = new ExtKey(Encoding.UTF8.GetBytes(seed));

            var wallet = new Wallet(extKey);

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
