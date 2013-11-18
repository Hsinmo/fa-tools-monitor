using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FA_TOOL_SOFTWARE
{
    class FA_Charger_Test_Class
    {
        private System.Windows.Forms.Timer Fa_TimeOut_Timer = new System.Windows.Forms.Timer();
        // The timer unit = 1ms;
        // Sets the timer interval : 1 seconds = 1000.
        private const int Fa_TimeOut_Timer_Interval_Time = 2000;  // 3000ms = 3 sec.

        //Charger test Cmd
        const byte Cmd_Charger_24V_Channel_Set_ID = (0x8A);
        const byte Cmd_Charger_36V_Channel_Set_ID = (0x8B);
        const byte Cmd_Charger_48V_Channel_Set_ID = (0x8C);

        const byte Cmd_Get_Charger_24V_Voltage = (0x8D);
        const byte Cmd_Get_Charger_36V_Voltage = (0x8E);
        const byte Cmd_Get_Charger_48V_Voltage = (0x8F);

        const byte Cmd_Charger_24V_Channel_Set_Vin = (0xA0);
        const byte Cmd_Charger_36V_Channel_Set_Vin = (0xA1);
        const byte Cmd_Charger_48V_Channel_Set_Vin = (0xA2);

        const byte Cmd_Charger_All_Channel_ID_Set_OFF = (0xA3);
        const byte Cmd_Charger_All_Channel_Set_Vin = (0xA4);
        const byte Cmd_Get_All_Charger_Channel_Voltage = (0xA5);

        bool Start_Test = false;
        bool inActive = false;


        public enum ChargerType { Unknow_chg_Type, chg_24V_Type,  chg_36V_Type, chg_48V_Type};
        public delegate void FaTool_USB_CMD_SEND_Handler(byte Cmd, byte[] data);
        private FaTool_USB_CMD_SEND_Handler UsbCmdHandler;
        private delegate void Charger_Tset_Result_Event_Handler(bool ChargerFound, ChargerType type);
        private Charger_Tset_Result_Event_Handler Charger_Tset_Result_Event_CallBack;

        public FA_Charger_Test_Class(FaTool_USB_CMD_SEND_Handler callbackHanlder)
        {
            UsbCmdHandler = callbackHanlder;
            Fa_TimeOut_Timer.Interval = Fa_TimeOut_Timer_Interval_Time;
            Fa_TimeOut_Timer.Tick += new EventHandler(Fa_TimeOut_TimerEventProcessor);
        }
        public static bool isChagrgerCmdReceived(byte cmd)
        {
            if ((cmd >= Cmd_Charger_24V_Channel_Set_ID) && (cmd <= Cmd_Get_Charger_48V_Voltage))
            {
                return true;
            }
            if ((cmd >= Cmd_Charger_24V_Channel_Set_Vin) && (cmd <= Cmd_Get_All_Charger_Channel_Voltage))
            {
                return true;
            }
            return false;
        }
        public void ReceivedData_for_Fa_charger_Test(byte cmd, byte[] data){
            Fa_TimeOut_Timer.Stop();

        }
        public void Start_FA_Charger_Test()
        {
            if (Start_Test)
            {
                return;
            }
            Start_Test = true;
        }
        public void Stop_FA_Charger_Test()
        {
            Start_Test = false;
        }
        private void ChargerTestStep_Turn_ON_All_Vin_Channel()
        {
            byte[] data = new byte[1];
            data[0] = 1;
            UsbCmdHandler(Cmd_Charger_All_Channel_Set_Vin, data);
        }
        private void ChargerTestStep_Turn_Off_All_ID_Channel()
        {
            byte[] data = new byte[1];
            data[0] = 0;
            UsbCmdHandler(Cmd_Charger_All_Channel_ID_Set_OFF, data);
        }

        /// <summary>
        /// Timer Scanning Process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fa_TimeOut_TimerEventProcessor(Object sender, EventArgs e)
        {
            if (inActive)
            {
                Charger_Tset_Result_Event_CallBack(false, ChargerType.Unknow_chg_Type);
            }
        }
    }//class FA_Charger_Test_Class
}
