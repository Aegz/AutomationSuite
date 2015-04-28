using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.Expressions.DataObjects
{
    class VariableUseExpression : ExpressionOrConst
    {
        public VariableUseExpression(String xsTag, ExpressionAttributes xoAttributes = new ExpressionAttributes())
            : base(xsTag, xsTag, xoAttributes)
        {

        }

        public override String OutputCalculatedString(ExecutionJobEnvironment xoEnvironment)
        {
            // Build an array of values that are valid and not empty
            List<String> asCoordinates = new String[] 
                { 
                    Attributes["ROW"], 
                    Attributes["COLUMN"] 
                }.Where(
                    (sValue) => !String.IsNullOrWhiteSpace(sValue)
                    ).ToList();

            // Only do this work if we have a valid string
            if (asCoordinates.Count > 0)
            {
                // 1. Get this objects value first
                Value = xoEnvironment.PopDataContainer().GetItemByCoordinate(asCoordinates.ToArray()).ToString();
            }

            // 2. Calculate this value and prepend it
            return base.OutputCalculatedString(xoEnvironment);
        }

        public override String OutputCalculatedString(CompositeOrValue xoContainer)
        {
            // Build an array of values that are valid and not empty
            List<String> asCoordinates = new String[] 
                { 
                    Attributes["ROW"], 
                    Attributes["COLUMN"] 
                }.Where(
                    (sValue) => !String.IsNullOrWhiteSpace(sValue)
                    ).ToList();

            // Only do this work if we have a valid string
            if (asCoordinates.Count > 0)
            {
                Value = xoContainer.GetItemByCoordinate(asCoordinates.ToArray()).ToString();
            }

            // Then return the value
            return Value;
        }

    }
}
