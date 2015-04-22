using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationSuite.Expressions.DataObjects
{
    class ExpressionOrConst : IEnumerable<ExpressionOrConst>
    {
        protected List<ExpressionOrConst> aoChildren;

        public ExpressionOrConst Parent { get; set; }

        public String Tag { get; set; }

        public String Value
        {
            get;
            set;
        }

        public ExpressionOrConst() : this(String.Empty, String.Empty)
        {

        }

        public ExpressionOrConst(String xsTag) : this(xsTag, String.Empty)
        {

        }

        public ExpressionOrConst(String xsTag, String xsValue)
        {
            // Set the tag and default the value
            Tag = xsTag;
            Value = xsValue;
        }

        protected List<ExpressionOrConst> Children
        {
            get 
            { 
                if (aoChildren == null)
                {
                    aoChildren = new List<ExpressionOrConst>();
                }
                return aoChildren; 
            }
            set { aoChildren = value; }
        }

        public void AddChild(ExpressionOrConst xoChild)
        {
            xoChild.Parent = this;
            Children.Add(xoChild);
        }

        public override String ToString()
        {
            // Recursive call on children to get their string values
            return Tag + (aoChildren != null ? String.Join("", Children.Select((oVar) => oVar.ToString())) : "");
        }

        public virtual String OutputCalculatedString(ExecutionJobEnvironment xoEnvironment)
        {
            // Intermediate var to help with debugging
            String sOutput = "";

            // 1. DFT (Get all childrens values first)
            if (aoChildren != null && aoChildren.Count > 0)
            {
                List<String> asChildrenValues = aoChildren.Select((oChild) => oChild.OutputCalculatedString(xoEnvironment)).ToList();
                sOutput = String.Join("", asChildrenValues);
            }

            // 2. Calculate this value and prepend it
            return Value + sOutput;
        }

        public virtual String OutputCalculatedString(DataItemContainer xoContainer)
        {
            return Value + String.Join("", xoContainer.ToString());
        }

        IEnumerator<ExpressionOrConst> IEnumerable<ExpressionOrConst>.GetEnumerator()
        {
            return aoChildren.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return aoChildren.GetEnumerator();
        }
    }
}
