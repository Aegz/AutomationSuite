using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutomationSuiteFrontEnd.Utilities.Data
{
    public enum ControlType
    {
        ftDropdown,
        
        ftTextInput,
        ftNumberInput,

        ftCheckbox,
        ftRadioBox,

        ftDateControl
    }

    public class ExpandableRow
    {
            
        public String Name { get; set; }

        public String Value { get; set; }  

        public Type InputType { get; set; }

        public ExpandableRow (String xsName, String xsValue, Type xeType)
        {
            Name = xsName;
            Value = xsValue;
            InputType = xeType;
        }



        public ControlType ConvertTypeToControlType (Type xeType)
        {
            if (xeType == typeof(DateTime))
            {
                return ControlType.ftDateControl;
            }
            else if (xeType == typeof(int))
            {
                return ControlType.ftNumberInput;
            }
            else if (xeType == typeof(Boolean))
            {
                return ControlType.ftCheckbox;
            }
            else
            {
                return ControlType.ftTextInput;
            }
        }
    }
}