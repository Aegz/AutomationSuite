using AutomationService.Data.DynamicDataItem;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Utility
{
    class FileIOController
    {
        #region Singleton
        private static FileIOController oInstance = null;
        private static object oLock = new object();

        public static FileIOController Instance
        {
            get
            {
                if (oInstance == null)
                {
                    lock(oLock)
                    {
                        if (oInstance == null)
                        {
                            oInstance = new FileIOController();
                        }
                    }         
                }
                return oInstance;
            }
        }
        #endregion


        public CompositeOrValue ReadFile(String xsFilePath, Char xcDelimiter)
        {
            String sExtension = System.IO.Path.GetExtension(xsFilePath);

            switch(sExtension)
            {      
                case ".xml":
                    // ?? TODO: XML has its own interaction
                    return null;
                default:
                    return ReadTxtFileToContainer(xsFilePath, xcDelimiter);
            }
        }

        #region Read Delimited Text

        public CompositeOrValue ReadTxtFileToContainer(String xsFilePath, Char xcDelimiter)
        {
            // Read all possible lines into an array
            String[] asLines = File.ReadAllLines(GetNormalisedPath(xsFilePath));
            // Initialise a return var
            CompositeItem aoReturnContainer;

            // If we have anything to process
            if (asLines.Length > 0)
            {
                // Split it and get the number of columns
                List<String> asColumns = asLines[0].Split(new char[] { xcDelimiter }).ToList();

                int iNumberOfColumns = asColumns.Count;

                if (iNumberOfColumns == 1)
                {
                    // Initialise a String specific Container
                    aoReturnContainer = new CompositeItem();

                    // Add all the strings
                    foreach (String sLoopingVar in asLines)
                    {
                        aoReturnContainer.Add(new ValueItem<String>(sLoopingVar));
                    }
                }
                else
                {
                    // Create a new Container with a Schema
                    aoReturnContainer = new TableItemComposite(asColumns);

                    // Try and populate the data
                    for (int iIndex = 0; iIndex < asLines.Length; iIndex++)
                    {
                        // Intermediate variables
                        String sLine = asLines[iIndex];
                        String[] asCells = asLines[iIndex].Split(new char[] { xcDelimiter });

                        // Initialise a new row
                        RowItemComposite oRow = new RowItemComposite(asColumns);

                        // For each cell
                        for (int iCellIndex = 0; iCellIndex < iNumberOfColumns; iCellIndex++)
                        {
                            oRow[asColumns[iCellIndex]] = new ValueItem<String>(asCells[iCellIndex]);
                        }

                        // Add the row
                        aoReturnContainer.Add(oRow);
                    }
                }

                // Return a valid list
                return aoReturnContainer;
            }

            // Return a list
            return null;
        }

        #endregion

        #region Create/Delete Delimited Text

        public Boolean CreateFile(String xsFilePath)
        {
            try
            {
                String sFilePath = GetNormalisedPath(xsFilePath);
                // Create the file       
                if (File.Exists(sFilePath))
                {
                    File.Create(sFilePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public Boolean DeleteFile(String xsFilePath)
        {
            try
            {
                // Create the file
                File.Delete(GetNormalisedPath(xsFilePath));
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Write Delimited

        public Boolean WriteLinesToFile(String xsFilePath, List<String> xasText)
        {
            try
            {
                // Write the lines
                File.WriteAllLines(GetNormalisedPath(xsFilePath), xasText);
                return true;
            }
            catch
            {
                // IO, Permissions etc.
                return false;
            }
        }

        public Boolean WriteLinesToFile(String xsFilePath, List<IFormattable> xasText)
        {
            try
            {

                // Write the lines
                File.WriteAllLines(GetNormalisedPath(xsFilePath), xasText.Select((s) => s.ToString()));
                return true;
            }
            catch
            {
                // IO, Permissions etc.
                return false;
            }
        }

        #endregion

        public String GetNormalisedPath(String xsFilePath)
        {
            return Path.GetFullPath(new Uri(xsFilePath).LocalPath)
               .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
