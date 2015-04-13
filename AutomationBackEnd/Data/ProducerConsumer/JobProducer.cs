using AutomationService.Config;
using AutomationService.Data.Actions;
using AutomationService.Data.Frequency;
using AutomationService.Data.ImportExport;
using AutomationService.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationService.Data
{
    class JobProducer
    {

        Thread oWorker;
        int iSleepTime;
        Boolean bIsActive;
        JobQueue oQueue;

        public JobProducer(int xiSleepTime, JobQueue xoJobQueue)
        {
            iSleepTime = xiSleepTime;
            bIsActive = true;
            oQueue = xoJobQueue;
        }

        public void Execute()
        {
            // Only spawn a new thread when the execution is stopped
            if (oWorker == null || oWorker.ThreadState == ThreadState.Stopped)
            {
                oWorker = new Thread(new ThreadStart(ScanFoldersForChanges));
                oWorker.Start();
            }
        }

        public void StopWork()
        {
            // Set to false
            bIsActive = false;
        }

        private void ScanFoldersForChanges()
        {
            // Just keep running until told to die
            while (true)
            {
                // Always sleep a little bit to start with
                Thread.Sleep(iSleepTime);

                // Check if this is still active
                if (!bIsActive)
                {
                    LogController.LogText("Producer: Exit");
                    // Exit the loop and end thread
                    break;
                }

                // Get every directory in our working directory
                ScanDirectoryForJobs(Configuration.Instance.GetSetting("ApprovedDirectory"));

                // Yield some time
                Thread.Yield();
            }
        }

        private void ScanDirectoryForJobs (String xsFolderPath)
        {
            // Get every directory in our working directory
            String[] asDirectories = Directory.GetDirectories(xsFolderPath);

            // For each directory we have found
            foreach (String sNewDirectory in asDirectories)
            {
                // Scan it for a config file
                if (File.Exists(Path.Combine(sNewDirectory, Configuration.Instance.GetSetting("ConfigFileName"))))
                {
                    // Get the new job from the folder
                    ExecutionJob oNewJob = ExecutionJobSerializer.CreateJobFromFolder(sNewDirectory);

                    // Move it to the pending folder
                    oNewJob.MoveJobToFolder(Configuration.Instance.GetSetting("PendingJobsDirectory"));

                    // Import the job
                    oQueue.InsertJob(oNewJob);
                }
            }
        }
    }
}
