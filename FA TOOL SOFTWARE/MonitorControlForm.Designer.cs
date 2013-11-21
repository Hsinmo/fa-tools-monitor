namespace FA_TOOL_SOFTWARE
{
    partial class MonitorControlForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.FA_TOOLS_Connect_Status_Lable = new System.Windows.Forms.Label();
            this.Connection_groupBox = new System.Windows.Forms.GroupBox();
            this.Message_groupBox = new System.Windows.Forms.GroupBox();
            this.Clear_Message_button = new System.Windows.Forms.Button();
            this.Infor_richTextBox = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SendData_ForFATool_button = new System.Windows.Forms.Button();
            this.SendData_ForFATool_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Charger_Test_button = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.SendDataToUart_button = new System.Windows.Forms.Button();
            this.SendDataToUart_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Connection_groupBox.SuspendLayout();
            this.Message_groupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fa Tool Connection : ";
            // 
            // FA_TOOLS_Connect_Status_Lable
            // 
            this.FA_TOOLS_Connect_Status_Lable.AutoSize = true;
            this.FA_TOOLS_Connect_Status_Lable.Location = new System.Drawing.Point(119, 18);
            this.FA_TOOLS_Connect_Status_Lable.Name = "FA_TOOLS_Connect_Status_Lable";
            this.FA_TOOLS_Connect_Status_Lable.Size = new System.Drawing.Size(70, 12);
            this.FA_TOOLS_Connect_Status_Lable.TabIndex = 1;
            this.FA_TOOLS_Connect_Status_Lable.Text = "DisConnected";
            // 
            // Connection_groupBox
            // 
            this.Connection_groupBox.Controls.Add(this.label1);
            this.Connection_groupBox.Controls.Add(this.FA_TOOLS_Connect_Status_Lable);
            this.Connection_groupBox.Location = new System.Drawing.Point(12, 12);
            this.Connection_groupBox.Name = "Connection_groupBox";
            this.Connection_groupBox.Size = new System.Drawing.Size(208, 43);
            this.Connection_groupBox.TabIndex = 2;
            this.Connection_groupBox.TabStop = false;
            this.Connection_groupBox.Text = "Connecting Status";
            // 
            // Message_groupBox
            // 
            this.Message_groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Message_groupBox.Controls.Add(this.Clear_Message_button);
            this.Message_groupBox.Controls.Add(this.Infor_richTextBox);
            this.Message_groupBox.Location = new System.Drawing.Point(12, 198);
            this.Message_groupBox.Name = "Message_groupBox";
            this.Message_groupBox.Size = new System.Drawing.Size(779, 248);
            this.Message_groupBox.TabIndex = 4;
            this.Message_groupBox.TabStop = false;
            this.Message_groupBox.Text = "Messages";
            // 
            // Clear_Message_button
            // 
            this.Clear_Message_button.Location = new System.Drawing.Point(679, 12);
            this.Clear_Message_button.Name = "Clear_Message_button";
            this.Clear_Message_button.Size = new System.Drawing.Size(94, 23);
            this.Clear_Message_button.TabIndex = 1;
            this.Clear_Message_button.Text = "Clear Message";
            this.Clear_Message_button.UseVisualStyleBackColor = true;
            this.Clear_Message_button.Click += new System.EventHandler(this.Clear_Message_button_Click);
            // 
            // Infor_richTextBox
            // 
            this.Infor_richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Infor_richTextBox.Location = new System.Drawing.Point(7, 41);
            this.Infor_richTextBox.Name = "Infor_richTextBox";
            this.Infor_richTextBox.ReadOnly = true;
            this.Infor_richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.Infor_richTextBox.Size = new System.Drawing.Size(766, 201);
            this.Infor_richTextBox.TabIndex = 0;
            this.Infor_richTextBox.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SendData_ForFATool_button);
            this.groupBox1.Controls.Add(this.SendData_ForFATool_textBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(13, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(357, 62);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sending Out For FA Tool";
            // 
            // SendData_ForFATool_button
            // 
            this.SendData_ForFATool_button.Location = new System.Drawing.Point(276, 31);
            this.SendData_ForFATool_button.Name = "SendData_ForFATool_button";
            this.SendData_ForFATool_button.Size = new System.Drawing.Size(75, 23);
            this.SendData_ForFATool_button.TabIndex = 4;
            this.SendData_ForFATool_button.Text = "Send Data";
            this.SendData_ForFATool_button.UseVisualStyleBackColor = true;
            this.SendData_ForFATool_button.Click += new System.EventHandler(this.SendData_ForFATool_button_Click);
            // 
            // SendData_ForFATool_textBox
            // 
            this.SendData_ForFATool_textBox.Location = new System.Drawing.Point(11, 33);
            this.SendData_ForFATool_textBox.Name = "SendData_ForFATool_textBox";
            this.SendData_ForFATool_textBox.Size = new System.Drawing.Size(259, 22);
            this.SendData_ForFATool_textBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Sending Data (Hex) :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.richTextBox1);
            this.groupBox2.Controls.Add(this.Charger_Test_button);
            this.groupBox2.Location = new System.Drawing.Point(469, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(236, 140);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Charger Test";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Charger Result :";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(6, 51);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(224, 83);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // Charger_Test_button
            // 
            this.Charger_Test_button.Location = new System.Drawing.Point(115, 13);
            this.Charger_Test_button.Name = "Charger_Test_button";
            this.Charger_Test_button.Size = new System.Drawing.Size(75, 23);
            this.Charger_Test_button.TabIndex = 0;
            this.Charger_Test_button.Text = "Start Check";
            this.Charger_Test_button.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.SendDataToUart_button);
            this.groupBox3.Controls.Add(this.SendDataToUart_textBox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(13, 130);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(357, 62);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sending to UART through FA Tool";
            // 
            // SendDataToUart_button
            // 
            this.SendDataToUart_button.Location = new System.Drawing.Point(276, 31);
            this.SendDataToUart_button.Name = "SendDataToUart_button";
            this.SendDataToUart_button.Size = new System.Drawing.Size(75, 23);
            this.SendDataToUart_button.TabIndex = 4;
            this.SendDataToUart_button.Text = "Send Data";
            this.SendDataToUart_button.UseVisualStyleBackColor = true;
            this.SendDataToUart_button.Click += new System.EventHandler(this.SendDataToUart_button_Click);
            // 
            // SendDataToUart_textBox
            // 
            this.SendDataToUart_textBox.Location = new System.Drawing.Point(11, 33);
            this.SendDataToUart_textBox.Name = "SendDataToUart_textBox";
            this.SendDataToUart_textBox.Size = new System.Drawing.Size(259, 22);
            this.SendDataToUart_textBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Sending Data (Hex) :";
            // 
            // MonitorControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 452);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Message_groupBox);
            this.Controls.Add(this.Connection_groupBox);
            this.Name = "MonitorControlForm";
            this.Text = "MonitorControlForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MonitorControlForm_FormClosing);
            this.Connection_groupBox.ResumeLayout(false);
            this.Connection_groupBox.PerformLayout();
            this.Message_groupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label FA_TOOLS_Connect_Status_Lable;
        private System.Windows.Forms.GroupBox Connection_groupBox;
        private System.Windows.Forms.GroupBox Message_groupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox SendData_ForFATool_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SendData_ForFATool_button;
        private System.Windows.Forms.RichTextBox Infor_richTextBox;
        private System.Windows.Forms.Button Clear_Message_button;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button Charger_Test_button;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button SendDataToUart_button;
        private System.Windows.Forms.TextBox SendDataToUart_textBox;
        private System.Windows.Forms.Label label4;
    }
}