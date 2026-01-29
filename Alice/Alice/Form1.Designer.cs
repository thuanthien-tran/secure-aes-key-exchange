namespace Alice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnOpenServer = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSendKeys = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.btnViewChatHistory = new System.Windows.Forms.Button();
            this.btnViewHistory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOpenServer
            // 
            this.btnOpenServer.BackColor = System.Drawing.Color.SkyBlue;
            this.btnOpenServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.150944F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenServer.Location = new System.Drawing.Point(12, 89);
            this.btnOpenServer.Name = "btnOpenServer";
            this.btnOpenServer.Size = new System.Drawing.Size(152, 62);
            this.btnOpenServer.TabIndex = 0;
            this.btnOpenServer.Text = "Mở Server";
            this.btnOpenServer.UseVisualStyleBackColor = false;
            this.btnOpenServer.Click += new System.EventHandler(this.btnOpenServer_Click);
            // 
            // txtInput
            // 
            this.txtInput.BackColor = System.Drawing.Color.LightGray;
            this.txtInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F);
            this.txtInput.Location = new System.Drawing.Point(170, 89);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(496, 61);
            this.txtInput.TabIndex = 6;
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.SkyBlue;
            this.btnSend.Image = global::Alice.Properties.Resources.icons8_send_20;
            this.btnSend.Location = new System.Drawing.Point(672, 88);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(84, 62);
            this.btnSend.TabIndex = 10;
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.BackColor = System.Drawing.Color.LightGray;
            this.txtOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(170, 157);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(496, 357);
            this.txtOutput.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 25.81132F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label1.Location = new System.Drawing.Point(278, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 44);
            this.label1.TabIndex = 21;
            this.label1.Text = "Alice Server";
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Salmon;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(549, 520);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(117, 51);
            this.btnExit.TabIndex = 23;
            this.btnExit.Text = "Thoát";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSendKeys
            // 
            this.btnSendKeys.BackColor = System.Drawing.Color.LightGray;
            this.btnSendKeys.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.150944F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendKeys.Location = new System.Drawing.Point(12, 157);
            this.btnSendKeys.Name = "btnSendKeys";
            this.btnSendKeys.Size = new System.Drawing.Size(152, 62);
            this.btnSendKeys.TabIndex = 22;
            this.btnSendKeys.Text = "Gửi khóa RSA";
            this.btnSendKeys.UseVisualStyleBackColor = false;
            this.btnSendKeys.Click += new System.EventHandler(this.btnSendKeys_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.btnOpenFolder.Image = global::Alice.Properties.Resources.icons8_folder_20__2_;
            this.btnOpenFolder.Location = new System.Drawing.Point(676, 331);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(84, 51);
            this.btnOpenFolder.TabIndex = 24;
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            // 
            // btnViewChatHistory
            // 
            this.btnViewChatHistory.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.btnViewChatHistory.Image = global::Alice.Properties.Resources.icons8_time_machine_26;
            this.btnViewChatHistory.Location = new System.Drawing.Point(677, 397);
            this.btnViewChatHistory.Name = "btnViewChatHistory";
            this.btnViewChatHistory.Size = new System.Drawing.Size(83, 51);
            this.btnViewChatHistory.TabIndex = 26;
            this.btnViewChatHistory.UseVisualStyleBackColor = false;
            this.btnViewChatHistory.Click += new System.EventHandler(this.btnViewChatHistory_Click);
            // 
            // btnViewHistory
            // 
            this.btnViewHistory.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.btnViewHistory.Image = global::Alice.Properties.Resources.icons8_connection_24;
            this.btnViewHistory.Location = new System.Drawing.Point(676, 463);
            this.btnViewHistory.Name = "btnViewHistory";
            this.btnViewHistory.Size = new System.Drawing.Size(83, 51);
            this.btnViewHistory.TabIndex = 25;
            this.btnViewHistory.UseVisualStyleBackColor = false;
            this.btnViewHistory.Click += new System.EventHandler(this.btnViewHistory_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MediumAquamarine;
            this.BackgroundImage = global::Alice.Properties.Resources.anh_nen_xanh_den_chuyen_sac_780x488;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(767, 587);
            this.Controls.Add(this.btnViewChatHistory);
            this.Controls.Add(this.btnViewHistory);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSendKeys);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnOpenServer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alice";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenServer;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSendKeys;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Button btnViewChatHistory;
        private System.Windows.Forms.Button btnViewHistory;
    }
}
