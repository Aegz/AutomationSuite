using AutomationService.Config;
using AutomationService.Data.Actions;
using AutomationService.Data.Frequency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.ImportExport
{
    class ExecutionJobSerializer
    {
        public static ExecutionJob CreateJobFromFolder(String xsFolderPath)
        {
            // Create a XML Serializer that can read in an ExecutionJob
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(ExecutionJob), new Type[] 
                { 
                    typeof(JobFrequency),
                    typeof(JobFrequencyCount),
                    typeof(JobFrequencyDate),

                    typeof(ExecutionJob),
                    typeof(FileAction),
                    typeof(SQLAction),
                }
                );

            // Create a Reader to take in the files data
            System.IO.StreamReader file = new System.IO.StreamReader(Path.Combine(xsFolderPath, Configuration.Instance.GetSetting("ConfigFileName")));

            try
            {
                ExecutionJob oTemp = (ExecutionJob)reader.Deserialize(file);

                // Set the operating path
                oTemp.Details.OperatingPath = xsFolderPath;

                // Store the converted XML file
                return oTemp;
            }
            finally
            {
                // Close the reader
                file.Close();
            }
        }

        public static void ExportJobToFolder(ExecutionJob xoJob, String xsFolder)
        {
            // Create a folder if necessary
            if (!Directory.Exists(xsFolder))
            {
                Directory.CreateDirectory(xsFolder);
            }

            // Create an intermediate path
            String sFullJobFolderPath = xsFolder + Path.DirectorySeparatorChar + xoJob.Details.Name;

            // Create the job folder if necessary
            if (!Directory.Exists(sFullJobFolderPath))
            {
                Directory.CreateDirectory(sFullJobFolderPath);
            }
            // Initialise a Serializer that will change the ExecutionJobs to XML
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(ExecutionJob), new Type[] 
                { 
                    typeof(JobFrequency),
                    typeof(JobFrequencyCount),
                    typeof(JobFrequencyDate),

                    typeof(ExecutionJob),
                    typeof(FileAction),
                    typeof(SQLAction),
                }
                );

            // Initialise a StreamWriter to read the text
            System.IO.StreamWriter ObjectSerializer = new System.IO.StreamWriter(
                sFullJobFolderPath + Path.DirectorySeparatorChar + Configuration.Instance.GetSetting("ConfigFileName"));

            // Serialize the job
            writer.Serialize(ObjectSerializer, xoJob);

            // Close the serializer
            ObjectSerializer.Close();
        }        
    }
}
