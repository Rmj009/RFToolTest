using System;
using System.Collections.Generic;
using System.Text;

namespace Instrument
{
    public class OUT_BT_POWER 
    {
        public string power;
        public double peakPower;
        public double avgPower;
        public double minPower;

        public OUT_BT_POWER() 
        {
            power = "";
            peakPower = 0.0;
            avgPower = 0.0;
            minPower = 0.0;
        }
    }

    public class OUT_BT_FRAN
    {
        public string fran;
        public double centerFreq;
        public double lowFreq;
        public double highFreq;

        public OUT_BT_FRAN()
        {
            fran = "";
            centerFreq = 0.0;
            lowFreq = 0.0;
            highFreq = 0.0;
        }
    }

    //public class OUT_BT_BAND
    //{
    //    string band;
    //    int db20Band;
    //    int specBand;

    //    public OUT_BT_BAND()
    //    {
    //        band = "";
    //        db20Band = 0;
    //        specBand = 0;
    //    }
    //}

    //public class OUT_BT_ACP
    //{
    //    string acp;
    //    double channel_sub_5;
    //    double channel_sub_4;
    //    double channel_sub_3;
    //    double channel_sub_2;
    //    double channel_sub_1;
    //    double channel;
    //    double channel_add_5;
    //    double channel_add_4;
    //    double channel_add_3;
    //    double channel_add_2;
    //    double channel_add_1;

    //    public OUT_BT_ACP()
    //    {
    //        acp = "";
    //        channel_sub_5 = 0.0;
    //        channel_sub_4 = 0.0;
    //        channel_sub_3 = 0.0;
    //        channel_sub_2 = 0.0;
    //        channel_sub_1 = 0.0;
    //        channel = 0.0;
    //        channel_add_5 = 0.0;
    //        channel_add_4 = 0.0;
    //        channel_add_3 = 0.0;
    //        channel_add_2 = 0.0;
    //        channel_add_1 = 0.0;
    //    }
    //}

    public class OUT_BT_MEASURE_RESULT 
    {
        public OUT_BT_POWER power;
        public OUT_BT_FRAN fran;
        //public OUT_BT_BAND band;
        //public OUT_BT_ACP acp;

        public void Clear()
        {
            power = null;
            fran = null;
            //band = null;
            //acp = null;
        }
    }

    public class OUT_CW_MEASURE_RESULT
    {
        public double power;
        public double freq;

        public void Clear()
        {
            power = 0.0;
            freq = 0.0;
        }
    }

    public enum EQUIPMENT_STANDARD
    {
        INSTR_WLANSTD_B = 1,
        INSTR_WLANSTD_G,
        INSTR_WLANSTD_A,
        INSTR_WLANSTD_2_4N,
        INSTR_WLANSTD_5N,
        INSTR_WLANSTD_AC,
        INSTR_WLANSTD_AX,
        INSTR_CW,
        INSTR_BT,
        INSTR_BLE,
        INSTR_AUTODSSS,
        INSTR_AUTOOFDM,
        INSTR_AUTOBT,
        INSTR_GPS,
        INSTR_GLONASS,
    }
    public enum MEASURE_STANDARD_TYPE { WIFI_TYPE = 0, BT_TYPE, BLE_TYPE, GPS_TYPE, };
    public enum MEASURE_TRIGGER_MODE { TRIGGER_MODE_LEVEL = 0, TRIGGER_MODE_IMMEDIATE };

    public class S_STANDARD_PARAM_VSA
    {
        public uint uiCalType;
        public EQUIPMENT_STANDARD uiStandardIndex;
        public string uiPort;
        //uint uiPacketCount;
        //uint uiPortIndex;
        //uint uiFilterType;
        //uint uiPPDUType;
        public ulong ulFrequency;
        //double dDurationTime;
        //double dPowerLV;
        //double dGaussianFilterBTLV;
        public double dCWPower;
        //bool bCaptureMode;
        //bool bTriggerMode;
        //bool bEVMSetting;
        //bool bPPDUFormatType;
        //bool bGuardIntervalType;
        //bool bSpectrumType;
        //string cstrDataRate_MCS;
        //string cstrPPDUType;

        // ----- BT
        public string cstrBTAddr;
        public string cstrBTPack;
        public string cstrBTPayl;
    }

    interface IRFInstrument
    {
        void Init(String visaPath);
        void GetEquipmentMeasureFreqOffsetAndPowerByCW(UInt16 channel, ref OUT_CW_MEASURE_RESULT result);
        void GetEquipmentBTMeasure(String payloadType, UInt16 channel ,ref OUT_BT_MEASURE_RESULT result);
        void SetLossCable(List<string> table);
        void ResetCableLoss();
        void Close();


        void Init_RX_Settings(ulong freq, double maxDBM, double minDBM, string port, string waveFile, uint packetSize);
      //  void InitWLAN_RX_Settings(ulong freq, double maxDBM, double minDBM, string port, string waveFile, uint packetSize);
        //void CheckAfterCrystalPowerItem(string key);
        //void CheckBTTxItem();
        //void CheckBTRxItem();
    }
}
