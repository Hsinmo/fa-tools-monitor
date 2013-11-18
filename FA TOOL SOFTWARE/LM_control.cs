using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Runtime.InteropServices;

namespace FA_TOOL_SOFTWARE
{
    public partial class LM_control : Form
    {
        [DllImport("POCBCommand_B6.dll")]
        public static extern double POCBSet1(double ID, double COM, double Action, double V, double I, double P, double OV, double UV, double OC, double UC, double OT);
        [DllImport("POCBCommand_B6.dll")]
        public static extern double POCBStart(double ID, double COM);
        [DllImport("POCBCommand_B6.dll")]
        public static extern double POCBStatusInquiry(double ID, double COM, ref double V, ref double I, ref double T);
        [DllImport("POCBCommand_B6.dll")]
        public static extern double POCBStop(double ID, double COM);

        public LM_control()
        {
            InitializeComponent();
        }

        private void LM_control_Load(object sender, EventArgs e)
        {
            COMtxb.Text = Find_Machine_SerialNumber();
            if (COMtxb.Text == "")
            {
                COMtxb.Text = "000000000000";
            }
        }
        public void Readstatus(TextBox Vtextbox, TextBox Itextbox, TextBox Ttextbox, TextBox returnbox, TextBox IDbox, TextBox COMbox)
        {
            //Timebox.Text = Convert.ToString(DateTime.Now);
            Double ID;
            Double COM;
            Double RV;
            Double RI;
            Double RT;
            Double R, V, I, T;
            V = 0.0f;
            I = 0.0f;
            T = 0.0f;
            ID = Convert.ToDouble(IDbox.Text);
            COM = Convert.ToDouble(COMbox.Text);

            R = POCBStatusInquiry(ID, COM, ref V, ref I, ref T);
            if (R == 1)
            {
                returnbox.Text = "通訊正常";
            }
            else if (R == 2)
            {
                returnbox.Text = "通訊失敗";
            }
            else if (R == 3)
            {
                returnbox.Text = "ID或COM輸入錯誤";
            }
            else
            {
                returnbox.Text = "輸入值超出範圍";
            }
            
            RV = V;
            RI = I;
            RT = T;

            if (R == 1)
            {
                Vtextbox.Text = Convert.ToString(RV);
                Itextbox.Text = Convert.ToString(RI);
                Ttextbox.Text = Convert.ToString(RT);
            }
            else
            {
                Vtextbox.Text = "";
                Itextbox.Text = "";
                Ttextbox.Text = "";
            }
        }
        public void learning_machine_setting(TextBox Actbox, TextBox Vbox, TextBox Ibox, TextBox Pbox, TextBox OVbox, TextBox UVbox,
            TextBox OCbox, TextBox UCbox, TextBox OTbox, TextBox returnbox, TextBox IDbox, TextBox COMbox)
        {
            Double ID;
            Double COM;
            Double Action;
            Double V;
            Double I;
            Double P;
            Double OV;
            Double UV;
            Double OC;
            Double UC;
            Double OT;
            Double R;

            ID = Convert.ToDouble(IDbox.Text);
            COM = Convert.ToDouble(COMbox.Text);
            Action = Convert.ToDouble(Actbox.Text);
            V = Convert.ToDouble(Vbox.Text);
            I = Convert.ToDouble(Ibox.Text);
            P = Convert.ToDouble(Pbox.Text);
            OV = Convert.ToDouble(OVbox.Text);
            UV = Convert.ToDouble(UVbox.Text);
            OC = Convert.ToDouble(OCbox.Text);
            UC = Convert.ToDouble(UCbox.Text);
            OT = Convert.ToDouble(OTbox.Text);
            R = POCBSet1(ID, COM, Action, V, I, P, OV, UV, OC, UC, OT);
            if (R == 1)
            {
                returnbox.Text = "通訊正常";
            }
            else if (R == 2)
            {
                returnbox.Text = "通訊失敗";
            }
            else if (R == 3)
            {
                returnbox.Text = "ID或COM輸入錯誤";
            }
            else
            {
                returnbox.Text = "輸入值超出範圍";
            }
        }

