using AutomationSuite.Expressions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationSuite.Expressions.Lex
{
    class LexingContext
    {
        public ExpressionOrConst RootNode;
        public ExpressionOrConst CurrentNode; // Used to maintain location

        public Queue<char> CurrentExpression = new Queue<char>();

        public LexingContext()
        {
            // Dummy Node
            CurrentNode = new ExpressionOrConst();
            RootNode = CurrentNode;
        }

    }
}
