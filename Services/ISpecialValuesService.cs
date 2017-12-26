using System.Collections.Generic;
using System.Threading.Tasks;

namespace mywebapi.Services
{
    public interface ISpecialValuesService
    {
        Task<IEnumerable<int>> GetEvenValues();
        Task<IEnumerable<int>> GetOddValues();
    }
}