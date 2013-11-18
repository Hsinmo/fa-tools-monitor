namespace FA_TOOL_SOFTWARE
{
    partial class Information
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
            this.infor_textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // infor_textBox
            // 
            this.infor_textBox.Location = new System.Drawing.Point(2, 4);
            this.infor_textBox.Multiline = true;
            this.infor_textBox.Name = "infor_textBox";
            this.infor_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.infor_textBox.Size = new System.Drawing.Size(491, 397);
            this.infor_textBox.TabIndex = 0;
            // 
            // Information
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 413);
            this.Controls.Add(this.infor_textBox);
            this.Name = "Information";
            this.Text = "Information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox infor_textBox;

    }
}