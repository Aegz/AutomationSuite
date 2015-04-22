using AutomationService.Data;
using AutomationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.WCF
{
    class WCFController
    {
        private ServiceHost m_svcHost = null;

        public WCFController()
        {
            LogController.LogText("WCF Controller: Initialising");

            // Close an existing service host if necessary
            if (m_svcHost != null) m_svcHost.Close();

            // Intermediate variables for endpoints
            string strAdrHTTP = "http://localhost:9001/AutomationService";

            // Declare an endpoint
            Uri[] adrbase = { 
                                new Uri(strAdrHTTP), 
                            };

            // Initialise the new Service host on the endpoints
            m_svcHost = new ServiceHost(typeof(WCFFacade), adrbase);

            // Create a behaviour object and add it to the description of the Svchost
            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior
            {
                HttpGetEnabled = true,
                MetadataExporter =
                {
                    // see: www.w3.org/TR/ws-policy/
                    PolicyVersion = PolicyVersion.Policy15
                }
            };

            m_svcHost.Description.Behaviors.Add(mBehave);

            // Create a HTTP binding and add the binding to the WCF interface
            BasicHttpBinding httpb = new BasicHttpBinding();
            m_svcHost.AddServiceEndpoint(typeof(IWCFInterface), httpb, strAdrHTTP);
            m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

        }

        public void InitialiseWCF()
        {
            // Open the service host
            try
            {
                m_svcHost.Open();
            }
            catch (System.ServiceModel.AddressAccessDeniedException)
            {
                LogController.LogText("WCF Controller: Failed due to inadequate permissions. Run again as administrator.");
                //throw new Exception("Could not get the adequate permissions. Please run as administrator");
            }
            
        }
    }
}
