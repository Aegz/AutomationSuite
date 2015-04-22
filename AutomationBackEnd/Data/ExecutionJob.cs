using AutomationService.Config;
using AutomationService.Data.Actions;
using AutomationService.Data.DynamicDataItem;
using AutomationService.Data.Frequency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutomationService.Data
{
    [DataContract]
    [XmlSerializerFormat]
    public class ExecutionJob
    {
        [XmlIgnore]
        private JobDetails oDetails;

        [XmlIgnore]
        private JobFrequency oFreqType;

        [DataMember, XmlElement]
        public JobDetails Details
        {
            get
            {
                if (oDetails == null)
                {
                    oDetails = new JobDetails();
                }
                return oDetails;
            }
            set
            {
                oDetails = value;
            }
        }

        [DataMember, XmlElement]
        public JobFrequency FreqType
        {
            get
            {
                if (oFreqType == null)
                {
                    // Default to a count 1
                    oFreqType = new JobFrequencyCount(1);
                }
                return oFreqType;
            }
            set
            {
                oFreqType = value;
            }
        }

        // What conditions need to be fulfilled to run the actions
        [DataMember, XmlElement]
        public List<ExecutionAction> aoTriggers;

        [DataMember, XmlElement]
        // What actions will be run for this job
        public List<ExecutionAction> aoActions;

        // Environment object which keeps track of what data is available
        [DataMember]
        private ExecutionJobEnvironment oEnvironment;

        public ExecutionJob() 
            : this (new JobDetails(), new JobFrequencyDate(FrequencySetting.fsAdhoc, DateTime.Now), new List<ExecutionAction>(), new List<ExecutionAction>())
        {

        }

        public ExecutionJob(JobDetails xoDetails, JobFrequency xoFreq)
            : this(xoDetails, xoFreq, new List<ExecutionAction>(), new List<ExecutionAction>())
        {

        }

        /// <summary>
        /// Basic constructor which initialises the core object variables
        /// </summary>
        /// <param name="xsName"></param>
        /// <param name="xsDescription"></param>
        /// <param name="xaoTriggers"></param>
        /// <param name="xaoActions"></param>
        public ExecutionJob(
            JobDetails xoDetails,
            JobFrequency xoFreq, 
            List<ExecutionAction> xaoTriggers, 
            List<ExecutionAction> xaoActions)
        {
            Details.Name = xoDetails.Name;
            Details.Description = xoDetails.Description;

            FreqType = xoFreq;
            aoTriggers = xaoTriggers;
            aoActions = xaoActions;

            oEnvironment = new ExecutionJobEnvironment();
        }

        /// <summary>
        /// Runs the triggers and then runs the actions if the triggers
        /// leave a YES or TRUE
        /// </summary>
        public void Execute()
        {
            // Move the job to the completed folder
            MoveJobToFolder(Configuration.Instance.GetSetting("ActiveJobsDirectory"));
            
            // Tell the frequency that it last ran now
            FreqType.LastRun = DateTime.Now; 

            // Run all actions
            foreach (ExecutionAction oLoopingVar in aoTriggers)
            {
                // Intermediate var
                DataItemContainer oTemp = oLoopingVar.Execute(this.oEnvironment);

                // If some data was returned to the stack
                if (oTemp != null && oTemp.Value != null)
                {
                    // Store the data returned by the action
                    oEnvironment.PushDataContainer(oTemp);
                }

                // Yield some time after each execution
                Thread.Yield();
            }

            // Intermediate var
            DataItemContainer oTopItem = oEnvironment.PeekDataContainer();

            // If the topmost object on the stack is valid, we can run the actions
            if ((oTopItem.Key == typeof(Boolean) && (Boolean)oTopItem.Value == true) || // Boolean and true
                (oTopItem.Key == typeof(String) && !String.IsNullOrWhiteSpace((String)oTopItem.Value))  // String and not null
                ) 
            {
                // Run all actions
                RunActions();

                // Set the last successful run as now
                FreqType.RecordSuccessfulExecution();
            }
        }

        /// <summary>
        /// Performs all of the execution actions in the Actions Array
        /// </summary>
        public void RunActions()
        {
            foreach (ExecutionAction oLoopingVar in aoActions)
            {
                // Intermediate var
                DataItemContainer oTemp = oLoopingVar.Execute(oEnvironment);

                // If some data was returned to the stack
                if (oTemp != null && oTemp.Value != null)
                {
                    // Store the data returned by the action
                    oEnvironment.PushDataContainer(oTemp);
                }

                // Yield some time after each execution
                Thread.Yield();
            }
        }

        public Boolean CanBeExecuted()
        {
            return FreqType.CanBeExecuted();
        }

        public Boolean CanBeRemoved()
        {
            return FreqType.CanBeRemoved();
        }

        public void CloseJob()
        {
            // Move the job to the completed folder
            MoveJobToFolder(Configuration.Instance.GetSetting("CompletedDirectory"));
        }

        public void MoveJobToFolder(String xsNewFolder)
        {
            String sDestination = Path.Combine(xsNewFolder, Details.Name);

            // Move to the completed folder
            if (Directory.Exists(Details.OperatingPath) && !Details.OperatingPath.Equals(sDestination))
            {
                // Actually move the folder
                Directory.Move(Details.OperatingPath, sDestination);
                // Assign the new operating path
                Details.OperatingPath = sDestination;
            }
        }

    }
}
