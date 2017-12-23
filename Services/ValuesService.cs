using System.Collections.Generic;
using Autofac.Extras.DynamicProxy;
using mywebapi.Infrastructure;

namespace mywebapi.Services
{
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