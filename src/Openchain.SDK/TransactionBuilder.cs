using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Openchain.Infrastructure;

namespace Openchain.SDK
{
    public class TransactionBuilder
    {
        private readonly ApiClient _apiClient;
        private List<Record> _records;
        private List<MutationSigner> _keys;
        private ByteString _metaData;

        public TransactionBuilder(ApiClient apiClient)
        {
            if (apiClient.Namespace == null)
            {
                throw new Exception("The API client has not been initialized");
            }

            _apiClient = apiClient;
            _records = new List<Record>();
            _keys = new List<MutationSigner>();
            _metaData = ByteString.Empty;
        }

        public TransactionBuilder AddRecord(ByteString key, ByteString value, ByteString version)
        {
            /*if (value != null)
            {
                var valueData = new { data = value };

                value = new ByteString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(valueData)));
            }*/

            var newRecord = new Record(key, value, version);

            _records.Add(newRecord);

            return this;
        }

        public TransactionBuilder SetMetaData(object data)
        {
            _metaData = new ByteString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));

            return this;
        }

        public TransactionBuilder AddAccountRecord(AccountStatus previous, Int64 delta)
        {
            return AddRecord(previous.AccountKey.Key.ToBinary(), new ByteString(BitConverter.GetBytes(previous.Balance + delta)), previous.Version);
        }

        public async Task<TransactionBuilder> UpdateAccountRecord(string account, string asset, Int64 delta)
        {
            if (account.StartsWith("@"))
            {
                account = $"/aka/{account.Slice(1, account.Length)}/";
            }

            var dataRecord = await _apiClient.GetDataRecord(account, "goto");

            var accountResult = string.Empty;

            if (dataRecord.Data == null)
            {
                accountResult = account;
            }
            else
            {
                AddRecord(dataRecord.Key, null, dataRecord.Version);
                accountResult = dataRecord.Data;
            }

            var accountRecord = await _apiClient.GetAccountRecord(accountResult, asset);

            return AddAccountRecord(new AccountStatus(accountRecord.AccountKey, accountRecord.Balance, accountRecord.Version), delta);
        }

        public TransactionBuilder AddSigningKey(MutationSigner key)
        {
            _keys.Add(key);

            return this;
        }

        public static ParsedMutation Parse(Mutation mutation)
        {
            List<AccountStatus> accountMutations = new List<AccountStatus>();
            List<KeyValuePair<RecordKey, ByteString>> dataRecords = new List<KeyValuePair<RecordKey, ByteString>>();

            foreach (Record record in mutation.Records)
            {
                // This is used for optimistic concurrency and does not participate in the validation
                if (record.Value == null)
                    continue;

                try
                {
                    RecordKey key = RecordKey.Parse(record.Key);
                    switch (key.RecordType)
                    {
                        case RecordType.Account:
                            accountMutations.Add(FromRecord(key, record));
                            break;
                        case RecordType.Data:
                            dataRecords.Add(new KeyValuePair<RecordKey, ByteString>(key, record.Value));
                            break;
                    }
                }
                catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "keyData")
                {
                    // Deserializing and re-serializing the record gives a different result
                    throw new TransactionInvalidException("NonCanonicalSerialization");
                }
                catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "path")
                {
                    // The path is invalid
                    throw new TransactionInvalidException("InvalidPath");
                }
                catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "recordType")
                {
                    // The specified record type is unknown
                    throw new TransactionInvalidException("InvalidRecord");
                }
                catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "record")
                {
                    // The value of an ACC record could not be deserialized
                    throw new TransactionInvalidException("InvalidRecord");
                }
            }

            return new ParsedMutation(accountMutations, dataRecords);
        }

        public static AccountStatus FromRecord(RecordKey key, Record record)
        {
            if (key.RecordType != RecordType.Account)
                throw new ArgumentOutOfRangeException(nameof(key));

            long amount;
            if (record.Value.Value.Count == 0)
                amount = 0;
            else if (record.Value.Value.Count == 8)
                amount = BitConverter.ToInt64(record.Value.Value.Reverse().ToArray(), 0);
            else
                throw new ArgumentOutOfRangeException(nameof(record));

            return new AccountStatus(new AccountKey(key.Path, LedgerPath.Parse(key.Name)), amount, record.Version);
        }

        public ByteString Build()
        {
            var mutation = new Mutation(_apiClient.Namespace, _records, _metaData);


            var x = mutation.Namespace;

            var y = _apiClient.Namespace;
            //Parse(mutation);

            return new ByteString(MessageSerializer.SerializeMutation(mutation));
        }

        public async Task<TransactionData> Submit()
        {
            var mutation = Build();

            var signatures = new List<SigningKey>();

            _keys.ForEach(key =>
            {
                signatures.Add(new SigningKey
                {
                    Signature = key.Sign(mutation).ToString(),
                    PublicKey = key.PublicKey.ToString()
                });
            });

            return await _apiClient.Submit(mutation, signatures);
        }
    }
}
