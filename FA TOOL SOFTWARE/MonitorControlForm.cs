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
    public enum LogMsgType { Incoming, Outgoing, Normal, Warning, Error, Test };

    public partial class MonitorControlForm : Form
    {
        // Various colors for logging info
        private Color[] LogMsgTypeColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red, Color.Pink };

        private System.Windows.Forms.Timer Scaning_Fa_DeviceTimer = new System.Windows.Forms.Timer();

        // The timer unit = 1ms;
        // Sets the timer interval : 1 seconds = 1000.
        private const int Scaning_Fa_DeviceTimer_Interval_Time = 2000;  // 3000ms = 3 sec.


        _Driver_SerialPortClass CommunicationPort;
        private bool IsConnectionToFaDevice = false;

        public MonitorControlForm()
        {
            InitializeComponent();
            FA_DisConnection();


            CommunicationPort = new _Driver_SerialPortClass();

            CommunicationPort.SerialPort_DataReceived_EventSendOut += new EventHandler(Communication_ReceivedDataFunction);   // 訂閱(subscribe) SerialPort_DataReceived_EventSendOut 事件 

            IsConnectionToFaDevice = false;
            Scaning_Fa_DeviceTimer.Interval = Scaning_Fa_DeviceTimer_Interval_Time;
            Scaning_Fa_DeviceTimer.Tick += new EventHandler(Scaning_Fa_DeviceTimerEventProcessor);
            Scaning_Fa_DeviceTimer.Start();

            DriverInstallClass dic = new DriverInstallClass();
            dic.copysysFile();
        }
        private bool DetectingDisconnection()
        {
            byte[] TransmitData = USB_CDC_Packet_Forming_and_Decoding.CMD_Forming_For_Transmitting((byte)USB_CDC_Cmd.Cmd_For_Connect_Detection, new byte[0]);

            if (!CommunicationPort.ComPortSendData(TransmitData))
            {
                //fail to send
                return false;
            }
            else
            {
                //success to send
                return true;
            }
        }
        private void FA_Connection()
        {
            FA_TOOLS_Connect_Status_Lable.Text = "Connected";
            FA_TOOLS_Connect_Status_Lable.BackColor = Color.LightGreen;
        }
        private void FA_DisConnection()
        {
            FA_TOOLS_Connect_Status_Lable.Text = "DisConnected";
            FA_TOOLS_Connect_Status_Lable.BackColor = Color.Pink;
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
        /// <summary>
        /// Timer Scanning Process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scaning_Fa_DeviceTimerEventProcessor(Object sender, EventArgs e)
        {
            if (IsConnectionToFaDevice)
            {
                //being connection
                if (!DetectingDisconnection())
                {
                    IsConnectionToFaDevice = false;
                    FA_DisConnection();
                }

            }
            else
            {
                //being disconnection
                if (CommunicationPort.FindingFAToolComPort())
                {
                    if (CommunicationPort.OpenComPort())
                    {
                        IsConnectionToFaDevice = true;
                        FA_Connection();
                    }
                }
            }
        }//private void Scaning_Fa_DeviceTimerEventProcessor


        //// The method that implements the delegated functionality  
        //事件處理方法
        private void Communication_ReceivedDataFunction(Object sender, EventArgs e)
        {
            // 判斷物件是否為 SerialPort_EventArgsClass_DataReceived 實體
            if (e is SerialPort_EventArgsClass_DataReceived)
            {
                // 將物件由 EventArgs 轉型 SerialPortClass_DataReceivedEventArgs
                SerialPort_EventArgsClass_DataReceived DataReceivedClass = e as SerialPort_EventArgsClass_DataReceived;
                //SetText(HM_Utilitys.ByteArrayToHexString(DataReceivedClass.ReceivedBuffer) + Environment.NewLine);
                Log(LogMsgType.Incoming, "RX Pack: " + HM_Utilitys.ByteArrayToHexString(DataReceivedClass.ReceivedBuffer) + Environment.NewLine);

                byte ReceivedCommand;
                byte[] decoding_Parameter;
                bool is_found = USB_CDC_Packet_Forming_and_Decoding.CMD_Decoding_For_Receiving(DataReceivedClass.ReceivedBuffer, out ReceivedCommand, out decoding_Parameter);
                if (is_found)
                {
                    Log(LogMsgType.Incoming, "rx cmd: " + HM_Utilitys.ByteToHexString(ReceivedCommand) + Environment.NewLine);
                    Log(LogMsgType.Incoming, "rx data: " + HM_Utilitys.ByteArrayToHexString(decoding_Parameter) + Environment.NewLine);

                    //Receiving_Data_UnPAcking_By_USBPacket(ReceivedCommand, decoding_Parameter);
                    //form1.SetText("L1 receivind data 2" + Environment.NewLine);
                    //form1.SetText(HM_Utilitys.ByteArrayToHexString(decoding_Parameter) + Environment.NewLine);
                    //USB_CDC_DataReceived_EventSendOut receivedEvent = new USB_CDC_DataReceived_EventSendOut(ReceivedCommand, decoding_Parameter);
                    //OnUSB_CDC_DataReceived_EventSendOut(receivedEvent);
                }
                else
                {
                    Log(LogMsgType.Error, "received pack error. " + Environment.NewLine);
                }
            }
        }

        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetTextCallback(string text);
        public void SetText(string text)
        {
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

        private delegate void myUICallBack(string myStr, Control ctl);
        private void myUI(string myStr, Control ctl)
        {
            if (this.InvokeRequired)
            {
                myUICallBack myUpdate = new myUICallBack(myUI);
                this.Invoke(myUpdate, myStr, ctl);
            }
            else
            {
                ctl.Text = myStr;
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

            CommunicationPort.ComPortSendData(TransmitData);
        }

        private void SendDataToUart_button_Click(object sender, EventArgs e)
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
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = pre_data[i + 1];
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

            byte[] TransmitViaUart_As_Usb_Parameter = (byte[])levPacket.Clone();
            Send_USB_TO_FATOOL_CMD_Data((byte)USB_CDC_Cmd.Cmd_UART_RS485_Transmit_Data, TransmitViaUart_As_Usb_Parameter);
        }
    }//public partial class MonitorControlForm : Form
}
