using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;

namespace OCHistory
{
    public partial class Form1 : Form
    {
        private static SqlConnection SQLConnection;
        private static string myId = "";
        private string userId = "";
        public Form1()
        {

            //sip:jerzy.suzanowicz@comarch.com <Contacts=sip:michal.czerwiec@comarch.com>
            string[] startupParameters = Environment.GetCommandLineArgs();
            if (startupParameters.Length == 3)
            {
                myId = startupParameters[1];
                myId = myId.Remove(0, 4);
                userId = startupParameters[2];
                userId = userId.Remove(0, 14);
                userId = userId.Remove(userId.IndexOf(">"));
            }

            InitializeComponent();
            FetchSessions();
            /*
            //this.conversationWebBrowser.DocumentText = myId + "" + userId;
            foreach (String param in startupParameters)
            {
                userId += param;
            }
            this.conversationWebBrowser.DocumentText = userId;*/
        }
        private void ConnectToDB()
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

            SQLConnection = new SqlConnection();

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

        }

        private void CloseConnectionToDB()
        {
            #region Close the database link

            SQLConnection.Close();
            SQLConnection.Dispose();

            #endregion
        }

        private void FetchSessions()
        {
            ConnectToDB();

            #region Execute a SQL query

            string SQLStatement = "SELECT	Conferences.SessionIdTime, 1 AS Conferenece FROM	Conferences INNER JOIN                 ConferenceMessages AS cm ON cm.SessionIdTime = Conferences.SessionIdTime INNER JOIN                 ConferenceMessageRecipientList AS cmr ON cmr.SessionIdTime = Conferences.SessionIdTime INNER JOIN         Users AS u1 ON cm.FromId = u1.UserId INNER JOIN         Users AS u2 ON cmr.UserId = u2.UserId WHERE	( (u1.UserUri = '" + myId + "') AND (u2.UserUri = '" + userId + "') OR         (u1.UserUri = '" + userId + "') AND (u2.UserUri = '" + myId + "')) UNION SELECT	Messages.SessionIdTime, 0 FROM	Messages INNER JOIN                 Users AS u1 ON Messages.FromId = u1.UserId INNER JOIN                 Users AS u2 ON Messages.ToId = u2.UserId WHERE ( (u1.UserUri = '" + myId + "') AND (u2.UserUri = '" + userId + "') OR         (u1.UserUri = '" + userId + "') AND (u2.UserUri = '" + myId + "')) AND Messages.Toast = 1";

            // Create a SqlDataAdapter to get the results as DataTable
            SqlDataAdapter SQLDataAdapter = new SqlDataAdapter(SQLStatement, SQLConnection);

            // Create a new DataTable
            DataTable dtResult = new DataTable();

            // Fill the DataTable with the result of the SQL statement
            SQLDataAdapter.Fill(dtResult);

            this.sessionTreeView.BeginUpdate();

            List<String> activeSessions = new List<string>();

            // Loop through all entries
            foreach (DataRow drRow in dtResult.Rows)
            {
                // Show a message box with the content of 
                // the "Name" column
                //System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode(drRow["SessionIdTime"].ToString());



                if (this.sessionTreeView.Nodes.ContainsKey(((DateTime)(drRow["SessionIdTime"])).Date.ToString()))
                {

                    TreeNode dayNode = this.sessionTreeView.Nodes[((DateTime)(drRow["SessionIdTime"])).Date.ToString()];
                    dayNode.Text = String.Format("{0:dd/MM/yyyy}", (DateTime)(drRow["SessionIdTime"]));
                    TreeNode childNode = new TreeNode(((DateTime)(drRow["SessionIdTime"])).ToString());
                    childNode.Tag = String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", (DateTime)(drRow["SessionIdTime"]));
                    dayNode.Nodes.Add(childNode);
                }
                else
                {
                    this.sessionTreeView.Nodes.Add(((DateTime)(drRow["SessionIdTime"])).Date.ToString(), ((DateTime)(drRow["SessionIdTime"])).ToString());
                    if (this.sessionTreeView.Nodes.ContainsKey(((DateTime)(drRow["SessionIdTime"])).Date.ToString()))
                    {

                        TreeNode dayNode = this.sessionTreeView.Nodes[((DateTime)(drRow["SessionIdTime"])).Date.ToString()];
                        dayNode.Text = String.Format("{0:dd/MM/yyyy}", (DateTime)(drRow["SessionIdTime"]));
                        TreeNode childNode = new TreeNode(((DateTime)(drRow["SessionIdTime"])).ToString());
                        childNode.Tag = String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", (DateTime)(drRow["SessionIdTime"]));
                        dayNode.Nodes.Add(childNode);
                    }
                }

                activeSessions.Add(String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", (DateTime)(drRow["SessionIdTime"])));
            }

            this.sessionTreeView.EndUpdate();

            // We don't need the data adapter any more
            SQLDataAdapter.Dispose();

            #endregion

            CloseConnectionToDB();

        }

        private void FetchConvo(List<String> sessId)
        {
            ConnectToDB();
            #region Execute a SQL query

            String convoHtml = "<html><body style=\"background-color: #F5F5F5;\">";
            foreach (string session in sessId)
            {

                string chatsSQLStatement = "SELECT [MessageIdTime], [SessionIdTime], u1.UserUri as [from], [ContentTypeId], [Body], [Toast] FROM [LcsLog].[dbo].[Messages] INNER JOIN  Users AS u1 ON Messages.FromId = u1.UserId WHERE [SessionIdTime] = '" + session + "' AND Toast = '0' ";
                string convSQLStatement = "SELECT [MessageId], [SessionIdTime], [Date] as [MessageIdTime], u1.UserUri as [from], [ContentTypeId], [Body] FROM [LcsLog].[dbo].[ConferenceMessages] INNER JOIN  Users AS u1 ON ConferenceMessages.FromId = u1.UserId WHERE SessionIdTime = '" + session + "' ";

                SqlDataAdapter SQLDataAdapter = new SqlDataAdapter(chatsSQLStatement, SQLConnection);
                DataTable dtResult = new DataTable();
                SQLDataAdapter.Fill(dtResult);
                if (dtResult.Rows.Count > 0)
                {
                    convoHtml += makeHtml(dtResult);
                }

                SQLDataAdapter = new SqlDataAdapter(convSQLStatement, SQLConnection);
                DataTable convoResult = new DataTable();
                SQLDataAdapter.Fill(convoResult);
                convoHtml += makeHtml(convoResult);
            }
            convoHtml += "</body></html>";
            this.conversationWebBrowser.DocumentText = convoHtml;
            #endregion            
            CloseConnectionToDB();

        }

        private String makeHtml(DataTable dtResult)
        {
            String convoHtml = "";
            DateTime convoTime = new DateTime(2010, 1, 1, 0, 0, 1);
            bool addHeader = true;
            String from = "";
            foreach (DataRow drRow in dtResult.Rows)
            {
                DateTime tempConvTime = (DateTime)(drRow["MessageIdTime"]);
                int tempTime = ((DateTime)(drRow["MessageIdTime"])).CompareTo(convoTime);
                if (from == String.Empty || from != drRow["from"].ToString() || ((DateTime)(drRow["MessageIdTime"])).CompareTo(convoTime) == 1)
                {

                    addHeader = true;
                }
                else
                {
                    addHeader = false;
                }


                if (drRow["from"].ToString() == myId)
                {
                    if (addHeader)
                    {
                        convoHtml += "<DIV style=\"POSITION: relative; PADDING-BOTTOM: 0px; PADDING-LEFT: 3px; PADDING-RIGHT: 3px; CLEAR: both; FONT-SIZE: 10pt; PADDING-TOP: 0px\" id=Normalheader><SPAN style=\"BORDER-BOTTOM-COLOR: #666666; PADDING-BOTTOM: 0px; BORDER-TOP-COLOR: #666666; PADDING-LEFT: 0px; PADDING-RIGHT: 0px; COLOR: #666666; BORDER-RIGHT-COLOR: #666666; FONT-SIZE: 8pt; BORDER-LEFT-COLOR: #666666; PADDING-TOP: 2px\" id=imsendtimestamp>" + String.Format("{0:HH:mm }", (DateTime)drRow["MessageIdTime"]) + "</SPAN><SPAN style=\"BORDER-BOTTOM-COLOR: #666666; BORDER-TOP-COLOR: #666666; COLOR: #666666; BORDER-RIGHT-COLOR: #666666; BORDER-LEFT-COLOR: #666666\" id=imsendname>" + drRow["from"].ToString() + "</SPAN><SPAN style=\"CLEAR: both\"></SPAN></DIV>" +
                            "<DIV style=\"POSITION: relative; PADDING-BOTTOM: 0px; PADDING-LEFT: 3px; PADDING-RIGHT: 3px; CLEAR: both; PADDING-TOP: 0px\" id=Normalcontent>" +
                            "<DIV style=\"MARGIN-LEFT: 5px\" id=imwidget></DIV>" +
                            "<DIV style=\"MARGIN-LEFT: 12px\" id=imcontent><SPAN>" +
                            drRow["Body"].ToString() + "</SPAN></DIV></DIV></DIV>";
                    }
                    else
                    {
                        convoHtml += "<DIV>" +
                            "<DIV " +
                            "style=\"POSITION: relative; PADDING-BOTTOM: 0px; PADDING-LEFT: 3px; PADDING-RIGHT: 3px; CLEAR: both; PADDING-TOP: 0px\" " +
                            "id=Normalcontent>" +
                            "<DIV style=\"MARGIN-LEFT: 5px\" id=imwidget></DIV>" +
                            "<DIV style=\"MARGIN-LEFT: 12px\" id=imcontent><SPAN>" +
                            drRow["Body"].ToString() + "</SPAN></DIV></DIV></DIV>";
                    }
                    from = drRow["from"].ToString();

                }
                else
                {
                    if (addHeader)
                    {
                        convoHtml += "<DIV>" +
                            "<DIV " +
                            "style=\"POSITION: relative; PADDING-BOTTOM: 0px; BACKGROUND-COLOR: #ffffff; PADDING-LEFT: 3px; PADDING-RIGHT: 3px; CLEAR: both; FONT-SIZE: 10pt; PADDING-TOP: 0px\" " +
                            "id=ColorBandedheader><SPAN " +
                            "style=\"BORDER-BOTTOM-COLOR: #666666; PADDING-BOTTOM: 0px; BORDER-TOP-COLOR: #666666; PADDING-LEFT: 0px; PADDING-RIGHT: 0px; COLOR: #666666; BORDER-RIGHT-COLOR: #666666; FONT-SIZE: 8pt; BORDER-LEFT-COLOR: #666666; PADDING-TOP: 2px\" " +
                            "id=imsendtimestamp>" + String.Format("{0:HH:mm }", (DateTime)drRow["MessageIdTime"]) + "</SPAN><SPAN " +
                            "style=\"BORDER-BOTTOM-COLOR: #666666; BORDER-TOP-COLOR: #666666; COLOR: #666666; BORDER-RIGHT-COLOR: #666666; BORDER-LEFT-COLOR: #666666\" " +
                            "id=imsendname>" + drRow["from"].ToString() + "</SPAN><SPAN " +
                            "style=\"CLEAR: both\"></SPAN></DIV>" +
                            "<DIV " +
                            "style=\"POSITION: relative; PADDING-BOTTOM: 0px; BACKGROUND-COLOR: #ffffff; PADDING-LEFT: 3px; PADDING-RIGHT: 3px; CLEAR: both; PADDING-TOP: 0px\" " +
                            "id=ColorBandedcontent>" +
                            "<DIV style=\"MARGIN-LEFT: 5px\" id=imwidget></DIV>" +
                            "<DIV style=\"MARGIN-LEFT: 12px\" id=imcontent><SPAN>" +
                            drRow["Body"].ToString() + "</SPAN></DIV></DIV></DIV>";
                    }
                    else
                    {
                        convoHtml += "<DIV>" +
                            "<DIV " +
                            "style=\"POSITION: relative; PADDING-BOTTOM: 0px; BACKGROUND-COLOR: #ffffff; PADDING-LEFT: 3px; PADDING-RIGHT: 3px; CLEAR: both; PADDING-TOP: 0px\" " +
                            "id=ColorBandedcontent>" +
                            "<DIV style=\"MARGIN-LEFT: 5px\" id=imwidget></DIV>" +
                            "<DIV style=\"MARGIN-LEFT: 12px\" id=imcontent><SPAN>" +
                            drRow["Body"].ToString() + "</SPAN></DIV></DIV></DIV>";
                    }
                    from = drRow["from"].ToString();
                }
                convoTime = (DateTime)drRow["MessageIdTime"];
                convoTime = convoTime.AddSeconds(60);

            }
            return convoHtml;
        }
        private void sessionTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            foreach (TreeNode nodes in this.sessionTreeView.Nodes)
            {
                nodes.BackColor = Color.White;
                foreach (TreeNode inNode in nodes.Nodes)
                {
                    inNode.BackColor = Color.White;
                }
            }

            List<String> sessions = new List<String>();
            TreeNode node = e.Node;
            node.BackColor = Color.LightGray;
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode session in node.Nodes)
                {
                    session.BackColor = Color.LightGray;
                    //activeSessions.Add(String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", (DateTime)(drRow["SessionIdTime"])));
                    sessions.Add(session.Tag.ToString());
                }
            }
            else
            {
                sessions.Add(node.Tag.ToString());
            }
            FetchConvo(sessions);
        }

    }
}
