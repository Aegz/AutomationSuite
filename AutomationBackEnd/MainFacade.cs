using AutomationService.Config;
using AutomationService.Data;
using AutomationService.WCF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService
{
    class MainFacade
    {
        // Singleton behaviour (The WCF Service can't see anything)
        private static MainFacade oFacadeInstance = null;
        private static object oLock = new object();

        public static MainFacade Instance
        {
            get
            {
                // Doublely locked Singleton Check
                if (oFacadeInstance == null)
                {
                    lock (oLock)
                    {
                        if (oFacadeInstance == null)
                        {
                            oFacadeInstance = new MainFacade();
                        }
                    }
                }

                // Return something
                return oFacadeInstance;
            }
        }

        // WCF Controller
        private WCFController oWCF;
        private JobScheduler oScheduler;

        private MainFacade()
        {
            // Initialise the config
            Configuration.Instance.ImportSettings();

            // Initialise the Scheduler
            oScheduler = new JobScheduler();

            // Start processing jobs (and initialise too if necessary)
            oScheduler.RunJobs();

            // Initialise a WCF Controller
            oWCF = new WCFController();

            // Start the WCF host
            oWCF.InitialiseWCF();
        }
        
        public List<ExecutionJob> GetJobs(String[] xasCodes)
        {

            /*
             * 
        public List<ExecutionJob> GetAllJobs(String[] xasStatusCodes)
        {
            // Initialise something to return
            List<ExecutionJob> aoReturnList = new List<ExecutionJob>();

            if (xasStatusCodes.Length == 0)
            {
                // Just return the active jobs
                xasStatusCodes = new String[] 
                { 
                    JobDetails.STATUS_ACTIVE,
                    JobDetails.STATUS_QUEUED,
                    JobDetails.STATUS_PENDING,
                };
            }

            // Remove any duplicate codes
            String[] asUniqueStatusCodes = xasStatusCodes.Distinct().ToArray();
            lock (this)
            {
                // Loop through all of the criteria and add as necessary
                foreach (String xsCode in asUniqueStatusCodes)
                {
                    // the codes should be a single char
                    String sTrimmedCode = xsCode.Substring(0, 1);

                    // Switch on the code
                    if (sTrimmedCode.Equals(JobDetails.STATUS_ACTIVE))
                    {
                        aoReturnList.AddRange(aoActiveJobQueue.Select((oItem) => oItem).ToList());
                    }
                    else if (sTrimmedCode.Equals(JobDetails.STATUS_QUEUED))
                    {
                        aoReturnList.AddRange(aoScheduledJobs);
                    }
                    else if (sTrimmedCode.Equals(JobDetails.STATUS_COMPLETE))
                    {
                        //aoReturnList.AddRange(aoCompletedJobs);
                    }
                    else if (sTrimmedCode.Equals(JobDetails.STATUS_PENDING))
                    {
                        aoReturnList.AddRange(aoJobsToBeAddedQueue.Select((oItem) => oItem).ToList());
                    }
                }
            }
      

            // Only return unique jobs, not duplicates
            return aoReturnList.GroupBy(oJob => oJob.Details.Name).Select(grp => grp.First()).ToList();   
        }
             */
            return new List<ExecutionJob>();
        }
    }
}
