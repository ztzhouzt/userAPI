﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Resilience
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> PostAsync<T>(string url, T Item, string authorizationToken, string requestId = null, string authorizationMethod = "Bearer");
    }
}
