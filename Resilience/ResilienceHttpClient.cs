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
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;

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

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            Func<HttpRequestMessage> func = () => CreateHttpRequestmessage(HttpMethod.Post, url, item);
            return await DoPostPutAsync(HttpMethod.Post, url, func, authorizationToken, requestId, authorizationMethod);
        }


        public async Task<HttpResponseMessage> PostAsync(string url, Dictionary<string, string> form, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            Func<HttpRequestMessage> func = () => CreateHttpRequestmessage(HttpMethod.Post, url, form);
            return await DoPostPutAsync(HttpMethod.Post, url, func, authorizationToken, requestId, authorizationMethod);
        }


        private Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method, string url,Func<HttpRequestMessage> requestMessageAction,
            string authorizationToken=null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            var origin = GetOriginFromUri(url);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = requestMessageAction();
               
                SetAuthorizationHeader(requestMessage);

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }

                var response = await _httpClient.SendAsync(requestMessage);

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }
                return response;
            });
        }


        private HttpRequestMessage CreateHttpRequestmessage<T>(HttpMethod method,string url,T item)
        {
            return new HttpRequestMessage(method, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json")
            };
        }

        private HttpRequestMessage CreateHttpRequestmessage(HttpMethod method, string url, Dictionary<string,string> form)
        {
            return new HttpRequestMessage(method, url)
            {
                Content = new FormUrlEncodedContent(form)
            };
        }


        private async Task<T> HttpInvoker<T>(string orgin, Func<Task<T>> action)
        {
            var normalizedOrgin = NormalizeOrigin(orgin);
            if (!_policyWraps.TryGetValue(normalizedOrgin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrgin).ToArray());
                _policyWraps.TryAdd(normalizedOrgin, policyWrap);
            }
            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrgin));
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

        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
        }

    }
}
