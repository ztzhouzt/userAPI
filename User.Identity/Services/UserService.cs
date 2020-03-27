using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resilience;
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
        private IHttpClient _httpClient;
        //private readonly string _userServiceUrl = "http://localhost:57998";
        private string _userServiceUrl;
        private ILogger<UserService> _logger;

        public UserService(IHttpClient httpClient,
            IOptions<ServiceDiscoveryOptions> serviceDiscoveryOptions,
            IDnsQuery dnsQuery,
            ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            var address = dnsQuery.ResolveService("service.consul", serviceDiscoveryOptions.Value.UserServiceName);
            var addressList = address.First().AddressList;
            var host = addressList.Any() ? addressList.First().ToString() : "localhost";
            var port = address.First().Port;


            _userServiceUrl = $"http://{host}:{port}";
        }

        public async Task<int> CheckOrCreate(string phone)
        {
            _logger.LogTrace($"Enter into CheckOrCreate:{phone}");

            var form = new Dictionary<string, string> { { "phone", phone } };
            try
            {
                //得到接口的返回内容
                var response = await _httpClient.PostAsync(_userServiceUrl + "/api/user/check-or-create", form);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //读取接口返回的内容
                    var userId = await response.Content.ReadAsStringAsync();
                    int.TryParse(userId, out int intUserId);
                    _logger.LogTrace($"Completed CheckOrCreate with userId:{intUserId}");
                    return intUserId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("CheckOrCreate 在重试之后失败," + ex.Message + ex.StackTrace);
                throw ex;
            }

            return 0;
        }
    }
}
