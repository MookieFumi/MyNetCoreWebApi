using System.Collections.Generic;
using mywebapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace mywebapi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IValuesService _valuesService;
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(IValuesService valuesService, ILogger<ValuesController> logger)
        {
            _valuesService = valuesService;
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<int> Get()
        {
            return _valuesService.GetValues(666);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
