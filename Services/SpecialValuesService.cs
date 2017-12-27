using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApi.Services
{
    public class SpecialValuesService : ISpecialValuesService
    {
        private readonly IValuesService _valuesService;

        public SpecialValuesService(IValuesService valuesService)
        {
            _valuesService = valuesService;
        }

        public async Task<IEnumerable<int>> GetEvenValues()
        {
            await Task.Delay(2500);
            return _valuesService.GetValues().Where(n => n % 2 == 0);
        }

        public async Task<IEnumerable<int>> GetOddValues()
        {
            await Task.Delay(100);
            return _valuesService.GetValues().Where(n => n % 2 != 0);
        }
    }
}