using AutomationService.Config;
using AutomationService.Data.Actions;
using AutomationService.Data.Frequency;
using AutomationService.Utility;
using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationService.Data.ProducerConsumer
{
    class JobRepository
    {
        Thread oWorker;
        JobProducer oProducer;
        
        // Configuration specific variables
        Boolean bIsActive = true;
        int iConsumerSleepTime = 5000;
        int iIteration = 0;

        // 1. Jobs found, that need to be added to the schedule
        private JobQueue aoJobsToBeAddedQueue;

        // 2. Jobs that we need to run
        private List<ExecutionJob> aoScheduledJobs;
        
        // 3. Jobs that can be executed are queued here
        private JobQueue aoActiveJobQueue;

        public JobRepository(JobQueue xoQueue, int xiSleepTime)
        {
            // Store the queue reference for the consumers
            aoActiveJobQueue = xoQueue;
            // Set the sleep time
            iConsumerSleepTime = xiSleepTime;

            // Initialise a queue to be used for the producer
            aoJobsToBeAddedQueue = new JobQueue(100);

            // Initialise a producer to start finding new jobs
            oProducer = new JobProducer(1000, aoJobsToBeAddedQueue);

            // Initialise the core lists
            aoScheduledJobs = new List<ExecutionJob>();

            CleanseActiveAndPendingFolders();
        }

        /// <summary>
        /// Cleans up the active and pending folders for initialisation.
        /// The folders should be empty
        /// </summary>
        private void CleanseActiveAndPendingFolders()
        {
            // Get every directory in our working directory
            String[] asDirectories = Directory.GetDirectories(Configuration.Instance.GetSetting("PendingJobsDirectory"));

            // For each directory we have found
            foreach (String sNewDirectory in asDirectories)
            {
                Directory.Move(
                    sNewDirectory,
                    Path.Combine(Configuration.Instance.GetSetting("ApprovedDirectory"), Path.GetFileName(sNewDirectory)));
            }

            // Get every directory in our working directory
            asDirectories = Directory.GetDirectories(Configuration.Instance.GetSetting("ActiveJobsDirectory"));

            // For each directory we have found
            foreach (String sNewDirectory in asDirectories)
            {
                Directory.Move(
                    sNewDirectory,
                    Path.Combine(Configuration.Instance.GetSetting("ApprovedDirectory"), Path.GetFileName(sNewDirectory)));
            }
        }

        /// <summary>
        /// Start the worker which will consume and produce jobs
        /// </summary>
        public void Execute()
        {
            // Start running the producer
            oProducer.Execute();

            // Only spawn a new thread when the execution is stopped
            if (oWorker == null || oWorker.ThreadState == ThreadState.Stopped)
            {
                oWorker = new Thread(new ThreadStart(StartConsuming));
                oWorker.Start();
            }
        }

        /// <summary>
        /// Start finding and adding jobs for the consumers
        /// </summary>
        public void StartConsuming()
        {
            // Just keep running until told to die
            for (; ; iIteration++)
            {
                // Always sleep a little bit to start with
                Thread.Sleep(iConsumerSleepTime);

                // Check if this is still active
                if (!bIsActive)
                {
                    LogController.LogText("Repository: Exit");
                    // Exit the loop and end thread
                    break;
                }

                // Adds jobs to the schedule if they will be run
                ScheduleJobsToBeRun();

                // Add jobs that will be consumed (run soon)
                MoveScheduledJobsToExecutionQueue();

                // Every iteration, destroy anything that is complete
                lock (aoActiveJobQueue)
                {
                    // Flush out dead jobs
                    ClearDeactiveJobs();
                }

                // Yield some time to other processes
                Thread.Yield();
            }
        }

        #region Adding/Removing Jobs from Queue

        /// <summary>
        /// Schedules the jobs to be run some time in the future
        /// </summary>
        private void ScheduleJobsToBeRun()
        {
            // Add new jobs (try and get 4 jobs)
            for (int iIndex = 0; iIndex < 4; iIndex++)
            {
                // Try and retrieve a job
                ExecutionJob oNewJob = aoJobsToBeAddedQueue.GetJob();

                // If we got a job out of the queue
                if (oNewJob != null)
                {
                    // If the job has not already been added
                    if (!aoScheduledJobs.Any((oJob) => oJob.Details.Name == oNewJob.Details.Name))
                    {
                        LogController.LogText("Repository: Job has been scheduled (" + oNewJob.Details.Name + ")");

                        // Add to the schedule
                        aoScheduledJobs.Add(oNewJob);
                    }
                }
            }
        }

        /// <summary>
        /// Looks through the schedule for jobs that can be run now
        /// </summary>
        private void MoveScheduledJobsToExecutionQueue()
        {
            // Loop through the scheduled jobs and insert if necessary
            foreach (ExecutionJob oJob in aoScheduledJobs)
            {
                // If the job can be executed and it hasn't already been added
                if (oJob.FreqType.CanBeExecuted() && !aoActiveJobQueue.ContainsJob(oJob))
                {
                    LogController.LogText("Repository: Job has been queued (" + oJob.Details.Name + ")");

                    // Add the job
                    aoActiveJobQueue.InsertJob(oJob);
                }
            }
        }

        /// <summary>
        /// Clears out jobs that are complete or can be removed.
        /// </summary>
        private void ClearDeactiveJobs()
        {
            // Loop through the scheduled jobs and remove if necessary
            for (int iIndex = aoScheduledJobs.Count - 1; iIndex > -1; iIndex--)
            {
                // Intermediate Vars
                ExecutionJob oJob = aoScheduledJobs[iIndex];
                JobFrequency oFreq = oJob.FreqType;

                // If adhoc or it can be removed
                if (oFreq.CanBeRemoved())
                {
                    LogController.LogText("Job Removed: " + oJob.Details.Name + " " + (oFreq.CanBeRemoved() ? "Can Be Removed" : "Adhoc"));
                    // Move the job from scheduled to inactive
                    aoScheduledJobs.Remove(oJob);
                    oJob.CloseJob();
                }
            } 
        }
        
        #endregion

    }
}
