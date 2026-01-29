namespace TWatcher
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
            this.btnStartMITM = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnSendToAlice = new System.Windows.Forms.Button();
            this.btnSendKeyToAlice = new System.Windows.Forms.Button();
            this.btnSendKeyToBob = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSendToBob = new System.Windows.Forms.Button();
            this.txtAliceMessage = new System.Windows.Forms.TextBox();
            this.txtChatAlice = new System.Windows.Forms.TextBox();
            this.txtChatBob = new System.Windows.Forms.TextBox();
            this.txtBobMessage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpenFakeFolder = new System.Windows.Forms.Button();
            this.btnViewHistory = new System.Windows.Forms.Button();
            this.btnOpenStolenFolder = new System.Windows.Forms.Button();
            this.btnSaveChatHistory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartMITM
            // 
            this.btnStartMITM.BackColor = System.Drawing.Color.Violet;
            this.btnStartMITM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartMITM.Location = new System.Drawing.Point(12, 81);
            this.btnStartMITM.Name = "btnStartMITM";
            this.btnStartMITM.Size = new System.Drawing.Size(152, 62);
            this.btnStartMITM.TabIndex = 24;
            this.btnStartMITM.Text = "Start MITM";
            this.btnStartMITM.UseVisualStyleBackColor = false;
            this.btnStartMITM.Click += new System.EventHandler(this.btnStartMITM_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(12, 149);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(615, 130);
            this.txtOutput.TabIndex = 25;
            // 
            // btnSendToAlice
            // 
            this.btnSendToAlice.BackColor = System.Drawing.Color.SkyBlue;
            this.btnSendToAlice.Image = ((System.Drawing.Image)(resources.GetObject("btnSendToAlice.Image")));
            this.btnSendToAlice.Location = new System.Drawing.Point(282, 592);
            this.btnSendToAlice.Name = "btnSendToAlice";
            this.btnSendToAlice.Size = new System.Drawing.Size(110, 62);
            this.btnSendToAlice.TabIndex = 26;
            this.btnSendToAlice.UseVisualStyleBackColor = false;
            this.btnSendToAlice.Click += new System.EventHandler(this.btnSendToAlice_Click);
            // 
            // btnSendKeyToAlice
            // 
            this.btnSendKeyToAlice.BackColor = System.Drawing.Color.LightGray;
            this.btnSendKeyToAlice.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.150944F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendKeyToAlice.Location = new System.Drawing.Point(633, 149);
            this.btnSendKeyToAlice.Name = "btnSendKeyToAlice";
            this.btnSendKeyToAlice.Size = new System.Drawing.Size(152, 62);
            this.btnSendKeyToAlice.TabIndex = 27;
            this.btnSendKeyToAlice.Text = "Gửi khóa cho Alice";
            this.btnSendKeyToAlice.UseVisualStyleBackColor = false;
            this.btnSendKeyToAlice.Click += new System.EventHandler(this.btnSendKeyToAlice_Click);
            // 
            // btnSendKeyToBob
            // 
            this.btnSendKeyToBob.BackColor = System.Drawing.Color.LightGray;
            this.btnSendKeyToBob.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.150944F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendKeyToBob.Location = new System.Drawing.Point(633, 217);
            this.btnSendKeyToBob.Name = "btnSendKeyToBob";
            this.btnSendKeyToBob.Size = new System.Drawing.Size(152, 62);
            this.btnSendKeyToBob.TabIndex = 28;
            this.btnSendKeyToBob.Text = "Gửi khóa cho Bob";
            this.btnSendKeyToBob.UseVisualStyleBackColor = false;
            this.btnSendKeyToBob.Click += new System.EventHandler(this.btnSendKeyToBob_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Salmon;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(675, 681);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(110, 62);
            this.btnExit.TabIndex = 29;
            this.btnExit.Text = "Thoát";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSendToBob
            // 
            this.btnSendToBob.BackColor = System.Drawing.Color.SandyBrown;
            this.btnSendToBob.Image = ((System.Drawing.Image)(resources.GetObject("btnSendToBob.Image")));
            this.btnSendToBob.Location = new System.Drawing.Point(675, 592);
            this.btnSendToBob.Name = "btnSendToBob";
            this.btnSendToBob.Size = new System.Drawing.Size(110, 62);
            this.btnSendToBob.TabIndex = 31;
            this.btnSendToBob.UseVisualStyleBackColor = false;
            this.btnSendToBob.Click += new System.EventHandler(this.btnSendToBob_Click);
            // 
            // txtAliceMessage
            // 
            this.txtAliceMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAliceMessage.Location = new System.Drawing.Point(12, 344);
            this.txtAliceMessage.Multiline = true;
            this.txtAliceMessage.Name = "txtAliceMessage";
            this.txtAliceMessage.ReadOnly = true;
            this.txtAliceMessage.Size = new System.Drawing.Size(380, 242);
            this.txtAliceMessage.TabIndex = 32;
            // 
            // txtChatAlice
            // 
            this.txtChatAlice.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChatAlice.Location = new System.Drawing.Point(12, 592);
            this.txtChatAlice.Multiline = true;
            this.txtChatAlice.Name = "txtChatAlice";
            this.txtChatAlice.Size = new System.Drawing.Size(264, 62);
            this.txtChatAlice.TabIndex = 34;
            // 
            // txtChatBob
            // 
            this.txtChatBob.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChatBob.Location = new System.Drawing.Point(405, 592);
            this.txtChatBob.Multiline = true;
            this.txtChatBob.Name = "txtChatBob";
            this.txtChatBob.Size = new System.Drawing.Size(264, 62);
            this.txtChatBob.TabIndex = 35;
            // 
            // txtBobMessage
            // 
            this.txtBobMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBobMessage.Location = new System.Drawing.Point(405, 344);
            this.txtBobMessage.Multiline = true;
            this.txtBobMessage.Name = "txtBobMessage";
            this.txtBobMessage.ReadOnly = true;
            this.txtBobMessage.Size = new System.Drawing.Size(380, 242);
            this.txtBobMessage.TabIndex = 36;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.69811F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.LightGray;
            this.label2.Location = new System.Drawing.Point(8, 308);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(218, 33);
            this.label2.TabIndex = 37;
            this.label2.Text = "Alice Message";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.69811F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.LightGray;
            this.label3.Location = new System.Drawing.Point(399, 308);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(203, 33);
            this.label3.TabIndex = 37;
            this.label3.Text = "Bob Message";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 25.81132F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(291, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(228, 44);
            this.label1.TabIndex = 21;
            this.label1.Text = "T - Watcher";
            // 
            // btnOpenFakeFolder
            // 
            this.btnOpenFakeFolder.BackColor = System.Drawing.Color.Gray;
            this.btnOpenFakeFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.150944F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenFakeFolder.Location = new System.Drawing.Point(12, 681);
            this.btnOpenFakeFolder.Name = "btnOpenFakeFolder";
            this.btnOpenFakeFolder.Size = new System.Drawing.Size(128, 62);
            this.btnOpenFakeFolder.TabIndex = 38;
            this.btnOpenFakeFolder.Text = "TWacher";
            this.btnOpenFakeFolder.UseVisualStyleBackColor = false;
            this.btnOpenFakeFolder.Click += new System.EventHandler(this.btnOpenFakeFolder_Click);
            // 
            // btnViewHistory
            // 
            this.btnViewHistory.BackColor = System.Drawing.Color.Gray;
            this.btnViewHistory.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnViewHistory.Image = global::TWatcher.Properties.Resources.icons8_connection_241;
            this.btnViewHistory.Location = new System.Drawing.Point(280, 681);
            this.btnViewHistory.Name = "btnViewHistory";
            this.btnViewHistory.Size = new System.Drawing.Size(83, 62);
            this.btnViewHistory.TabIndex = 40;
            this.btnViewHistory.UseVisualStyleBackColor = false;
            this.btnViewHistory.Click += new System.EventHandler(this.btnViewHistory_Click);
            // 
            // btnOpenStolenFolder
            // 
            this.btnOpenStolenFolder.BackColor = System.Drawing.Color.Gray;
            this.btnOpenStolenFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.150944F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenStolenFolder.Location = new System.Drawing.Point(146, 680);
            this.btnOpenStolenFolder.Name = "btnOpenStolenFolder";
            this.btnOpenStolenFolder.Size = new System.Drawing.Size(128, 62);
            this.btnOpenStolenFolder.TabIndex = 38;
            this.btnOpenStolenFolder.Text = "Khóa đánh cắp";
            this.btnOpenStolenFolder.UseVisualStyleBackColor = false;
            this.btnOpenStolenFolder.Click += new System.EventHandler(this.btnOpenStolenFolder_Click);
            // 
            // btnSaveChatHistory
            // 
            this.btnSaveChatHistory.BackColor = System.Drawing.Color.Gray;
            this.btnSaveChatHistory.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSaveChatHistory.Image = global::TWatcher.Properties.Resources.icons8_download_28;
            this.btnSaveChatHistory.Location = new System.Drawing.Point(369, 682);
            this.btnSaveChatHistory.Name = "btnSaveChatHistory";
            this.btnSaveChatHistory.Size = new System.Drawing.Size(83, 62);
            this.btnSaveChatHistory.TabIndex = 40;
            this.btnSaveChatHistory.UseVisualStyleBackColor = false;
            this.btnSaveChatHistory.Click += new System.EventHandler(this.btnSaveChatHistory_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.BackgroundImage = global::TWatcher.Properties.Resources.Picture5;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(797, 764);
            this.Controls.Add(this.btnSaveChatHistory);
            this.Controls.Add(this.btnViewHistory);
            this.Controls.Add(this.btnOpenStolenFolder);
            this.Controls.Add(this.btnOpenFakeFolder);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnStartMITM);
            this.Controls.Add(this.btnSendKeyToAlice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSendKeyToBob);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBobMessage);
            this.Controls.Add(this.txtChatBob);
            this.Controls.Add(this.txtChatAlice);
            this.Controls.Add(this.txtAliceMessage);
            this.Controls.Add(this.btnSendToBob);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSendToAlice);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TWatcher";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnStartMITM;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnSendKeyToAlice;
        private System.Windows.Forms.Button btnSendKeyToBob;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSendToAlice;
        private System.Windows.Forms.Button btnSendToBob;
        private System.Windows.Forms.TextBox txtAliceMessage;
        private System.Windows.Forms.TextBox txtChatAlice;
        private System.Windows.Forms.TextBox txtChatBob;
        private System.Windows.Forms.TextBox txtBobMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpenFakeFolder;
        private System.Windows.Forms.Button btnViewHistory;
        private System.Windows.Forms.Button btnOpenStolenFolder;
        private System.Windows.Forms.Button btnSaveChatHistory;
    }
}

