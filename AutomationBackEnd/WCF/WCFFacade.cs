
using AutomationService.Data;
using AutomationService.Data.DynamicDataItem;
using AutomationService.Data.Frequency;
using AutomationService.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService
{
    class WCFFacade : IWCFInterface
    {

        public List<String> GetLog()
        {
            return Program.GetForm().GetAllText();
        }

        public ExecutionJob GetJob(String xsJobName)
        {
            return new ExecutionJob(new JobDetails(), new JobFrequencyCount(1));
        }

        public List<ExecutionJob> GetJobs(String[] xasCodes)
        {
            return MainFacade.Instance.GetJobs(xasCodes);
        }
    }
}
