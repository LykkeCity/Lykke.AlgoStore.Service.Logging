using System;
using System.Net.Http;

namespace Lykke.Service.Logging.Client.AutorestClient //Do not change namespace
{
    public partial class AlgoStoreLoggingAPI
    {
        public AlgoStoreLoggingAPI(Uri baseUri, HttpClient client) : base(client)
        {
            Initialize();
            BaseUri = baseUri ?? throw new ArgumentNullException("baseUri");
        }
    }
}
