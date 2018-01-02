using System.Collections.Generic;
using System.Linq;

namespace MyWebApi.Services
{
    public class ValuesService : IValuesService
    {
        public IEnumerable<int> GetValues()
        {
            return Enumerable
                .Range(0, 20)
                .OrderBy(x => x);
        }
    }
}