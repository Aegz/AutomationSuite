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
    public enum FileActionType
    {
        [EnumMember]
        fatRead,
        [EnumMember]
        fatWrite,

        [EnumMember]
        fatCreate,
        [EnumMember]
        fatDelete
    }

    [DataContract]
    [XmlSerializerFormat]
    public class FileAction : ExecutionAction
    {
        
        private FileActionType eType;

        [DataMember, XmlElement]
        public FileActionType FileType
        {
            get { return eType; }
            set { eType = value; }
        }

        private char cDelimiter;

        public FileAction() : base()
        {

        }

        /// <summary>
        /// Primary constructor which takes in a delimiter
        /// </summary>
        /// <param name="xsName"></param>
        /// <param name="xsDesc"></param>
        /// <param name="xeType"></param>
        /// <param name="xsFileName"></param>
        /// <param name="xcDelimiter"></param>
        public FileAction(
            String xsName, 
            String xsDesc, 
            String xsFileName, 
            FileActionType xeType = FileActionType.fatRead, 
            Char xcDelimiter = ' ')
            : base(xsName, xsDesc)
        {
            ParameterString = xsFileName;
            eType = xeType;
            cDelimiter = xcDelimiter;
        }

        /// <summary>
        /// Tries to execute this action by firstly determining what type of
        /// action to perform (by the enum type) then tells the FileIOController to 
        /// work.
        /// </summary>
        /// <param name="xoGiven"></param>
        /// <returns></returns>
        public override DataItemComposite Execute(ExecutionJobEnvironment xoGiven)
        {
            // Add a reference to the environment
            oEnvironment = xoGiven;

            // Switch based on the type of action
            switch (eType)
            {
                case FileActionType.fatRead:
                    return FileIOController.Instance.ReadFile(ParameterString, cDelimiter);

                case FileActionType.fatWrite:
                    // Look above to see what data we have to play with
                    DataItemComposite oTemp = xoGiven.PopDataContainer();

                    // If it is null, try popping again
                    if (oTemp.Value == null)
                    {
                        // try again
                        return Execute(xoGiven);
                    }
                    // Try catch block for the conversion
                    try
                    {      
                        // Initialise a return list
                        List<String> xasTextToAdd;

                        // Convert the container into a string list
                        xasTextToAdd = oTemp.ToList();

                        // Normalise the parameter string to be safe
                        String sFilePath = ParameterString;
                     
                        // Return something saying it passed
                        return new BooleanItemComposite(
                            FileIOController.Instance.WriteLinesToFile(sFilePath, xasTextToAdd));
                    }
                    catch
                    {
                        //?? ERROR: Invalid operation
                        throw new Exception("Invalid operation");
                    }

                case FileActionType.fatCreate:
                    return new BooleanItemComposite(
                        FileIOController.Instance.CreateFile(ParameterString));

                case FileActionType.fatDelete:
                    return new BooleanItemComposite(
                        FileIOController.Instance.DeleteFile(ParameterString));
                default:
                    break;
            }

            // Return a boolean thats true
            return base.Execute(xoGiven);
        }
    

    }
}
