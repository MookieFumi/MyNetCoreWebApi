using System.Collections.Generic;
using Autofac.Extras.DynamicProxy;

namespace mywebapi.Services
{
    [Intercept("log-calls")]
    public class ValuesService : IValuesService
    {
        public IEnumerable<int> GetValues(int customerId)
        {
            return new List<int>
            {
                2, 6, 8, 10
            };
        }
    }
}