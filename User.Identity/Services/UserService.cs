using DnsClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using User.Identity.Dtos;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private HttpClient _httpClient;
        //private readonly string _userServiceUrl = "http://localhost:57998";
        private string _userServiceUrl;

        public UserService(HttpClient httpClient,
            IOptions<ServiceDiscoveryOptions> serviceDiscoveryOptions,
            IDnsQuery dnsQuery)
        {
            _httpClient = httpClient;

            var address = dnsQuery.ResolveService("service.consul", serviceDiscoveryOptions.Value.UserServiceName);
            var addressList = address.First().AddressList;
            var host = addressList.Any() ? addressList.First().ToString() : "localhost";
            var port = address.First().Port;

            _userServiceUrl = $"http://{host}:{port}";
        }

        public async Task<int> CheckOrCreate(string phone)
        {
            var form = new Dictionary<string, string> { { "phone", phone } };
            var content = new FormUrlEncodedContent(form);
            //得到接口的返回内容
            var response = await _httpClient.PostAsync(_userServiceUrl + "/api/user/check-or-create",content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //读取接口返回的内容
                var userId = await response.Content.ReadAsStringAsync();
                int.TryParse(userId, out int intUserId);
                return intUserId;
            }
            return 0;
        }
    }
}
