namespace OCHistory
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sessionTreeView = new System.Windows.Forms.TreeView();
            this.conversationWebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // sessionTreeView
            // 
            this.sessionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionTreeView.Location = new System.Drawing.Point(12, 12);
            this.sessionTreeView.Name = "sessionTreeView";
            this.sessionTreeView.Size = new System.Drawing.Size(511, 170);
            this.sessionTreeView.TabIndex = 0;
            this.sessionTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.sessionTreeView_NodeMouseClick);
            // 
            // conversationWebBrowser
            // 
            this.conversationWebBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.conversationWebBrowser.Location = new System.Drawing.Point(12, 188);
            this.conversationWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.conversationWebBrowser.Name = "conversationWebBrowser";
            this.conversationWebBrowser.Size = new System.Drawing.Size(511, 311);
            this.conversationWebBrowser.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 511);
            this.Controls.Add(this.conversationWebBrowser);
            this.Controls.Add(this.sessionTreeView);
            this.Name = "Form1";
            this.Text = "OCHistory";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView sessionTreeView;
        private System.Windows.Forms.WebBrowser conversationWebBrowser;        
    }
}

