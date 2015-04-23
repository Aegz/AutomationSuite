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
    public class JobSession
    {
        public static readonly String STATUS_PENDING = "Pending";
        public static readonly String STATUS_SUCCESS = "Sucessful";
        public static readonly String STATUS_FAILED = "Failed";
        public static readonly String STATUS_ERROR = "Error";
        
        [XmlIgnore]
        public DateTime ExecutionTime { get; set; }

        [DataMember, XmlElement]
        public String ExecutionTimeAsString
        {
            get { return ExecutionTime.ToString("g"); }
            set { ExecutionTime = DateTime.Parse(value); }
        }

        [DataMember, XmlElement]
        public String Status { get; set; }

        [DataMember, XmlElement]
        public List<String> Log { get; set; }

        public JobSession() : this (DateTime.Now, STATUS_PENDING)
        {

        }

        public JobSession(DateTime xdExecutionTime, String xsStatus) : this(xdExecutionTime, xsStatus, new List<string>())
        {

        }

        public JobSession(DateTime xdExecutionTime, String xsStatus, List<String> xasLog)
        {
            ExecutionTime = xdExecutionTime;
            Status = xsStatus;
            Log = xasLog;
        }
    }
}
