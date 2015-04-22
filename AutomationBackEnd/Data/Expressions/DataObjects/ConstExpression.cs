using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationSuite.Expressions.DataObjects
{
    class ConstExpression : ExpressionOrConst
    {
        public ConstExpression(String xsValue) : base(xsValue, xsValue)
        {

        }

        public override string OutputCalculatedString(ExecutionJobEnvironment xoEnvironment)
        {
            // Simply return the value
            return Value;
        }

        public override string OutputCalculatedString(DataItemContainer xoContainer)
        {
            // Simply return the value
            return Value;
        }
    }
}
