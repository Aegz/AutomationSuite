using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ServiceModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;
using AutomationService.Data;


namespace AutomationService
{
    [ServiceContract]
    interface IWCFInterface
    {
        [OperationContract]
        List<String> GetLog();

        [OperationContract]
        ExecutionJob GetJob(String xsJobName);

        [OperationContract]
        List<ExecutionJob> GetJobs(String[] xasCodes);
    }


}
