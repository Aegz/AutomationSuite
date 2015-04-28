using AutomationService.Data.DynamicDataItem;
using AutomationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.Actions
{
    class SMTPAction : ExecutionAction
    {
        protected String sRecipient;
        protected String sSubject;

        public SMTPAction() : base()
        {

        }

        public SMTPAction(
            String xsName, 
            String xsDesc, 
            String xsMessage,
            String xsSubject,
            String xsRecipients)
            : base(xsName, xsDesc)
        {
            sRecipient = xsRecipients;
            sSubject = xsSubject;
            ParameterString = xsMessage;
        }

        public override StackFrame Execute(ExecutionJobEnvironment xoGiven)
        {
            // Message will always be ParamString (allowing it to pull data from the environment)
            return new StackFrame(
                new ValueItem<Boolean>(
                    SMTPController.Instance.SendEmail(sRecipient, sSubject, ParameterString)));
        }
    }
}
