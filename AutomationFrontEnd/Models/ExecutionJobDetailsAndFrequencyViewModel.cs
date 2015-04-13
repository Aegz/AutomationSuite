using AutomationSuiteFrontEnd.AutomationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationSuiteFrontEnd.Models
{
    public class ExecutionJobDetailsAndFrequencyViewModel
    {
        public JobDetails Details { get; set; }

        public JobFrequency Frequency { get; set; }
    }
}