        public void learning_machine_start(TextBox returnbox, TextBox IDbox, TextBox COMbox)
        {
            Double ID;
            Double COM;
            Double R;

            ID = Convert.ToDouble(IDbox.Text);
            COM = Convert.ToDouble(COMbox.Text);
            R = POCBStart(ID, COM);
            if (R == 1)
            {
                returnbox.Text = "通訊正常";
            }
            else if (R == 2)
            {
                returnbox.Text = "通訊失敗";
            }
            else if (R == 3)
            {
                returnbox.Text = "ID或COM輸入錯誤";
            }
            else
            {
                returnbox.Text = "輸入值超出範圍";
            }
        }

        public void learning_machine_stop(TextBox returnbox, TextBox IDbox, TextBox COMbox)
        {
            Double ID;
            Double COM;
            Double R;

            ID = Convert.ToDouble(IDbox.Text);
            COM = Convert.ToDouble(COMbox.Text);
            R = POCBStop(ID, COM);
            if (R == 1)
            {
                returnbox.Text = "通訊正常";
            }
            else if (R == 2)
            {
                returnbox.Text = "通訊失敗";
            }
            else if (R == 3)
            {
                returnbox.Text = "ID或COM輸入錯誤";
            }
            else
            {
                returnbox.Text = "輸入值超出範圍";
            }
        }

        public string DeviceName = "USB";
        public string DeviceVID = "VID_10C4";
        public string DevicePID = "PID_EA80";

        public string Find_Machine_SerialNumber()
        {
            bool error = true;
            string sn = string.Empty;
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity");// WHERE ConfigManagerErrorCode = 0");
                //new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM WIN32_SerialPort");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    //inforForm.infor_textBox.AppendText(queryObj.ToString() + Environment.NewLine);
                    string str = queryObj["DeviceID"].ToString();
                    string[] str_split;
                    if (str.IndexOf(DeviceName) >= 0)
                    {
                        if ((str.IndexOf(DeviceVID) >= 4) && (str.IndexOf(DevicePID) >= 8))
                        {
                            //inforForm.infor_textBox.AppendText(queryObj["DeviceID"] + Environment.NewLine);
                            str = str.Trim();
                            str_split = str.Split(new char[] { '\\', '/' });
                            sn = str_split[str_split.Length - 1];
                            //inforForm.infor_textBox.AppendText(str_split[str_split.Length - 1] + Environment.NewLine);
                            error = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = true;
                //MessageBox.Show("Exception fetching processor id: " + ex.ToString()); // doesn't show
                //MessageBox.Show(ex.Message);
            }
            if (error)
            {
                return string.Empty;
            }
            else
            {
                return sn;
            }
        }

        private void settingbut_Click(object sender, EventArgs e)
        {
            learning_machine_setting(Actiontxb,Vtxb,Itxb,Ptxb,OVtxb,UVtxb,OCtxb,UCtxb,OTtxb,Rsetting,IDtxb,COMtxb);
        }

        private void startbut_Click(object sender, EventArgs e)
        {
            learning_machine_start(Rstart, IDtxb, COMtxb);
        }

        private void stopbut_Click(object sender, EventArgs e)
        {
            learning_machine_stop(Rstop, IDtxb, COMtxb);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IDtxb.Text == "" || COMtxb.Text == "")
            {
                timer1.Enabled = false;
                MessageBox.Show("未輸入ID或COM");
            }
            else
            {
                Readstatus(Vstatus, Istatus, Tstatus, Rbox, IDtxb, COMtxb);
                if (Rbox.Text == "通訊失敗")
                {
                    COMtxb.Text = "000000000000";
                    timer1.Enabled = false;
                }
            }          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            COMtxb.Text = Find_Machine_SerialNumber();
            if (COMtxb.Text == "")
            {
                COMtxb.Text = "000000000000";
            }
            if (COMtxb.Text == "" || IDtxb.Text == "")
            {

                MessageBox.Show("未輸入ID或COM");
                timer1.Enabled = false;
            }
            else
            {
                timer1.Enabled = true;
            }
        }
    }
}
