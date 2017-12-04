using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultlessExecutionSample.DemoServices
{
    public class AwesomeSqlDatabase
    {
        public IEnumerable<Person> GetAllThePeopleInTheWorld()
        {
            if (ThrowError) throw new ApplicationException("Sql Error Getting all the people in the world");

            return new List<Person>()
            {
                new Person(){ Name="You"},
                new Person(){ Name="Me"},
                new Person(){ Name="Him"},
                new Person(){ Name="Her"},
             };

        }

        public async Task<IEnumerable<Person>> GetAllThePeopleInTheWorldAsync()
        {
            await Task.Delay(100);
            return this.GetAllThePeopleInTheWorld();
        }

        public bool ThrowError { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
    }

}
