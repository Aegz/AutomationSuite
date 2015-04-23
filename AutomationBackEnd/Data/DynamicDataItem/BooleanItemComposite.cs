using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.DynamicDataItem
{
    class BooleanItemComposite : DataItemComposite
    {
        public BooleanItemComposite(object xoGiven)
            : base(xoGiven)
        {

        }

        public override Boolean IsValidOrContainsData()
        {
            // Explicitly change the type
            return (Boolean)Value;     
        }

        public override List<String> ToList()
        {
            if (aoItems.Count == 1)
            {
                // Transform into a string list
                return new List<String> { ((Boolean)aoItems[0]).ToString() };
            }
            else
            {
                return aoItems.Select((xbGiven) => ((Boolean)xbGiven).ToString()).ToList();
            }
        }
    }
}
