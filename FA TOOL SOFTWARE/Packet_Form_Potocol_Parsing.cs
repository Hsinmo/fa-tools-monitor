using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FA_TOOL_SOFTWARE
{
    public enum USB_CDC_Cmd
    {
        // H/W setting cmd
        Cmd_Multiplex_Reset = (0x80),
        Cmd_Multiplex_Set_Channel = (0x81),
        Cmd_UART_Set_Baud_Rate = (0x82),
        Cmd_UART_Set_Default_Baud_Rate = (0x83),
        Cmd_UART_RS485_Enable = (0x84),
        Cmd_UART_RS485_Disable = (0x85),
        Cmd_One_Wire_Commu_Enable = (0x86),
        Cmd_One_Wire_Commu_Disable = (0x87),
        Cmd_I2C_Reset = (0x88),
        Cmd_I2C_Set_Address = (0x89),
        // Data Status cmd
        Cmd_I2C_Transmit_Data = (0x90),
        Cmd_I2C_Receive_Data = (0x91),
        Cmd_UART_RS485_Transmit_Data = (0x92),
        Cmd_UART_RS485_Receive_Data = (0x93),
        Cmd_One_Wire_Transmit_Data = (0x94),
        Cmd_One_Wire_Receive_Data = (0x95),
        // Test/Debug Status cmd
        Cmd_Error_Cmd = (0xE0),
        Cmd_For_Connect_Detection = (0xE1),
        Cmd_Test_Data_Send_Back = (0xE2)
    }
    public enum LEV_Cmd
    {
        // setting cmd
        Cmd_One_Wire_Data_Transmit_Once = (0xB2),
        // responsed Status cmd
        Cmd_Respond_Accept_Check_Code = (0x80),
        Cmd_Respond_Error_Check_Code = (0x83)
    }


    public static class USB_CDC_Packet_Forming_and_Decoding
    {
        private const byte LeadingCode = 0x3A;   //起始字元
        private const byte SlaveAddressCode = 0xA6;    //Slave Address
        private const byte EndingCode1 = 0x0D;//結束字元 1
        private const byte EndingCode2 = 0x0A; //結束字元 2

        private const int Transmitting_Max_Data_Length_PcToUSB = (0x1f);  //not whole structure Length, only Data buffer Length
        private const int Receiving_Max_Data_Length_UsbToPc = 512;//(0xff);   //not whole structure Length, only Data buffer Length
        /*  USB_CDC_Transmit_Packet
        struct USB_CDC_Transmit_Packet
        {
            private const byte LeadingCode = CDC_LeadingCode;   //起始字元
            private const byte SlAdd = CDC_SlaveAddressCode;    //Slave Address
            public byte command;    //命令
            private byte DataLenExpected_low;//數據長度, 
            private byte DataLenExpected_high;//數據長度,
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=CDC_Transmitting_Max_Data_Length)]
            private byte[] DataBuf;//數據內容
            private byte LRCDataLow;           //checkSum16 Low byte, included slave address, command, length and data.
            private byte LRCDataHigh;	      //checkSum16 High byte, included slave address, command, length and data.
            private const byte cEND1 = CDC_EndingCode1;//結束字元 1
            private const byte cEND2 = CDC_EndingCode2;//結束字元 2
        }*/
        public static byte[] CMD_Forming_For_Transmitting(byte Cmd, byte[] Transmit_Parameter)
        {
            byte[] TempData;
            List<byte> TransmitData = new List<byte>();
            UInt16 LenExpected;
            if (Cmd == 0x92)
            {
                LenExpected = 0;
            }

            if (Transmit_Parameter.Length > Transmitting_Max_Data_Length_PcToUSB)
            {
                LenExpected = Transmitting_Max_Data_Length_PcToUSB;
                TempData = new byte[LenExpected];
                for (UInt16 i = 0; i < LenExpected; i++)
                {
                    TempData[i] = Transmit_Parameter[i];
                }
            }
            else
            {
                LenExpected = (UInt16)Transmit_Parameter.Length;
                TempData = (byte[])Transmit_Parameter.Clone();
            }

            TransmitData.Add(SlaveAddressCode);
            TransmitData.Add(Cmd);
            TransmitData.Add((byte)LenExpected);
            TransmitData.Add((byte)(LenExpected >> 8));
            TransmitData.AddRange(TempData);


            byte[] CheckBuffer = TransmitData.ToArray<Byte>();
            ushort checkSum = HM_Utilitys.ComputeCheckSum16(CheckBuffer);

            TransmitData.Add((byte)(checkSum));           // low byte     //4 to last byte for packet
            TransmitData.Add((byte)(checkSum >> 8));      // high byte    //3 to last byte for packet
            TransmitData.Add(EndingCode1);                //2 to last byte for packet
            TransmitData.Add(EndingCode2);                //last byte for packet

            TransmitData.Insert(0, LeadingCode);        // add first Item

            return TransmitData.ToArray<Byte>();
        }

        /*   USB_CDC_Receiving_Packet
         struct USB_CDC_Receiving_Packet
         {
             private const byte LeadingCode = CDC_LeadingCode;   //起始字元
             private const byte SlAdd = CDC_SlaveAddressCode;    //Slave Address
             public byte command;    //命令
            private byte DataLenExpected_low;//數據長度, 
            private byte DataLenExpected_high;//數據長度,
         * [MarshalAs(UnmanagedType.ByValArray, SizeConst=CDC_Receiving_Max_Data_Length)]
             public byte[] DataBuf;//數據內容
             public byte LRCDataLow;           //checkSum16 Low byte, included slave address, command, length and data.
             public byte LRCDataHigh;	      //checkSum16 High byte, included slave address, command, length and data.
             private const byte cEND1 = CDC_EndingCode1;//結束字元 1
             private const byte cEND2 = CDC_EndingCode2;//結束字元 2
         }*/
        public static bool CMD_Decoding_For_Receiving(byte[] receivingRawData, out byte ReceivedCommand, out byte[] decoding_Parameter)
        {
            bool IsFound_Packet_Form = false;

            ReceivedCommand = 0;
            decoding_Parameter = new byte[0];
            List<byte> ReceivedDataList = new List<byte>();

            int startFormIndex;
            int dataLength, endCode1_idx, endCode2_idx;
            for (int i = 0; i < (receivingRawData.Length - 1); i++)
            {
                //finding leading codes
                if ((receivingRawData[i] == LeadingCode) && (receivingRawData[i + 1] == SlaveAddressCode))
                {
                    startFormIndex = i;
                    dataLength = (receivingRawData[startFormIndex + 4] << 8) + receivingRawData[startFormIndex + 3];    //offset 3 to get receiving data length low, offset 4 to get receiving data length high
                    endCode1_idx = startFormIndex + 4 + dataLength + 2 + 1;  //add offset 2 is two of Cuecksum bytes
                    endCode2_idx = startFormIndex + 4 + dataLength + 2 + 2;  //add offset 2 is two of Cuecksum bytes
                    //finding Ending Codes
                    if ((receivingRawData[endCode1_idx] == EndingCode1) && (receivingRawData[endCode2_idx] == EndingCode2))
                    {
                        //calculate checkSum
                        byte[] CheckBuffer = new byte[dataLength + 4]; //included slave address, command, length and data.
                        Array.Copy(receivingRawData, startFormIndex + 1, CheckBuffer, 0, CheckBuffer.Length);
                        ushort checkSum = HM_Utilitys.ComputeCheckSum16(CheckBuffer);
                        int checkSumLowByte_Index = endCode1_idx - 2;
                        int checkSumHighByte_Index = checkSumLowByte_Index + 1;
                        if (receivingRawData[checkSumLowByte_Index] == ((byte)(checkSum)) && receivingRawData[checkSumHighByte_Index] == ((byte)(checkSum >> 8)))
                        {
                            //Save data to structure
                            ReceivedCommand = receivingRawData[startFormIndex + 2];

                            for (int j = 0; j < dataLength; j++)
                            {
                                ReceivedDataList.Add(receivingRawData[startFormIndex + 5 + j]);
                            }
                            IsFound_Packet_Form = true;
                            break;
                        }
                    }//if((ReceivingData[endCode1_idx] == EndingCode1) && (ReceivingData[endCode2_idx] == EndingCode2)){
                }//if((ReceivingData[i] == LeadingCode) && (ReceivingData[i+1] == SlaveAddressCode)){
            }//for(int i = 0; i < (ReceivingData.Length - 1); i++){
            if (IsFound_Packet_Form)
            {
                decoding_Parameter = ReceivedDataList.ToArray<byte>();
            }
            return IsFound_Packet_Form;
        }
    }

    public static class LEV_One_Wire_Receiving_Packet_Decoding
    {

        //========One Wire Data Structure=======================================================
        //========One Wire Transmiting Data Structure=======================================================
        //typedef struct{
        //    static uint cStart1 = 0x80f8;	//起始字元 // send High byte first, then send Low byte second.
        //    static uint cStart2 = 0x80A0;	//function字元 // send High byte first, then send Low byte second.
        //                                    // 0x80A0 : data 資料,
        //                                    // 0x80D0 ~ 0x80DF : EEPROM Seg 0 ~ 15 (a segment = 64 bytes),
        //    unsigned int LenExpected;//數據長度 ; send High byte first, then send Low byte second.
        //    unsigned char DataBuf[LenExpected];//數據內容
        //    unsigned char LRCDataHigh;	//CRC or checkSum High byte, calculating only for DataBuf
        //    unsigned char LRCDataLow;	//CRC or checkSum Low byte, calculating only for DataBuf
        //    static uint cEND1= 0x70f7;	//結束字組 1 // send High byte first, then send Low byte second. 
        //    static uint cEND1= 0x70f7;	//結束字組 2 // send High byte first, then send Low byte second.
        //} ModbusProtocolPacket;//RTU mode
        //========One Wire Transmiting EEPROM Data Structure=======================================================
        //typedef struct{
        //    static uint cStart1 = 0x80f8;	//起始字元 // send High byte first, then send Low byte second.
        //    static uint cStart2 = 0x80D0;	//function字元 // send High byte first, then send Low byte second.
        //    unsigned char DataBuf[];//數據內容
        //    unsigned char LRCDataHigh;	//CRC or checkSum High byte, calculating only for DataBuf
        //    unsigned char LRCDataLow;	//CRC or checkSum Low byte, calculating only for DataBuf
        //    static uint cEND1= 0x70f7;	//結束字組 1 // send High byte first, then send Low byte second. 
        //    static uint cEND1= 0x70f7;	//結束字組 2 // send High byte first, then send Low byte second.
        //} ModbusProtocolPacket;//RTU mode
        //
        /*
		G_Communication_Array[0] = ONE_WIRE_PrecedingCheckCode >> 8;        //high-bytes first
		G_Communication_Array[1] = ONE_WIRE_PrecedingCheckCode & 0x00ff;	//low-bytes second
		G_Communication_Array[2] = ONE_WIRE_Data_PrecedingCode >> 8;        //high-bytes first
		G_Communication_Array[3] = ONE_WIRE_Data_PrecedingCode & 0x00ff;	//low-bytes second
        G_Communication_Array[4] = Data Length      ////high-bytes first
        G_Communication_Array[5] = Data Length      ////low-bytes second
		G_Communication_Array[4 ~ 4 + data_len];
		G_Communication_Array[data_len + 1] = usCRC16 >> 8;			    //high-bytes first
		G_Communication_Array[data_len + 2] = usCRC16;					//low-bytes second
		G_Communication_Array[data_len + 3] = ONE_WIRE_EndCheckCode >> 8;       //high-bytes first
		G_Communication_Array[data_len + 4] = ONE_WIRE_EndCheckCode & 0x00ff;	//low-bytes second
		G_Communication_Array[data_len + 5] = ONE_WIRE_EndCheckCode >> 8;       //high-bytes first
		G_Communication_Array[data_len + 6] = ONE_WIRE_EndCheckCode & 0x00ff;	//low-bytes second
        */
        private const ushort ONE_WIRE_PrecedingCheckCode = (0x80f8);
        private const ushort ONE_WIRE_Data_PrecedingCode = (0x80A0);
        private const ushort ONE_WIRE_EEPROM_Seg_PrecedingCode = (0x80D0);
        private const ushort ONE_WIRE_EndCheckCode = (0x70f7);
        public enum Received_Data_Group
        {
            None,
            OneWire_SystemData_Group,
            OneWire_EEPROM_Group
        }
        private static bool data_CRC16_Check(byte[] data_with_crc16, out byte[] data_without_crc)
        {
            byte crc_hi, crc_lo;
            int crc_hi_idx, crc_lo_idx;
            crc_hi_idx = data_with_crc16.Length - 2;
            crc_lo_idx = data_with_crc16.Length - 1;
            crc_hi = data_with_crc16[crc_hi_idx];
            crc_lo = data_with_crc16[crc_lo_idx];
            data_without_crc = new byte[0];
            ushort receiveCRC = (ushort)((crc_hi << 8) + crc_lo);
            byte[] checkData = new byte[data_with_crc16.Length - 2];
            Array.Copy(data_with_crc16, 0, checkData, 0, data_with_crc16.Length - 2);

            ushort crc16 = HM_Utilitys.ComputeModBusCrc16(checkData);
            if (receiveCRC == crc16)
            {
                data_without_crc = (byte[])checkData.Clone();
                return true;
            }
            return false;
        }


        public static void Packet_Decoding_To_Group_And_Clear_List(ref List<byte> RawData, out List<Received_Data_Group> decoding_Group, out List<byte[]> decoding_Data_List)
        {
            int segStartIndex = 0;
            int segEndIndex = 0;
            int dataStartIndex = 0;
            int dataEndIndex = 0;
            byte [] decoding_Data = new byte[0];

            decoding_Group = new List<Received_Data_Group>();
            decoding_Data_List = new List<byte[]>();
            int RemoveEndIndex = 0;


            byte ONE_WIRE_PrecedingCheckCode_HighByte = (byte)(ONE_WIRE_PrecedingCheckCode >> 8);
            byte ONE_WIRE_PrecedingCheckCode_LowByte;
            byte ONE_WIRE_Data_PrecedingCode_HighByte = (byte)(ONE_WIRE_Data_PrecedingCode >> 8);
            byte ONE_WIRE_Data_PrecedingCode_LowByte;
            byte ONE_WIRE_EEPROM_Seg_PrecedingCode_HighByte = (byte)(ONE_WIRE_EEPROM_Seg_PrecedingCode >> 8);
            byte ONE_WIRE_EEPROM_Seg_PrecedingCode_LowByte;
            byte ONE_WIRE_EndCheckCode_HighByte = (byte)(ONE_WIRE_EndCheckCode >> 8);
            byte ONE_WIRE_EndCheckCode_LowByte;
            unchecked
            {
                ONE_WIRE_PrecedingCheckCode_LowByte = (byte)ONE_WIRE_PrecedingCheckCode;
                ONE_WIRE_Data_PrecedingCode_LowByte = (byte)ONE_WIRE_Data_PrecedingCode;
                ONE_WIRE_EEPROM_Seg_PrecedingCode_LowByte = (byte)ONE_WIRE_EEPROM_Seg_PrecedingCode;
                ONE_WIRE_EndCheckCode_LowByte = (byte)ONE_WIRE_EndCheckCode;
            }
            bool StartSrting = false;
            bool EndSrting = false;
            Received_Data_Group status = Received_Data_Group.None;
            byte[] receivingRawData = RawData.ToArray<byte>();
            for (int i = 0; i < receivingRawData.Length - 3; i++)
            {
                if ((StartSrting == false) && (receivingRawData[i] == ONE_WIRE_PrecedingCheckCode_HighByte) && (receivingRawData[i + 1] == ONE_WIRE_PrecedingCheckCode_LowByte))
                {
                    if ((receivingRawData[i + 2] == ONE_WIRE_Data_PrecedingCode_HighByte) &&
                        (receivingRawData[i + 3] == ONE_WIRE_Data_PrecedingCode_LowByte))
                    {
                        status = Received_Data_Group.OneWire_SystemData_Group;
                        dataStartIndex = i + 4;
                        segStartIndex = i;
                        StartSrting = true;
                        //break;
                    }
                    else if ((receivingRawData[i + 2] == ONE_WIRE_EEPROM_Seg_PrecedingCode_HighByte) &&
                             (receivingRawData[i + 3] == ONE_WIRE_EEPROM_Seg_PrecedingCode_LowByte))
                    {
                        status = Received_Data_Group.OneWire_EEPROM_Group;
                        dataStartIndex = i + 4;
                        segStartIndex = i;
                        StartSrting = true;
                        //break;
                    }
                }//if ((receivingRawData[i] == ONE_WIRE_PrecedingCheckCode_HighByte) && (receivingRawData[i + 1] == ONE_WIRE_PrecedingCheckCode_LowByte))
                if ((StartSrting) && (EndSrting == false))
                {
                    if ((receivingRawData[i] == ONE_WIRE_EndCheckCode_HighByte) &&
                        (receivingRawData[i + 1] == ONE_WIRE_EndCheckCode_LowByte) &&
                        (receivingRawData[i + 2] == ONE_WIRE_EndCheckCode_HighByte) &&
                        (receivingRawData[i + 3] == ONE_WIRE_EndCheckCode_LowByte))
                    {
                        dataEndIndex = i - 1;
                        segEndIndex = i + 3;
                        EndSrting = true;
                        //break;
                    }
                }//if ((status == Received_Data_Group.OneWire_SystemData_Group) || (status == Received_Data_Group.OneWire_EEPROM_Group))
                // Packet Form Found
                if (StartSrting && EndSrting)
                {
                    int length = dataEndIndex - dataStartIndex + 1;
                    decoding_Data = new byte[length];
                    Array.Copy(receivingRawData, dataStartIndex, decoding_Data, 0, length);
                    if (data_CRC16_Check(decoding_Data, out decoding_Data))
                    {
                        decoding_Group.Add(status);
                        decoding_Data_List.Add(decoding_Data);
                        RemoveEndIndex = segEndIndex;
                    }
                    status = Received_Data_Group.None;
                    dataEndIndex = 0;
                    dataStartIndex = 0;
                    StartSrting = false;
                    EndSrting = false;
                    segEndIndex = 0;
                    segStartIndex = 0;
                }
            }//for
            
            //Remove found Data
            if (decoding_Group.Count >= 1)
            {
                RawData.RemoveRange(0, RemoveEndIndex + 1);
            }
        }//public static void Packet_Decoding_To_Group_And_Clear_List

        public static Received_Data_Group Packet_Decoding(byte[] receivingRawData, out byte[] decoding_Data, out int segStartIndex, out int segEndIndex)
        {
            segStartIndex = 0;
            segEndIndex = 0;
            int dataStartIndex = 0;
            int dataEndIndex = 0;
            bool formFound = false;
            decoding_Data = new byte[0];

            Received_Data_Group status = Received_Data_Group.None;

            byte ONE_WIRE_PrecedingCheckCode_HighByte = (byte)(ONE_WIRE_PrecedingCheckCode >> 8);
            byte ONE_WIRE_PrecedingCheckCode_LowByte;
            byte ONE_WIRE_Data_PrecedingCode_HighByte = (byte)(ONE_WIRE_Data_PrecedingCode >> 8);
            byte ONE_WIRE_Data_PrecedingCode_LowByte;
            byte ONE_WIRE_EEPROM_Seg_PrecedingCode_HighByte = (byte)(ONE_WIRE_EEPROM_Seg_PrecedingCode >> 8);
            byte ONE_WIRE_EEPROM_Seg_PrecedingCode_LowByte;
            byte ONE_WIRE_EndCheckCode_HighByte = (byte)(ONE_WIRE_EndCheckCode >> 8);
            byte ONE_WIRE_EndCheckCode_LowByte;
            unchecked
            {
                ONE_WIRE_PrecedingCheckCode_LowByte = (byte)ONE_WIRE_PrecedingCheckCode;
                ONE_WIRE_Data_PrecedingCode_LowByte = (byte)ONE_WIRE_Data_PrecedingCode;
                ONE_WIRE_EEPROM_Seg_PrecedingCode_LowByte = (byte)ONE_WIRE_EEPROM_Seg_PrecedingCode;
                ONE_WIRE_EndCheckCode_LowByte = (byte)ONE_WIRE_EndCheckCode;
            }
            for (int i = 0; i < receivingRawData.Length - 4; i++)
            {
                if ((receivingRawData[i] == ONE_WIRE_PrecedingCheckCode_HighByte) && (receivingRawData[i + 1] == ONE_WIRE_PrecedingCheckCode_LowByte))
                {
                    if ((receivingRawData[i + 2] == ONE_WIRE_Data_PrecedingCode_HighByte) &&
                        (receivingRawData[i + 3] == ONE_WIRE_Data_PrecedingCode_LowByte))
                    {
                        status = Received_Data_Group.OneWire_SystemData_Group;
                        dataStartIndex = i + 4;
                        segStartIndex = i;
                        break;
                    }
                    else if ((receivingRawData[i + 2] == ONE_WIRE_EEPROM_Seg_PrecedingCode_HighByte) &&
                             (receivingRawData[i + 3] == ONE_WIRE_EEPROM_Seg_PrecedingCode_LowByte))
                    {
                        status = Received_Data_Group.OneWire_EEPROM_Group;
                        dataStartIndex = i + 4;
                        segStartIndex = i;
                        break;
                    }
                    else
                    {
                        return Received_Data_Group.None;
                    }
                }
            }//for
            if ((status == Received_Data_Group.OneWire_SystemData_Group) || (status == Received_Data_Group.OneWire_EEPROM_Group))
            {
                for (int j = dataStartIndex; j < receivingRawData.Length - 4; j++)
                {
                    if ((receivingRawData[j] == ONE_WIRE_EndCheckCode_HighByte) &&
                        (receivingRawData[j + 1] == ONE_WIRE_EndCheckCode_LowByte) &&
                        (receivingRawData[j + 2] == ONE_WIRE_EndCheckCode_HighByte) &&
                        (receivingRawData[j + 3] == ONE_WIRE_EndCheckCode_LowByte))
                    {
                        formFound = true;
                        dataEndIndex = j - 1;
                        segEndIndex = j + 3;
                        break;
                    }
                }//for
            }//if
            if (formFound)
            {
                int length = dataEndIndex - dataStartIndex + 1;
                decoding_Data = new byte[length];
                Array.Copy(receivingRawData, dataStartIndex, decoding_Data, 0, length);
                if (data_CRC16_Check(decoding_Data, out decoding_Data))
                {
                    return status;
                }
                else
                {
                    decoding_Data = new byte[0];
                    return Received_Data_Group.None;
                }
            }
            return Received_Data_Group.None;
        }//public static Received_Data_Group Packet_Decoding(byte[] receivingRawData, out byte[] decoding_Data)

    }

    public static class LEV_UART_Packet_Forming_and_Decoding
    {
        private const byte LeadingCode = (0x3A);
        private const byte SlaveAddressCode = (0x16);
        private const byte EndCode1 = (0x0D);
        private const byte EndCode2 = (0x0A);
        private const byte Respond_Error_Check_Code = (0xF3);
        private const byte Respond_Accept_Check_Code = (0xF0);
        //========Transmiting Data Structure===========================
        //typedef struct{
        //    static char cStart = 0X3A;	//起始字元
        //    unsigned char 0X16;		//Slave Address
        //    unsigned char Command;		//應回應的命令
        //    unsigned char LenExpected;	//數據長度
        //    unsigned char DataBuf[DATA_BUF_NUM];//數據內容
        //    unsigned char LRCDataLow;	    //checkSum with slave Low byte, included slave address, command, length and data.
        //    unsigned char LRCDataHigh;	//checkSum with slave High byte, included slave address, command, length and data.
        //    static char cEND1= 0X0D;	//結束字元 1 
        //    static char cEND1= 0X0A;	//結束字元 2
        //} LEV Protocol Packet;
        public static byte[] CMD_Forming_For_Transmitting(byte Cmd, byte[] Transmit_Parameter)
        {
            List<byte> TransmitData = new List<byte>();
            byte LenExpected = (byte)Transmit_Parameter.Length;
            TransmitData.Add(SlaveAddressCode);
            TransmitData.Add(Cmd);
            TransmitData.Add(LenExpected);
            TransmitData.AddRange(Transmit_Parameter);


            byte[] CheckBuffer = TransmitData.ToArray<Byte>();
            ushort checkSum = HM_Utilitys.ComputeCheckSum16(CheckBuffer);

            TransmitData.Add((byte)(checkSum));           // low byte     //4 to last byte for packet
            TransmitData.Add((byte)(checkSum >> 8));      // high byte    //3 to last byte for packet
            TransmitData.Add(EndCode1);                //2 to last byte for packet
            TransmitData.Add(EndCode2);                //last byte for packet

            TransmitData.Insert(0, LeadingCode);        // add first Item

            return TransmitData.ToArray<Byte>();
        }

        //=======Receiving Data Structure===============================
        //typedef struct{
        //    static char cStart = 0X3A;	//起始字元
        //    unsigned char 0X16;		    //Slave Address
        //    unsigned char Command;		//命令
        //    unsigned char LenExpected;	//數據長度
        //    unsigned char DataBuf[DATA_BUF_NUM];//數據內容
        //    unsigned char LRCDataLow;	    //checkSum with slave Low byte, included slave address, command, length and data.
        //    unsigned char LRCDataHigh;	//checkSum with slave High byte, included slave address, command, length and data.
        //    static char cEND1= 0X0D;	//結束字元 1 
        //    static char cEND1= 0X0A;	//結束字元 2
        //} LEV Protocol Packet;
        public static bool CMD_Decoding_For_Receiving(byte[] receivingRawData, out byte ReceivedCommand, out byte[] decoding_Parameter)
        {
            bool IsFound_Packet_Form = false;

            ReceivedCommand = 0;
            decoding_Parameter = new byte[0];
            List<byte> ReceivedDataList = new List<byte>();

            int startFormIndex;
            int dataLength, endCode1_idx, endCode2_idx;
            for (int i = 0; i < (receivingRawData.Length - 1); i++)
            {
                //finding leading codes
                if ((receivingRawData[i] == LeadingCode) && (receivingRawData[i + 1] == SlaveAddressCode))
                {
                    startFormIndex = i;
                    dataLength = receivingRawData[startFormIndex + 3];    //offset 3 to get receiving data length
                    endCode1_idx = startFormIndex + 3 + dataLength + 2 + 1;  //add offset 2 is two of Cuecksum bytes
                    endCode2_idx = startFormIndex + 3 + dataLength + 2 + 2;  //add offset 2 is two of Cuecksum bytes
                    //finding Ending Codes
                    if ((receivingRawData[endCode1_idx] == EndCode1) && (receivingRawData[endCode2_idx] == EndCode2))
                    {
                        //calculate checkSum
                        byte[] CheckBuffer = new byte[dataLength + 3]; //included slave address, command, length and data.
                        Array.Copy(receivingRawData, startFormIndex + 2, CheckBuffer, 0, CheckBuffer.Length);
                        ushort checkSum = HM_Utilitys.ComputeCheckSum16(CheckBuffer);
                        int checkSumLowByte_Index = endCode1_idx - 2;
                        int checkSumHighByte_Index = checkSumLowByte_Index + 1;
                        if (receivingRawData[checkSumLowByte_Index] == ((byte)(checkSum)) && receivingRawData[checkSumHighByte_Index] == ((byte)(checkSum >> 8)))
                        {
                            //Save data to structure
                            ReceivedCommand = receivingRawData[startFormIndex + 2];

                            for (byte j = 0; j < dataLength; j++)
                            {
                                ReceivedDataList.Add(receivingRawData[startFormIndex + 4 + j]);
                            }
                            IsFound_Packet_Form = true;
                            break;
                        }
                    }//if((ReceivingData[endCode1_idx] == EndingCode1) && (ReceivingData[endCode2_idx] == EndingCode2)){
                }//if((ReceivingData[i] == LeadingCode) && (ReceivingData[i+1] == SlaveAddressCode)){
            }//for(int i = 0; i < (ReceivingData.Length - 1); i++){
            if (IsFound_Packet_Form)
            {
                decoding_Parameter = ReceivedDataList.ToArray<byte>();
            }
            return IsFound_Packet_Form;
        }
    }

}
