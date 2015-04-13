using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.DynamicDataItem
{
    [DataContract]
    public class ExecutionJobEnvironment
    {
        // Visible for debugging
        public Stack<DataItemContainer> aoDataStack;

        // Debugging
        public List<DataItemContainer> aoDebuggingStack;

        public ExecutionJobEnvironment()
        {
            aoDataStack = new Stack<DataItemContainer>();
            aoDebuggingStack = new List<DataItemContainer>();
        }

        public void PushDataContainer(DataItemContainer xoContainer)
        {
            aoDataStack.Push(xoContainer);
            aoDebuggingStack.Add(xoContainer);
        }

        public DataItemContainer PeekDataContainer()
        {
            return aoDataStack.Peek();
        }

        public DataItemContainer PopDataContainer()
        {
            return aoDataStack.Pop();
        }
    }
}
