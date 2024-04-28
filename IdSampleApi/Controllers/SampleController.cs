using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdSampleApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Enumerable.Range(1, 5).Select(index => Summaries[Random.Shared.Next(Summaries.Length)]).ToArray();
        }
    }
}
