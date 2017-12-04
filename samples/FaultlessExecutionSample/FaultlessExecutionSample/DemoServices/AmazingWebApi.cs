using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultlessExecutionSample.DemoServices
{
    public class AmazingWebApi
    {
        public IEnumerable<City> GetSomeCities()
        {
            if (ThrowError) throw new ApplicationException("Web Api Error Getting some cities");

            return new List<City>()
            {
                new City(){ Name="City A"},
                new City(){ Name="City B"},
                
                //city C is stupid, i hate that city so i'm not including it

                new City(){ Name="City D"},
            };
        }

        public async Task<IEnumerable<City>> GetSomeCitiesAsync()
        {
            await Task.Delay(100);
            return this.GetSomeCities();
        }

        public bool ThrowError { get; set; }
    }
    public class City
    {
        public string Name { get; set; }
    }
}
