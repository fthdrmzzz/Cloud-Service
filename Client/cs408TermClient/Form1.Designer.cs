namespace cs408TermClient
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
            this.IP = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxLocation = new System.Windows.Forms.TextBox();
            this.log = new System.Windows.Forms.RichTextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.checkedListBox_rec_files = new System.Windows.Forms.CheckedListBox();
            this.button_file_req = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button_down_file = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_browse_down = new System.Windows.Forms.TextBox();
            this.button_browse_down = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.textboxPublish = new System.Windows.Forms.Button();
            this.buttonPublicFileReq = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // IP
            // 
            this.IP.AutoSize = true;
            this.IP.Location = new System.Drawing.Point(68, 38);
            this.IP.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.IP.Name = "IP";
            this.IP.Size = new System.Drawing.Size(17, 13);
            this.IP.TabIndex = 0;
            this.IP.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 76);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 132);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "File Location";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(88, 38);
            this.textBoxIP.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(154, 20);
            this.textBoxIP.TabIndex = 3;
            this.textBoxIP.Text = "127.0.0.1";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(88, 72);
            this.textBoxPort.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(154, 20);
            this.textBoxPort.TabIndex = 4;
            this.textBoxPort.Text = "1500";
            // 
            // textBoxLocation
            // 
            this.textBoxLocation.Location = new System.Drawing.Point(88, 133);
            this.textBoxLocation.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxLocation.Name = "textBoxLocation";
            this.textBoxLocation.ReadOnly = true;
            this.textBoxLocation.Size = new System.Drawing.Size(154, 20);
            this.textBoxLocation.TabIndex = 5;
            // 
            // log
            // 
            this.log.Location = new System.Drawing.Point(321, 36);
            this.log.Margin = new System.Windows.Forms.Padding(2);
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.Size = new System.Drawing.Size(236, 180);
            this.log.TabIndex = 6;
            this.log.Text = "";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(245, 38);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(2);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(71, 82);
            this.buttonConnect.TabIndex = 7;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Enabled = false;
            this.buttonDisconnect.Location = new System.Drawing.Point(245, 174);
            this.buttonDisconnect.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(71, 19);
            this.buttonDisconnect.TabIndex = 8;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Enabled = false;
            this.buttonBrowse.Location = new System.Drawing.Point(245, 132);
            this.buttonBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(71, 19);
            this.buttonBrowse.TabIndex = 9;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonSend
            // 
            this.buttonSend.Enabled = false;
            this.buttonSend.Location = new System.Drawing.Point(88, 174);
            this.buttonSend.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(153, 19);
            this.buttonSend.TabIndex = 10;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(88, 102);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(154, 20);
            this.textBoxName.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 106);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Name";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(372, 10);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(184, 21);
            this.progressBar1.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(319, 17);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Progress";
            // 
            // checkedListBox_rec_files
            // 
            this.checkedListBox_rec_files.Enabled = false;
            this.checkedListBox_rec_files.FormattingEnabled = true;
            this.checkedListBox_rec_files.Location = new System.Drawing.Point(9, 270);
            this.checkedListBox_rec_files.Margin = new System.Windows.Forms.Padding(2);
            this.checkedListBox_rec_files.Name = "checkedListBox_rec_files";
            this.checkedListBox_rec_files.Size = new System.Drawing.Size(553, 139);
            this.checkedListBox_rec_files.TabIndex = 15;
            // 
            // button_file_req
            // 
            this.button_file_req.Enabled = false;
            this.button_file_req.Location = new System.Drawing.Point(9, 244);
            this.button_file_req.Margin = new System.Windows.Forms.Padding(2);
            this.button_file_req.Name = "button_file_req";
            this.button_file_req.Size = new System.Drawing.Size(287, 19);
            this.button_file_req.TabIndex = 16;
            this.button_file_req.Text = "User File Request";
            this.button_file_req.UseVisualStyleBackColor = true;
            this.button_file_req.Click += new System.EventHandler(this.button_file_req_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label5.Location = new System.Drawing.Point(-1, 228);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(574, 8);
            this.label5.TabIndex = 17;
            // 
            // button_down_file
            // 
            this.button_down_file.Enabled = false;
            this.button_down_file.Location = new System.Drawing.Point(271, 426);
            this.button_down_file.Margin = new System.Windows.Forms.Padding(2);
            this.button_down_file.Name = "button_down_file";
            this.button_down_file.Size = new System.Drawing.Size(68, 20);
            this.button_down_file.TabIndex = 18;
            this.button_down_file.Text = "Download File";
            this.button_down_file.UseVisualStyleBackColor = true;
            this.button_down_file.Click += new System.EventHandler(this.button_down_file_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 427);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "File Location";
            // 
            // textBox_browse_down
            // 
            this.textBox_browse_down.Enabled = false;
            this.textBox_browse_down.Location = new System.Drawing.Point(80, 427);
            this.textBox_browse_down.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_browse_down.Name = "textBox_browse_down";
            this.textBox_browse_down.Size = new System.Drawing.Size(115, 20);
            this.textBox_browse_down.TabIndex = 20;
            // 
            // button_browse_down
            // 
            this.button_browse_down.Enabled = false;
            this.button_browse_down.Location = new System.Drawing.Point(199, 426);
            this.button_browse_down.Margin = new System.Windows.Forms.Padding(2);
            this.button_browse_down.Name = "button_browse_down";
            this.button_browse_down.Size = new System.Drawing.Size(68, 20);
            this.button_browse_down.TabIndex = 21;
            this.button_browse_down.Text = "Browse";
            this.button_browse_down.UseVisualStyleBackColor = true;
            this.button_browse_down.Click += new System.EventHandler(this.button_browse_down_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.Enabled = false;
            this.buttonCopy.Location = new System.Drawing.Point(343, 426);
            this.buttonCopy.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(68, 20);
            this.buttonCopy.TabIndex = 22;
            this.buttonCopy.Text = "Copy";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Location = new System.Drawing.Point(415, 427);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(68, 20);
            this.buttonDelete.TabIndex = 23;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // textboxPublish
            // 
            this.textboxPublish.Enabled = false;
            this.textboxPublish.Location = new System.Drawing.Point(488, 426);
            this.textboxPublish.Name = "textboxPublish";
            this.textboxPublish.Size = new System.Drawing.Size(73, 22);
            this.textboxPublish.TabIndex = 24;
            this.textboxPublish.Text = "Publish";
            this.textboxPublish.UseVisualStyleBackColor = true;
            this.textboxPublish.Click += new System.EventHandler(this.textboxPublish_Click);
            // 
            // buttonPublicFileReq
            // 
            this.buttonPublicFileReq.Enabled = false;
            this.buttonPublicFileReq.Location = new System.Drawing.Point(301, 244);
            this.buttonPublicFileReq.Name = "buttonPublicFileReq";
            this.buttonPublicFileReq.Size = new System.Drawing.Size(260, 19);
            this.buttonPublicFileReq.TabIndex = 25;
            this.buttonPublicFileReq.Text = "Public File Request";
            this.buttonPublicFileReq.UseVisualStyleBackColor = true;
            this.buttonPublicFileReq.Click += new System.EventHandler(this.buttonPublicFileReq_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 457);
            this.Controls.Add(this.buttonPublicFileReq);
            this.Controls.Add(this.textboxPublish);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.button_browse_down);
            this.Controls.Add(this.textBox_browse_down);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button_down_file);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_file_req);
            this.Controls.Add(this.checkedListBox_rec_files);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.log);
            this.Controls.Add(this.textBoxLocation);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.IP);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label IP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxLocation;
        private System.Windows.Forms.RichTextBox log;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox checkedListBox_rec_files;
        private System.Windows.Forms.Button button_file_req;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_down_file;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_browse_down;
        private System.Windows.Forms.Button button_browse_down;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button textboxPublish;
        private System.Windows.Forms.Button buttonPublicFileReq;
    }
}

