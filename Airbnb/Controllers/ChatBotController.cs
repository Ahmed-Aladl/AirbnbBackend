using System.Net.Http.Headers;
using System.Text;
using Application.DTOs.GroqRequestDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : BaseController
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly GroqSettings _groqSettings;

        public ChatBotController(
            IHttpClientFactory clientFactory,
            IOptions<GroqSettings> groqSettings)
        {
            _clientFactory = clientFactory;
            _groqSettings = groqSettings.Value;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> ProxyToGroq([FromBody] GroqRequestDto payload)
        {
            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _groqSettings.ApiKey);

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_groqSettings.BaseUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest(responseString);

            return Content(responseString, "application/json");
        }
    }
}
