using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using WebApiRequestLogs.Dtos;

namespace WebApiRequestLogs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetTimeZoneList")]
        public IActionResult GetTimeZoneList()
        {
            List<TimeZoneDto> objTimeZoneList = new List<TimeZoneDto>();
            ReadOnlyCollection<TimeZoneInfo> tzi;
            tzi = TimeZoneInfo.GetSystemTimeZones();
            foreach (TimeZoneInfo timeZone in tzi)
            {
                TimeZoneDto objTimeZoneDto = new TimeZoneDto();
                objTimeZoneDto.Id = timeZone.Id;
                objTimeZoneDto.DisplayName = timeZone.DisplayName;
                objTimeZoneList.Add(objTimeZoneDto);
            }
            return Ok(objTimeZoneList);
        }

        [HttpGet("GetTimeZoneByName/{name}")]
        public IActionResult GetTimeZoneByName(string name)
        {
            try
            {
                List<TimeZoneDto> objTimeZoneList = new List<TimeZoneDto>();
                ReadOnlyCollection<TimeZoneInfo> tzi;
                tzi = TimeZoneInfo.GetSystemTimeZones();
                foreach (TimeZoneInfo timeZone in tzi)
                {
                    TimeZoneDto objTimeZoneDto = new TimeZoneDto();
                    objTimeZoneDto.Id = timeZone.Id;
                    objTimeZoneDto.DisplayName = timeZone.DisplayName;
                    objTimeZoneList.Add(objTimeZoneDto);
                }

                var result = objTimeZoneList.Where(x => x.Id.ToLower().Contains(name.ToLower())).ToList();
                if (result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("throw-exception")]
        public IActionResult ThrowException()
        {
            throw new Exception("This is an unhandled exception!");
        }
    }
}
