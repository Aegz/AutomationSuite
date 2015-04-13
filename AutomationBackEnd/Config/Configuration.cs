using AutomationService.Utility;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomationService.Config
{
    class Configuration
    {
        private readonly String CONFIG_NAME = "Config.sqlite";

        // Set intermediate var once here
        private SQLiteConnection oConn;

        // Store all configurations here dynamically
        private Dictionary<String, String> dsConfiguration = new Dictionary<string, string>
        {
            // Set the conn strings to some default value (will be overridden later)
            {"ODBCConnString", "Driver={MySQL ODBC 5.3 ANSI Driver};Server=localhost;Uid=devtest;Pwd=dev123;Option=3"},
            {"TeradataConnString", "Something Interesting"},
            {"NeteezaConnString", "Something Interesting"},

            {"ConsumerRelaxTime", "5000"}, // in MS

            {"ConfigFileName", "config.xml"}, // The default name for a config file
            {"LockFileName", "this.lock"}, // Purely here for scalability
            {"ExecutionLog", "execution.log"}, // The log that will hold overall information

            {"SessionFolder", "Session {0}"}, // Session folder name
        };

        public Configuration()
        {
            // Initialise once
            oConn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", CONFIG_NAME));

            // Get the working file path
            //dsConfiguration.Add("WorkingDirectory", Directory.GetCurrentDirectory());
            dsConfiguration.Add("WorkingDirectory", @"C:\Temp");
            
            // Temporary string for the Execution Job sub folder
            String sJobFolder = Path.Combine(dsConfiguration["WorkingDirectory"], "ExecutionJobs");

            // Make sure we have all of the folders we need to operate properly
            Directory.CreateDirectory(sJobFolder);

            // 1. Pending (Not added yet)
            dsConfiguration.Add("StagingDirectory", Path.Combine(sJobFolder, "0 Staging"));
            Directory.CreateDirectory(dsConfiguration["StagingDirectory"]);

                // 1a. Approved (Needs to be added to queue?)
                dsConfiguration.Add("ApprovedDirectory", Path.Combine(dsConfiguration["StagingDirectory"], "Approved"));
                Directory.CreateDirectory(dsConfiguration["ApprovedDirectory"]);

                // 1b. Job was declined
                dsConfiguration.Add("DeclinedDirectory", Path.Combine(dsConfiguration["StagingDirectory"], "Declined"));
                Directory.CreateDirectory(dsConfiguration["DeclinedDirectory"]);

            // 1. Job is Queued
            dsConfiguration.Add("PendingJobsDirectory", Path.Combine(sJobFolder, "1 Pending"));
            Directory.CreateDirectory(dsConfiguration["PendingJobsDirectory"]);


            // 2. Job is actively running
            dsConfiguration.Add("ActiveJobsDirectory", Path.Combine(sJobFolder, "2 Active"));
            Directory.CreateDirectory(dsConfiguration["ActiveJobsDirectory"]);

            // 3. Job completed and has no more runs
            dsConfiguration.Add("CompletedDirectory", Path.Combine(sJobFolder, "3 Completed"));
            Directory.CreateDirectory(dsConfiguration["CompletedDirectory"]);



            LogController.LogText("Configuration Initialised");
        }

        public void ImportSettings()
        {
            // Singleton behaviour for Config SQLite3 File
            if (true)//if (!File.Exists(CONFIG_NAME))
            {
                // Create the file if it doesnt exist
                SQLiteConnection.CreateFile(CONFIG_NAME);

                // Initialise the SQLite config file
                InitialiseConfigurationFile();
            }
   
            try
            {
                // Open the connection to the DB first
                oConn.Open();
                        
                // Load all settings from it for DB connection etc.
                SQLiteCommand oSelectAll = new SQLiteCommand("SELECT * FROM General;", oConn);

                // Initialise a reader
                SQLiteDataReader oReader = oSelectAll.ExecuteReader();

                //
                while (oReader.Read())
                {
                    String sSettingName = Convert.ToString(oReader["SettingName"]);
                    String sSettingValue = Convert.ToString(oReader["SettingValue"]);

                    // if it exists
                    if (dsConfiguration.ContainsKey(sSettingName))
                    {
                        // Just update it
                        dsConfiguration[sSettingName] = sSettingValue;
                        LogController.LogText("Configuration: Updated Setting: " + sSettingName  + " - " + sSettingValue);
                    }
                    else
                    {
                        // Insert it
                        dsConfiguration.Add(sSettingName, sSettingValue);
                        LogController.LogText("Configuration: Added Setting: " + sSettingName + " - " + sSettingValue);
                    }
                }
            }
            catch (Exception e)
            {
                LogController.LogText("Configuration: Exception (" + e.Message + ")");
                //throw e;
            }
            finally
            {
                oConn.Close();
            }
        }


        private void InitialiseConfigurationFile()
        {
            try
            {
                // Open the connection so we can start working
                oConn.Open();

                // Create the initial table
                SQLiteCommand oComm = new SQLiteCommand("CREATE TABLE General (SettingName varchar(40), SettingValue varchar(255))", oConn);
                // Create the table
                oComm.ExecuteNonQuery();

                // Insert Default value
                oComm = new SQLiteCommand(@"INSERT INTO General VALUES ('NeteezaConnString', '" + dsConfiguration["NeteezaConnString"] + "')", oConn);
                oComm.ExecuteNonQuery();

                // Insert Default value
                oComm = new SQLiteCommand(@"INSERT INTO General VALUES ('TeradataConnString', '" + dsConfiguration["TeradataConnString"] + "')", oConn);
                oComm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally 
            {
                // Always close the conn after use
                oConn.Close();
            }
            

        }


        // ///////////////////////////////////////////////////////////////
        // Singleton Specific
        // ///////////////////////////////////////////////////////////////
        public String GetSetting(String xsSettingName)
        {
            // Try and return a setting string, else return empty string
            if (dsConfiguration.ContainsKey(xsSettingName))
            {
                return dsConfiguration[xsSettingName];
            }
            else
            {
                throw new Exception("Attempted to access a Setting that doesn't exist (" + xsSettingName + ")");
            }
            //return fdSettings.ContainsKey(xsSettingName) ? fdSettings[xsSettingName] : "";
        }


        public void SetSetting(String xsSetting, String xsNewValue)
        {
            if (dsConfiguration.ContainsKey(xsSetting))
            {
                dsConfiguration[xsSetting] = xsNewValue;
            }
        }
        
        private static Configuration oInstance;

        public static Configuration Instance
        {
            get
            {
                if (oInstance == null)
                {
                    oInstance = new Configuration();
                }

                return oInstance;
            }
        }
    }
}
