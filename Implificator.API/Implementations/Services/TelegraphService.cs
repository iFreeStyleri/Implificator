using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Implificator.Abstractions.Services;
using Kvyk.Telegraph;
using Kvyk.Telegraph.Models;

namespace Implificator.API.Implementations.Services
{
    public class TelegraphService : ITelegraphService
    {
        public TelegraphClient Client { get; init; }

        private TelegraphService()
        {
            Client = new TelegraphClient();
        }

        public static TelegraphService Create()
        {
            var service = new TelegraphService();
            var account = service.Client.CreateAccount("QR-Stats", "QR-Stats", "https://t.me/InfluencererBot").Result;
            service.Client.AccessToken = account.AccessToken;
            return service;
        }

        public async Task<string> CreatePage(string content)
        {
            var nodes = new List<Node>
            {
                Node.Li(content),
                Node.A("https://t.me/InfluencererBot", "@InfluencererBot")
            };
            var page = await Client.CreatePage(DateTime.UtcNow.ToOADate() + " \ud83d\udcec", nodes, "@InfluencererBot", "https://t.me/InfluencererBot");
            return page.Url;
        }
    }
}
