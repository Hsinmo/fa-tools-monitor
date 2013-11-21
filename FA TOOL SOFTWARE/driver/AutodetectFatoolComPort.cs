using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Management;
//using System.Runtime.CompilerServices;

namespace FA_TOOL_SOFTWARE.driver
{
    
    /// <summary>
    /// Provides automated detection and initiation of FaTools devices. This class cannot be inherited.
    /// </summary>
    public sealed class AutodetectFatoolComPort : IDisposable//, INotifyPropertyChanged
    {

        /// <summary>
        /// Device VendorID.
        /// </summary>
        const string USB_VID = "VID_2047";
        /// <summary>
        /// Device ProductID.
        /// </summary>
        const string USB_PID = "PID_0301";
        const string DEVICE_NAME = "Dynapack Virtual COM Port (CDC)";

        /// <summary>
        ///     A System Watcher to hook events from the WMI tree.
        ///     EventRemoved => EventType = 3
        ///     EventArrived => EventType = 2
        /// </summary>
        private readonly ManagementEventWatcher _deviceWatcher = new ManagementEventWatcher(new WqlEventQuery(
            "SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3"));

        /// <summary>
        ///     A System Watcher to hook events from the WMI tree for device attach
        /// </summary>
        //private readonly ManagementEventWatcher _deviceWatcherAttach = new ManagementEventWatcher(new WqlEventQuery(
            //"SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2"));
        /// <summary>
        ///     A System Watcher to hook events from the WMI tree for device Remove
        /// </summary>
        //private readonly ManagementEventWatcher _deviceWatcherRemove = new ManagementEventWatcher(new WqlEventQuery(
            //"SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3")); 

        /// <summary>
        ///     A list of all dynamically found SerialPorts.
        /// </summary>
        private Dictionary<string, SerialPort> _serialPorts = new Dictionary<string, SerialPort>();

        public bool isDeviceAttached = false;

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        
        public event EventHandler StatusMessageEventHandler;


