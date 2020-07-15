namespace WebApiLAKV.Controllers
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Newtonsoft.Json;

    [ApiController]
    [Route("[controller]")]
    public class LAQueryController : ControllerBase
    {
        private readonly ILogger<LAQueryController> _logger;
        IConfiguration configuration;

        private readonly KeyVaultValueSettings keyvaultValueSettings;
        private static string TGCCLASPSecret;

        public LAQueryController(ILogger<LAQueryController> logger, IOptions<KeyVaultValueSettings> keyvaultValueSettings, IConfiguration configuration)
        {
            _logger = logger;

            this.keyvaultValueSettings = keyvaultValueSettings.Value;
            TGCCLASPSecret = this.keyvaultValueSettings.TGCCLASP;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var authority = String.Format(CultureInfo.InvariantCulture, configuration.GetValue<string>("Instance"), configuration.GetValue<string>("TenantId"));

            var authContext = new AuthenticationContext(authority);
            var creadential = new ClientCredential(configuration.GetValue<string>("LASPId"), TGCCLASPSecret);
            var result = await authContext.AcquireTokenAsync("https://api.loganalytics.io/", creadential);

            // Query to be posted
            var query = JsonConvert.SerializeObject(new
            {
                query = "Heartbeat | take 10"
            });

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

                var postContent = new StringContent(query, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://api.loganalytics.io/v1/workspaces/{configuration.GetValue<string>("WorkspaceId")}/query", postContent);

                HttpContent responseContent = response.Content;
                //returning a string based Json
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
        }
    }
}
