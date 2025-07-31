using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.GroqRequestDto
{
    public class GroqRequestDto
    {
        public string model { get; set; }
        public List<GrokMessageDto> messages { get; set; }
        public int max_tokens { get; set; } = 150;
        public double temperature { get; set; } = 0.9;
    }

    public class GrokMessageDto
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class GroqSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }

}
