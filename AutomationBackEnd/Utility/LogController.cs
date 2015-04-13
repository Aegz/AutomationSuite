using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Utility
{
    class LogController
    {
        public static void LogText(String xsMessage)
        {
            // Format the message accordingly
            String sFormattedMessage = String.Format("[{0}]: {1}", DateTime.Now.ToString("u"), xsMessage);

            // Append the text to the main window
            Program.GetForm().InsertText(sFormattedMessage);
            Console.WriteLine(sFormattedMessage);
        }
    }
}
