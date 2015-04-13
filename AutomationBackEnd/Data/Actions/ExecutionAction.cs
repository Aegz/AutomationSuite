using AutomationService.Data.DynamicDataItem;
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
        [XmlIgnore]
        private readonly String PARAMETER_PATTERN = @"\<(?<FIRST>\d+)(\,(?<SECOND>\d+))??\>";

        [DataMember, XmlElement]
        public String Name;
        [DataMember, XmlElement]
        public String Description;
        [XmlIgnore]
        protected ExecutionJobEnvironment oEnvironment;

        // Parameter specific
        private String sParamString;
        private Boolean bNumberOfParametersCalculated = false;
        protected int iNumberOfParameters = 0;
        private List<String> asTextToReplace;
        private List<String[]> asCoordinates;

        #region Properties
        public int NumberOfParameters
        {
            get
            {
                if (!bNumberOfParametersCalculated)
                {
                    CalculateNumberOfParameters(); 
                }
                return iNumberOfParameters;
            }
        }

        [DataMember, XmlAttribute]
        public String ParameterString
        {
            get
            {
                // Manipulate the string (add items)
                return GenerateCompleteParameterString();
            }
            set
            {
                //
                sParamString = value;
                CalculateNumberOfParameters();        
            }
        }
        #endregion

        public ExecutionAction()
        {
            Name = "Blank";
            Description = "Blank";

            asTextToReplace = new List<string>();
            asCoordinates = new List<string[]>();
        }

        public ExecutionAction(String xsName, String xsDesc)
        {
            // Set the core object variables
            Name = xsName;
            Description = xsDesc;

            // Initialise the lists on construction
            asTextToReplace = new List<string>();
            asCoordinates = new List<String[]>();
        }

        public String GenerateCompleteParameterString()
        {
            if (asTextToReplace.Count > 0 && oEnvironment != null)
            {
                // Temp List
                List<String> aoDataToInsert = new List<String>();

                // For the number of parameters we need
                for (int iIndex = 0; iIndex < iNumberOfParameters; iIndex++)
                {
                    // Pop an item
                    DataItemContainer oContainer = oEnvironment.PopDataContainer();

                    // Get its data (normalise data if possible)
                    String sParameter = oContainer.GetItemByCoordinate(asCoordinates[iIndex]);

                    // put it in the list
                    aoDataToInsert.Add(sParameter);
                }

                // Replace all of our custom flags with proper String Format flags
                String sOutputString = sParamString;
                for (int iTextCount = 0; iTextCount < asTextToReplace.Count; iTextCount++)
                {
                    String sReplacementString = asTextToReplace[iTextCount];
                    sOutputString = sOutputString.Replace(sReplacementString, "{" + iTextCount + "}");
                }

                // Start replacing from the param string directly
                return String.Format(sOutputString, aoDataToInsert.ToArray());
            }

            //
            return sParamString;
        }

        public void CalculateNumberOfParameters()
        {          
            // Only do this work if we have a valid string
            if (!String.IsNullOrWhiteSpace(sParamString))
            {
                // Initialise regex
                Regex reRegex = new Regex(PARAMETER_PATTERN);
                MatchCollection amColl = reRegex.Matches(sParamString);

                foreach (Match oMatch in amColl)
                {
                    // Intermediate variable (trim the string)
                    String sMatchingText = oMatch.Value.Trim();

                    // Add to list of things we want to
                    asTextToReplace.Add(sMatchingText); // Need to replace in string

                    // The Coordinates (which will determine what we obtain
                    String sCoordinates = sMatchingText.Substring(1, oMatch.Value.Length - 2);

                    // Declare a variable for use later
                    String[] temp;

                    // If it is a two part coordinate
                    if (sCoordinates.Contains(','))
                    {
                        // Split and return an int array
                        temp = sCoordinates.Split(',');
                    }
                    else
                    {
                        // Just create an array with 1 value
                        temp = new String[] { sCoordinates.Trim() };
                    }

                    // Append to list of things to add
                    asCoordinates.Add(temp);
                }
            }

            // Regex, count numbers and types
            iNumberOfParameters = asTextToReplace.Count;
            bNumberOfParametersCalculated = true;
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
