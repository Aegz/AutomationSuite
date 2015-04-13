﻿using AutomationService.Config;
using AutomationService.Data.Actions;
using AutomationService.Data.DynamicDataItem;
using AutomationService.Utility.Adapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Utility
{
    class DBController
    {
        #region Singleton
        private static DBController oInstance = null;
        private static object oLock = new object();

        public static DBController Instance
        {
            get
            {
                // Double locking (for multi threaded singleton)
                if (oInstance == null)
                {
                    lock (oLock)
                    {
                        if (oInstance == null)
                        {
                            oInstance = new DBController();
                        }
                    }                  
                }
                return oInstance;
            }
        }
        #endregion

        Dictionary<DatabaseType, IDBInterface> aoDBAdapters = new Dictionary<DatabaseType, IDBInterface>();

        public DBController()
        {
            // Add the neteeza adapter
            //aoDBAdapters.Add(DatabaseType.dbtNeteeza, new ODBCDataAdapter(Configuration.Instance().GetSetting("NeteezaConnString")));
        }
        
        public DataItemContainer GetQueryResultsAsContainer (DatabaseType xeType, String xsQuery)
        {
            // Adapter for the database type is not initialised
            if (!aoDBAdapters.ContainsKey(xeType))
            {
                // Lock the dictionary so we can add the Adapter
                lock (aoDBAdapters)
                {
                    // Double lock check
                    if (!aoDBAdapters.ContainsKey(xeType))
                    {
                        // Initialise an adapter when needed
                        switch (xeType)
                        {
                            case DatabaseType.dbtNeteeza:
                                break;
                            case DatabaseType.dbtODBC:
                                aoDBAdapters.Add(xeType, new ODBCDataAdapter(
                                    Configuration.Instance.GetSetting("ODBCConnString")));
                                break;
                            case DatabaseType.dbtTeradata:
                                break;
                        }
                    }
                }
            }

            // If there is already a cached database
            if (aoDBAdapters.ContainsKey(xeType))
            {
                // Lock the adapter we need
                lock (aoDBAdapters[xeType])
                {
                    if (aoDBAdapters.ContainsKey(xeType))
                    {
                        // Query the database
                        DataTable oTable = aoDBAdapters[xeType].getQueryAsDataTable(xsQuery);

                        // Initialise a return var
                        DataItemContainer oReturnContainer = new DataItemContainer(typeof(DataRow));

                        // If there are rows to process
                        if (oTable.Rows.Count > 0)
                        {
                            // For each row that isnt the schema
                            for (int iRowIndex = 0; iRowIndex < oTable.Rows.Count; iRowIndex++)
                            {
                                // Add a single data item
                                oReturnContainer.Add(oTable.Rows[iRowIndex]);
                            }
                        }
                        // Return the container
                        return oReturnContainer;
                    }
                }
            }

            // Fail.
            return null;
        }

    }
}
