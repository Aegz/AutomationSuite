using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutomationService.Data.DynamicDataItem
{
    [DataContract]
    [XmlSerializerFormat]
    public class JobDetails
    {
        public static readonly String STATUS_ACTIVE = "A";
        public static readonly String STATUS_PENDING = "P";
        public static readonly String STATUS_QUEUED = "Q";
        public static readonly String STATUS_COMPLETE = "C";

        [DataMember, XmlElement]
        public String Name = "";

        [DataMember, XmlElement]
        public String Description = "";

        [DataMember, XmlElement]
        public String OperatingPath = "";       

        [DataMember, XmlElement]
        public String Owner = "";

        [DataMember, XmlElement]
        public String ApprovedBy = "";

        [DataMember, XmlElement]
        public String Status = "";
    }
}
