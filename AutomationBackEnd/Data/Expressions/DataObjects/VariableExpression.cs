using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationSuite.Expressions.DataObjects
{
    /// <summary>
    /// Holds some data 
    /// </summary>
    class VariableExpression : ExpressionOrConst
    {
        public DataItemContainer Data;

        public VariableExpression(String xsTag) : base(xsTag)
        {

        }

    }

    class BlockExpression : VariableExpression
    {       
        public BlockExpression(String xsTag) : base(xsTag)
        {

        }


        public override String OutputCalculatedString(ExecutionJobEnvironment oEnvironment)
        {
            // Store some data here so the children can use it
            this.Data = oEnvironment.PopDataContainer();

            return base.OutputCalculatedString(oEnvironment);
        }
    }

    public enum ForEachType
    {
        feRow,
        feColumn
    }

    class ForEachExpression : BlockExpression
    {
        protected ForEachType eType;

        public ForEachExpression(String xsTag, ForEachType xeType) : base (xsTag)
        {
            eType = xeType;
            Value = ""; // Blocks do not return a specific value
        }

        public override string OutputCalculatedString(ExecutionJobEnvironment oEnvironment)
        {
            List<String> asResults = new List<string>();

            // 1. DFT (Get all childrens values first)
            if (eType == ForEachType.feRow)
            {
                // For each row we have available in the DataContainer
                foreach (DataItemContainer asRow in Data.ConvertToTable())
                {
                    // Add the child values for each row
                    asResults.Add(
                        String.Join(
                        "", 
                        aoChildren.Select(
                            (oChild) => 
                                oChild.OutputCalculatedString(asRow))
                                .ToList()));
                }
            }

            // 2. Calculate this value and prepend it
            return this.Value + String.Join("", asResults);
        }
    }
}