        /// <summary>
        ///     Initialises a new instance of the <see cref="ArduinoDeviceManager"/> class.
        /// </summary>
        public AutodetectFatoolComPort()
        {

            // Attach an event listener to the device watcher.
            _deviceWatcher.EventArrived += _deviceWatcher_EventArrived;

            // Start monitoring the WMI tree for changes in SerialPort devices.
            _deviceWatcher.Start();

            // Initially populate the devices list.
            //DiscoverDevices();

            //FiresTheMessageEvent("test");
        }
        public void setmsg()
        {
            FiresTheMessageEvent("test");
        }
        /// <summary>
        ///     Gets a list of all dynamically found SerialPorts.
        /// </summary>
        /// <value>A list of all dynamically found SerialPorts.</value>
        public Dictionary<string, SerialPort> SerialPorts
        {
            get { return _serialPorts; }
            private set
            {
                _serialPorts = value;
                //OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            FiresTheMessageEvent("Dispose");
            // Stop the WMI monitors when this instance is disposed.
            _deviceWatcher.Stop();
        }


        /// <summary>
        ///     Handles the EventArrived event of the _deviceWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArrivedEventArgs"/> instance containing the event data.</param>
        private void _deviceWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            // show event info

            //Event_DeviceAttached  => EventType = 2
            //Event_DeviceRemoved   => EventType = 3
            string eventType = e.NewEvent.GetPropertyValue("EventType").ToString();

            ////Get the Event object and display its properties (all)
            //foreach (PropertyData pd in e.NewEvent.Properties)
            //{
            //    ManagementBaseObject mbo = null;
            //    if ((mbo = pd.Value as ManagementBaseObject) != null)
            //    {
            //        foreach (PropertyData prop in mbo.Properties)
            //            FiresTheMessageEvent(prop.Name + "-" + prop.Value);
            //    }
            //    FiresTheMessageEvent(pd.Qualifiers+ "-" + pd.Origin + "-" + pd.Name + "-" + pd.Value + "-" + pd.Type);
            //}

            bool isFind = false;
            string comPortName = string.Empty;
            try
            {
                // Scan through each SerialPort registered in the WMI.
                foreach (ManagementObject device in
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_SerialPort").Get())
                {
                    //FiresTheMessageEvent("device:" + device["PNPDeviceID"].ToString()); //ex: USB\VID_2047&PID_0301\A4F3984627000F00 ==>vid/pid/guid
                    //FiresTheMessageEvent("device:" + device["Description"].ToString()); //ex: Dynapack Virtual COM Port (CDC)
                    //FiresTheMessageEvent("device:" + device["DeviceID"].ToString());    //ex: COM48
                    //FiresTheMessageEvent("device:" + device["Name"].ToString());    //ex: Dynapack Virtual COM Port (CDC) (COM48)
                    //FiresTheMessageEvent("device:" + device["SystemName"].ToString());  //ex: DP1_HSINMO_LIN ==>owner computer name
                    //FiresTheMessageEvent("device:" + device["Caption"].ToString()); //ex: Dynapack Virtual COM Port (CDC) (COM48)

                    // Ignore all devices that do not have a relevant VendorID.
                    string str = device["PNPDeviceID"].ToString();
                    string str1 = device["Name"].ToString();
                    if (str.Contains(USB_VID) && str.Contains(USB_PID) && str1.Contains(DEVICE_NAME))
                    {
                        isFind = true;
                        comPortName = device["DeviceID"].ToString();
                        FiresTheMessageEvent("find");
                    }
                }
            }
            catch (ManagementException mex)
            {
                // Send a message to debug.
                Debug.WriteLine(@"An error occurred while querying for WMI data: " + mex.Message);
            }
            //DiscoverDevices();
        }

        /// <summary>
        ///     Dynamically populates the SerialPorts property with relevant devices discovered from the WMI Win32_SerialPorts class.
        /// </summary>
        private void DiscoverDevices()
        {
            // Create a temporary dictionary to superimpose onto the SerialPorts property.
            var dict = new Dictionary<string, SerialPort>();

            try
            {
                // Scan through each SerialPort registered in the WMI.
                foreach (ManagementObject device in
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_SerialPort").Get())
                {
                    // Ignore all devices that do not have a relevant VendorID.
                    if (!device["PNPDeviceID"].ToString().Contains("VID_2341") && // Arduino
                        !device["PNPDeviceID"].ToString().Contains("VID_04d0")) return; // Digi International (X-Bee)

                    // Create a SerialPort to add to the collection.
                    var port = new SerialPort();

                    // Gather related configuration details for the Arduino Device.
                    var config = device.GetRelated("Win32_SerialPortConfiguration")
                                       .Cast<ManagementObject>().ToList().FirstOrDefault();

                    // Set the SerialPort's PortName property.
                    port.PortName = device["DeviceID"].ToString();

                    // Set the SerialPort's BaudRate property. Use the devices maximum BaudRate as a fallback.
                    port.BaudRate = (config != null)
                                        ? int.Parse(config["BaudRate"].ToString())
                                        : int.Parse(device["MaxBaudRate"].ToString());

                    // Add the SerialPort to the dictionary. Key = Arduino device description.
                    dict.Add(device["Description"].ToString(), port);
                }

                // Return the dictionary.
                SerialPorts = dict;
            }
            catch (ManagementException mex)
            {
                // Send a message to debug.
                Debug.WriteLine(@"An error occurred while querying for WMI data: " + mex.Message);
            }
        }
/*
        /// <summary>
        ///     Called when a property is set.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        //[NotifyPropertyChangedInvocator]
        //private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        private void OnPropertyChanged( string propertyName = null)
        {
            //var handler = PropertyChanged;
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
*/


        // The method which fires the Event 
        private void FiresTheMessageEvent(string msg)
        {

            if (StatusMessageEventHandler != null)
                StatusMessageEventHandler(this, new StatusMessage_EventArgs(msg));    // 觸發StatusMessage_EventArgs 事件
        }

    }
    // The class to hold the information about the event
    public class StatusMessage_EventArgs : EventArgs
    {
        public string Message { get; private set; }
        public StatusMessage_EventArgs(string str)
        {
            Message = str;
        }
    }//class StatusMessage_EventArgs : EventArgs
}
