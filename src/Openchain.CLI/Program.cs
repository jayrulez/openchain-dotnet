using Openchain.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Openchain.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var client = new ApiClient("http://openchain-server.beta.caricoin.com");

            client.Initialize();

            Console.WriteLine(client.Namespace.ToString());

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

            Console.WriteLine($"Data: {dataRecordTask.Result.Data}");

            Console.ReadLine();
        }
    }
}
