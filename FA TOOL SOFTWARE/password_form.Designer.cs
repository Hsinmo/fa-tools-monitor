namespace FA_TOOL_SOFTWARE
{
    partial class password_form
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
            this.passwordtxb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.yesbut = new System.Windows.Forms.Button();
            this.nobut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // passwordtxb
            // 
            this.passwordtxb.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordtxb.Location = new System.Drawing.Point(8, 31);
            this.passwordtxb.Name = "passwordtxb";
            this.passwordtxb.PasswordChar = '*';
            this.passwordtxb.Size = new System.Drawing.Size(157, 29);
            this.passwordtxb.TabIndex = 0;
            this.passwordtxb.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(27, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "请输入密码";
            // 
            // yesbut
            // 
            this.yesbut.Location = new System.Drawing.Point(8, 66);
            this.yesbut.Name = "yesbut";
            this.yesbut.Size = new System.Drawing.Size(75, 23);
            this.yesbut.TabIndex = 2;
            this.yesbut.Text = "确定";
            this.yesbut.UseVisualStyleBackColor = true;
            this.yesbut.Click += new System.EventHandler(this.button1_Click);
            // 
            // nobut
            // 
            this.nobut.Location = new System.Drawing.Point(90, 66);
            this.nobut.Name = "nobut";
            this.nobut.Size = new System.Drawing.Size(75, 23);
            this.nobut.TabIndex = 3;
            this.nobut.Text = "取消";
            this.nobut.UseVisualStyleBackColor = true;
            this.nobut.Click += new System.EventHandler(this.nobut_Click);
            // 
            // password_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(172, 99);
            this.Controls.Add(this.nobut);
            this.Controls.Add(this.yesbut);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.passwordtxb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "password_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.password_form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox passwordtxb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button yesbut;
        private System.Windows.Forms.Button nobut;
    }
}