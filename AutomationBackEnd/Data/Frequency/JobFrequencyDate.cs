using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutomationService.Data.Frequency
{
    [DataContract]
    [XmlSerializerFormat]
    public class JobFrequencyDate : JobFrequency
    {
        [XmlIgnore]
        DateTime dLastScheduledTimeToRun;

        public JobFrequencyDate() : this(FrequencySetting.fsAdhoc) { }

        public JobFrequencyDate(FrequencySetting xeType) : this(xeType, DateTime.Now)
        {

        }

        public JobFrequencyDate(FrequencySetting xeType, DateTime xdFinishDate)
            : this(xeType, DateTime.Now, xdFinishDate)
        {
        
        }

        public JobFrequencyDate(FrequencySetting xeType, DateTime xdScheduledTime, DateTime xdLastScheduledTime) : base(xeType, xdScheduledTime)
        {
            dLastScheduledTimeToRun = xdLastScheduledTime;
        }

        [XmlIgnore]
        public DateTime LastScheduledRun
        {
            get
            {
                return dLastScheduledTimeToRun;
            }
        }

        [DataMember, XmlElement]
        public String LastScheduledRunAsString
        {
            get { return dLastScheduledTimeToRun.ToString("g"); }
            set { dLastScheduledTimeToRun = DateTime.Parse(value); }
        }

        public override bool CanBeExecuted()
        {
            // Store once for later use
            DateTime oNow = DateTime.Now;

            // If the first scheduled time is valid
            if (LastScheduledRun > oNow && FirstScheduled < oNow)
            {
                // Check if the job has been run at least once
                Boolean bJobHasBeenRunOnce = LastRun != JobFrequency.NEVER_RUN;

                // Do not remove jobs that have not been run
                if (!bJobHasBeenRunOnce)
                {
                    return true;
                }

                // Else check if enough time has elapsed since then
                switch (FrequencyType)
                {
                    case FrequencySetting.fsAdhoc:
                        return true;
                    case FrequencySetting.fsMonthly:
                        // Run if we have run this more than 31 days ago
                        return LastRun.AddDays(31) < oNow;
                    case FrequencySetting.fsWeekly:
                        // Run if we have run this more than 7 days ago
                        return LastRun.AddDays(7) < oNow;
                    case FrequencySetting.fsDaily:
                        // Run if we have run this more than 24 hours ago
                        return LastRun.AddHours(24) < oNow;
                    case FrequencySetting.fsHourly:
                        // Run if we have run this more than 1 hour ago
                        return LastRun.AddHours(1) < oNow;
                    case FrequencySetting.fs5Minutes:
                        // Run if we have run this more than 5 minutes ago
                        return LastRun.AddMinutes(5) < oNow;
                    case FrequencySetting.fs1Minute:
                        // Run if we have run this more than 59 minutes ago
                        return LastRun.AddSeconds(60) < oNow;
                    // Default includes adhoc
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override bool CanBeRemoved()
        {
            // If it is adhoc
            if (FrequencyType == FrequencySetting.fsAdhoc)
            {
                // Has been run once
                if (LastRun != NEVER_RUN)
                {
                    // can be removed.
                    return true;
                }
            }
            else if (DateTime.Now > LastScheduledRun) // The job has expired
            {
                // Move the job from scheduled to inactive
                return true;
            }

            //default to false
            return false;
        }
    }
}
