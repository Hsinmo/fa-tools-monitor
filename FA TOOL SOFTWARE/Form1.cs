using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace FA_TOOL_SOFTWARE
{
    public partial class Form1 : Form
    {
        double ID, COM;
        LM_control_for_user call_function;
        public Form1()
        {
            InitializeComponent();
            if (基本版ToolStripMenuItem.Checked == true)
            {
                int Height = Screen.PrimaryScreen.Bounds.Height;
                int Width = Screen.PrimaryScreen.Bounds.Width;
                this.Width = 395;
                this.Height = 638;
            }
            call_function = new LM_control_for_user();

            ForForm1MainControlLoad();
        }
        private void detect_but_Click_1(object sender, EventArgs e)
        {
            if (!IsConnectionToFaDevice)
            {
                MessageBox.Show("请检查是否连线至检测系统");
                return;
            }
            //get_ID_and_COM();
            if (barcode.Text == "" || failure_cause.Text == "")
            {
                bool Natural_Number = true;

                Natural_Number = IsNatural_Number(barcode.Text);
                if (Natural_Number == true)
                {
                    basic_monitor_Panel.Enabled = false;
                    groupBox1.Enabled = false;
                    groupBox2.Enabled = false;
                    panel1.Enabled = false;
                    clear_all_value();
                    if (failure_cause.Text == "")
                    {
                        MessageBox.Show("未选择故障原因");
                    }
                }
                else
                {
                    basic_monitor_Panel.Enabled = false;
                    groupBox1.Enabled = false;
                    groupBox2.Enabled = false;
                    panel1.Enabled = false;
                    clear_all_value();
                    MessageBox.Show("请输入条码(英文和数字)");
                }
            }
            else
            {
                basic_monitor_Panel.Enabled = true;
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                Set_CMD_For_Get_LEV_Whole_Data();
            }
        }
        public bool IsNatural_Number(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return reg1.IsMatch(str);
        }
        private void 基本版ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            基本版ToolStripMenuItem.Checked = true;
            进阶版ToolStripMenuItem.Checked = false;
            int Height = Screen.PrimaryScreen.Bounds.Height;
            int Width = Screen.PrimaryScreen.Bounds.Width;
            this.Width = 395;
            this.Height = 638;
        }

        private void 进阶版ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            基本版ToolStripMenuItem.Checked = false;
            进阶版ToolStripMenuItem.Checked = true;
            int Height = Screen.PrimaryScreen.Bounds.Height;
            int Width = Screen.PrimaryScreen.Bounds.Width;
            this.Width = 660;
            this.Height = 638;
        }

        private void 關於ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 settingsForm = new Form3();
            settingsForm.Show();
        }

        private void 离开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(Environment.ExitCode);

            InitializeComponent();
        }
        public void show_capecity(int percentage1)
        {

            capacity.Value = percentage1;
            percentage.Text = Convert.ToString(percentage1) + "％";
        }
        public void show_cycle_count(int count_number1)
        {
            cycle_count.Value = count_number1/10;
            count_number.Text = Convert.ToString(count_number1);
        }
        
        public void show_manufacture_name(bool enable)
        {
            if(enable)
            {
                manufacture_name.Text = "Dynapack";
            }
            else
            {
                manufacture_name.Text = "无法辨识";
            }
        }
        public void show_manufacture_name(string str)
        {
            manufacture_name.Text = str;
        }
        public void OV_Protect(bool enable)
        {
            if (enable)
            {
                OVP.ForeColor = Color.Red;
                OVP.Text = "启动";
            }
            else
            {
                OVP.ForeColor = Color.Green;
                OVP.Text = "未启动";
            }
        }
        public void UV_Protect(bool enable)
        {
            if (enable)
            {
                UVP.ForeColor = Color.Red;
                UVP.Text = "启动";
            }
            else
            {
                UVP.ForeColor = Color.Green;
                UVP.Text = "未启动";
            }
        }
        public void OC_Protect_CHG(bool enable)
        {
            if (enable)
            {
                OCP_CHG.ForeColor = Color.Red;
                OCP_CHG.Text = "启动";
            }
            else
            {
                OCP_CHG.ForeColor = Color.Green;
                OCP_CHG.Text = "未启动";
            }
        }
        public void OC_Protect_DSG(bool enable)
        {
            if (enable)
            {
                OCP_DSG.ForeColor = Color.Red;
                OCP_DSG.Text = "启动";
            }
            else
            {
                OCP_DSG.ForeColor = Color.Green;
                OCP_DSG.Text = "未启动";
            }
        }
        public void OT_Protect_CHG(bool enable)
        {
            if (enable)
            {
                OTP_CHG.ForeColor = Color.Red;
                OTP_CHG.Text = "启动";
            }
            else
            {
                OTP_CHG.ForeColor = Color.Green;
                OTP_CHG.Text = "未启动";
            }
        }
        public void OT_Protect_DSG(bool enable)
        {
            if (enable)
            {
                OTP_DSG.ForeColor = Color.Red;
                OTP_DSG.Text = "启动";
            }
            else
            {
                OTP_DSG.ForeColor = Color.Green;
                OTP_DSG.Text = "未启动";
            }
        }
        public enum Current_Status
        {
            Static,
            Charging,
            Discharging
        }
        public void show_manufacture_date(string value)
        {
            Manufacture_date.Text = value;
        }
        public void show_voltage_value(int value)
        {
            voltage_value.Text = Convert.ToString(value);
        }
        public void show_current_value(int value, Current_Status c_status)
        {
            //current_value.Text = Convert.ToString(value) + "毫安";
            if (c_status == Current_Status.Static)
            {
                current_value.Text = Convert.ToString(value) + "(静置中)";
                //current_status.Text = "静置中";
            }
            else if (c_status == Current_Status.Charging)
            {
                current_value.Text = Convert.ToString(value) + "(充電中)";
                //current_status.Text = "充电中";
            }
            else
            {
                current_value.Text = Convert.ToString(value) + "(放電中)";
                //current_status.Text = "放电中";
            }
        }
        public void show_communication_status(bool enable)
        {
            if (enable)
            {
                communication_status.ForeColor = Color.Red;
                communication_status.Text = "已连线";
            }
            else
            {
                if (communication_status.Text.Equals("已连线"))
                {
                    communication_status.ForeColor = Color.Green;
                    communication_status.Text = "未连线";
                    MessageBox.Show("请检查是否连线至检测系统");
                }
                else
                {
                    communication_status.ForeColor = Color.Green;
                    communication_status.Text = "未连线";
                }
            }
        }
        
        public void show_temperature_value(int value)
        {
            temperature_value.Text = Convert.ToString(value);
        }
        public void show_FCC_value(int value)
        {
            FCC_value.Text = Convert.ToString(value);
        }
        public void show_CHG_MOS_status(bool enable)
        {
            if (enable)
            {
                MOS_STATUS_CHG.ForeColor = Color.Green;
                MOS_STATUS_CHG.Text = "正常";
            }
            else
            {
                MOS_STATUS_CHG.ForeColor = Color.Red;
                MOS_STATUS_CHG.Text = "异常";
            }
        }
        public void show_DSG_MOS_status(bool enable)
        {
            if (enable)
            {
                MOS_STATUS_DSG.ForeColor = Color.Green;
                MOS_STATUS_DSG.Text = "正常";
            }
            else
            {
                MOS_STATUS_DSG.ForeColor = Color.Red;
                MOS_STATUS_DSG.Text = "异常";
            }
        }
        public void clear_all_value()
        {
            capacity.Value = 0;
            percentage.Text = "";
            cycle_count.Value = 0;
            count_number.Text = "";
            manufacture_name.Text = "";
            OVP.Text = "";
            UVP.Text = "";
            current_value.Text = "";
            OTP_DSG.Text = "";
            OTP_CHG.Text = "";
            OCP_DSG.Text = "";
            OCP_CHG.Text = "";
            voltage_value.Text = "";
            temperature_value.Text = "";
            FCC_value.Text = "";
            MOS_STATUS_CHG.Text = "";
            MOS_STATUS_DSG.Text = "";
            //communication_status.Text = "";
            Manufacture_date.Text = "";

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //textBox1.Text = tt.Find_Machine_SerialNumber();
        }
        private void form_lock()
        {
            panel2.Enabled = false;
        }
        private void form_unlock()
        {
            panel2.Enabled = true;
        }     

        private void 工程測試ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            password_form aForm2 = new password_form();
            aForm2.Show();
        }

        private void get_ID_and_COM()
        {
            COM = Convert.ToDouble(call_function.Find_Machine_SerialNumber());
            COMtxb.Text = Convert.ToString(COM);
            ID = 2;
            IDtxb.Text = Convert.ToString(ID);
            timer1.Enabled = true;
        }

        private void get_LM_status(Double ID, Double COM)
        {
            Double V = 0.0f;
            Double I = 0.0f;
            Double T = 0.0f;
            call_function.Readstatus(ID, COM, ref V, ref I, ref T);
        }

        public enum Action
        {
            CC_C,
            CC_CV_C,
            CP_C,
            CC_D,
            CC_CV_D,
            CP_D,
            Rest
        }

        private void setting_LM(Double ID, Double COM, Double Action, Double V, Double I, Double P,
            Double OV, Double UV, Double OC, Double UC, Double OT)
        {
            Double Return_value;
            Return_value = call_function.learning_machine_setting(ID, COM, Action, V, I, P, OV, UV, OC, UC, OT);
        }

        private void start_LM(Double ID, Double COM)
        {
            Double Return_value;
            Return_value = call_function.learning_machine_start(ID, COM);
        } 

        private void stop_LM(Double ID, Double COM)
        {
            Double Return_value;
            Return_value = call_function.learning_machine_stop(ID, COM);
        }
    }
}
