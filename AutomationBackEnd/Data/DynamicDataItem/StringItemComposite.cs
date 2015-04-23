using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.DynamicDataItem
{
    class StringItemComposite : DataItemComposite
    {
        public StringItemComposite() : base ()
        {

        }

        public StringItemComposite(object xoGiven) : base(xoGiven)
        {

        }

        public override Boolean IsValidOrContainsData()
        {
            if (aoItems.Count == 1)
            {
                String sTemp = (String)aoItems[0];
                // Transform into a string list
                return sTemp.Length > 0;
            }
            else
            {
                return aoItems.Count > 0;
            }
        }

        public override List<String> ToList()
        {
            if (aoItems.Count == 1)
            {
                // Intermediate var and typecast
                String sTemp = (String)aoItems[0];
                // Transform into a string list
                return sTemp.Select((xcGiven) => Convert.ToString(xcGiven)).ToList();
            }
            else
            {
                return aoItems.Select((xsGiven) => Convert.ToString(xsGiven)).ToList();
            }
        }

        public override string ToString()
        {
            if (aoItems.Count == 1)
            {
                // Transform into a string
                return (String)aoItems[0];
            }
            else
            {
                return String.Join("", aoItems.Select((xsGiven) => xsGiven.ToString()).ToList());
            }
        }
    }
}
