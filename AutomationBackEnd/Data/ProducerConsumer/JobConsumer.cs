using AutomationService.Config;
using AutomationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationService.Data
{
    class JobConsumer
    {
        Thread oWorker;
        JobQueue oQueue;

        // Configuration specific variables
        Boolean bIsActive = true;
        int iConsumerSleepTime = 2500;

        public JobConsumer(JobQueue xoQueue, int xiSleepTime)
        {
            //
            oQueue = xoQueue;
            iConsumerSleepTime = xiSleepTime;
        }

        public void Execute()
        {
            // Only spawn a new thread when the execution is stopped
            if (oWorker == null || oWorker.ThreadState == ThreadState.Stopped)
            {
                oWorker = new Thread(new ThreadStart(StartConsuming));
                oWorker.Start();
            }
        }

        public void StopWork()
        {
            // Set to false
            bIsActive = false;       
        }

        public void StartConsuming()
        {
            // Just keep running until told to die
            while (true)
            {
                // Always sleep a little bit to start with
                Thread.Sleep(iConsumerSleepTime);

                // Check if this is still active
                if (!bIsActive)
                {
                    LogController.LogText("Consumer: EXIT");
                    // Exit the loop and end thread
                    break;
                }

                // Try and get a job
                ExecutionJob oJob = oQueue.GetJob();

                // if a job was retrieved
                if (oJob != null)
                {
                    //
                    LogController.LogText("Consumer: Execute job (" + oJob.Details.Name + ")");
                    // Run it
                    oJob.Execute();
                }

                // Yield some time after each run
                Thread.Yield();
            }
        }


    }
}
