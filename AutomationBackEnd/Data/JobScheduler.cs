using AutomationService.Config;
using AutomationService.Data.Actions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AutomationService.Utility;
using AutomationService.Data.ProducerConsumer;
using AutomationService.Data.Frequency;
using AutomationService.Data.DynamicDataItem;
using AutomationService.Data.ImportExport;

namespace AutomationService.Data
{
    class JobScheduler
    {
        // Initialise a queue for the consumers
        JobQueue oQueue;
        JobRepository oRepository;

        // List to track the consumers
        List<JobConsumer> oConsumers;

        // A limit on the maximum number of queueable jobs for now
        int iMaximumNumberOfQueuedJobs = 10;
        int iFolderScanTime = 1000;

        public JobScheduler()
        {
            // Initialise object vars
            oQueue = new JobQueue(iMaximumNumberOfQueuedJobs);

            // Initialise the repository and start it (so it can find some jobs)
            oRepository = new JobRepository(oQueue, iFolderScanTime);

            ImportDefaultJobs();
            oRepository.Execute();

            // Initialise a list of consumers
            oConsumers = new List<JobConsumer>();
        }

        public void ImportDefaultJobs()
        {
            LogController.LogText("JobScheduler: Insert Default Jobs");

            for (int iIndex = 0; iIndex < 6; iIndex++ )
            {
                ExecutionJob oTemp;

                JobDetails oDetails = new JobDetails();
                oDetails.Description = "This is Job " + iIndex + " of " + 6 + ".";
                oDetails.Name = "Job" + iIndex;

                if (iIndex % 2 == 0)
                {
                    // Initialise a temp var
                    oTemp = new ExecutionJob(
                        oDetails,
                        new JobFrequencyCount(5),
                        new List<ExecutionAction> 
                            { 
                                new FileAction("Test","Read a file",  @"C:\Temp\ReadFile"  + iIndex + ".txt", FileActionType.fatRead, ','),
                                new SQLAction("SQLTest", "Query the DB for some information", "SELECT * FROM APSHARE_FP.WILL_REPORTS", DatabaseType.dbtODBC),
                                new FileAction("Test","Read a file",  @"C:\Temp\ReadFile"  + iIndex + ".txt", FileActionType.fatRead, ','),
                                new FileAction("Test","Write a File", @"C:\Temp\<$1,1/>"  + iIndex + ".txt",  FileActionType.fatWrite, ','),
                            },
                        new List<ExecutionAction> { });
                }
                else
                {
                    // Initialise a temp var
                    oTemp = new ExecutionJob(
                        oDetails,
                        new JobFrequencyDate(FrequencySetting.fs5Minutes, DateTime.Now.AddHours(2)),
                        new List<ExecutionAction> 
                            { 
                                new FileAction("Test","Read a file",  @"C:\Temp\ReadFile"  + iIndex + ".txt", FileActionType.fatRead, ','),
                                new SQLAction("SQLTest", "Query the DB for some information", "SELECT * FROM APSHARE_FP.WILL_REPORTS", DatabaseType.dbtODBC),
                                new FileAction("Test","Read a file",  @"C:\Temp\ReadFile"  + iIndex + ".txt", FileActionType.fatRead, ','),
                                new FileAction("Test","Write a File", @"C:\Temp\<$1,1/>"  + iIndex + ".txt",  FileActionType.fatWrite, ','),
                            },
                        new List<ExecutionAction> { });
                }

                ExecutionJobSerializer.ExportJobToFolder(oTemp, Configuration.Instance.GetSetting("ApprovedDirectory"));
                //oQueue.InsertJob(oTemp);
            }
        }

        public void RunJobs()
        {
            LogController.LogText("Scheduler: Initialising");

            // Initialise a random number generator
            Random oRandomTimeGenerator = new Random();
            int iConsumerSleepTime = Convert.ToInt32(Configuration.Instance.GetSetting("ConsumerRelaxTime"));

            // Create a consumer for each logical process found
            //for (int iCoreCount = 0; iCoreCount < Environment.ProcessorCount; iCoreCount++ )
            for (int iCoreCount = 0; iCoreCount < 1; iCoreCount++)
            {
                // Create a random time to add to the core sleep time
                int iRandomTime = oRandomTimeGenerator.Next(1, 100);

                LogController.LogText("New Consumer Created: " + iCoreCount + ", SleepTime: " + (iConsumerSleepTime + iRandomTime));

                // Create a new consumer to keep track of
                JobConsumer oNewConsumer = new JobConsumer(
                        oQueue,
                        iConsumerSleepTime + iRandomTime
                        );

                // Add to the list to keep track of it
                oConsumers.Add(oNewConsumer);

                // Start running the consumer
                oNewConsumer.Execute();
            }
        }

    }
}
