using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.Actions
{
    public enum LogDestination
    {
        ldConsole,
        ldFile,
        ldSQL
    };


    class WriteToLogAction : ExecutionAction
    {
        LogDestination eDestination;
            
        public WriteToLogAction (String xsName, String xsDesc, LogDestination xeDestination) 
            : base(xsName, xsDesc)
        {
            eDestination = xeDestination;
        }

        public override DataItemComposite Execute(ExecutionJobEnvironment xoGiven)
        {
            // Add a reference to the environment
            oEnvironment = xoGiven;

            //
            try
            {
                // Try and retrieve the item we will log
                DataItemComposite oTemp = xoGiven.PeekDataContainer();

                // If there is something to write
                if (oTemp.Value != null)
                {
                    // Convert and store the data
                    List<String> asTextToLog = oTemp.ToList();

                    // Switch on where this needs to be logged
                    switch (eDestination)
                    {
                        case LogDestination.ldConsole:
                            Console.WriteLine("WriteToLog: ");
                            foreach (String sLoopingVar in asTextToLog)
                            {
                                Console.WriteLine(sLoopingVar);
                            }
                            break;
                        case LogDestination.ldFile:
                            // File Path?
                            break;
                        case LogDestination.ldSQL:
                            // SQL DB and table?
                            break;
                    }
                }

                // 
                return new BooleanItemComposite(true);
            }
            catch
            {
                return new BooleanItemComposite(false);
            }
        }

    }
}
