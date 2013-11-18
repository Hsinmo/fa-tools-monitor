using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Runtime.InteropServices;

namespace FA_TOOL_SOFTWARE
{
    class LM_control_for_user
    {
        
        [DllImport("POCBCommand_B6.dll")]
        public static extern double POCBSet1(double ID, double COM, double Action, double V, double I, double P, double OV, double UV, double OC, double UC, double OT);
        [DllImport("POCBCommand_B6.dll")]
        public static extern double POCBStart(double ID, double COM);
        [DllImport("POCBCommand_B6.dll")]
        public static extern double POCBStatusInquiry(double ID, double COM, ref double V, ref double I, ref double T);
        [DllImport("POCBCommand_B6.dll")]
        public static extern double POCBStop(double ID, double COM);

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

        //Read learning machine status(V, I, T)
        public Double Readstatus(Double ID, Double COM,ref Double V,ref Double I,ref Double T)
        {
            Double RT;
            RT = POCBStatusInquiry(ID, COM, ref V, ref I, ref T);
            return RT;
        }

        //Setting learngin machine parameter (Charge, discharge, Voltage, current, protection) 
        public Double learning_machine_setting(Double ID, Double COM, Double Action, Double V, Double I, Double P,
            Double OV, Double UV, Double OC, Double UC, Double OT)
        {
            Double RT;
            RT = POCBSet1(ID, COM, Action, V, I, P, OV, UV, OC, UC, OT);
            return RT;
        }

        //Learning machine start
        public Double learning_machine_start(Double ID, Double COM)
        {
            Double RT;
            RT = POCBStart(ID, COM);
            return RT;
        }

        //Learning machine stop
        public Double learning_machine_stop(Double ID, Double COM)
        {
            Double RT;
            RT = POCBStop(ID, COM);
            return RT;
        }

    }
}
