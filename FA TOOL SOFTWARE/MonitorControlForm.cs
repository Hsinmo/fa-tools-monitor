using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FA_TOOL_SOFTWARE.driver;


namespace FA_TOOL_SOFTWARE
{
    public enum LogMsgType { Incoming, Outgoing, Normal, Warning, Error, Test };

    public partial class MonitorControlForm : Form
    {
        // Various colors for logging info
        private Color[] LogMsgTypeColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red, Color.Pink };

        private System.Windows.Forms.Timer Scaning_Fa_DeviceTimer = new System.Windows.Forms.Timer();

        // The timer unit = 1ms;
        // Sets the timer interval : 1 seconds = 1000.
        private const int Scaning_Fa_DeviceTimer_Interval_Time = 2000;  // 3000ms = 3 sec.


        //_Driver_SerialPortClass CommunicationPort;
        private bool IsConnectionToFaDevice = false;

        FAToolSerialPortConnection FaConnect;
        public MonitorControlForm()
        {
            InitializeComponent();

            FaConnect = new FAToolSerialPortConnection();
            FaConnect.SerialPortMessage_EventHandler += new EventHandler(FaReceivedMessageChange);   // 訂閱(subscribe) 事件 

            if (FaConnect.Connection == ConnectingStatusTypes.Connected)
            {
                FA_Connection();
            }
            else
            {
                FA_DisConnection();
            }


            DriverInstallClass dic = new DriverInstallClass();
            dic.copysysFile();
        }



        //private bool DetectingDisconnection()
        //{
        //    byte[] TransmitData = USB_CDC_Packet_Forming_and_Decoding.CMD_Forming_For_Transmitting((byte)USB_CDC_Cmd.Cmd_For_Connect_Detection, new byte[0]);

        //    if (!CommunicationPort.ComPortSendData(TransmitData))
        //    {
        //        //fail to send
        //        return false;
        //    }
        //    else
        //    {
        //        //success to send
        //        return true;
        //    }
        //}
        private void FA_Connection()
        {
            UpdateMyUI("Connected", FA_TOOLS_Connect_Status_Lable, Color.LightGreen);
            IsConnectionToFaDevice = true;
            //FA_TOOLS_Connect_Status_Lable.Text = "Connected";
            //FA_TOOLS_Connect_Status_Lable.BackColor = Color.LightGreen;
        }
        private void FA_DisConnection()
        {
            UpdateMyUI("DisConnected", FA_TOOLS_Connect_Status_Lable, Color.Pink);
            IsConnectionToFaDevice = false;
            //FA_TOOLS_Connect_Status_Lable.Text = "DisConnected";
            //FA_TOOLS_Connect_Status_Lable.BackColor = Color.Pink;
        }

        //// The method that implements the delegated functionality  
        //事件處理方法
        private void FaReceivedMessageChange(Object sender, EventArgs e)
        {
            // 判斷物件是否為 ComPortStatusUpdate_EventArgs 實體
            if (e is SerialPortMessage_EventArgs)
            {
                // 將物件由 EventArgs 轉型 ComPortStatusUpdate_EventArgs
                SerialPortMessage_EventArgs msg = e as SerialPortMessage_EventArgs;
                Log(LogMsgType.Incoming, "[FaReceivedMessage] Msg Type: " + msg.MsgType + ", ConectedStatus: " + msg.ConectedStatus + Environment.NewLine);
                Log(LogMsgType.Incoming, "Com port information : " + msg.ComPortInformation + Environment.NewLine);

                if (msg.MsgType == MessageTypes.ConnectingStatus)
                {
                    if (msg.ConectedStatus == ConnectingStatusTypes.Connected)
                    {
                        FA_Connection();
                    }
                    else
                    {
                        FA_DisConnection();
                    }
                }


                if (msg.MsgType == MessageTypes.ReceiveData)
                {
                    Log(LogMsgType.Incoming, "RX Packet: " + HM_Utilitys.ByteArrayToHexString(msg.ReceivedBuffer) + Environment.NewLine);
                    byte ReceivedCommand;
                    byte[] decoding_Parameter;
                    bool is_found = USB_CDC_Packet_Forming_and_Decoding.CMD_Decoding_For_Receiving(msg.ReceivedBuffer, out ReceivedCommand, out decoding_Parameter);
                    if (is_found)
                    {
                        Log(LogMsgType.Incoming, "rx cmd: " + HM_Utilitys.ByteToHexString(ReceivedCommand) + Environment.NewLine);
                        Log(LogMsgType.Incoming, "rx data: " + HM_Utilitys.ByteArrayToHexString(decoding_Parameter) + Environment.NewLine);
                    }
                    else
                    {
                        Log(LogMsgType.Error, "received packet error. " + Environment.NewLine);
                    }
                }
            }
            else
            {
                Log(LogMsgType.Incoming, "[FaReceivedMessage] Unknow Message ....." + Environment.NewLine);
            }
        }

