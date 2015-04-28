using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.Expressions.DataObjects
{
    class ConstExpression : ExpressionOrConst
    {
        public ConstExpression(String xsValue, ExpressionAttributes xoAttributes = new ExpressionAttributes())
            : base(xsValue, xsValue, xoAttributes)
        {

        }

        public override string OutputCalculatedString(ExecutionJobEnvironment xoEnvironment)
        {
            // Simply return the value
            return Value;
        }

        public override string OutputCalculatedString(CompositeOrValue xoContainer)
        {
            // Simply return the value
            return Value;
        }
    }
}
