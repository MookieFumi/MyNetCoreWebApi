using System.Collections.Generic;

namespace MyWebApi.Services
{
    public interface IValuesService
    {
        IEnumerable<int> GetValues();
    }
}