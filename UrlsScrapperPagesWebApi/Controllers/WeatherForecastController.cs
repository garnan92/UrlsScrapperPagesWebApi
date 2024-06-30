using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace UrlsScrapperPagesWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        class Page
        {
            public Guid id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string element_name { get; set; }
            public byte[] elements { get; set; }
        }

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ApplicationDbContext _dbcontext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext dbcontext)
        {
            _logger = logger;
            _dbcontext = dbcontext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<ActionResult<object>> GetAsync()
        {

            var result = await _dbcontext.ExecuteStoredProcedureAsync<Page>("dbo.PagesList");

            return Ok(result);
        }
    }
}
