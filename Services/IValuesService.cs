using System.Collections.Generic;

namespace mywebapi.Services
{
    public interface IValuesService
    {
        IEnumerable<int> GetValues(int customerId);
    }
}