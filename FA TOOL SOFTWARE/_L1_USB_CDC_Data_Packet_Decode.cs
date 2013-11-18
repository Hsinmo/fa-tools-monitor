using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FA_TOOL_SOFTWARE
{




    class _L1_USB_CDC_Driver_Control
    {




        public event EventHandler DetectingConnection_EventSendOut;
        public event EventHandler USB_CDC_DataReceived_EventSendOut;
       
        static System.Windows.Forms.Timer ScaningDeviceTimer = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer DetectingDeviceOffLineTimer = new System.Windows.Forms.Timer();
        //private bool DetectingDeviceOffLineTimer_Being_Stop = false;

        // The timer unit = 1ms;
        // Sets the timer interval : 1 seconds = 1000.
        private const int ScaningDeviceTimer_Interval_Time = 3000;  // 3000ms = 3 sec.
        private const int DetectingOffLineTimer_Interval_Time = 2000;  // 2000ms

        _Driver_SerialPortClass CommunicationPort;

        public bool IsCommunicationPortOpen = false;

        Form1 form1;
        public _L1_USB_CDC_Driver_Control(Form1 f1)
        {
            form1 = f1;
            _L1_USB_CDC_Driver_Control_For_new();
        }
        public _L1_USB_CDC_Driver_Control()
        {
            _L1_USB_CDC_Driver_Control_For_new();
        }
        private void _L1_USB_CDC_Driver_Control_For_new()
        {
            IsCommunicationPortOpen = false;
            ScaningDeviceTimer.Interval = ScaningDeviceTimer_Interval_Time;
            DetectingDeviceOffLineTimer.Interval = DetectingOffLineTimer_Interval_Time;
            ScaningDeviceTimer.Tick += new EventHandler(ScaningDeviceTimerEventProcessor);
            DetectingDeviceOffLineTimer.Tick += new EventHandler(DetectingDeviceOffLineTimerEventProcessor);

            //CommunicationPort = new _Driver_SerialPortClass(form1);
            CommunicationPort.SerialPort_DataReceived_EventSendOut += new EventHandler(Communication_ReceivedDataFunction);   // 訂閱(subscribe) SerialPort_DataReceived_EventSendOut 事件 



        }

        public void EnableScan_USB_CDC()
        {
            ScaningDeviceTimer.Start();
        }
        private void ConnectingAction()
        {
            ScaningDeviceTimer.Stop();
            IsCommunicationPortOpen = true;
            // If anyone has subscribed, notify them
            DetectingConnection_EventSendOut event_arg = new DetectingConnection_EventSendOut(IsCommunicationPortOpen);
            OnConnectionStatus_EventSendOut(event_arg);
            DetectingDeviceOffLineTimer.Start();
        }
        private void DisconnectingAction()
        {
            DetectingDeviceOffLineTimer.Stop();

            IsCommunicationPortOpen = false;
            // If anyone has subscribed, notify them
            DetectingConnection_EventSendOut event_arg = new DetectingConnection_EventSendOut(IsCommunicationPortOpen);
            OnConnectionStatus_EventSendOut(event_arg);

            ScaningDeviceTimer.Start();
        }
        private void ScaningDeviceTimerEventProcessor(Object sender, EventArgs e)
        {
            if (CommunicationPort.FindingFAToolComPort())
            {
                if (CommunicationPort.OpenComPort())
                {
                    
                    ConnectingAction();
                }
                else
                {
                    IsCommunicationPortOpen = false;
                }
            }
        }
        private void DetectingDeviceOffLineTimerEventProcessor(Object sender, EventArgs e)
        {

            if (!USB_CDC_SendData_With_Packet(USB_CDC_Cmd.Cmd_For_Connect_Detection, new byte[0]))
            {
                DisconnectingAction();
                //DetectingDeviceOffLineTimer_Being_Stop = true;  //for the next calling to stop, set as delay
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
                //form1.SetText("L1 receivind data 2" + Environment.NewLine);
                //form1.SetText(HM_Utilitys.ByteArrayToHexString(decoding_Parameter) + Environment.NewLine);
                    USB_CDC_DataReceived_EventSendOut receivedEvent = new USB_CDC_DataReceived_EventSendOut(ReceivedCommand, decoding_Parameter);
                    OnUSB_CDC_DataReceived_EventSendOut(receivedEvent);
                }
            }
        }

        public bool USB_CDC_SendData_With_Packet(USB_CDC_Cmd cmd, byte[] sendingData)
        {
            byte[] TransmitData = USB_CDC_Packet_Forming_and_Decoding.CMD_Forming_For_Transmitting((byte)cmd, sendingData);



            if (cmd != USB_CDC_Cmd.Cmd_For_Connect_Detection)
            {
                //Information inforForm = new Information("L1 USB Sending Data");
                //inforForm.Show();
                //inforForm.infor_textBox.AppendText(HM_Utilitys.ByteArrayToHexString(TransmitData));
            }          

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


        // The method which fires the Event 
        private void OnUSB_CDC_DataReceived_EventSendOut(USB_CDC_DataReceived_EventSendOut e)
        {
            if (USB_CDC_DataReceived_EventSendOut != null)
                USB_CDC_DataReceived_EventSendOut(this, e);    // 觸發USB_CDC_DataReceived_EventSendOut 事件
        }
        // The method which fires the Event 
        private void OnConnectionStatus_EventSendOut(DetectingConnection_EventSendOut e)
        {
            if (DetectingConnection_EventSendOut != null)
                DetectingConnection_EventSendOut(this, e);    // 觸發DetectingConnection_EventSendOut 事件
        }
    }//class USB_CDC_Data_Packet_Decode
//////////////////////////////////////////////////////////////////////////////////////////////
// Event Class
//////////////////////////////////////////////////////////////////////////////////////////////
    // The class to hold the information about the event
    public class USB_CDC_DataReceived_EventSendOut : EventArgs
    {
        public byte USB_Cmd;
        public byte[] USB_ReceivedData;
        public USB_CDC_DataReceived_EventSendOut(byte Usb_Cmd, byte[] Usb_Data)
        {
            USB_Cmd = Usb_Cmd;
            USB_ReceivedData = (byte[])Usb_Data.Clone();
        }
    }//class SerialPortClass_DataReceivedEventArgs : EventArgs

    // The class to hold the information about the event
    public class DetectingConnection_EventSendOut : EventArgs
    {
        public bool IsConnection;
        public DetectingConnection_EventSendOut(bool connectStatus)
        {
            IsConnection = connectStatus;
        }
    }//class SerialPortClass_DataReceivedEventArgs : EventArgs

}//namespace FA_TOOL_SOFTWARE
