using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultlessExecutionSample
{
    public class ShowMessageBoxFaultlesExecutionService : FaultlessExecution.FaultlessExecutionService
    {
        protected override void OnException(Exception ex)
        {
            base.OnException(ex);

            System.Windows.MessageBox.Show(ex.Message);
        }
    }
}
