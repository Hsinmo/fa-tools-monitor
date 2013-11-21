using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Management;

namespace FA_TOOL_SOFTWARE.driver
{
    /// <summary>
    /// Provides automated detection and initiation of FaTools devices. This class cannot be inherited.
    /// </summary>
    public sealed class AutoDetectingComPorts : IDisposable//, INotifyPropertyChanged
    {
        /// <summary>
        ///     A System Watcher to hook events from the WMI tree.
        ///     EventRemoved => EventType = 3
        ///     EventArrived => EventType = 2
        /// </summary>
        private readonly ManagementEventWatcher _deviceWatcher = new ManagementEventWatcher(new WqlEventQuery(
            "SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3"));

        ///// <summary>
        /////     A System Watcher to hook events from the WMI tree for device attach
        ///// </summary>
        //private readonly ManagementEventWatcher _deviceWatcherAttach = new ManagementEventWatcher(new WqlEventQuery(
        //    "SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2"));
        ///// <summary>
        /////     A System Watcher to hook events from the WMI tree for device Remove
        ///// </summary>
        //private readonly ManagementEventWatcher _deviceWatcherRemove = new ManagementEventWatcher(new WqlEventQuery(
        //    "SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3"));

        /// <summary>
        ///     A list of all dynamically found SerialPorts.
        ///     _serialPorts.Key : com port name,
        ///     _serialPorts.Value : discription.
        /// </summary>
        ///
        private Dictionary<string, string> _serialPorts = new Dictionary<string, string>();
        
        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event EventHandler ComPortStatusUpdateEventHandler;

        /// <summary>
        ///     Initialises a new instance of the <see cref="AutoDetectingComPorts"/> class.
        /// </summary>
        public AutoDetectingComPorts()
        {
            // Attach an event listener to the device watcher.
            _deviceWatcher.EventArrived += _deviceWatcher_EventArrived;

            // Start monitoring the WMI tree for changes in SerialPort devices.
            _deviceWatcher.Start();

            // Initially populate the devices list.
            DiscoverDevices();
        }

        /// <summary>
        ///     Gets a list of all dynamically found SerialPorts.
        /// </summary>
        /// <value>A list of all dynamically found SerialPorts.</value>
        public Dictionary<string, string> SerialPorts
        {
            get { return _serialPorts; }
            //private set
            //{
            //    _serialPorts = value;
            //    //OnPropertyChanged();
            //}
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //FiresTheMessageEvent("Dispose");
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
            DiscoverDevices();
        }
        /// <summary>
        ///     Dynamically populates the SerialPorts property with relevant devices discovered from the WMI Win32_SerialPorts class.
        /// </summary>
        private void DiscoverDevices()
        {
            // Create a temporary dictionary to superimpose onto the SerialPorts property.
            Dictionary<string, string> dict = new Dictionary<string, string>();

            try
            {
                // Scan through each SerialPort registered in the WMI.
                foreach (ManagementObject device in
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_SerialPort").Get())
                {
                    //("device:" + device["PNPDeviceID"].ToString()); //ex: USB\VID_2047&PID_0301\A4F3984627000F00 ==>vid/pid/guid
                    //("device:" + device["Description"].ToString()); //ex: Dynapack Virtual COM Port (CDC)
                    //("device:" + device["DeviceID"].ToString());    //ex: COM48
                    //("device:" + device["Name"].ToString());    //ex: Dynapack Virtual COM Port (CDC) (COM48)
                    //("device:" + device["SystemName"].ToString());  //ex: DP1_HSINMO_LIN ==>owner computer name
                    //("device:" + device["Caption"].ToString()); //ex: Dynapack Virtual COM Port (CDC) (COM48)

                    //// Gather related configuration details for the Arduino Device.
                    //var config = device.GetRelated("Win32_SerialPortConfiguration").Cast<ManagementObject>().ToList().FirstOrDefault();
                    //// Set the SerialPort's BaudRate property. Use the devices maximum BaudRate as a fallback.
                    //port.BaudRate = (config != null)
                    //                    ? int.Parse(config["BaudRate"].ToString())
                    //                    : int.Parse(device["MaxBaudRate"].ToString());

                    try
                    {
                        // Set the SerialPort's PortName property.
                        string portName = device["DeviceID"].ToString();
                        string portDescription = device["Description"].ToString();
                        // Add the SerialPort to the dictionary. Key = Arduino device description.
                        dict.Add(portName, portDescription);
                    }
                    catch (ArgumentException)
                    {
                        Debug.WriteLine("An element with Key = \"txt\" already exists.");
                    }
                }

                // Return the dictionary.
                //SerialPorts = dict;
                _serialPorts = dict;
                FireTheUpdatedMessageEvent();
            }
            catch (ManagementException mex)
            {
                // Send a message to debug.
                Debug.WriteLine(@"An error occurred while querying for WMI data: " + mex.Message);
            }

        }

        // The method which fires the Event 
        private void FireTheUpdatedMessageEvent()
        {
            if (ComPortStatusUpdateEventHandler != null)
                ComPortStatusUpdateEventHandler(this, new ComPortStatusUpdate_EventArgs(SerialPorts));    // 觸發StatusMessage_EventArgs 事件
        }
    }//public sealed class AutoDetectingComPorts : IDisposable

    // The class to hold the information about the event
    public class ComPortStatusUpdate_EventArgs : EventArgs
    {
        public Dictionary<string, string> DictMessage { get; private set; }
        public ComPortStatusUpdate_EventArgs(Dictionary<string, string> dict)
        {
            DictMessage = dict;
        }
    }//class StatusMessage_EventArgs : EventArgs
}
