using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using CommunicatorAPI;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace OCHistoryOutlookAddIn
{
    public partial class ThisAddIn
    {
        private static Messenger _messenger;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            string Server = "OCSSRV\\RTC";
            string Username = "OCSlog";
            string Password = "tajn3haslo";
            string Database = "LcsLog";

            string ConnectionString = "Data Source=" + Server + ";";
            ConnectionString += "User ID=" + Username + ";";
            ConnectionString += "Password=" + Password + ";";
            ConnectionString += "Initial Catalog=" + Database;

            #region Try to establish a connection to the database

            SqlConnection SQLConnection = new SqlConnection();

            try
            {
                SQLConnection.ConnectionString = ConnectionString;
                SQLConnection.Open();

                // You can get the server version 
                // SQLConnection.ServerVersion
            }
            catch (Exception Ex)
            {
                // Try to close the connection
                if (SQLConnection != null)
                    SQLConnection.Dispose();

                // Create a (useful) error message
                string ErrorMessage = "A error occurred while trying to connect to the server.";
                ErrorMessage += Environment.NewLine;
                ErrorMessage += Environment.NewLine;
                ErrorMessage += Ex.Message;

                // Show error message (this = the parent Form object)
                MessageBox.Show(ErrorMessage, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Stop here
                return;
            }

            #endregion

            #region Execute a SQL query

            string myId = "jerzy.suzanowicz@comarch.com";
            string userId = "jerzy.chalupski@comarch.com";

            string SQLStatement = "SELECT	Conferences.SessionIdTime, 1 AS Conferenece FROM	Conferences INNER JOIN                 ConferenceMessages AS cm ON cm.SessionIdTime = Conferences.SessionIdTime INNER JOIN                 ConferenceMessageRecipientList AS cmr ON cmr.SessionIdTime = Conferences.SessionIdTime INNER JOIN         Users AS u1 ON cm.FromId = u1.UserId INNER JOIN         Users AS u2 ON cmr.UserId = u2.UserId WHERE	( (u1.UserUri = '%myId%') AND (u2.UserUri = '%userId%') OR         (u1.UserUri = '%userId%') AND (u2.UserUri = '%myId%')) UNION SELECT	Messages.SessionIdTime, 0 FROM	Messages INNER JOIN                 Users AS u1 ON Messages.FromId = u1.UserId INNER JOIN                 Users AS u2 ON Messages.ToId = u2.UserId WHERE ( (u1.UserUri = '%myId%') AND (u2.UserUri = '%userId%') OR         (u1.UserUri = '%userId%') AND (u2.UserUri = '%myId%')) AND Messages.Toast = 1";

            // Create a SqlDataAdapter to get the results as DataTable
            SqlDataAdapter SQLDataAdapter = new SqlDataAdapter(SQLStatement, SQLConnection);

            // Create a new DataTable
            DataTable dtResult = new DataTable();

            // Fill the DataTable with the result of the SQL statement
            SQLDataAdapter.Fill(dtResult);

            // Loop through all entries
            foreach (DataRow drRow in dtResult.Rows)
            {
                // Show a message box with the content of 
                // the "Name" column
                MessageBox.Show(drRow["SessionIdTime"].ToString());
            }

            // We don't need the data adapter any more
            SQLDataAdapter.Dispose();

            #endregion


            #region Close the database link

            SQLConnection.Close();
            SQLConnection.Dispose();

            #endregion

            //_messenger = new CommunicatorAPI.Messenger();
            //_messenger.OnIMWindowCreated += new DMessengerEvents_OnIMWindowCreatedEventHandler(_messenger_OnIMWindowCreated);


        }
        /*
        static void _messenger_OnIMWindowCreated(object pIMWindow)
        {
            string serviceId = "";
            IMessengerServices serviceCollection = (IMessengerServices)_messenger.Services;
            if (serviceCollection.Count > 0 && serviceCollection != null)
            {
                IMessengerService primaryService = (IMessengerService)serviceCollection.PrimaryService;
                serviceId = primaryService.ServiceId;

            }


            string msg = "Convo started" + serviceId.Trim();
            System.Windows.Forms.MessageBox.Show(msg);

        }*/

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
