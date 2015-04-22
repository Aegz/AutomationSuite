using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.Expressions
{
    public struct ExpressionAttributes
    {
        private Dictionary<String, String> dsAttributes;

        public String this[String xsIndex]
        {
            get
            {
                if (dsAttributes == null)
                {
                    dsAttributes = new Dictionary<string, string>();
                }

                if (dsAttributes.ContainsKey(xsIndex))
                {
                    return dsAttributes[xsIndex];
                }
                else
                {
                    // No such attribute
                    return String.Empty;
                }
            }
            set
            {
                if (dsAttributes == null)
                {
                    dsAttributes = new Dictionary<string, string>();
                }

                if (dsAttributes.ContainsKey(xsIndex))
                {
                    dsAttributes[xsIndex] = value;
                }
                else
                {
                    dsAttributes.Add(xsIndex, value);
                }
            }
        }
    
        public ExpressionAttributes(Dictionary<String, String> xdsAttributes)
        {
            dsAttributes = xdsAttributes;
        }
    }
}
