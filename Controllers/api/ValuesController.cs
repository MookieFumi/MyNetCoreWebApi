using System.Collections.Generic;
using System.Threading.Tasks;
using mywebapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace mywebapi.Controllers.api
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IValuesService _valuesService;
        private readonly ISpecialValuesService _specialValuesService;
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(IValuesService valuesService, ISpecialValuesService specialValuesService, ILogger<ValuesController> logger)
        {
            _valuesService = valuesService;
            _specialValuesService = specialValuesService;
            _logger = logger;
            _logger.LogDebug($"Ctor {nameof(ValuesController)}");
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<int> Get()
        {
            var values = _valuesService.GetValues();
            return values;
        }

        // GET api/values/stations
        [HttpGet("even")]
        public async Task<IEnumerable<int>> GetEvenValues()
        {
            var values = await _specialValuesService.GetEvenValues();
            return values;
        }

        // GET api/values/stations
        [HttpGet("odd")]
        public async Task<IEnumerable<int>> GetOddValues()
        {
            var values = await _specialValuesService.GetOddValues();
            return values;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return id.ToString();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
