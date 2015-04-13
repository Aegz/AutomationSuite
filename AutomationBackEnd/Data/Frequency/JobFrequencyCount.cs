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
    public class JobFrequencyCount : JobFrequency
    {
        [XmlIgnore]
        int iNumberOfRuns;
        [XmlIgnore]
        int iRunCount;

        public JobFrequencyCount() : this(1) { }

        public JobFrequencyCount(int xiCount) : this(FrequencySetting.fsAdhoc, DateTime.Now, xiCount) 
        { 

        }

        public JobFrequencyCount(FrequencySetting xeType, DateTime xdStartDateTime)
            : this(xeType, xdStartDateTime, 1)
        {

        }

        public JobFrequencyCount(FrequencySetting xeType, DateTime xdStartDateTime, int xiMaximumNumberOfRuns)
            : base(xeType, xdStartDateTime)
        {
            iNumberOfRuns = 0;
            iRunCount = xiMaximumNumberOfRuns;
        }
    
        public override Boolean CanBeExecuted()
        {
            // Has not reached the maximum number of runs
            return iNumberOfRuns < iRunCount;
        }

        public override bool CanBeRemoved()
        {
            // Has gone over the maximum number of runs
            return iNumberOfRuns >= iRunCount;
        }

        [DataMember, XmlElement]
        public int MaximumRunCount
        {
            get { return iRunCount; }
            set { iRunCount = value; }
        }
        
        public override void RecordSuccessfulExecution()
        {
            iNumberOfRuns += 1;
            base.RecordSuccessfulExecution();
        }
    }
}
