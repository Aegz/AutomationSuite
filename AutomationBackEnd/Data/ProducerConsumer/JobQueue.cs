using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data
{
    class JobQueue : IEnumerable<ExecutionJob>
    {
        private ConcurrentQueue<ExecutionJob> oQueue;

        private int iFixedLength = 10; // Default to 10

        public JobQueue(int xiMaxLength)
        {
            // Set the maximum length and initialise the queue
            iFixedLength = xiMaxLength;
            oQueue = new ConcurrentQueue<ExecutionJob>();
        }


        public void InsertJob(ExecutionJob xoJob)
        {
            // Simply add to the queue
            oQueue.Enqueue(xoJob);
        }

        public ExecutionJob GetJob()
        {
            //Intermediate Var
            ExecutionJob oJob;

            // Try and get an object off the queue
            if (oQueue.TryDequeue(out oJob))
            {
                // Requeue (infinite Queue)
                //oQueue.Enqueue(oJob);
                // Return a job to use
                return oJob;
            }
            else
            {
                // Return a fail
                return null;
            }
        }

        public Boolean ContainsJob(ExecutionJob xoJob)
        {
            return oQueue.Any(oItem => oItem.Details.Name == xoJob.Details.Name);
        }

        public Boolean HasRoomForNewJobs()
        { 
            // Always leave a tiny bit of room
            return oQueue.Count < iFixedLength - 1;
        }


        IEnumerator<ExecutionJob> IEnumerable<ExecutionJob>.GetEnumerator()
        {
            return oQueue.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return oQueue.GetEnumerator();
            throw new NotImplementedException();
        }
    }
}
