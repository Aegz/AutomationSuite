using AutomationSuiteFrontEnd.AutomationService;
using AutomationSuiteFrontEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutomationSuiteFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly String NEW_JOB_FLAG = "NEW JOB";
        //
        // GET: /Home/

        public ActionResult Index()
        {
            // Try and get the available actions we can use
            ViewBag.ExecutionJobs = new List<ExecutionJob>();

            for (int iIndex = 0; iIndex < 4; iIndex++)
            {
                ExecutionJob oTemp = new ExecutionJob();
                oTemp.Details = new JobDetails();
                oTemp.Details.Name = "JOB " + iIndex;
  
                oTemp.Details.Description = "This Experimental job could send reports out daily or automatically transfer files via FTP";

                JobFrequencyCount oFreq = new JobFrequencyCount();
                oFreq.FrequencyType = FrequencySetting.fs5Minutes;
                oFreq.MaximumRunCount = 5;
                oTemp.FreqType = oFreq;

                ViewBag.ExecutionJobs.Add(oTemp);
            }

            //
            return View("Dashboard");
        }

        public ActionResult JobList()
        {
            // Try and get the available actions we can use
            ViewBag.ExecutionJobs = new List<ExecutionJob>();

            for (int iIndex = 0; iIndex < 4; iIndex++)
            {
                ExecutionJob oTemp = new ExecutionJob();
                oTemp.Details = new JobDetails();
                oTemp.Details.Name = "JOB " + iIndex;
                oTemp.Details.Description = "TEST DESCRIPTION";

                JobFrequencyCount oFreq = new JobFrequencyCount();
                oFreq.FrequencyType = FrequencySetting.fs5Minutes;
                oFreq.MaximumRunCount = 5;
                oTemp.FreqType = oFreq;

                ViewBag.ExecutionJobs.Add(oTemp);
            }

            //
            return View("JobList");
        }

        /// <summary>
        /// This method is responsible for populating the toolbox item
        /// which means that any new items must be added here manually
        /// </summary>
        private void InitialiseToolbox()
        {
            FileAction oFileAction = new FileAction();
            oFileAction.Name = "FileAction";

            SQLAction oSQLAction = new SQLAction();
            oSQLAction.Name = "SQLAction";

            ViewBag.Toolbox = new List<ExecutionAction> 
            { 
                oFileAction,
                oSQLAction,
            };
        }

        //
        // GET: /Home/Details/5
        public ActionResult Details(String id)
        {
            ViewBag.JobName = id;

            // If we are not given the new job flag
            if (!id.Equals(NEW_JOB_FLAG))
            {
                // Query the System to get the job

                // If fails, return error page

            }
            else
            {

            }

            // Initialise a new Job Object

            JobDetails oDetails = new JobDetails();
            oDetails.Name = "";
            oDetails.Description = "";

            // Initialise a new Job Frequency object
            JobFrequencyCount oFreq = new JobFrequencyCount();
            oFreq.FirstScheduledAsString = DateTime.Now.ToString("u");
            oFreq.FrequencyType = FrequencySetting.fs5Minutes;
            oFreq.MaximumRunCount = 5;

            // Store in a Model for the view
            ExecutionJobDetailsAndFrequencyViewModel viewModel = new ExecutionJobDetailsAndFrequencyViewModel
            {
                Details = oDetails,
                Frequency = oFreq
            };

            return View(viewModel);
        }


        //
        // GET: /Home/Conditions/JobName
        public ActionResult Conditions(String id)
        {
            ViewBag.JobName = id;

            // prework
            InitialiseToolbox();

            // Try and get the available actions we can use
            List<ExecutionAction> aoJobs = new List<ExecutionAction>();
            for (int iIndex = 0; iIndex < 4; iIndex++)
            {
                FileAction oTEmp = new FileAction();
                oTEmp.Name = "BLAH FILE " + iIndex;
                oTEmp.Description = "DESC " + iIndex;
                oTEmp.FileType = FileActionType.fatRead;
                
                aoJobs.Add(oTEmp);
            }

            for (int iIndex = 0; iIndex < 4; iIndex++)
            {
                SQLAction oTEmp = new SQLAction();
                oTEmp.Name = "BLAH SQL " + iIndex;
                oTEmp.Description = "DESC " + iIndex;
                oTEmp.DatabaseType = DatabaseType.dbtNeteeza;

                aoJobs.Add(oTEmp);
            }

            return View(
                new List<ExecutionJobTriggerList>
                {
                    new ExecutionJobTriggerList { 
                        Actions = aoJobs
                    }
                }
                );
        }

        //
        // GET: /Home/Actions/JobName
        public ActionResult Actions(String id)
        {
            ViewBag.JobName = id;

            // prework
            InitialiseToolbox();

            // Try and get the available actions we can use
            List<ExecutionAction> aoJobs = new List<ExecutionAction>();
            for (int iIndex = 0; iIndex < 4; iIndex++)
            {
                FileAction oTEmp = new FileAction();
                oTEmp.Name = "BLAH FILE " + iIndex;
                oTEmp.Description = "DESC " + iIndex;
                oTEmp.FileType = FileActionType.fatRead;

                aoJobs.Add(oTEmp);
            }

            for (int iIndex = 0; iIndex < 4; iIndex++)
            {
                SQLAction oTEmp = new SQLAction();
                oTEmp.Name = "BLAH SQL " + iIndex;
                oTEmp.Description = "DESC " + iIndex;
                oTEmp.DatabaseType = DatabaseType.dbtNeteeza;

                aoJobs.Add(oTEmp);
            }

            return View(
                new List<ExecutionJobActionList>
                {
                    new ExecutionJobActionList { 
                        Actions = aoJobs
                    }
                }
                );
        }

        // 
        // GET: /Home/TestExecution/JobName
        public ActionResult TestExecution(String id)
        {
            return View("JobList");
        }
    }
}
