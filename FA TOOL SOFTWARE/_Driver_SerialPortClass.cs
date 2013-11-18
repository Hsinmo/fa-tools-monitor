using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

//using SerialPort_URAT.Properties;
using System.Threading;
using System.IO;
using System.IO.Ports;

using System.Management;

namespace FA_TOOL_SOFTWARE
{
    //abstract class SerialPortClass
    class _Driver_SerialPortClass
    {

        public event EventHandler SerialPort_DataReceived_EventSendOut;

        private const string Find_FaTool_String = "Dynapack Virtual COM Port";
        private const string Find_Com_String = "COM";
        // The main control for communicating through the RS-232 port
        private SerialPort gSerialComPort = new SerialPort();
        private const int Default_BaudRate = 9600;
        private const int Default_DataBits = 8;
        private const StopBits Default_StopBits = StopBits.One;
        private const Parity Default_Parity = Parity.None;
        private string Setting_ComPort_Name = String.Empty;




        public _Driver_SerialPortClass()
        {
           
        }
        public bool is_ComPort_Open(){
            return gSerialComPort.IsOpen;
        }
        public bool OpenComPort()
        {
            bool error = false;
            if (Setting_ComPort_Name.StartsWith(Find_Com_String, System.StringComparison.OrdinalIgnoreCase))
            {
                gSerialComPort.BaudRate = Default_BaudRate;
                gSerialComPort.DataBits = Default_DataBits;
                gSerialComPort.StopBits = Default_StopBits;
                gSerialComPort.Parity = Default_Parity;
                gSerialComPort.PortName = Setting_ComPort_Name;
                gSerialComPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                try
                {
                    // Open the port
                    gSerialComPort.Open();
                }
                catch (UnauthorizedAccessException) { error = true; }
                catch (IOException) { error = true; }
                catch (ArgumentException) { error = true; }
            }

            if (error)
            {
                MessageBox.Show("Could not open the COM port.  Most likely it is already in use, has been removed, or is unavailable.", "COM Port Unavalible", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            else
            {
                return true;
                // Show the initial pin states
                //UpdatePinState();
                //chkDTR.Checked = comport.DtrEnable;
                //chkRTS.Checked = comport.RtsEnable;
            }

        }

        public bool FindingFAToolComPort()
        {
            //string[] cmbPortName = UpdatedComPortAndDescription_WholeName();
            string str = "";
            string cmbPortStr = "";
            bool foundFaComPort = false;

            string[] comport = SerialPort.GetPortNames();

        //Using searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM26%'")
            //For Each queryObj As ManagementObject In searcher.Get()
                //oList.Add(CStr(queryObj("Caption")))
            //Next
        //End Using

            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE Caption like '%Dynapack Virtual COM Port%'");
                    //new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity");// WHERE ConfigManagerErrorCode = 0");
                    //new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM WIN32_SerialPort");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    //inforForm.infor_textBox.AppendText(queryObj["DeviceID"] + Environment.NewLine);
                    //inforForm.infor_textBox.AppendText(queryObj["Caption"] + Environment.NewLine);
                    //inforForm.infor_textBox.AppendText(queryObj.ToString() + Environment.NewLine);
                    
                    str = queryObj["Caption"].ToString();
                    if (str.Contains(Find_FaTool_String))
                    {
                        for (int i = 0; i < comport.GetLength(0); i++)
                        {
                            if (str.Contains(comport[i]))
                            {
                                cmbPortStr = comport[i];
                                foundFaComPort = true;
                                break;
                            }
                        }
                        break;
                    }//if

                }//foreach (ManagementObject queryObj in searcher.Get())
            }
            //catch (ManagementException exx)
            catch (Exception ex)
            {
                //MessageBox.Show("Exception fetching processor id: " + ex.ToString()); // doesn't show
            }

            if (foundFaComPort)
            {
                Setting_ComPort_Name = cmbPortStr;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool FindingFAToolComPort_org()
        {
            string[] cmbPortName = UpdatedComPortAndDescription_WholeName();
            string str = "";

            bool FaTool_found = false;
            string FaTool_ComName = "";
            for (int i = 0; i < cmbPortName.GetLength(0); i++)
            {
                str = cmbPortName[i];
                if (str.StartsWith(Find_Com_String, System.StringComparison.OrdinalIgnoreCase))
                {
                    int idx = str.IndexOf(Find_FaTool_String);
                    if (idx > 0)
                    {
                        FaTool_found = true;
                        //GetSelectedComPortName
                        FaTool_ComName = (string)str.Split(new char[] { ':' }).GetValue(0);
                        break;
                    }
                }
            }

            if (FaTool_found)
            {
                Setting_ComPort_Name = FaTool_ComName;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ComPortSendData(byte[] sendingData)
        {
            bool done_sending = false;
            bool error = false;
            if (gSerialComPort.IsOpen)
            {
                // Send the binary data out the port
                if (sendingData[0] == 0x3a && sendingData[1] == 0xa6 && sendingData[2] == 0xe1)
                {
                }
                else
                {
                    //Information inforForm = new Information("Deiver UART Send Data");
                    //inforForm.Show();
                    //inforForm.infor_textBox.AppendText(HM_Utilitys.ByteArrayToHexString(sendingData));
                }

                try
                {
                    gSerialComPort.Write(sendingData, 0, sendingData.Length);
                }
                catch (Exception) { error = true; }

                if (error)
                {
                    done_sending = false;
                    try
                    {
                        gSerialComPort.DiscardInBuffer();
                        gSerialComPort.DiscardOutBuffer(); 
                        gSerialComPort.Close();
                    }
                    catch (Exception) { error = true; }
                }
                else { done_sending = true; }
            }
            else
            {
                // Inform the user if the hex string was not properly formatted
                done_sending = false;
            }
            return done_sending;
        }
        public void CloseComPort()
        {
            bool error = false;
            try
            {
                if (gSerialComPort.IsOpen)
                {
                    gSerialComPort.DiscardInBuffer();
                    gSerialComPort.DiscardOutBuffer();
                    gSerialComPort.Close();
                }
            }
            catch (Exception) { error = true; }
        }

        private string[] UpdatedComPortAndDescription_WholeName()
        {
            string[] comport = SerialPort.GetPortNames();
            bool error = false;
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity");// WHERE ConfigManagerErrorCode = 0");
                //new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM WIN32_SerialPort");
                /*
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    //if (queryObj["Caption"].ToString().Contains("(COM"))
                    //if (queryObj["Caption"].ToString().Contains("COM"))
                    //if (queryObj["Caption"].ToString().StartsWith("serial port"))
                    if (queryObj["Caption"].ToString().Contains("serial port"))
                    {
                        //Console.WriteLine("serial port : {0}", queryObj["Caption"]);  //result : Description
                        //Console.WriteLine("serial port : {0}", queryObj["DeviceID"]); // result : COM20
                    }
                }
                */

                for (int i = 0; i < comport.GetLength(0); i++)
                {
                    string str1 = "";
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (queryObj["Caption"].ToString().Contains(comport[i]))
                        {
                            str1 += queryObj["Caption"].ToString();
                        }

                        /*
                        if (queryObj["DeviceID"].ToString().StartsWith(comport[i]))
                        {
                            str1 = queryObj["Caption"].ToString();
                            str1 = queryObj["Name"].ToString();
                        }*/
                    }//foreach (ManagementObject queryObj in searcher.Get())
                    comport[i] = comport[i] + ": " + str1;
                }
            }
            //catch (ManagementException exx)
            catch (Exception ex)
            {
                error = true;
                MessageBox.Show("Exception fetching processor id: " + ex.ToString()); // doesn't show
                //MessageBox.Show(ex.Message);
            }

            if (error)
            {
                return SerialPort.GetPortNames();
            }
            // Query for descending sort.
            IEnumerable<string> sortDescendingQuery =
                from dOrder in comport
                orderby dOrder //"ascending" is default
                //orderby dOrder descending
                select dOrder;

            return sortDescendingQuery.ToArray<string>();
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // If the com port has been closed, do nothing
            if (!gSerialComPort.IsOpen) return;


            // Obtain the number of bytes waiting in the port's buffer
            int readbytes = gSerialComPort.BytesToRead;
            if (readbytes > 0)
            {

                // Create a byte array buffer to hold the incoming data
                byte[] buffer = new byte[readbytes];
                // Read the data from the port and store it in our buffer
                gSerialComPort.Read(buffer, 0, readbytes);

                //form1.SetText("Deiver UART received Data" + Environment.NewLine);
                //form1.SetText(HM_Utilitys.ByteArrayToHexString(buffer) + Environment.NewLine);
                //if ((buffer[2] != 0x93) || (buffer[2] != 0x92))
                {
                    //Information inforForm = new Information("Deiver UART received Data");
                    //inforForm.Show();
                    //inforForm.infor_textBox.AppendText("Length : " + readbytes + Environment.NewLine);
                    //inforForm.infor_textBox.AppendText(HM_Utilitys.ByteArrayToHexString(buffer));
                }

                // If anyone has subscribed, notify them
                SerialPort_EventArgsClass_DataReceived event_arg = new SerialPort_EventArgsClass_DataReceived(buffer);
                OnSerialPort_DataReceived_EventSendOut(event_arg);
            }
        }//private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        // The method which fires the Event 
        private void OnSerialPort_DataReceived_EventSendOut(SerialPort_EventArgsClass_DataReceived e)
        {
            if (SerialPort_DataReceived_EventSendOut != null)
                SerialPort_DataReceived_EventSendOut(this, e);    // 觸發SerialPort_DataReceived_EventSendOut 事件
        }
    }//class SerialPortClass


    // The class to hold the information about the event
    public class SerialPort_EventArgsClass_DataReceived : EventArgs
    {
        public byte[] ReceivedBuffer;
        public SerialPort_EventArgsClass_DataReceived(byte[] received_buffer)
        {
            ReceivedBuffer = (byte[])received_buffer.Clone();
        }
    }//class SerialPortClass_DataReceivedEventArgs : EventArgs
}//namespace FA_TOOL_SOFTWARE
