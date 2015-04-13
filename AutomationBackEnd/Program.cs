using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AutomationService
{
    public class Program
    {
        // Store a reference to the form 
        private static MainForm oForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            oForm = new MainForm();
            Application.Run(oForm);
        }

        public static MainForm GetForm()
        {
            // Could run into Null Exceptions here
            return oForm;
        }
    }
}
