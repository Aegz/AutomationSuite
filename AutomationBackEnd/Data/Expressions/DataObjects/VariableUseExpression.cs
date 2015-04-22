using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationSuite.Expressions.DataObjects
{
    class VariableUseExpression : ExpressionOrConst
    {
        String[] asCoordinates;

        public VariableUseExpression(String xsTag) : base (xsTag, xsTag)
        {
            asCoordinates = xsTag.Split(new char[] {','});
        }

        public override String OutputCalculatedString(ExecutionJobEnvironment xoEnvironment)
        {
            // Only do this work if we have a valid string
            if (asCoordinates.Length > 0)
            {
                // 1. Get this objects value first
                Value = xoEnvironment.PopDataContainer().GetItemByCoordinate(asCoordinates);
            }

            // 2. Calculate this value and prepend it
            return base.OutputCalculatedString(xoEnvironment);
        }

        public override String OutputCalculatedString(DataItemContainer xoContainer)
        {
            // Only do this work if we have a valid string
            if (asCoordinates.Length > 0)
            {
                Value = xoContainer.GetItemByCoordinate(asCoordinates);
            }

            // Then return the value
            return Value;
        }

    }
}
