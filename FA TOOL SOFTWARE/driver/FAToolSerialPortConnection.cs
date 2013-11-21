using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace FA_TOOL_SOFTWARE.driver
{

    public enum MessageTypes { ConnectingStatus, ReceiveData, ErrorStatus, Test };
    public enum ConnectingStatusTypes { Connected, Disconnected };
    public enum ActionStatus { Failure, Success };
   
    
    /// <summary>
    /// Provides automated detection and initiation of FaTools devices. This class cannot be inherited.
    /// </summary>
    public sealed class FAToolSerialPortConnection : IDisposable//, INotifyPropertyChanged
    {
        public event EventHandler SerialPortMessage_EventHandler;


        private const string Find_FaTool_String = "Dynapack Virtual COM Port";
        private const string Find_Com_String = "COM";
        // The main control for communicating through the RS-232 port
        private SerialPort gSerialComPort = new SerialPort();
        private const int Default_BaudRate = 9600;
        private const int Default_DataBits = 8;
        private const StopBits Default_StopBits = StopBits.One;
        private const Parity Default_Parity = Parity.None;
        private string Setting_ComPort_Name = String.Empty;

        public ConnectingStatusTypes Connection = ConnectingStatusTypes.Disconnected;
        //public string ComPortInfor = String.Empty;

        private AutoDetectingComPorts autoDetectComPort;
        Dictionary<string, string> serialPortDict;

        public FAToolSerialPortConnection()
        {
            autoDetectComPort = new AutoDetectingComPorts();
            autoDetectComPort.ComPortStatusUpdateEventHandler += new EventHandler(AutodetectComPortStatusChange);   // 訂閱(subscribe) 事件 
            CheckAndConnectionWithDevices(autoDetectComPort.SerialPorts);
        }


        public string ComPortInforStr
        {
            get 
            {
                string str;
                if (gSerialComPort == null)
                {
                    gSerialComPort = new SerialPort();
                }
                str = ((gSerialComPort.IsOpen)?"Open,":"Close,") +
                    gSerialComPort.PortName + "," +
                    gSerialComPort.BaudRate + "," +
                    gSerialComPort.DataBits + "," +
                    gSerialComPort.Parity + "," +
                    gSerialComPort.StopBits;
                return str; 
            }
            //private set
            //{
            //    _serialPorts = value;
            //    //OnPropertyChanged();
            //}
        }

        private string getFaToolPortName(Dictionary<string, string> dict)
        {
            // When you use foreach to enumerate dictionary elements,
            // the elements are retrieved as KeyValuePair objects.
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                //string str = string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                if (kvp.Value.Contains(Find_FaTool_String))
                {
                    return kvp.Key;
                }
            }
            return string.Empty;
        }
        private bool isFaToolPortNameValid(string portName)
        {
            string str = string.Empty;
            try
            {
                str = serialPortDict[portName];
            }
            catch (ArgumentNullException ex)
            {
                str = string.Empty;
            }
            catch (KeyNotFoundException ex)
            {
                str = string.Empty;
            }
            if (str == string.Empty)
            {
                return false;
            }
            return true;
        }
        private ActionStatus OpenComPort(string portName)
        {
            bool error = false;

                gSerialComPort.BaudRate = Default_BaudRate;
                gSerialComPort.DataBits = Default_DataBits;
                gSerialComPort.StopBits = Default_StopBits;
                gSerialComPort.Parity = Default_Parity;
                gSerialComPort.PortName = portName;
                gSerialComPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                try
                {
                    // Open the port
                    gSerialComPort.Open();
                }
                catch (UnauthorizedAccessException) { error = true; }
                catch (IOException) { error = true; }
                catch (ArgumentException) { error = true; }

            if (error)
            {
                //MessageBox.Show("Could not open the COM port.  Most likely it is already in use, has been removed, or is unavailable.", "COM Port Unavalible", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                CloseComPort();
                Connection = ConnectingStatusTypes.Disconnected;
                return ActionStatus.Failure;
            }
            else
            {
                Connection = ConnectingStatusTypes.Connected;
                return ActionStatus.Success;
            }
        }
        private void CloseComPort()
        {
            try
            {
                if (gSerialComPort.IsOpen)
                {
                    Connection = ConnectingStatusTypes.Disconnected;
                    gSerialComPort.DataReceived -= new SerialDataReceivedEventHandler(port_DataReceived);
                    gSerialComPort.DiscardInBuffer();
                    gSerialComPort.DiscardOutBuffer();
                    gSerialComPort.Close();
                }
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }

        private void CheckAndConnectionWithDevices(Dictionary<string, string> dict)
        {
            serialPortDict = dict;

            string portName = getFaToolPortName(serialPortDict);

            if (gSerialComPort.IsOpen)
            {
                if (!isFaToolPortNameValid(gSerialComPort.PortName))
                {
                    CloseComPort();
                    FireFatoolMessageEvent(MessageTypes.ConnectingStatus, new byte[] { });
                }
            }
            else
            {
                if (portName != String.Empty)
                {
                    OpenComPort(portName);
                    FireFatoolMessageEvent(MessageTypes.ConnectingStatus, new byte[] { });
                }
            }
        }

        //// The method that implements the delegated functionality  
        //事件處理方法
        private void AutodetectComPortStatusChange(Object sender, EventArgs e)
        {
            // 判斷物件是否為 ComPortStatusUpdate_EventArgs 實體
            if (e is ComPortStatusUpdate_EventArgs)
            {
                // 將物件由 EventArgs 轉型 ComPortStatusUpdate_EventArgs
                ComPortStatusUpdate_EventArgs msg = e as ComPortStatusUpdate_EventArgs;
                Dictionary<string, string> dict = msg.DictMessage;

                CheckAndConnectionWithDevices(dict);

                //// When you use foreach to enumerate dictionary elements,
                //// the elements are retrieved as KeyValuePair objects.
                //foreach (KeyValuePair<string, string> kvp in dict)
                //{
                //    string str = string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                //    //Log(LogMsgType.Incoming, str + Environment.NewLine);
                //}
            }
        }

        /// <summary>
        /// sending bytes by serial port
        /// </summary>
        /// <param name="sendingData"></param>
        /// <returns>return if sending success.</returns>
        public ActionStatus ComPortSendData(byte[] sendingData)
        {
            bool done_sending = false;
            bool error = false;
            if (gSerialComPort.IsOpen)
            {
                try
                {
                    gSerialComPort.Write(sendingData, 0, sendingData.Length);
                    done_sending = true;
                }
                catch (Exception) { error = true; }

                if (error)
                {
                    CloseComPort();
                }
            }
            if (done_sending)
            {
                return ActionStatus.Success;
            }
            else
            {
                return ActionStatus.Failure;
            }
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
                FireFatoolMessageEvent(MessageTypes.ReceiveData, buffer);

            }
        }//private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)

        // The method which fires the Event 
        // If anyone has subscribed, notify them
        private void FireFatoolMessageEvent(MessageTypes mtp, byte[] received_buffer)
        {
            if (SerialPortMessage_EventHandler != null)
                SerialPortMessage_EventHandler(this, new SerialPortMessage_EventArgs(mtp, Connection, received_buffer, ComPortInforStr));    // 觸發StatusMessage_EventArgs 事件
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CloseComPort();
            autoDetectComPort.Dispose();
        }
    }//public sealed class FAToolSerialPortConnection

    // The class to hold the information about the event
    public class SerialPortMessage_EventArgs : EventArgs
    {
        public MessageTypes MsgType;
        public ConnectingStatusTypes ConectedStatus;
        public byte[] ReceivedBuffer;
        public string ComPortInformation;
        public SerialPortMessage_EventArgs(MessageTypes mtp, ConnectingStatusTypes status, byte[] received_buffer, string comPortInfor)
        {
            MsgType = mtp;
            ConectedStatus = status;
            ComPortInformation = comPortInfor;
            ReceivedBuffer = (byte[])received_buffer.Clone();
        }
    }//class SerialPortClass_DataReceivedEventArgs : EventArgs
}
