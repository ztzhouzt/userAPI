using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Polly;
using System.Collections.Concurrent;
using Polly.Wrap;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Resilience
{
    public class ResilienceHttpClient : IHttpClient
    {
        private HttpClient _httpClient;
        
        //根据url origin去创建 policy
        private readonly Func<string, IEnumerable<Policy>> _policyCreator;
        //把policy打包组合policy wraper，进行本地缓存
        private readonly ConcurrentDictionary<string, PolicyWrap> _policyWraps;

        private ILogger<ResilienceHttpClient> _logger;
        private IHttpContextAccessor _httpContextAccessor;

        public ResilienceHttpClient(Func<string, IEnumerable<Policy>> policyCreator, ILogger<ResilienceHttpClient> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _policyWraps = new ConcurrentDictionary<string, PolicyWrap>();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<HttpResponseMessage> PostAsync<T>(string url, T Item, string authorizationToken,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            throw new NotImplementedException();
        }


        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);
            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";
            return origin;
        }

    }
}
