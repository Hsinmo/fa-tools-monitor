using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FA_TOOL_SOFTWARE
{
    public enum Logic_Byte_Bit:byte
    {
        Bit0=0x01,
        Bit1=0x02,
        Bit2=0x04,
        Bit3=0x08,
        Bit4=0x10,
        Bit5=0x20,
        Bit6=0x40,
        Bit7=0x80
    }
    partial class Form1
    {
        List<byte> g_ReceivedUartDataList = new List<byte>();

        private void ReceivedDataParsingLoad()
        {
        }

        private void Receiving_Data_UnPAcking_By_USBPacket(byte Usb_Cmd, byte[] Usb_Data)
        {

            DSG_Current_ADC = 0;
            CHG_Current_ADC = 0;
            VBAT_ADC = 0;
            VBAT_mV_To_ADC_Factor = 0.0f;
            DSG_mA_To_ADC_Factor = 0.0f;
            CHG_mA_To_ADC_Factor = 0.0f;
                //byte Usb_Cmd = ReceivedClass.USB_Cmd;
                //byte[] Usb_Data = (byte[])ReceivedClass.USB_ReceivedData.Clone();
                SetText("Receiving_Data_ByEvent CMD : " + HM_Utilitys.ByteToHexString(Usb_Cmd) + Environment.NewLine);
                SetText("Receive_Data byte: " + HM_Utilitys.ByteArrayToHexString(Usb_Data) + Environment.NewLine);

                //LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group group = LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group.None;
                List<LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group> outDataGroupList;
                List<byte[]> outDataList;

                if (Usb_Cmd == (byte)USB_CDC_Cmd.Cmd_UART_RS485_Receive_Data)
                {
                    g_ReceivedUartDataList.AddRange(Usb_Data);
                    SetText("Cmd_UART_RS485_Receive_Data byte: " + HM_Utilitys.ByteArrayToHexString(Usb_Data) + Environment.NewLine);
                    SetText("Cmd_UART_RS485_Receive_Data List : " + HM_Utilitys.ByteArrayToHexString(g_ReceivedUartDataList.ToArray<byte>()) + Environment.NewLine);
                    LEV_One_Wire_Receiving_Packet_Decoding.Packet_Decoding_To_Group_And_Clear_List(ref g_ReceivedUartDataList, out outDataGroupList, out outDataList);

                    if ((outDataGroupList.Count >= 1) && (outDataGroupList.Count == outDataList.Count))
                    {
                        for (int i = 0; i < outDataGroupList.Count; i++)
                        {
                            if (outDataGroupList[i] == LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group.OneWire_SystemData_Group)
                            {
                                SetText("OneWire_SystemData_Group : " + Environment.NewLine);
                                SetText("Group data : " + HM_Utilitys.ByteArrayToHexString(outDataList[i]) + Environment.NewLine);
                                Set_OneWire_SystemData(outDataList[i]);
                            }
                            else if (outDataGroupList[i] == LEV_One_Wire_Receiving_Packet_Decoding.Received_Data_Group.OneWire_EEPROM_Group)
                            {
                                SetText("OneWire_EEPROM_Group : " + Environment.NewLine);
                                SetText("Group data : " + HM_Utilitys.ByteArrayToHexString(outDataList[i]) + Environment.NewLine);
                                Set_OneWire_EEPROMData(outDataList[i]);

                                myUI_CalculationInformatio(VBAT_ADC, DSG_Current_ADC, CHG_Current_ADC, VBAT_mV_To_ADC_Factor, DSG_mA_To_ADC_Factor, CHG_mA_To_ADC_Factor);
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
                        LEV_One_Wire_Receiving_Packet_Decoding.Packet_Decoding_To_Group_And_Clear_List(ref g_ReceivedUartDataList, out outDataGroupList, out outDataList);
                    }
                }//if(Usb_Cmd == (byte)USB_CDC_Cmd.Cmd_UART_RS485_Receive_Data){

        }//private void Receiving_Data_UnPAcking_By_USBPacket(byte Usb_Cmd, byte[] Usb_Data)

        int DSG_Current_ADC;
        int CHG_Current_ADC;
        int VBAT_ADC;
        private void Set_OneWire_SystemData(byte[] data)
        {
            int DOC_Index = 4;
            Logic_Byte_Bit DOC_Set_Bit = Logic_Byte_Bit.Bit0;
            bool DOC = false;
            DOC = ((data[DOC_Index] & (byte)DOC_Set_Bit) > 0) ? true : false;

            int COC_Index = 4;
            Logic_Byte_Bit COC_Set_Bit = Logic_Byte_Bit.Bit1;
            bool COC = false;
            COC = ((data[COC_Index] & (byte)COC_Set_Bit) > 0) ? true : false;

            int BatOV_Index = 4;
            Logic_Byte_Bit BatOV_Set_Bit = Logic_Byte_Bit.Bit2;
            bool BatOV = false;
            BatOV = ((data[BatOV_Index] & (byte)BatOV_Set_Bit) > 0) ? true : false;

            int BatUV_Index = 4;
            Logic_Byte_Bit BatUV_Set_Bit = Logic_Byte_Bit.Bit3;
            bool BatUV = false;
            BatUV = ((data[BatUV_Index] & (byte)BatUV_Set_Bit) > 0) ? true : false;

            int CHG_OT_Index = 4;
            Logic_Byte_Bit CHG_OT_Set_Bit = Logic_Byte_Bit.Bit6;
            bool CHG_OT = false;
            CHG_OT = ((data[CHG_OT_Index] & (byte)CHG_OT_Set_Bit) > 0) ? true : false;

            int DSG_OT_Index = 4;
            Logic_Byte_Bit DSG_OT_Set_Bit = Logic_Byte_Bit.Bit7;
            bool DSG_OT = false;
            DSG_OT = ((data[DSG_OT_Index] & (byte)DSG_OT_Set_Bit) > 0) ? true : false;

            myUI_ControlItems_Protection(CHG_OT, DSG_OT, COC, DOC, BatOV, BatUV);

            int CHG_MOS_Index = 11;
            Logic_Byte_Bit CHG_MOS_Set_Bit = Logic_Byte_Bit.Bit0;
            bool CHG_MOS = false;
            CHG_MOS = ((data[CHG_MOS_Index] & (byte)CHG_MOS_Set_Bit) > 0) ? true : false;

            int DSG_MOS_Index = 11;
            Logic_Byte_Bit DSG_MOS_Set_Bit = Logic_Byte_Bit.Bit1;
            bool DSG_MOS = false;
            DSG_MOS = ((data[DSG_MOS_Index] & (byte)DSG_MOS_Set_Bit) > 0) ? true : false;



            int DSG_Current_ADC_hi_index = 16;
            int DSG_Current_ADC_lo_index = 17;
            DSG_Current_ADC = (data[DSG_Current_ADC_hi_index] << 8) + data[DSG_Current_ADC_lo_index];

            int CHG_Current_ADC_hi_index = 18;
            int CHG_Current_ADC_lo_index = 19;
            CHG_Current_ADC = (data[CHG_Current_ADC_hi_index] << 8) + data[CHG_Current_ADC_lo_index];

            int VBAT_ADC_hi_index = 20;
            int VBAT_ADC_lo_index = 21;
            VBAT_ADC = (data[VBAT_ADC_hi_index] << 8) + data[VBAT_ADC_lo_index];

            int NTC1_ADC_hi_index = 22;
            int NTC1_ADC_lo_index = 23;
            int NTC1_ADC = (data[NTC1_ADC_hi_index] << 8) + data[NTC1_ADC_lo_index];

            int Current_Capacity_hi_index = 28;
            int Current_Capacity_lo_index = 29;
            int Current_Capacity = (data[Current_Capacity_hi_index] << 8) + data[Current_Capacity_lo_index];

            int Cycle_Count_RECORD_hi_index = 56;
            int Cycle_Count_RECORD_lo_index = 57;
            int Cycle_Count_RECORD = (data[Cycle_Count_RECORD_hi_index] << 8) + data[Cycle_Count_RECORD_lo_index];

            int fcc = 0;
            myUI_AdvanceInformation(Current_Capacity, Cycle_Count_RECORD, NTC_TABLE.Get_CentigradeDegree_By_NTC_ADC(NTC1_ADC), fcc, CHG_MOS, DSG_MOS);



            //myUI_AdvanceInformation(Current_Capacity, Cycle_Count_RECORD, VBAT_ADC, curr, dir, NTC1_ADC, fcc, CHG_MOS, DSG_MOS);
            
        }
        float VBAT_mV_To_ADC_Factor = 0.0f;
        float DSG_mA_To_ADC_Factor = 0.0f;
        float CHG_mA_To_ADC_Factor = 0.0f;
        private void Set_OneWire_EEPROMData(byte[] data)
        {
            float tempf = 0.0f;

            //////////////////////////////////////////////////////////////////////////////
            int CHG_mA_To_ADC_Factor_float_hi_index = 0;
            tempf = 0.0f;
            unsafe
            {
                byte* byteTemp = (byte*)&tempf;
                *byteTemp++ = data[CHG_mA_To_ADC_Factor_float_hi_index + 3];
                *byteTemp++ = data[CHG_mA_To_ADC_Factor_float_hi_index + 2];
                *byteTemp++ = data[CHG_mA_To_ADC_Factor_float_hi_index + 1];
                *byteTemp++ = data[CHG_mA_To_ADC_Factor_float_hi_index];
            }
             CHG_mA_To_ADC_Factor = tempf; 
   
            //////////////////////////////////////////////////////////////////////////////
            int DSG_mA_To_ADC_Factor_float_hi_index = 4;
            tempf = 0.0f;
            unsafe
            {
                byte* byteTemp = (byte*)&tempf;
                *byteTemp++ = data[DSG_mA_To_ADC_Factor_float_hi_index + 3];
                *byteTemp++ = data[DSG_mA_To_ADC_Factor_float_hi_index + 2];
                *byteTemp++ = data[DSG_mA_To_ADC_Factor_float_hi_index + 1];
                *byteTemp++ = data[DSG_mA_To_ADC_Factor_float_hi_index];
            }
             DSG_mA_To_ADC_Factor = tempf; 
   
            //////////////////////////////////////////////////////////////////////////////
            int VBAT_mV_To_ADC_Factor_float_hi_index = 8;
            tempf = 0.0f;
            unsafe
            {
                byte* byteTemp = (byte*)&tempf;
                *byteTemp++ = data[VBAT_mV_To_ADC_Factor_float_hi_index + 3];
                *byteTemp++ = data[VBAT_mV_To_ADC_Factor_float_hi_index + 2];
                *byteTemp++ = data[VBAT_mV_To_ADC_Factor_float_hi_index + 1];
                *byteTemp++ = data[VBAT_mV_To_ADC_Factor_float_hi_index];
            }
             VBAT_mV_To_ADC_Factor = tempf; 
   

            //////////////////////////////////////////////////////////////////////////////
            int DSG_OP_ADC_OFFSET_index = 16;
            sbyte DSG_OP_ADC_OFFSET = (sbyte)data[DSG_OP_ADC_OFFSET_index];
            //////////////////////////////////////////////////////////////////////////////
            int CHG_OP_ADC_OFFSET_index = 17;
            sbyte CHG_OP_ADC_OFFSET = (sbyte)data[CHG_OP_ADC_OFFSET_index];
            //////////////////////////////////////////////////////////////////////////////
            int VBAT_ADC_OFFSET_index = 18;
            sbyte VBAT_ADC_OFFSET = (sbyte)data[VBAT_ADC_OFFSET_index];
            //////////////////////////////////////////////////////////////////////////////
            int MANUFACTURE_NAME_LENGTH_index = 119;
            int MANUFACTURE_NAME_LENGTH = data[MANUFACTURE_NAME_LENGTH_index];
            int MANUFACTURE_NAME_index = 120;
            byte[] strByte = new byte[MANUFACTURE_NAME_LENGTH];
            Array.Copy(data, MANUFACTURE_NAME_index, strByte, 0, strByte.Length);
            string MANUFACTURE_NAME_String = System.Text.Encoding.Default.GetString(strByte);
            //////////////////////////////////////////////////////////////////////////////
            int MANUFACTURE_DATE_hi_index= 106;
            int MANUFACTURE_DATE_lo_index= 107;
            int MANUFACTURE_DATE_Value = (data[MANUFACTURE_DATE_hi_index] << 8) + data[MANUFACTURE_DATE_lo_index];
            DateTime MANUFACTURE_DATE = ManufactureData_Translate(MANUFACTURE_DATE_Value);
            //////////////////////////////////////////////////////////////////////////////
            myUI_ManufactureInfo(MANUFACTURE_NAME_String, MANUFACTURE_DATE);
        }



        private void SetControlItems_Protection(bool COT, bool DOT, bool COC, bool DOC, bool OVP, bool UVP)
        {
            this.OT_Protect_CHG(COT);
            this.OT_Protect_DSG(DOT);
            this.OC_Protect_CHG(COC);
            this.OC_Protect_DSG(DOC);
            this.OV_Protect(OVP);
            this.UV_Protect(UVP);
        }
        // ///////////////////////////////////////////////////////////////////////////////
        private delegate void myUICallBack_ControlItems_Protection(bool COT, bool DOT, bool COC, bool DOC, bool OVP, bool UVP);
        private void myUI_ControlItems_Protection(bool COT, bool DOT, bool COC, bool DOC, bool OVP, bool UVP)
        {
            if (this.InvokeRequired)
            {
                myUICallBack_ControlItems_Protection myUpdate = new myUICallBack_ControlItems_Protection(myUI_ControlItems_Protection);
                this.Invoke(myUpdate, COT,  DOT,  COC,  DOC,  OVP,  UVP);
            }
            else
            {
                SetControlItems_Protection(COT, DOT, COC, DOC, OVP, UVP);
            }
        }
        // ///////////////////////////////////////////////////////////////////////////////
        private void Set_CalculationInformation(int vol, int dsg_current, int chg_current, float v_factor, float d_factor, float c_factor)
        {

            Current_Status dir;
            float curr;
            if (dsg_current > chg_current)
            {
                dir = Current_Status.Discharging;
                curr = dsg_current / d_factor;
            }
            else
            {
                if (Math.Abs(dsg_current - chg_current) < 5)
                {
                    dir = Current_Status.Static;
                    curr = dsg_current / d_factor;
                }
                else
                {
                    dir = Current_Status.Charging;
                    curr = chg_current / c_factor;
                }
            }

            show_voltage_value((int)(vol / v_factor));
            show_current_value((int)curr, dir);

        }
        // ///////////////////////////////////////////////////////////////////////////////
        private delegate void myUICallBack_CalculationInformatio(int vol, int dsg_current, int chg_current, float v_factor, float d_factor, float c_factor);
        private void myUI_CalculationInformatio(int vol, int dsg_current, int chg_current, float v_factor, float d_factor, float c_factor)
        {
            if (this.InvokeRequired)
            {
                myUICallBack_CalculationInformatio myUpdate = new myUICallBack_CalculationInformatio(myUI_CalculationInformatio);
                this.Invoke(myUpdate, vol, dsg_current, chg_current, v_factor, d_factor, c_factor);
            }
            else
            {
                Set_CalculationInformation(vol, dsg_current, chg_current, v_factor, d_factor, c_factor);
            }
        }
        // ///////////////////////////////////////////////////////////////////////////////
        private void SetControlItems_AdvanceInformation(int cap, int cycle_count, int temp, int fcc, bool Cmos, bool Dmos)
        {
            show_capecity(cap);
            show_cycle_count(cycle_count);
            show_temperature_value(temp);
            show_FCC_value(fcc);
            show_CHG_MOS_status(Cmos);
            show_DSG_MOS_status(Dmos);
        }
        // ///////////////////////////////////////////////////////////////////////////////
        private delegate void myUICallBack_AdvanceInformation(int cap, int cycle_count, int temp, int fcc, bool Cmos, bool Dmos);
        private void myUI_AdvanceInformation(int cap, int cycle_count, int temp, int fcc, bool Cmos, bool Dmos)
        {
            if (this.InvokeRequired)
            {
                myUICallBack_AdvanceInformation myUpdate = new myUICallBack_AdvanceInformation(myUI_AdvanceInformation);
                this.Invoke(myUpdate,  cap,  cycle_count, temp, fcc,  Cmos,  Dmos);
            }
            else
            {
                SetControlItems_AdvanceInformation(cap, cycle_count, temp, fcc, Cmos, Dmos);
            }
        }
        // ///////////////////////////////////////////////////////////////////////////////
        private void SetControlItems_Manufacture(string name, DateTime date)
        {
            show_manufacture_name(name);
            show_manufacture_date(date.ToLongDateString());
        }
        // ///////////////////////////////////////////////////////////////////////////////
        private delegate void myUICallBack_ManufactureInfo(string name, DateTime date);
        private void myUI_ManufactureInfo(string name, DateTime date)
        {
            if (this.InvokeRequired)
            {
                myUICallBack_ManufactureInfo myUpdate = new myUICallBack_ManufactureInfo(myUI_ManufactureInfo);
                this.Invoke(myUpdate, name, date);
            }
            else
            {
                SetControlItems_Manufacture(name, date);
            }
        }
        // ///////////////////////////////////////////////////////////////////////////////
        private DateTime ManufactureData_Translate(int date)
        {
            //(MANUFACTURE_DATE_YEAR - 1980) * 512 + MANUFACTURE_DATE_MONTH * 32 + MANUFACTURE_DATE_DAY  // 2 bytes
            //MANUFACTURE_DATE_DAY = 5 bits, MANUFACTURE_DATE_MONTH = 4 bits, MANUFACTURE_DATE_YEAR = 7 bits
            int Day_Mask = 0x1f;
            int Month_Mask = 0x0f;

            int Day = (date & Day_Mask);
            int Month = ((date >> 5) & Month_Mask);
            int Year = (date >> 9) + 1980;

            return new DateTime(Year, Month, Day);
        }

    }//partial class Form1


    public static class NTC_TABLE
    {
        private static int TEMPERATURE_TABLE_POINTS = 141;
        private static int NTC_DATA_LINE = 2;
        private static int TEMPERATURE_Celsius_Index = 0;
        private static int TEMPERATURE_ADC_Index = 1;


        private static int[,] NTC_ADC_TO_TEMP_Table = {
          // NTC 溫度(度), ADC
          //    Celsius
            {  -40,    3872},    //index = 000
            {  -39,    3860},    //index = 001
            {  -38,    3848},    //index = 002
            {  -37,    3836},    //index = 003
            {  -36,    3823},    //index = 004
            {  -35,    3809},    //index = 005
            {  -34,    3796},    //index = 006
            {  -33,    3781},    //index = 007
            {  -32,    3767},    //index = 008
            {  -31,    3751},    //index = 009
            {  -30,    3735},    //index = 010
            {  -29,    3719},    //index = 011
            {  -28,    3702},    //index = 012
            {  -27,    3685},    //index = 013
            {  -26,    3667},    //index = 014
            {  -25,    3649},    //index = 015
            {  -24,    3630},    //index = 016
            {  -23,    3610},    //index = 017
            {  -22,    3590},    //index = 018
            {  -21,    3569},    //index = 019
            {  -20,    3548},    //index = 020
            {  -19,    3526},    //index = 021
            {  -18,    3504},    //index = 022
            {  -17,    3481},    //index = 023
            {  -16,    3457},    //index = 024
            {  -15,    3433},    //index = 025
            {  -14,    3408},    //index = 026
            {  -13,    3382},    //index = 027
            {  -12,    3356},    //index = 028
            {  -11,    3329},    //index = 029
            {  -10,    3301},    //index = 030
            {  -9 ,    3273},    //index = 031
            {  -8 ,    3244},    //index = 032
            {  -7 ,    3215},    //index = 033
            {  -6 ,    3185},    //index = 034
            {  -5 ,    3155},    //index = 035
            {  -4 ,    3123},    //index = 036
            {  -3 ,    3091},    //index = 037
            {  -2 ,    3059},    //index = 038
            {  -1 ,    3025},    //index = 039
            {  0  ,    2992},    //index = 040
            {  1  ,    2957},    //index = 041
            {  2  ,    2923},    //index = 042
            {  3  ,    2887},    //index = 043
            {  4  ,    2851},    //index = 044
            {  5  ,    2815},    //index = 045
            {  6  ,    2778},    //index = 046
            {  7  ,    2741},    //index = 047
            {  8  ,    2704},    //index = 048
            {  9  ,    2666},    //index = 049
            {  10 ,    2628},    //index = 050
            {  11 ,    2591},    //index = 051
            {  12 ,    2552},    //index = 052
            {  13 ,    2513},    //index = 053
            {  14 ,    2475},    //index = 054
            {  15 ,    2436},    //index = 055
            {  16 ,    2397},    //index = 056
            {  17 ,    2358},    //index = 057
            {  18 ,    2319},    //index = 058
            {  19 ,    2280},    //index = 059
            {  20 ,    2241},    //index = 060
            {  21 ,    2202},    //index = 061
            {  22 ,    2163},    //index = 062
            {  23 ,    2124},    //index = 063
            {  24 ,    2086},    //index = 064
            {  25 ,    2048},    //index = 065
            {  26 ,    2009},    //index = 066
            {  27 ,    1971},    //index = 067
            {  28 ,    1934},    //index = 068
            {  29 ,    1896},    //index = 069
            {  30 ,    1859},    //index = 070
            {  31 ,    1822},    //index = 071
            {  32 ,    1786},    //index = 072
            {  33 ,    1750},    //index = 073
            {  34 ,    1714},    //index = 074
            {  35 ,    1678},    //index = 075
            {  36 ,    1644},    //index = 076
            {  37 ,    1609},    //index = 077
            {  38 ,    1575},    //index = 078
            {  39 ,    1542},    //index = 079
            {  40 ,    1508},    //index = 080
            {  41 ,    1476},    //index = 081
            {  42 ,    1444},    //index = 082
            {  43 ,    1412},    //index = 083
            {  44 ,    1381},    //index = 084
            {  45 ,    1350},    //index = 085
            {  46 ,    1320},    //index = 086
            {  47 ,    1291},    //index = 087
            {  48 ,    1262},    //index = 088
            {  49 ,    1233},    //index = 089
            {  50 ,    1205},    //index = 090
            {  51 ,    1177},    //index = 091
            {  52 ,    1151},    //index = 092
            {  53 ,    1124},    //index = 093
            {  54 ,    1098},    //index = 094
            {  55 ,    1073},    //index = 095
            {  56 ,    1048},    //index = 096
            {  57 ,    1024},    //index = 097
            {  58 ,    1000},    //index = 098
            {  59 ,    976 },    //index = 099
            {  60 ,    953 },    //index = 100
            {  61 ,    931 },    //index = 101
            {  62 ,    909 },    //index = 102
            {  63 ,    888 },    //index = 103
            {  64 ,    867 },    //index = 104
            {  65 ,    846 },    //index = 105
            {  66 ,    826 },    //index = 106
            {  67 ,    807 },    //index = 107
            {  68 ,    787 },    //index = 108
            {  69 ,    769 },    //index = 109
            {  70 ,    750 },    //index = 110
            {  71 ,    733 },    //index = 111
            {  72 ,    715 },    //index = 112
            {  73 ,    698 },    //index = 113
            {  74 ,    682 },    //index = 114
            {  75 ,    666 },    //index = 115
            {  76 ,    650 },    //index = 116
            {  77 ,    634 },    //index = 117
            {  78 ,    619 },    //index = 118
            {  79 ,    605 },    //index = 119
            {  80 ,    590 },    //index = 120
            {  81 ,    577 },    //index = 121
            {  82 ,    563 },    //index = 122
            {  83 ,    550 },    //index = 123
            {  84 ,    537 },    //index = 124
            {  85 ,    524 },    //index = 125
            {  86 ,    512 },    //index = 126
            {  87 ,    500 },    //index = 127
            {  88 ,    488 },    //index = 128
            {  89 ,    477 },    //index = 129
            {  90 ,    466 },    //index = 130
            {  91 ,    455 },    //index = 131
            {  92 ,    445 },    //index = 132
            {  93 ,    434 },    //index = 133
            {  94 ,    424 },    //index = 134
            {  95 ,    414 },    //index = 135
            {  96 ,    405 },    //index = 136
            {  97 ,    396 },    //index = 137
            {  98 ,    387 },    //index = 138
            {  99 ,    378 },    //index = 139
            {  100,    369 }     //index = 140
        };

        public static int Get_CentigradeDegree_By_NTC_ADC(int ntc_adc)
        {
            int Celsius_temp;
            int i;
            Celsius_temp = 0;
            if (ntc_adc >= NTC_ADC_TO_TEMP_Table[0,TEMPERATURE_ADC_Index])
            {
                Celsius_temp = NTC_ADC_TO_TEMP_Table[0,TEMPERATURE_Celsius_Index];
            }
            else if (ntc_adc <= NTC_ADC_TO_TEMP_Table[TEMPERATURE_TABLE_POINTS - 1,TEMPERATURE_ADC_Index])
            {
                Celsius_temp = NTC_ADC_TO_TEMP_Table[TEMPERATURE_TABLE_POINTS - 1,TEMPERATURE_Celsius_Index];
            }
            else
            {
                for (i = 1; i < TEMPERATURE_TABLE_POINTS; i++)
                {
                    if (ntc_adc >= NTC_ADC_TO_TEMP_Table[i,TEMPERATURE_ADC_Index])
                    { //取較小值
                        Celsius_temp = NTC_ADC_TO_TEMP_Table[i,TEMPERATURE_Celsius_Index];
                        break;
                    }
                }//for
            }
            return Celsius_temp;
        }

/*
unsigned int Get_CentigradeDegree_By_NTC_ADC(unsigned int ntc_adc){
    unsigned int Celsius_temp;
    unsigned int i;
    
    Celsius_temp = 0;
    if(ntc_adc >= NTC_ADC_TO_TEMP_Table[0][TEMPERATURE_ADC_Index]){
        Celsius_temp = NTC_ADC_TO_TEMP_Table[0][TEMPERATURE_Celsius_Index];
    }else if(ntc_adc <= NTC_ADC_TO_TEMP_Table[TEMPERATURE_TABLE_POINTS - 1][TEMPERATURE_ADC_Index]){
        Celsius_temp = NTC_ADC_TO_TEMP_Table[TEMPERATURE_TABLE_POINTS - 1][TEMPERATURE_Celsius_Index];
    }else{
      for(i = 1; i < TEMPERATURE_TABLE_POINTS; i++){
          if(ntc_adc >= NTC_ADC_TO_TEMP_Table[i][TEMPERATURE_ADC_Index]){ //取較小值
              Celsius_temp = NTC_ADC_TO_TEMP_Table[i][TEMPERATURE_Celsius_Index];
              break;
          }
      }//for
    }    
    return Celsius_temp;
}
unsigned int Get_KelvinDegree_By_NTC_ADC(unsigned int ntc_adc){
    unsigned int Celsius_temp;
    
    Celsius_temp = Get_CentigradeDegree_By_NTC_ADC(ntc_adc);
    Celsius_temp = Celsius_temp * 10;   // transfor to 0.1 degrees unit
    Celsius_temp = 2731 + Celsius_temp; // transfor to Kelvin Degree unit
    return Celsius_temp;
}
*/
    }//public static class NTC_TABLE



}
