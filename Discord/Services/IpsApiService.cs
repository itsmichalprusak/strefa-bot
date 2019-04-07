using System;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Discord.Services
{
    public class IpsApiService
    {
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;
        private readonly string _apiKey;
        private bool _enabled = false;
        
        public IpsApiService(
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            _config = config;
            _provider = provider;
            _apiKey = _config["tokens:ipsApi"];
        }

        public async Task Setup()
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new Exception("No Invision API key specified! API querying will not be available " +
                                    "until you resolve this.");
            _enabled = true;
        }
        
        private async Task QueryApi(string queryString)
        {
            
        }
    }
}