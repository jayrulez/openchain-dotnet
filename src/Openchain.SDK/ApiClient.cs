using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Openchain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Openchain.SDK
{
    public class ApiClient
    {
        public ByteString Namespace { get; private set; }

        private readonly HttpClient _httpClient;

        public ApiClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Initialize()
        {
            GetInfo().ContinueWith((task) =>
            {
                var info = task.Result;

                Namespace = new ByteString(Encoding.UTF8.GetBytes(info.Namespace));
            }).Wait();
        }

        public async Task<LedgerInfo> GetInfo()
        {
            var response = await _httpClient.GetAsync("info");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get info.");
            }

            var info = JsonConvert.DeserializeObject<LedgerInfo>(await response.Content.ReadAsStringAsync());

            return info;
        }

        public async Task<Record> GetRecord(object key, ByteString version = null)
        {
            ByteString recordKey;

            if (key is ByteString)
            {
                recordKey = (ByteString)key;
            }
            else if (key is string)
            {
                recordKey = ByteString.Parse((string)key);
            }
            else if (key is RecordKey)
            {
                var keyObj = (RecordKey)key;
                recordKey = keyObj.ToBinary();
            }
            else
            {
                throw new Exception("Invalid record key.");
            }

            HttpResponseMessage response;

            if (version == null)
            {
                response = await _httpClient.GetAsync($"record?key={recordKey.ToString()}");
            }
            else
            {
                response = await _httpClient.GetAsync($"query/recordversion?key={recordKey.ToString()}&version={version.ToString()}");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get record.");
            }

            var content = await response.Content.ReadAsStringAsync();

            var record = ParseRecord(JObject.Parse(content));

            return record;
        }

        public async Task<DataRecord> GetDataRecord(string path, string recordName, ByteString version = null)
        {
            var recordKey = new RecordKey(RecordType.Data, LedgerPath.Parse(path), recordName);

            var key = recordKey.ToBinary().ToString();

            var record = await GetRecord(key, version);

            DataRecord dataRecord;

            var bytes = record.Value.ToByteArray();

            if (bytes.Count() == 0)
            {
                dataRecord = new DataRecord(record.Key, record.Value, record.Version, null);
            }
            else
            {
                dataRecord = new DataRecord(record.Key, record.Value, record.Version, Encoding.UTF8.GetString(bytes));
            }

            return dataRecord;
        }

        public async Task<AccountStatus> GetAccountRecord(string path, string asset, ByteString version = null)
        {
            var accountKey = AccountKey.Parse(path, asset);

            var record = await GetRecord(accountKey.Key, version);

            return AccountStatus.FromRecord(accountKey.Key, record);
        }

        public async Task<List<AccountStatus>> GetAccountRecords(string account)
        {
            var response = await _httpClient.GetAsync($"query/account?account={Uri.EscapeUriString(account.ToString())}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get account.");
            }

            var records = new List<AccountStatus>();

            var recordArray = JArray.Parse(await response.Content.ReadAsStringAsync());

            foreach (var item in recordArray)
            {
                var record = ParseRecord(item);

                var recordKey = RecordKey.Parse(record.Key);

                var accountRecord = AccountStatus.FromRecord(recordKey, record);

                records.Add(accountRecord);
            }

            return records;
        }

        public async Task<List<Record>> GetSubAccounts(string account)
        {
            var response = await _httpClient.GetAsync($"query/subaccounts?account={Uri.EscapeUriString(account.ToString())}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get account.");
            }

            var records = new List<Record>();

            var subAccounts = JArray.Parse(await response.Content.ReadAsStringAsync());

            foreach (var subAccount in subAccounts)
            {
                records.Add(ParseRecord(subAccount));
            }

            return records.ToList();
        }

        public async Task<List<ByteString>> GetRecordMutations(object key)
        {
            ByteString recordKey;

            if (key is ByteString)
            {
                recordKey = (ByteString)key;
            }
            else if (key is string)
            {
                recordKey = ByteString.Parse((string)key);
            }
            else if (key is RecordKey)
            {
                var keyObj = (RecordKey)key;
                recordKey = keyObj.ToBinary();
            }
            else
            {
                throw new Exception("Invalid record key.");
            }

            var response = await _httpClient.GetAsync($"query/recordmutations?key={recordKey.ToString()}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get record.");
            }

            var records = new List<ByteString>();

            var content = await response.Content.ReadAsStringAsync();

            var mutations = JArray.Parse(content);

            foreach (var mutation in mutations)
            {
                var mutationObj = JObject.Parse(mutation.ToString());

                var mutationHash = ByteString.Parse((string)mutationObj["mutation_hash"]);

                records.Add(mutationHash);
            }

            return records;
        }

        public async Task<TransactionData> GetTransaction(object mutationHashObj)
        {
            ByteString mutationHash;

            if (mutationHashObj is ByteString)
            {
                mutationHash = (ByteString)mutationHashObj;
            }
            else if (mutationHashObj is string)
            {
                mutationHash = ByteString.Parse((string)mutationHashObj);
            }
            else
            {
                throw new Exception("Invalid mutation hash.");
            }

            var response = await _httpClient.GetAsync($"query/transaction?format=raw&mutation_hash={mutationHash.ToString()}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get transaction.");
            }

            var body = JObject.Parse(await response.Content.ReadAsStringAsync());

            var raw = (string)body["raw"];

            var buffer = ByteString.Parse(raw);

            var transaction = MessageSerializer.DeserializeTransaction(buffer);

            var mutation = MessageSerializer.DeserializeMutation(transaction.Mutation);

            var transactionBuffer = buffer.ToByteArray().ToArray();
            var transactionHash = MessageSerializer.ComputeHash(transactionBuffer);

            var data = new TransactionData
            {
                Transaction = transaction,
                Mutation = mutation,
                MutationHash = transaction.Mutation,
                TransactionHash = new ByteString(transactionHash)
            };

            return data;
        }

        public async Task<TransactionData> Submit(ByteString mutation, List<SigningKey> signatures)
        {
            var data = new Dictionary<string, object>
            {
                { "mutation", mutation.ToString() },
                { "signatures", signatures }
            };

            var response = await _httpClient.PostAsync("submit", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to submit transaction.");
            }

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TransactionData>(content); ;

            return result;
        }

        private Record ParseRecord(JToken record)
        {
            return new Record(ByteString.Parse((string)record["key"]), ByteString.Parse((string)record["value"]), ByteString.Parse((string)record["version"]));
        }
    }
}
