﻿using AutomationService.Config;
using AutomationService.Data.Actions;
using AutomationService.Data.DynamicDataItem;
using AutomationService.Data.Frequency;
using System;
using System.Collections.Generic;
using System.Data;
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
        public List<ExecutionAction> Triggers;

        [DataMember, XmlElement]
        // What actions will be run for this job
        public List<ExecutionAction> Actions;

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
            Triggers = xaoTriggers;
            Actions = xaoActions;

            // Store the message logging function as a delegate
            oEnvironment = new ExecutionJobEnvironment(FreqType.LogMessage);
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
            FreqType.NewSession(Details.OperatingPath);

            // Run all actions
            foreach (ExecutionAction oLoopingVar in Triggers)
            {
                // Intermediate var
                StackFrame oTemp = oLoopingVar.Execute(this.oEnvironment);

                // If some data was returned to the stack
                if (oTemp != null && oTemp.Item != null)
                {
                    // Store the data returned by the action
                    oEnvironment.PushDataContainer(oTemp);
                }

                // Yield some time after each execution
                Thread.Yield();
            }

            // If the topmost object on the stack is valid, we can run the actions
            if (CheckIfTriggersWereSuccessful())
            {
                // Run all actions
                RunActions();

                // Set the last successful run as now
                FreqType.RecordSuccessfulExecution();
            }
        }

        /// <summary>
        /// Returns true if we can confirm the triggers passed
        /// </summary>
        /// <returns></returns>
        private Boolean CheckIfTriggersWereSuccessful()
        {
            // Intermediate var
            StackFrame oTopItem = oEnvironment.PeekDataContainer();

            // return the bool value
            return oTopItem.IsValidOrContainsData();
        }

        /// <summary>
        /// Performs all of the execution actions in the Actions Array
        /// </summary>
        public void RunActions()
        {
            foreach (ExecutionAction oLoopingVar in Actions)
            {
                // Intermediate var
                StackFrame oTemp = oLoopingVar.Execute(oEnvironment);

                // If some data was returned to the stack
                if (oTemp != null && oTemp.Item != null)
                {
                    // Store the data returned by the action
                    oEnvironment.PushDataContainer(oTemp);
                }

                // Yield some time after each execution
                Thread.Yield();
            }
        }

        /// <summary>
        /// Finalise the job by moving it to the complete folder
        /// </summary>
        public void CloseJob()
        {
            // Move the job to the completed folder
            MoveJobToFolder(Configuration.Instance.GetSetting("CompletedDirectory"));
        }

        /// <summary>
        /// Move the Job to a new folder
        /// </summary>
        /// <param name="xsNewFolder"></param>
        public void MoveJobToFolder(String xsNewFolder)
        {
            String sDestination = Path.Combine(xsNewFolder, Details.Name);

            // Move to the completed folder
            if (Directory.Exists(Details.OperatingPath) && !Details.OperatingPath.Equals(sDestination))
            {
                // If we find a conflict
                if (Directory.Exists(sDestination))
                {
                    // Add a number to the end of the file if necessary
                    for (int iIndex = 0; iIndex < 100; iIndex++)
                    {
                        String sNewDirectory = sDestination + iIndex;

                        // If we find a name we can use
                        if (!File.Exists(sNewDirectory) && !Directory.Exists(sNewDirectory))
                        {
                            sDestination = sNewDirectory;
                            break;
                        }
                    }
                }

                // Actually move the folder
                Directory.Move(Details.OperatingPath, sDestination);

                // Assign the new operating path
                Details.OperatingPath = sDestination;
            }
        }

    }
}
