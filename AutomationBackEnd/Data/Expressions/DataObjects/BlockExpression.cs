using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.Expressions.DataObjects
{
    public enum ForEachType
    {
        feRow,
        feColumn
    }
    class BlockExpression : VariableExpression
    {
        public BlockExpression(String xsTag, ExpressionAttributes xoAttributes = new ExpressionAttributes())
            : base(xsTag, xoAttributes)
        {

        }


        public override String OutputCalculatedString(ExecutionJobEnvironment oEnvironment)
        {
            // Store some data here so the children can use it
            this.Data = oEnvironment.PopDataContainer();

            return base.OutputCalculatedString(oEnvironment);
        }
    }
    class ForEachExpression : BlockExpression
    {
        public ForEachExpression(String xsTag, ExpressionAttributes xoAttributes = new ExpressionAttributes())
            : base(xsTag)
        {
            Value = ""; // Blocks do not return a specific value
        }

        public override string OutputCalculatedString(ExecutionJobEnvironment oEnvironment)
        {
            List<String> asResults = new List<string>();

            // 1. DFT (Get all childrens values first)
            if (Attributes["TYPE"].Equals("ROW"))
            {
                // For each row we have available in the DataContainer
                foreach (DataItemContainer asRow in Data.ConvertToTable())
                {
                    // Add the child values for each row
                    asResults.Add(
                        String.Join(
                        Attributes["DELIMITER"],
                        aoChildren.Select(
                            (oChild) =>
                                oChild.OutputCalculatedString(asRow))
                                .ToList()));
                }
            }
            else if (Attributes["TYPE"].Equals("COLUMN"))
            {

            }

            // 2. Calculate this value and prepend it
            return this.Value + String.Join("", asResults);
        }
    }
}
