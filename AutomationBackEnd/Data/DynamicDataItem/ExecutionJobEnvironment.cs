using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.DynamicDataItem
{
    public delegate void LogMessageFn(String xsMessage);

    [DataContract]
    public class ExecutionJobEnvironment
    {
        // Visible for debugging
        public Stack<DataItemComposite> aoDataStack;

        // Debugging
        public List<DataItemComposite> aoDebuggingStack;

        // Logging messages
        public LogMessageFn oLogMessage;

        public ExecutionJobEnvironment(LogMessageFn xoLoggingFunction)
        {
            oLogMessage = xoLoggingFunction;
            aoDataStack = new Stack<DataItemComposite>();
            aoDebuggingStack = new List<DataItemComposite>();
        }

        public void PushDataContainer(DataItemComposite xoContainer)
        {
            aoDataStack.Push(xoContainer);
            aoDebuggingStack.Add(xoContainer);
        }

        public DataItemComposite PeekDataContainer()
        {
            return aoDataStack.Peek();
        }

        public DataItemComposite PopDataContainer()
        {
            return aoDataStack.Pop();
        }

    }
}
