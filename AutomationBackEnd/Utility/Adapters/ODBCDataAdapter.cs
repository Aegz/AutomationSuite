using AutomationService.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Utility.Adapters
{
    class ODBCDataAdapter : IDBInterface
    {
        private OdbcConnection oConnection;

        public ODBCDataAdapter(String xsConnString)
        {
            oConnection = new OdbcConnection(xsConnString);
            
        }

        // Query the DB and return the result as a DataTable
        public DataTable getQueryAsDataTable(String xsQuery)
        {
            // Initialise a return value
            DataTable oTemp = new DataTable();

            // Try and open a connection and execute the query
            try
            {
                using (OdbcDataAdapter oAdapter = new OdbcDataAdapter(xsQuery, oConnection))
                {
                    oAdapter.Fill(oTemp);
                }
            }
            finally
            {
                oConnection.Close();
            }

            //
            return oTemp;
        }


    }
}
