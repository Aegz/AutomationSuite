using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomationService
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            // Initialise the form
            InitializeComponent();
        }

        public void InsertText(string xsMessage)
        {
            // Check if we need to invoke this
            if (InvokeRequired)
            {
                // if so, invoke a new action with this function
                this.Invoke(new Action<string>(InsertText), new object[] { xsMessage });
                return;
            }
            // Append so the transition is a bit smoother
            textboxLog.AppendText(xsMessage + Environment.NewLine);
        }

        public List<String> GetAllText()
        {
            // Return all active lines in the textbox
            return textboxLog.Lines.ToList<String>();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //mynotifyicon.Visible = true;
                this.Hide();
                e.Cancel = true;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            MainFacade oFacade = MainFacade.Instance;
        }
    }
}
