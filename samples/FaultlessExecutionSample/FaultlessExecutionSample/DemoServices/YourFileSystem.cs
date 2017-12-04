using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultlessExecutionSample.DemoServices
{
    public class YourFileSystem
    {
        public void CheckAllOfYourFiles()
        {
            if (ThrowError) throw new ApplicationException("Error checking all your files");

            return;
        }

        public bool ThrowError { get; set; }
    }
}
