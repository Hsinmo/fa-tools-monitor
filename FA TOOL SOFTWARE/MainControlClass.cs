using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace FA_TOOL_SOFTWARE
{
    partial class Form1
    {
        private System.Windows.Forms.Timer ScaningDeviceTimer = new System.Windows.Forms.Timer();

        // The timer unit = 1ms;
        // Sets the timer interval : 1 seconds = 1000.
        private const int ScaningDeviceTimer_Interval_Time = 2000;  // 3000ms = 3 sec.
        
        
        _Driver_SerialPortClass CommunicationPort;
        private bool IsConnectionToFaDevice = false;

        private void ForForm1MainControlLoad()
        {
            CommunicationPort = new _Driver_SerialPortClass();

            CommunicationPort.SerialPort_DataReceived_EventSendOut += new EventHandler(Communication_ReceivedDataFunction);   // 訂閱(subscribe) SerialPort_DataReceived_EventSendOut 事件 

            IsConnectionToFaDevice = false;
            ScaningDeviceTimer.Interval = ScaningDeviceTimer_Interval_Time;
            ScaningDeviceTimer.Tick += new EventHandler(ScaningDeviceTimerEventProcessor);
            ScaningDeviceTimer.Start();

            ReceivedDataParsingLoad();
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

        private void Send_USB_TO_FATOOL_CMD_Data(byte Cmd, byte[] data)
        {
            if (!IsConnectionToFaDevice)
            {
                return;
            }

            byte[] TransmitData = USB_CDC_Packet_Forming_and_Decoding.CMD_Forming_For_Transmitting(Cmd, data);

            CommunicationPort.ComPortSendData(TransmitData);
        }
        public void Set_CMD_For_Get_LEV_Whole_Data()
        {

            byte getLEVwholeDataCmd = 0xb2;
            byte[] getLEVwholeDataCmd_Parameter = new byte[] { 0x00 };

            Send_LEV_CMD_Data(getLEVwholeDataCmd, getLEVwholeDataCmd_Parameter);
        }

        private void Connection()
        {
            show_communication_status(true);
        }
        private void DisConnection()
        {
            show_communication_status(false);
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
        //// The method that implements the delegated functionality  
        //事件處理方法
        private void Communication_ReceivedDataFunction(Object sender, EventArgs e)
        {
            // 判斷物件是否為 SerialPort_EventArgsClass_DataReceived 實體
            if (e is SerialPort_EventArgsClass_DataReceived)
            {
                // 將物件由 EventArgs 轉型 SerialPortClass_DataReceivedEventArgs
                SerialPort_EventArgsClass_DataReceived DataReceivedClass = e as SerialPort_EventArgsClass_DataReceived;


                //form1.SetText("L1 receivind data 1" + Environment.NewLine);
                //form1.SetText(HM_Utilitys.ByteArrayToHexString(DataReceivedClass.ReceivedBuffer) + Environment.NewLine);

                byte ReceivedCommand;
                byte[] decoding_Parameter;
                bool is_found = USB_CDC_Packet_Forming_and_Decoding.CMD_Decoding_For_Receiving(DataReceivedClass.ReceivedBuffer, out ReceivedCommand, out decoding_Parameter);
                if (is_found)
                {
                    Receiving_Data_UnPAcking_By_USBPacket(ReceivedCommand, decoding_Parameter);
                    //form1.SetText("L1 receivind data 2" + Environment.NewLine);
                    //form1.SetText(HM_Utilitys.ByteArrayToHexString(decoding_Parameter) + Environment.NewLine);
                    //USB_CDC_DataReceived_EventSendOut receivedEvent = new USB_CDC_DataReceived_EventSendOut(ReceivedCommand, decoding_Parameter);
                    //OnUSB_CDC_DataReceived_EventSendOut(receivedEvent);
                }
            }
        }
        /// <summary>
        /// Timer Scanning Process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScaningDeviceTimerEventProcessor(Object sender, EventArgs e)
        {
            if (IsConnectionToFaDevice)
            {
                //being connection
                if (!DetectingDisconnection())
                {
                    IsConnectionToFaDevice = false;
                    DisConnection();
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
                        Connection();
                    }
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
            if (this.infor_textBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.infor_textBox.AppendText(text);
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
    }//partial class Form1











/*
    class Form1_org
    {
        _L1_USB_CDC_Driver_Control USB_CDC_Driver_Control;

        private bool IsConnectionToFaDevice = false;

        // This BackgroundWorker is used to demonstrate the 
        // preferred way of performing asynchronous operations.\

        

        private void ForForm1MainControlLoad()
        {

            USB_CDC_Driver_Control = new _L1_USB_CDC_Driver_Control();

            IsConnectionToFaDevice = false;
            USB_CDC_Driver_Control.DetectingConnection_EventSendOut += new EventHandler(Receiving_ConnectionStatus_ByEvent);   // 訂閱(subscribe) DetectingConnection_EventSendOut 事件 
            USB_CDC_Driver_Control.USB_CDC_DataReceived_EventSendOut += new EventHandler(Receiving_Data_ByEvent);   // 訂閱(subscribe) USB_CDC_DataReceived_EventSendOut 事件 
            USB_CDC_Driver_Control.EnableScan_USB_CDC();

            
        }
        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetTextCallback(string text);
        public void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.infor_textBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.infor_textBox.AppendText( text);
            }
        }

        List<byte> receivedUartDataList = new List<byte>();
        //// The method that implements the delegated functionality  
        //事件處理方法
        private void Receiving_Data_ByEvent(Object sender, EventArgs e)
        {
            //SetText("Receiving_Data_ByEvent at Main Control : " + Environment.NewLine);
            // 判斷物件是否為 DetectingConnection_EventSendOut 實體
            if (e is USB_CDC_DataReceived_EventSendOut)
            {
                //SetText("Receiving_Data_ByEvent at Main Control : if (e is USB_CDC_DataReceived_EventSendOut)" + Environment.NewLine);
                // 將物件由 EventArgs 轉型 SerialPortClass_DataReceivedEventArgs
                USB_CDC_DataReceived_EventSendOut ReceivedClass = e as USB_CDC_DataReceived_EventSendOut;

                byte Usb_Cmd = ReceivedClass.USB_Cmd;
                byte[] Usb_Data = (byte[])ReceivedClass.USB_ReceivedData.Clone();
                SetText("Receiving_Data_ByEvent CMD : " + Usb_Cmd + Environment.NewLine);

                //LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group group = LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group.None;
                List<LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group> outDataGroupList;
                List<byte[]> outDataList;

                if(Usb_Cmd == (byte)USB_CDC_Cmd.Cmd_UART_RS485_Receive_Data){
                    receivedUartDataList.AddRange(Usb_Data);
                    SetText("Cmd_UART_RS485_Receive_Data byte: " + HM_Utilitys.ByteArrayToHexString(Usb_Data) + Environment.NewLine);
                    SetText("Cmd_UART_RS485_Receive_Data List : " + HM_Utilitys.ByteArrayToHexString(receivedUartDataList.ToArray<byte>()) + Environment.NewLine);
                    LEV_One_Wire_Receiving_Packet_Decoding.Packet_Decoding_To_Group_And_Clear_List(ref receivedUartDataList, out outDataGroupList, out outDataList);

                    if ((outDataGroupList.Count >= 1) && (outDataGroupList.Count == outDataList.Count))
                    {
                        for (int i = 0; i < outDataGroupList.Count; i++)
                        {
                            if (outDataGroupList[i] == LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group.OneWire_SystemData_Group)
                            {
                                SetText("OneWire_SystemData_Group : " + Environment.NewLine);
                                SetText("Group data : " + HM_Utilitys.ByteArrayToHexString(outDataList[i]) + Environment.NewLine);
                            }
                            else if (outDataGroupList[i] == LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group.OneWire_EEPROM_Group)
                            {
                                SetText("OneWire_EEPROM_Group : " + Environment.NewLine);
                                SetText("Group data : " + HM_Utilitys.ByteArrayToHexString(outDataList[i]) + Environment.NewLine);
                            }
                            else
                            {
                                SetText("[Fail] None OneWire_Data : " + outDataGroupList[i] + Environment.NewLine);
                                SetText("Group data : " + HM_Utilitys.ByteArrayToHexString(outDataList[i]) + Environment.NewLine);
                            }
                        }
                    }//if (outDataGroupList.Count >= 1)
                    else
                    {
                        SetText("[Fail] OneWire_Data List Count Fail.  " + Environment.NewLine);
                        LEV_One_Wire_Receiving_Packet_Decoding.Packet_Decoding_To_Group_And_Clear_List(ref receivedUartDataList, out outDataGroupList, out outDataList);
                    }
                }//if(Usb_Cmd == (byte)USB_CDC_Cmd.Cmd_UART_RS485_Receive_Data){

            }//if (e is USB_CDC_DataReceived_EventSendOut)
        }
        //// The method that implements the delegated functionality  
        //事件處理方法
        private void Receiving_ConnectionStatus_ByEvent(Object sender, EventArgs e)
        {
            // 判斷物件是否為 DetectingConnection_EventSendOut 實體
            if (e is DetectingConnection_EventSendOut)
            {
                // 將物件由 EventArgs 轉型 SerialPortClass_DataReceivedEventArgs
                DetectingConnection_EventSendOut ConnectionStatusClass = e as DetectingConnection_EventSendOut;
                if (ConnectionStatusClass.IsConnection)
                {
                    //connection
                    //get_communication_status(true);
                    IsConnectionToFaDevice = true;
                    //this.infor_textBox.AppendText("Connection Status : " + IsConnectionToFaDevice + Environment.NewLine);
                    MessageBox.Show("Connection Status : " + ConnectionStatusClass.IsConnection);
                }
                else
                {
                    //disconnection
                    //get_communication_status(false);
                    IsConnectionToFaDevice = false;
                    //this.infor_textBox.AppendText("Connection Status : " + IsConnectionToFaDevice + Environment.NewLine);
                    MessageBox.Show("Connection Status : " + ConnectionStatusClass.IsConnection);
                }
            }
        }

        public void Set_CMD_For_Get_LEV_Whole_Data()
        {
            if (!IsConnectionToFaDevice)
            {
                return;
            }
            byte getLEVwholeDataCmd = 0xb2;
            byte[] getLEVwholeDataCmd_Parameter = new byte[] {0x00};

            byte[] levPacket = LEV_UART_Packet_Forming_and_Decoding.CMD_Forming_For_Transmitting(getLEVwholeDataCmd, getLEVwholeDataCmd_Parameter);

            byte[] TransmitViaUartByUsbCmd_Parameter = (byte[])levPacket.Clone();
            USB_CDC_Driver_Control.USB_CDC_SendData_With_Packet(USB_CDC_Cmd.Cmd_UART_RS485_Transmit_Data, TransmitViaUartByUsbCmd_Parameter);
        }


    }//class MainControlClass
 * 
 */ 
}//namespace FA_TOOL_SOFTWARE
