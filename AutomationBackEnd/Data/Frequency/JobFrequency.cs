using AutomationService.Data.Frequency;
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
    public enum FrequencySetting
    {
        fsAdhoc,
        fsMonthly,
        fsWeekly,
        fsDaily,
        fsHourly,
        fs5Minutes,
        fs1Minute
    }

    [DataContract]
    [KnownType(typeof(JobFrequencyDate))]
    [KnownType(typeof(JobFrequencyCount))]
    [XmlSerializerFormat]
    public abstract class JobFrequency
    {
        public static readonly DateTime NEVER_RUN = new DateTime(0001,1,1);

        [XmlIgnore]
        DateTime dLastRun;
        [XmlIgnore]
        DateTime dLastSuccessfulRun;
        [XmlIgnore]
        DateTime dFirstScheduledTimeToRun;
        [XmlIgnore]
        FrequencySetting eFreqType;

        public JobFrequency() : this (FrequencySetting.fsAdhoc)
        {

        }

        public JobFrequency(FrequencySetting xeType) : this(xeType, DateTime.Now)
        {

        }

        public JobFrequency(FrequencySetting xeType, DateTime xdScheduledTime)
        {
            dFirstScheduledTimeToRun = xdScheduledTime;
            eFreqType = xeType;
        }

        public virtual Boolean CanBeExecuted()
        {
            return true;
        }

        public virtual Boolean CanBeRemoved()
        {
            return true;
        }

        public virtual void RecordSuccessfulExecution()
        {
            dLastSuccessfulRun = DateTime.Now;
        }


        [DataMember, XmlElement]
        public FrequencySetting FrequencyType
        {
            get
            {
                return eFreqType;
            }
            set
            {
                eFreqType = value;
            }
        }

        [XmlIgnore]
        public DateTime FirstScheduled
        {
            get
            {
                return dFirstScheduledTimeToRun;
            }
        }

        [DataMember, XmlElement]
        public String FirstScheduledAsString
        {
            get { return dFirstScheduledTimeToRun.ToString("g"); }
            set { dFirstScheduledTimeToRun = DateTime.Parse(value); }
        }

        [XmlIgnore]
        public DateTime LastRun
        {
            get
            {
                if (dLastRun == null)
                {
                    return NEVER_RUN;
                }
                return dLastRun;
            }
            set
            {
                dLastRun = value;
            }
        }

        [DataMember, XmlElement]
        public String LastRunAsString
        {
            get { return dLastRun.ToString("g"); }
            set { dLastRun = DateTime.Parse(value); }
        }
        
        [XmlIgnore]
        public DateTime LastSuccessfulRun
        {
            get
            {
                if (dLastSuccessfulRun == null)
                {
                    return NEVER_RUN;
                }
                return dLastSuccessfulRun;
            }
            set
            {
                dLastSuccessfulRun = value;
            }
        }

        [DataMember, XmlElement]
        public String LastSuccessfulRunAsString
        {
            get { return dLastSuccessfulRun.ToString("g"); }
            set { dLastSuccessfulRun = DateTime.Parse(value); }
        }
    }
}