        /// <summary> Log data to the terminal window. </summary>
        /// <param name="msgtype"> The type of message to be written. </param>
        /// <param name="msg"> The string containing the message to be shown. </param>
        private void Log_Clear()
        {
            Infor_richTextBox.Invoke(new EventHandler(delegate
            {
                Infor_richTextBox.Clear();
            }));
        }
        private void Log(LogMsgType msgtype, string msg)
        {
            Infor_richTextBox.Invoke(new EventHandler(delegate
            {
                Infor_richTextBox.SelectedText = string.Empty;
                Infor_richTextBox.SelectionFont = new Font(Infor_richTextBox.SelectionFont, FontStyle.Bold);
                Infor_richTextBox.SelectionColor = LogMsgTypeColor[(int)msgtype];
                Infor_richTextBox.AppendText(msg);
                Infor_richTextBox.ScrollToCaret();
            }));
        }



        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetTextCallback(string text);
        public void SetText(string text)
        {
            if (Infor_richTextBox.IsDisposed)
            {
                return;
            }
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.Infor_richTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                //this.Infor_richTextBox.AppendText(text);
                Log(LogMsgType.Normal, text);
            }
        }

        private delegate void myUICallBack(string myStr, Control ctl, Color back_clor);
        private void UpdateMyUI(string myStr, Control ctl, Color back_clor)
        {
            if (ctl.IsDisposed)
            {
                return;
            }
            if (this.InvokeRequired)
            {
                myUICallBack myUpdate = new myUICallBack(UpdateMyUI);
                this.Invoke(myUpdate, myStr, ctl, back_clor);
            }
            else
            {
                ctl.Text = myStr;
                ctl.BackColor = back_clor;
            }
        }
        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetFunctionStatusCallback(bool flag);
        public void SetFunctionStatus(bool flag)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                SetFunctionStatusCallback statusUpdate = new SetFunctionStatusCallback(SetFunctionStatus);
                this.Invoke(statusUpdate, flag);//, ctl);
            }
            else
            {
                //show_communication_status(flag);
            }
        }
        private void SendData_ForFATool_button_Click(object sender, EventArgs e)
        {
            if (SendData_ForFATool_textBox.Text == string.Empty)
            {
                return;
            }
            try
            {
                byte[] pre_data = HM_Utilitys.HexStringToByteArray(SendData_ForFATool_textBox.Text);
                Log(LogMsgType.Normal, HM_Utilitys.ByteArrayToHexString(pre_data) + Environment.NewLine);
                byte cmd = pre_data[0];
                byte[] data = new byte[pre_data.Length - 1];
                for(int i =0; i< data.Length; i++){
                    data[i] = pre_data[i+1];
                }
                Send_USB_TO_FATOOL_CMD_Data(cmd, data);
            }
            //catch (FormatException)
            catch (Exception ex)
            {
                // Inform the user if the hex string was not properly formatted
                //Log(LogMsgType.Error, "Not properly formatted hex string: " + sendData_textBox.Text + "\n");
                Log(LogMsgType.Error, "Error: " + ex.ToString() + Environment.NewLine);
            }
        }

        private void Clear_Message_button_Click(object sender, EventArgs e)
        {
            Log_Clear();
        }

        private void Send_USB_TO_FATOOL_CMD_Data(byte Cmd, byte[] data)
        {
            if (!IsConnectionToFaDevice)
            {
                Log(LogMsgType.Error, "[Error] Could not send while Fa-Tool Device have not found." + Environment.NewLine);
                return;
            }
            byte[] TransmitData = USB_CDC_Packet_Forming_and_Decoding.CMD_Forming_For_Transmitting(Cmd, data);
            Log(LogMsgType.Outgoing, "Transmitting: " + HM_Utilitys.ByteArrayToHexString(TransmitData) + Environment.NewLine);
            FaConnect.ComPortSendData(TransmitData);
        }

        private void SendDataToUart_button_Click(object sender, EventArgs e)
        {

            if (SendDataToUart_textBox.Text == string.Empty)
            {
                return;
            }
            try
            {
                byte[] pre_data = HM_Utilitys.HexStringToByteArray(SendDataToUart_textBox.Text);
                Log(LogMsgType.Normal, HM_Utilitys.ByteArrayToHexString(pre_data) + Environment.NewLine);
                byte cmd = pre_data[0];
                byte[] data = new byte[pre_data[1]];
                int index = 0;
                for (int i = 2; i < pre_data.Length; i++)
                {
                    data[index++] = pre_data[i];
                }
                Send_LEV_CMD_Data(cmd, data);
                //Send_USB_TO_FATOOL_CMD_Data(cmd, data);
            }
            //catch (FormatException)
            catch (Exception ex)
            {
                // Inform the user if the hex string was not properly formatted
                //Log(LogMsgType.Error, "Not properly formatted hex string: " + sendData_textBox.Text + "\n");
                Log(LogMsgType.Error, "Error: " + ex.ToString() + Environment.NewLine);
            }

        }
        private void Send_LEV_CMD_Data(byte Cmd, byte[] data)
        {
            if (!IsConnectionToFaDevice)
            {
                return;
            }


            byte[] levPacket = LEV_UART_Packet_Forming_and_Decoding.CMD_Forming_For_Transmitting(Cmd, data);
            Log(LogMsgType.Outgoing, "TransmittinglevPacket: " + HM_Utilitys.ByteArrayToHexString(levPacket) + Environment.NewLine);
            byte[] TransmitViaUart_As_Usb_Parameter = (byte[])levPacket.Clone();
            Send_USB_TO_FATOOL_CMD_Data((byte)USB_CDC_Cmd.Cmd_UART_RS485_Transmit_Data, TransmitViaUart_As_Usb_Parameter);
        }



        private void MonitorControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FaConnect != null)
            {
                FaConnect.Dispose();
            }
        }
    }//public partial class MonitorControlForm : Form
}
