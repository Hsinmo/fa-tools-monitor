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
    public partial class Information : Form
    {
        static int implement_count = 0;
        public Information()
        {
            InitializeComponent();
            implement_count++;
        }
        public Information(string str)
        {
            InitializeComponent();
            implement_count++;
            this.Text = str + " : " + implement_count;
        }
    }
}
