using AutomationService.Data.DynamicDataItem;
using AutomationSuite.Expressions.DataObjects;
using AutomationSuite.Expressions.Lex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutomationService.Data.Actions
{
    [DataContract]
    [KnownType(typeof(FileAction))]
    [KnownType(typeof(SQLAction))]
    [XmlSerializerFormat]
    public abstract class ExecutionAction
    {
        [DataMember, XmlElement]
        public String Name;
        [DataMember, XmlElement]
        public String Description;
        [XmlIgnore]
        protected ExecutionJobEnvironment oEnvironment;

        // Parameter specific
        private String sParamString;
        private Lexer oLexer;

        #region Properties

        [DataMember, XmlAttribute]
        public String ParameterString
        {
            get
            {
                // Only initialise when we need to start working
                InitialiseLexer();     
                // Manipulate the string (add items)
                return GenerateCompleteParameterString();
            }
            set
            {
                //
                sParamString = value;
            }
        }
        #endregion

        public ExecutionAction() : this("Blank", "Blank")
        {

        }

        public ExecutionAction(String xsName, String xsDesc)
        {
            // Set the core object variables
            Name = xsName;
            Description = xsDesc;
        }

        public String GenerateCompleteParameterString()
        {
            // Only try and lex if we have a valid environment
            if (oEnvironment != null)
            {
                ExpressionOrConst oIntermediateVar = oLexer.GenerateExpressionTree();
                sParamString = oIntermediateVar.OutputCalculatedString(oEnvironment);
            }

            // Always default to the param string
            return sParamString;
        }

        public void InitialiseLexer()
        {
            oLexer = new Lexer(sParamString);
        }

        /// <summary>
        /// This is set to virtual to make sure we can just have some actions default to something
        /// </summary>
        /// <param name="xoGiven"></param>
        /// <returns></returns>
        public virtual DataItemContainer Execute(ExecutionJobEnvironment xoGiven)
        {
            // Add a reference to the environment
            oEnvironment = xoGiven;

            // default to true
            return new DataItemContainer(typeof(Boolean), true);
        }
    }

}
