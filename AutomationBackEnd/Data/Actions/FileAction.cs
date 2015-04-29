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

        [DataMember, XmlElement]
        public FileActionType FileType { get; set; }

        [DataMember, XmlElement]
        public char Delimiter { get; set; }

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
            FileType = xeType;
            Delimiter = xcDelimiter;
        }

        /// <summary>
        /// Tries to execute this action by firstly determining what type of
        /// action to perform (by the enum type) then tells the FileIOController to 
        /// work.
        /// </summary>
        /// <param name="xoGiven"></param>
        /// <returns></returns>
        public override StackFrame Execute(ExecutionJobEnvironment xoGiven)
        {
            // Add a reference to the environment
            oEnvironment = xoGiven;

            // Switch based on the type of action
            switch (FileType)
            {
                case FileActionType.fatRead:
                    return new StackFrame(FileIOController.Instance.ReadFile(ParameterString, Delimiter));

                case FileActionType.fatWrite:
                    // Look above to see what data we have to play with
                    StackFrame oTemp = xoGiven.PopDataContainer();

                    // If it is null, try popping again
                    if (oTemp.Item == null)
                    {
                        // try again
                        return Execute(xoGiven);
                    }
                    // Try catch block for the conversion
                    try
                    {      
                        // Normalise the parameter string to be safe
                        String sFilePath = ParameterString;
                     
                        // Return something saying it passed
                        return new StackFrame(
                            new ValueItem<Boolean>(
                                FileIOController.Instance.WriteLinesToFile(
                                    sFilePath, 
                                    oTemp.Item.Items.Select(
                                        (oItem) => oItem.ToString()
                                        )
                                    .ToList()
                                )));
                    }
                    catch (Exception e)
                    {
                        //?? ERROR: Invalid operation
                        throw new Exception("Invalid operation:" + e.Message);
                    }

                case FileActionType.fatCreate:
                    return new StackFrame(new ValueItem<Boolean>(
                        FileIOController.Instance.CreateFile(ParameterString)));

                case FileActionType.fatDelete:
                    return new StackFrame(new ValueItem<Boolean>(
                        FileIOController.Instance.DeleteFile(ParameterString)));
                default:
                    break;
            }

            // Return a boolean thats true
            return base.Execute(xoGiven);
        }
    

    }
}
