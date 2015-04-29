using AutomationService.Data.DynamicDataItem;
using AutomationService.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutomationService.Data.Actions
{
    [DataContract]
    public enum DatabaseType
    {
        [EnumMember]
        dbtTeradata,
        [EnumMember]
        dbtNeteeza,
        [EnumMember]
        dbtODBC
    };

    [DataContract]
    [XmlSerializerFormat]
    public class SQLAction: ExecutionAction
    {
        [DataMember, XmlElement]
        public DatabaseType DatabaseType { get; set; }

        public SQLAction() : base()
        {

        }

        public SQLAction(String xsName, String xsDesc, String xsQuery, DatabaseType xeType) 
            : base(xsName, xsDesc)
        {
            ParameterString = xsQuery;
            DatabaseType = xeType;
        }

        public override StackFrame Execute(ExecutionJobEnvironment xoGiven)
        {
            // Add a reference to the environment
            oEnvironment = xoGiven;
            // Call the DB Controller and try to retrieve a datatable
            return new StackFrame(DBController.Instance.GetQueryResultsAsContainer(DatabaseType, ParameterString));
        }
    }
}
