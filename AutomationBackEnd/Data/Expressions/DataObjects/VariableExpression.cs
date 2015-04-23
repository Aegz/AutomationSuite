using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.Expressions.DataObjects
{
    /// <summary>
    /// Holds some data 
    /// </summary>
    class VariableExpression : ExpressionOrConst
    {
        public DataItemComposite Data;

        public VariableExpression(String xsTag, ExpressionAttributes xoAttributes = new ExpressionAttributes())
            : base(xsTag, xoAttributes)
        {

        }
    }

}
