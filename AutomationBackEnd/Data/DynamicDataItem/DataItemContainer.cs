using AutomationService.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.DynamicDataItem
{
    [DataContract(Name="ListOrder")]
    public enum ListOrder
    {
        [EnumMember]
        loASC,
        [EnumMember]
        loDESC,
        [EnumMember]
        loNULL
    }

    [DataContract]
    public struct FormatOptions
    {
        [DataMember]
        public char Delimiter;
        [DataMember]
        public ListOrder Order;
        [DataMember]
        public String StringFormat;

        public FormatOptions(Char xcDelimiter, ListOrder xeOrder, String xsStringFormat)
        {
            Delimiter = xcDelimiter;
            Order = xeOrder;
            StringFormat = xsStringFormat;
        }
    }

    [DataContract]
    [XmlSerializerFormat]
    public class DataItemContainer
    {
        [DataMember]
        // Store type here instead of generics
        Type DataType;

        // Store the schema of the expected 
        [DataMember]
        List<String> asSchema;

        // List of items 
        [DataMember]
        List<object> aoItems;

        #region Properties

        public object Value
        {
            get
            {
                if (aoItems.Count == 1)
                {
                    return aoItems[0];
                }
                else
                {
                    return aoItems;
                }
            }
        }

        public Type Key
        {
            get { return DataType; }
        }

        #endregion

        #region Construction

        public DataItemContainer(Type xtType)
        {
            DataType = xtType;
            // Initialise object variables
            aoItems = new List<object>();
        }

        public DataItemContainer(Type xtType, object xoValue)
        {
            DataType = xtType;
            // Initialise object variables
            aoItems = new List<object>() { xoValue };
        }

        public DataItemContainer(Type xtType, List<String> xasSchema) 
        {
            DataType = xtType;
            // Initialise object variables
            aoItems = new List<object>() {  };
            asSchema = xasSchema;
        }

        #endregion

        #region Get/Add Functions

        public void Add(object xoItem)
        {
            // If the type is correct
            if (xoItem.GetType() == DataType)
            {
                // If our schema needs to be initialised
                if (DataType == typeof(DataRow) && asSchema == null)
                {
                    // Cache the schema
                    this.InitialiseSchema((DataRow)xoItem);
                }

                // Try and add the item
                aoItems.Add(xoItem);
            }
            else
            {
                throw new Exception("DataItemContainer: Type mismatch" + DataType.ToString() + " <- " + xoItem.GetType().ToString());
            }
        }

        public String GetItemByCoordinate(params String[] xasCoordinates)
        {
            // Handle Integer cases (most common)
            if (xasCoordinates.Length == 1)
            {
                // Intermediate var
                int iNumber;

                // if it is a number
                if (int.TryParse(xasCoordinates[0], out iNumber))
                {
                    // Just return the item
                    return ConvertItemToString(aoItems[iNumber]);
                }
                else
                {
                    // Return the correct value corresponding to the flag
                    return InterpretGlobalFlag(xasCoordinates[0]);
                }
            }
            else if (xasCoordinates.Length >= 2)
            {
                // Get the appropriate row
                object oRow = InterpretRowFlag(xasCoordinates[0]);
                // Get the correct column from that row
                return InterpretColumnFlag(xasCoordinates[1], oRow);   
            }
            else // Invalid
            {
                throw new Exception("Invalid coordinates given" + xasCoordinates.ToString());
            }            
        }
        
        #endregion

        #region Flag specific Functions

        public String InterpretGlobalFlag(String xsFlag)
        {
            int iIntegerValue;
            if (!int.TryParse(xsFlag, out iIntegerValue))
            {
                // Make sure the flag is upper case
                xsFlag = xsFlag.ToUpper();

                // Interpret the flags here
                switch (xsFlag)
                {
                    case "LAST":
                        List<String> asTemp = ConvertItemToStringList(aoItems[aoItems.Count - 1]);
                        return asTemp[asTemp.Count - 1];
                    case "FIRST":
                        return ConvertItemToStringList(aoItems[0])[0];
                    case "WORKINGDIR":
                        return Configuration.Instance.GetSetting("WorkingDirectory") + Path.DirectorySeparatorChar;
                    default:
                        return "";
                }
            }
            else
            {
                return ConvertItemToString(aoItems[iIntegerValue]);
            }
        }

        public object InterpretRowFlag(String xsFlag)
        {
            int iIntegerValue;
            if (!int.TryParse(xsFlag, out iIntegerValue))
            {
                // Make sure the flag is upper case
                xsFlag = xsFlag.ToUpper();

                // Interpret the flags here
                switch (xsFlag)
                {
                    case "LAST":
                        return aoItems[aoItems.Count - 1];
                    case "FIRST":
                        return aoItems[0];
                    default:
                        return "";
                }
            }
            else
            {
                if (iIntegerValue >= aoItems.Count)
                {
                    throw new Exception("Index Out of Bounds for Given Parameter");
                }
                else
                {
                    return aoItems[iIntegerValue];
                }
            }
        }

        public String InterpretColumnFlag(String xsFlag, object xoRow)
        {
            int iIntegerValue;
            if (!int.TryParse(xsFlag, out iIntegerValue))
            {
                // Make sure the flag is upper case
                xsFlag = xsFlag.ToUpper();

                // Interpret the flags here
                switch (xsFlag)
                {
                    case "LAST":
                        List<String> asTemp = ConvertItemToStringList(xoRow);
                        return asTemp[asTemp.Count - 1];
                    case "FIRST":
                        return ConvertItemToStringList(xoRow)[0];
                    default:
                        return "";
                }
            }
            else
            {
                // If its a string
                if (DataType == typeof(String))
                {
                    // Just return the item
                    return Convert.ToString(xoRow);
                }
                else
                {
                    return ConvertItemToStringList(xoRow)[iIntegerValue];
                }
            }
        }

        #endregion

        #region DataTable/Row Specific

        private void InitialiseSchema(DataRow xoRow)
        {
            // Initialise the schema once
            asSchema = new List<string>();
            // Add the column names
            foreach (DataColumn oLoopingVar in xoRow.Table.Columns)
            {
                asSchema.Add(oLoopingVar.ColumnName);
            }
        }

        #endregion

        #region Conversion and Normalisation

        public List<DataItemContainer> ConvertToTable(FormatOptions xsOptions = new FormatOptions())
        {
            List<DataItemContainer> asReturnList = aoItems.Select((oItem) => GetItemAsContainer(oItem)).ToList();

            // If we need it in descending order, flip it
            if (xsOptions.Order == ListOrder.loDESC)
            {
                asReturnList.Reverse();
            }

            return asReturnList;
        }

        public List<String> ConvertToStringList(FormatOptions xsOptions = new FormatOptions())
        {
            // Transform the objects list into a string
            List<String> asReturnList = aoItems.Select((oItem) => ConvertItemToString(oItem)).ToList();

            // If we need it in descending order, flip it
            if (xsOptions.Order == ListOrder.loDESC)
            {
                asReturnList.Reverse();
            }

            return asReturnList;
        }

        private List<String> ConvertItemToStringList(object xoGiven)
        {
            // Handle specific types
            if (DataType == typeof(List<String>))
            {
                // Explicitly change the type
                return (List<String>)xoGiven;
            }
            else if (DataType == typeof(DataRow))
            {
                // Explicitly change the type
                DataRow oTemp = (DataRow)xoGiven;
                List<String> asReturnList = new List<string>();

                // Add every cell into the list
                for (int iIndex = 0; iIndex < asSchema.Count; iIndex++)
                {
                    String sColumn = asSchema[iIndex];
                    asReturnList.Add(Convert.ToString(oTemp[sColumn]));
                }

                return asReturnList;
            }
            else if (DataType == typeof(String))
            {
                // Intermediate var and typecast
                String sTemp = (String)xoGiven;
                // Transform into a string list
                return sTemp.Select((xcGiven) => Convert.ToString(xcGiven)).ToList();
            }
            else
            {
                // Convert the object to a string then try and parse it
                return ConvertItemToStringList(Convert.ToString(xoGiven));
            }

        }

        private String ConvertItemToString(object xoGiven, FormatOptions xsOptions = new FormatOptions())
        {
            // Initialise a single string to hold the line
            String sLine = "";
            Char cDelimiter = xsOptions.Delimiter;

            // If there was no format option
            if (String.IsNullOrEmpty(xsOptions.StringFormat))
            {

                // Handle specific types
                if (DataType == typeof(List<String>) || 
                    DataType == typeof(DataRow))
                {
                    // Just convert the item to a list and join by the delimiter
                    sLine = String.Join(Convert.ToString(cDelimiter), ConvertItemToStringList(xoGiven));
                }
                else
                {
                    // Try the generic convert
                    return Convert.ToString(xoGiven);
                }
            }
            else
            {
                // ?? TODO: Fix this
                return String.Format(xsOptions.StringFormat, xoGiven);
            }

            // Return a converted string
            return sLine;
        }

        public DataItemContainer GetItemAsContainer(int xiIndex)
        {
            return new DataItemContainer(Key, aoItems[xiIndex]);
        }

        public DataItemContainer GetItemAsContainer(object xoGiven)
        {
            return new DataItemContainer(Key, xoGiven);
        }

        public override string ToString()
        {
            return String.Join("", ConvertToStringList());
        }


        #endregion
    }
}
