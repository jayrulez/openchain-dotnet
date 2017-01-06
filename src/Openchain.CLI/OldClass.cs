using NBitcoin;
using Openchain.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openchain.CLI
{
    public class OldClass
    {
        public class Program
        {
            public void Boo()
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

                /*var seed = "protect morning submit wrap wrestle warfare amateur disease sun right broccoli curtain";

                var extKey = new ExtKey(Encoding.UTF8.GetBytes(seed));
                var pKey = extKey.PrivateKey;

                Console.WriteLine(pKey.ToString());

                Console.WriteLine(pKey.PubKey.Hash.ToString());

                Console.ReadLine();

                var address = pKey.PubKey.GetAddress(Network.Main).ToString();

                var issuancePath = $"/asset/{address}/";

                var assetPath = issuancePath;

                var walletPath = $"/p2pkh/Xu9bgXdY64bsWTGjLtLm6Q1N9e1W8tCwip/";

                assetPath = "/asset/p2pkh/Xu9bgXdY64bsWTGjLtLm6Q1N9e1W8tCwip/";

                issuancePath = "/asset/p2pkh/Xu9bgXdY64bsWTGjLtLm6Q1N9e1W8tCwip/";

                Console.WriteLine($"Issuance path: {issuancePath}");
                Console.WriteLine($"Wallet path: {walletPath}");

                var tSigner = new MutationSigner(pKey);

                var client = new ApiClient("http://localhost:5000");
                client.Initialize().Wait();

                var builder = new SDK.TransactionBuilder(client)
                    .AddSigningKey(tSigner)
                    /*.SetMetaData(new
                    {
                        memo = $"Issued 100 units from {assetPath}"
                    })*/;

                /*var dataRecordTask = client.GetDataRecord(walletPath, "acl");

                dataRecordTask.Wait();

                var dataRecord = dataRecordTask.Result;

                var aclObject = new List<AclRecord>
                {
                    new AclRecord
                    {
                        Subjects = new List<Subject>
                        {
                            new Subject {
                                Addresses = new List<string>
                                {
                                    address
                                },
                                Required = 0
                            }
                        },
                        Permissions = new Permissions
                        {
                            AccountCreate = "Permit",
                            AccountModify = "Permit",
                            AccountSpend = "Permit"
                        }
                    },
                    new AclRecord
                    {
                        Subjects = new List<Subject>
                        {
                            new Subject {
                                Addresses = new List<string>
                                {
                                    address
                                },
                                Required = 1
                            }
                        },
                        Permissions = new Permissions
                        {
                            AccountSpend = "Permit"
                        }
                    }
                };

                var aclData = JsonConvert.SerializeObject(aclObject);

                var acl = new ByteString(Encoding.UTF8.GetBytes(aclData));

                builder.AddRecord(dataRecord.Key, acl, dataRecord.Version);*/

                //builder.UpdateAccountRecord(issuancePath, assetPath, -751393).Wait();

                //builder.UpdateAccountRecord(walletPath, assetPath, 751393).Wait();

                //builder.Submit().Wait();

                //var x = ParsedMutation.Parse(MessageSerializer.DeserializeMutation(ByteString.Parse("0a1038656363623161353738383663626535129f010a652f61737365742f7032706b682f3132776a6d6e46747039716d516a3166784b676e5a3172684c3264634c62764865732f3a4143433a2f61737365742f7032706b682f3132776a6d6e46747039716d516a3166784b676e5a3172684c3264634c62764865732f12360a347b2264617461223a7b2256616c7565223a5b3135362c3235352c3235352c3235352c3235352c3235352c3235352c3235355d7d7d128b010a5f2f7032706b682f3132776a6d6e46747039716d516a3166784b676e5a3172684c3264634c62764865732f3a4143433a2f61737365742f7032706b682f3132776a6d6e46747039716d516a3166784b676e5a3172684c3264634c62764865732f12280a267b2264617461223a7b2256616c7565223a5b3130302c302c302c302c302c302c302c305d7d7d1a517b226d656d6f223a224973737565642031303020756e6974732066726f6d202f61737365742f7032706b682f3132776a6d6e46747039716d516a3166784b676e5a3172684c3264634c62764865732f227d")));



                //Console.ReadLine();
            }
        }
    }
}
