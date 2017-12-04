using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultlessExecutionSample.DemoServices
{
    public class TheMysteriousCloud
    {
        public IEnumerable<Answer> GetAllTheAnswers()
        {
            if (ThrowError) throw new ApplicationException("Cloud error getting all the answers");

            return new List<Answer>()
            {
                new Answer(){ Name="Choose C"},
                new Answer(){ Name="Pizza"},
            };
        }
        public bool ThrowError { get; set; }
    }
    public class Answer
    {
        public string Name { get; set; }
    }

}
