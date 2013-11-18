using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FA_TOOL_SOFTWARE
{
    public partial class password_form : Form
    {
        public password_form()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void password_form_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (passwordtxb.Text == "123456")
            {
                this.Close();
                LM_control LMC = new LM_control();
                LMC.Show();
            }
            else
            {
                MessageBox.Show("密碼錯誤");
            }
        }

        private void nobut_Click(object sender, EventArgs e)
        {
            this.Close();
            //Environment.Exit(Environment.ExitCode);
            InitializeComponent();
        }
    }
}
