using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Instrument
{
    class MT8872 : NIVisaCmd, IRFInstrument
    {
        public ulong[] BT_FREQ = new ulong[79];
        private const string BT_HIT_ADDRESS = @"000000c6967e";
        public enum EQUIPMENT_QUERY
        {
            SUM_CW_POWER = 1,
            SUM_CW_FREQ,
            SUM_POWER,
            SUM_PRAMP,
            SUM_DSSS_EVM,
            SUM_OFDM_EVM,
            SUM_OFDM_AEVM,
            SUM_SPEC_NUM,
            SUM_SPEC_VIOL,
            SUM_BT_POWER,
            SUM_BT_ERTP,
            SUM_BLE_POWER,
            SUM_BT_EVM_MOD,
            SUM_BLE_EVM_MOD,
            SUM_BT_EVM_ICFT,
            SUM_BT_EVM_CDR,
            SUM_BLE_EVM_CDR,
            SUM_BT_EVM_ECM,
            SUM_BT_FRAN,
            SUM_BT_BAND,
            SUM_BT_SPEM_ACP,
            SUM_WLAN_PRAMS
        }

        public S_STANDARD_PARAM_VSA mGlobalParam;

        public void Init(string visaPath)
        {
            try
            {
                this.InitBTTable();
                string result = "";

                this.Close();
                this.OpenDevice(visaPath);
                this.StopCapture();
                this.WSCPI(@":CONFigure:SRWireless:SEGMent:CLEar");
                this.WSCPI(@"*CLS");

                result = this.QSCPI(@"*ESR?", 1);
                if (!result.Equals(@"0"))
                {
                    throw new Exception(@"*ESR? command must return 0 but return " + result + " error !!!");
                }

                this.WSCPI(@"SYST:LANG SCPI");

                result = this.QSCPI(@"*OPC?", 1);
                if (!result.Equals(@"1"))
                {
                    throw new Exception(@"*OPC? command must return 1 but return " + result + " error !!!");
                }

                result = this.QSCPI(@"*ESR?", 1);
                if (!result.Equals(@"0"))
                {
                    throw new Exception(@"*ESR? command must return 0 but return " + result + " error !!!");
                }

                result = this.QSCPI(@"SYST:ERR:COUN?", 1);
                if (!result.Equals(@"0")) 
                {
                    throw new Exception(@"SYST:ERR:COUN? command must return 0 but return " + result + " error !!!");
                }

                result = this.QSCPI(@"*ESR?", 1);
                if (!result.Equals(@"0"))
                {
                    throw new Exception(@"*ESR? command must return 0 but return " + result + " error !!!");
                }

                int errorCode = 0;
                result = QSCPIReadAllError(ref errorCode);
                if (!result.Equals(@"No error"))
                {
                    throw new Exception(@"*QSCPIReadAllError must return 'No error' but return " + result + "( " + errorCode + " ) error !!!");
                }

                result = this.QSCPI(@"*ESR?", 1);
                if (!result.Equals(@"0"))
                {
                    throw new Exception(@"*ESR? command must return 0 but return " + result + " error !!!");
                }

                this.WSCPI(@"INST:SEL SRW");

                result = this.QSCPI(@"*OPC?", 1);
                if (!result.Equals(@"1"))
                {
                    throw new Exception(@"*OPC? command must return 1 but return " + result + " error !!!");
                }

                result = this.QSCPI(@"*ESR?", 1);
                if (!result.Equals(@"0"))
                {
                    throw new Exception(@"*ESR? command must return 0 but return " + result + " error !!!");
                }

                result = "";
                result = this.QSCPI(@"SYST:VERS?", 8);
                if (result.Length == 0)
                {
                    throw new Exception(@"*SYST:VERS? command must len > 0 , but null error !!!");
                }

                result = "";
                result = this.QSCPI(@"SYST:INF:MAIN:DEV:ID?", 62);
                if (result.Length == 0)
                {
                    throw new Exception(@"*ESR? command must return 0 but return " + result + " error !!!");
                }

                this.WSCPI(@"SYST:RF:MODE NORMAL");

                result = this.QSCPI(@"SYST:RF:MODE?", 48);
                if (!result.Equals(@"NORMAL"))
                {
                    throw new Exception(@"*SYST:RF:MODE? command must return NORMAL but return " + result + " error !!!");
                }

                result = this.QSCPI(@"*IDN?", 48);
                if (result.Length == 0)
                {
                    throw new Exception(@"*IDN? command must len > 0 , but null error !!!");
                }

                this.WSCPI(@"CALC:EXTL:TABLE:SETT 1");
                this.WSCPI(@"CALC:EXTL:TABLE:DEL");
                this.WSCPI(@"CALC:EXTL:TABL:VAL:ALL 2412MHZ, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0");
                this.WSCPI(@"EXTL:TABL:SWIT ON");
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public void GetEquipmentMeasureFreqOffsetAndPowerByCW(UInt16 channel, ref OUT_CW_MEASURE_RESULT result)
        {
            try
            {
                mGlobalParam.uiStandardIndex = EQUIPMENT_STANDARD.INSTR_CW;
                mGlobalParam.ulFrequency = this.BT_FREQ[channel];
                SetTestMode();
                InitCrystalCalSettings("PORT1", 0.01);
                StartMeasure(MEASURE_STANDARD_TYPE.BT_TYPE);
                CWAutoLevel();
                CaptureCWResult(ref result);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetEquipmentBTMeasure(string payloadType, ushort channel, ref OUT_BT_MEASURE_RESULT result)
        {
            try
            {
                mGlobalParam.uiStandardIndex = EQUIPMENT_STANDARD.INSTR_BT;
                mGlobalParam.ulFrequency = this.BT_FREQ[channel];
                mGlobalParam.cstrBTAddr = BT_HIT_ADDRESS;
                mGlobalParam.cstrBTPack = payloadType;
                mGlobalParam.cstrBTPayl = @"PRBS9";
                //mGlobalParam.cstrBTAddr = 

                SetTestMode();
                InitAutoBTSettings(mGlobalParam.uiPort);
                StartMeasure(MEASURE_STANDARD_TYPE.BT_TYPE);
                AutoBTLevel();
                CaptureBT(ref result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetLossCable(List<string> table)
        {          
            try
            {
                ResetCableLoss();

                foreach (var s in table)
                {
                    this.WSCPI(@"CALC:EXTL:TABL:VAL:ALL " + s);
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ResetCableLoss()
        {
            try
            {
                this.WSCPI(@"CALC:EXTL:TABLE:SETT 1");
                this.WSCPI(@"CALC:EXTL:TABLE:DEL");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void Close()
        //{
        //    this.CloseDevice();
        //}

        public void Init_RX_Settings(ulong freq, double maxDBM, double minDBM, string port, string waveFile, uint packetSize)
        {
            try
            {
                this.WSCPI(@"CALC:EXTL:TABL:SETT 1");
                string msg = this.QSCPI(@"*OPC?", 1);
                if (!msg.Equals(@"1"))
                {
                    throw new Exception(@"*OPC? command must return 1 but return " + msg + @" error !!!");
                }
                this.WSCPI(@"ROUT:PORT:CONN:DIR PORT1,PORT1");
                this.WSCPI(@"SOUR:GPRF:GEN:MODE NORMAL");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:REIN ON");
                this.WSCPI(@"SOUR:GPRF:GEN:BBM CW");

                this.WSCPI(@"SOUR:GPRF:GEN:RFS:FREQ %dMHZ" + freq);
                this.WSCPI(@"SOUR:GPRF:GEN:RFS:LEV -120.0");
                this.WSCPI(@"SOUR:GPRF:GEN:ARB:NOIS:STAT OFF");
                this.WSCPI(@"SOUR:GPRF:GEN:RFS:DM:POL NORMAL");

                bool isAlreadyLoad = false;
                string param = string.Format(@"SOUR:GPRF:GEN:ARB:FILE:LOAD? '{0}'", waveFile);
                msg = this.QSCPI(param, 1);
                if (msg.Equals(@"0"))
                {
                    isAlreadyLoad = true;
                }
                else if (msg.Equals(@"1"))
                {
                    isAlreadyLoad = false;
                }
                else if (msg.Equals(@"2"))
                {
                    throw new Exception(@"InitWLAN_RX_Settings " + param + @" Fail: No license !!!");
                }
                else if (msg.Equals(@"3"))
                {
                    throw new Exception(@"InitWLAN_RX_Settings " + param + " Fail: No Support WaveForm File !!!");
                }

                if (!isAlreadyLoad)
                {
                    this.WSCPI(string.Format(@"SOUR:GPRF:GEN:ARB:FILE:LOAD '{0}'", waveFile));
                    if (!QueryRXArbFileLoad())
                    {
                        throw new Exception(@"InitWLAN_RX_Settings " + param + " Fail: Can't Load File !!");
                    }
                }

                this.WSCPI(string.Format(@"SOUR:GPRF:GEN:ARB:WAV:PATT:SEL '{0}', 1, 1", waveFile));
                this.QSCPI(@"SOUR:GPRF:GEN:ARB:WAV:SCL:RATE?", 1022);

                isAlreadyLoad = false;
                msg = this.QSCPI(@"SOUR:GPRF:GEN:ARB:FILE:LOAD? 'ZERO_200000000Hz_100000p'", 1);
                if (msg.Equals(@"0"))
                {
                    isAlreadyLoad = true;
                }
                else if (msg.Equals(@"1"))
                {
                    isAlreadyLoad = false;
                }
                else if (msg.Equals(@"2"))
                {
                    throw new Exception(@"InitWLAN_RX_Settings " + param + @" Fail: No license !!!");
                }
                else if (msg.Equals(@"3"))
                {
                    throw new Exception(@"InitWLAN_RX_Settings " + param + " Fail: No Support WaveForm File !!!");
                }

                if (!isAlreadyLoad)
                {
                    this.WSCPI(@"SOUR:GPRF:GEN:ARB:FILE:LOAD 'ZERO_200000000Hz_100000p'");
                    if (!QueryRXArbFileLoad())
                    {
                        throw new Exception(@"InitWLAN_RX_Settings " + param + " Fail: Can't Load File !!");
                    }
                }

                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:RX:GEN:SST 1, 1, 2");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:RX:GEN:REP 1, SINGLE");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:RX:GEN:GOTO 1, 1");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:RX:ENDC 1, 1, TRIGGER");

                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:RX:TRIG:SOUR 1, 1, WFGEND");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:RX:TRIG:DEL 1, 1, 0.000");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:RX:NSLC 1, 1, NSEGMENT");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:RX:BBM 1, 1, ARB");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:RX:WCTR 1, 1, OFF");

                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:PATT:SEL 1, 1, 'ZERO_200000000Hz_100000p', 1");
                this.WSCPI(string.Format(@"SOUR: GPRF:GEN: SEQ:WAV: PATT:SEL 1, 2, '{0}'", waveFile));
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:PATT:SEL 1, 3, 'ZERO_200000000Hz_100000p', 1");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:PATT:DEL 1, 4");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:ENDC 1, 1, REPEAT");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:ENDC 1, 2, REPEAT");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:ENDC 1, 3, REPEAT");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:IREP 1, 1, 1");

                this.WSCPI(string.Format(@"SOUR:GPRF:GEN:SEQ:WAV:IREP 1, 2, {0}", packetSize));
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:IREP 1, 3, 1");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:GETR 1, 1, 0");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:GETR 1, 2, 1");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:WAV:GETR 1, 3, 0");

                this.WSCPI(string.Format(@"SOUR:GPRF:GEN:SEQ:RX:FREQ 1, 1, {0}MHZ", freq));
                this.WSCPI(string.Format(@"SOUR:GPRF:GEN:SEQ:RX:LEV 1, 1, {0:0.0}DBM", maxDBM));
                this.WSCPI(string.Format(@"SOUR:GPRF:GEN:SEQ:RX:LEV 1, 2, {0:0.0}DBM", minDBM));
                this.WSCPI(string.Format(@"SOUR:GPRF:GEN:SEQ:RX:OUTP:STAT 1, 1, {0}", port));
                this.WSCPI(string.Format(@"SOUR:GPRF:GEN:SEQ:RX:OUTP:STAT 1, 2, {0}", port));

                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:COMB:PATT 1, 1");
                this.WSCPI(@"SOUR:GPRF:GEN:SEQ:COMB:PATT:SYNC NONE");
                this.WSCPI(@"SOUR:GPRF:GEN:MODE SEQUENCE");

                msg = this.QSCPI(@"SOUR:GPRF:GEN:SEQ:RX:ERR? 1", 1022);
                if (!msg.Equals(@"0,0,0"))
                {
                    throw new Exception(@"InitWLAN_RX_Settings SOUR:GPRF:GEN:SEQ:RX:ERR? 1   Fail: Output not 0,0,0 , real Output: " + msg);
                }

                this.WSCPI(@"SOUR:GPRF:GEN:STAT ON");
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }



        // =============================

        private void InitBTTable() 
        {
            try
            {
                for (int i = 0; i < 79; i++)
                {
                    BT_FREQ[i] = Convert.ToUInt64(2402 + i);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        private void StopCapture() 
        {
            try
            {
                this.WSCPI(@"ABOR:SRW");
                this.QueryMeasuermentStatus();
                this.WSCPI(@":CONFigure:SRWireless:SEGMent:CLEar");
                this.WSCPI(@"*CLS");

                this.QSCPI(@"*ESR?");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void QueryMeasuermentStatus() 
        {
            try
            {
                int catchResult = 0;
                const int retryLimit = 30;
                int retry = 0;

                do
                {

                    catchResult = 0;
                    string r = this.QSCPI(@"STAT:SRW:MEAS?", 1);
                    if (r.Length > 0)
                        catchResult = Convert.ToInt32(r);
                    else
                        catchResult = 0;
                    retry++;
                    System.Threading.Thread.Sleep(10);
                } while (catchResult != 1 && retry <= retryLimit);

                if (catchResult != 1)
                {
                    throw new Exception(@"QueryMeasuermentStatus catchResult != 1");
                }
                //this.QSCPI(@"*CLS");              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetTestMode()
        {
            try
            {
                InitCalTable1Loss();
                this.WSCPI(@"CONF:SRW:SEGM:CLE");
                this.WSCPI(@"CONF:SRW:SEGM:APP " + ConvertIndexToStandardStr(mGlobalParam.uiStandardIndex));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void InitCalTable1Loss()
        {
            try
            {
                this.WSCPI(@"CALC:EXTL:TABL:SETT %d");

                string msg = this.QSCPI(@"*OPC?", 1);
                if (!msg.Equals(@"1"))
                {
                    throw new Exception(@"*OPC? command must return 1 but return " + msg + " error !!!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        private string ConvertIndexToStandardStr(EQUIPMENT_STANDARD uiStandardIndex)
        {
            try
            {
                string str = "";
                switch (uiStandardIndex)
                {
                    case EQUIPMENT_STANDARD.INSTR_WLANSTD_A: str = @"WLA"; break;
                    case EQUIPMENT_STANDARD.INSTR_WLANSTD_B: str = @"WLB"; break;
                    case EQUIPMENT_STANDARD.INSTR_WLANSTD_G: str = @"WLG"; break;
                    case EQUIPMENT_STANDARD.INSTR_WLANSTD_2_4N:
                    case EQUIPMENT_STANDARD.INSTR_WLANSTD_5N: str = @"WLN"; break;
                    case EQUIPMENT_STANDARD.INSTR_WLANSTD_AC: str = @"WLAC"; break;
                    case EQUIPMENT_STANDARD.INSTR_WLANSTD_AX: str = @"WLAX"; break;
                    case EQUIPMENT_STANDARD.INSTR_CW: str = @"CW"; break;
                    case EQUIPMENT_STANDARD.INSTR_BT: str = @"BT"; break; //cstrStr =	_T("BT");	break;
                    case EQUIPMENT_STANDARD.INSTR_BLE: str = @"BLE"; break; // BLE
                    case EQUIPMENT_STANDARD.INSTR_AUTODSSS: str = @"AUTODSSS"; break;
                    case EQUIPMENT_STANDARD.INSTR_AUTOOFDM: str = @"AUTOOFDM"; break;
                    case EQUIPMENT_STANDARD.INSTR_AUTOBT: str = @"AUTOBT"; break;
                    default: break;
                }
                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void InitCrystalCalSettings(string port, double levelingTime)
        {
            try
            {
                SetTriggerTimeout();
                SetFreq(mGlobalParam.ulFrequency);
                SetSEGMPort(port);
                SetAutomaticLevelingTime(levelingTime);
                SetDbmPower(mGlobalParam.dCWPower);
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        private void SetTriggerTimeout(byte time = 10)
        {
            try
            {
                this.WSCPI(@"CONF:SRW:TTIM " + time.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        private void SetFreq(ulong freq)
        {
            try
            {
                this.WSCPI(@"CONF:SRW:FREQ " + freq + "MHZ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetSEGMPort(string port)
        {
            try
            {
                this.WSCPI(@"CONF:SRW:SEGM:PORT " + port);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetAutomaticLevelingTime(double time)
        {
            try
            {
                this.WSCPI(@"CONF:SRW:ALEV:TIME " + time);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetDbmPower(double power)
        {
            try
            {
                this.WSCPI(@"CONF:SRW:POW " + power);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void StartMeasure(MEASURE_STANDARD_TYPE uiType)
        {
            try
            {
                if (uiType.Equals(MEASURE_STANDARD_TYPE.WIFI_TYPE))
                    EnableSetting_WLAN();
                else if (uiType.Equals(MEASURE_STANDARD_TYPE.BT_TYPE))
                    EnableSetting_BT();
                else if (uiType.Equals(MEASURE_STANDARD_TYPE.BLE_TYPE))
                    EnableSetting_BLE();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void EnableSetting_WLAN()
        {
            try
            {
                if (mGlobalParam.uiStandardIndex.Equals(EQUIPMENT_STANDARD.INSTR_CW))
                {
                    EnableCWFrequencyMeas(true);
                    EnableCWTxPowerMeas(true);
                }
                else
                {
                    EnableTxPowerMeas_WLAN(true);
                    EnableTxPRAMS(true);
                    EnableEVMMeas(true);
                    EnableTxSpecNum(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void EnableCWFrequencyMeas(bool bEnable)
        {
            try
            {
                this.WSCPI(String.Format(@"CONF:SRW:SEL:CW:FREQ {0}", bEnable ? @"ON" : @"OFF"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void EnableCWTxPowerMeas(bool bEnable)
        {
            try
            {
                this.WSCPI(String.Format(@"CONF:SRW:SEL:CW:POW {0}", bEnable ? @"ON" : @"OFF"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void EnableTxPowerMeas_WLAN(bool bEnable)
        {
            try
            {
                this.WSCPI(String.Format(@"CONF:SRW:SEL:WLAN:POW {0}", bEnable ? @"ON" : @"OFF"));
                this.WSCPI(@"CONF:SRW:WLAN:POW:GATE:ADD 1");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void EnableTxPRAMS(bool bEnable)
        {
            try
            {
                this.WSCPI(String.Format(@":CONF:SRW:SEL:WLAN:PRAM {0}", bEnable ? @"ON" : @"OFF"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void EnableEVMMeas(bool bEnable)
        {
            try
            {
                this.WSCPI(String.Format(@"CONF:SRW:SEL:WLAN:EVM {0}", bEnable ? @"ON" : @"OFF"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void EnableTxSpecNum(bool bEnable)
        {
            try
            {
                this.WSCPI(String.Format(@"CONF:SRW:SEL:WLAN:SPEC:NUM {0}", bEnable ? @"ON" : @"OFF"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void EnableSetting_BT()
        {
            try
            {
                if (mGlobalParam.uiStandardIndex.Equals(EQUIPMENT_STANDARD.INSTR_CW))
                {
                    EnableCWFrequencyMeas(true);
                    EnableCWTxPowerMeas(true);
                }
                else
                {
                    this.WSCPI(@"CONF:SRW:SEL:BT:POW ON");
                    this.WSCPI(@"CONF:SRW:SEL:BT:FRAN ON");
                    this.WSCPI(@"CONF:SRW:SEL:BT:BAND ON");
                    this.WSCPI(@"CONF:SRW:SEL:BT:ACP ON");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void EnableSetting_BLE()
        {
            try
            {               
                this.WSCPI(@"CONF:SRW:SEL:BLE:POW ON");
                this.WSCPI(@"CONF:SRW:SEL:BLE:IBEM ON");
                this.WSCPI(@"CONF:SRW:SEL:BLE:MOD ON");
                this.WSCPI(@"CONF:SRW:SEL:BLE:CDR ON");               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CWAutoLevel()
        {
            try
            {
                this.WSCPI(@"INIT:SRW:ALEV");
                this.WSCPI(@"*WAI");

                QueryMeasuermentStatus();
                this.QSCPI(@"CONF:SRW:POW?", 3);

                this.WSCPI(@"INIT:SRW");
                this.WSCPI(@"*WAI");

                QueryMeasuermentStatus();
                this.QSCPI(@"FETC:SRW:CINF?", 30);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CaptureCWResult(ref OUT_CW_MEASURE_RESULT result)
        {
            try
            {
                result.Clear();
                string str = QueryMeasureSumItemValue(1, EQUIPMENT_QUERY.SUM_CW_POWER, 30);
                result.power = Convert.ToDouble(str);

                str = QueryMeasureSumItemValue(1, EQUIPMENT_QUERY.SUM_CW_FREQ, 30);
                result.freq = Convert.ToDouble(str);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string QueryMeasureSumItemValue(ulong ulSegmentIndex, EQUIPMENT_QUERY queryType, int catchSize) 
        {
            try
            {
                string cmd = "";
                switch (queryType)
                {
                    case EQUIPMENT_QUERY.SUM_CW_POWER: cmd = String.Format(@"FETC:SRW:SUMM:CW:POW? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_CW_FREQ: cmd = String.Format(@"FETC:SRW:SUMM:CW:FREQ? 1, {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_POWER: cmd = String.Format(@"FETC:SRW:SUMM:WLAN:POW? 1, {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_PRAMP: cmd = String.Format(@"FETC:SRW:SUMM:WLAN:DSSS:PRAM? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_DSSS_EVM: cmd = String.Format(@"FETC:SRW:SUMM:WLAN:DSSS:EVM? 1, {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_OFDM_EVM: cmd = String.Format(@"FETC:SRW:SUMM:WLAN:OFDM:EVM? 1, {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_OFDM_AEVM: cmd = String.Format(@"FETC:SRW:SUMM:WLAN:OFDM:AEVM? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_SPEC_NUM: cmd = String.Format(@"FETC:SRW:SUMM:WLAN:SPEC:NUM? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_SPEC_VIOL: cmd = String.Format(@":FETC:SRW:PACK:WLAN:SPEC:NUM? 1,1, {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BT_POWER: cmd = String.Format(@"FETC:SRW:SUMM:BT:POW? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BT_ERTP: cmd = String.Format(@"FETC:SRW:SUMM:BT:ERTP? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BLE_POWER: cmd = String.Format(@"FETC:SRW:SUMM:BLE:POW? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BT_EVM_MOD: cmd = String.Format(@"FETC:SRW:SUMM:BT:MOD? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BLE_EVM_MOD: cmd = String.Format(@"FETC:SRW:SUMM:BLE:MOD? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BT_EVM_ICFT: cmd = String.Format(@"FETC:SRW:SUMM:BT:ICFT? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BT_EVM_CDR: cmd = String.Format(@"FETC:SRW:SUMM:BT:CDR? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BLE_EVM_CDR: cmd = String.Format(@"FETC:SRW:SUMM:BLE:CDR? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BT_EVM_ECM: cmd = String.Format(@"FETC:SRW:SUMM:BT:ECM? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BT_FRAN: cmd = String.Format(@"FETC:SRW:SUMM:BT:FRAN? 1, {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BT_BAND: cmd = String.Format(@"FETC:SRW:SUMM:BT:BAND? 1, {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_BT_SPEM_ACP: cmd = String.Format(@"FETC:SRW:SUMM:BT:ACP? {0}", ulSegmentIndex); break;
                    case EQUIPMENT_QUERY.SUM_WLAN_PRAMS: cmd = String.Format(@":FETC:SRW:SUMM:WLAN:PRAM? 1"); break;
                    default: break;
                }

                string msg = this.QSCPI(cmd, catchSize);
                if (msg.Length == 0)
                {
                    throw new Exception("QueryMeasureSumItemValue Fail, Type : " + queryType.ToString());
                }

                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        private void InitAutoBTSettings(string port)
        {
            try
            {
                SetTriggerTimeout(2);
                SetTriggerMode(MEASURE_TRIGGER_MODE.TRIGGER_MODE_LEVEL);
                SetTriggerDelayTime("-1E-05");
                SetFreq(mGlobalParam.ulFrequency);
                SetSEGMPort(port);
                SetAutomaticLevelingTime(0.01);
                SetBT_Param();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetTriggerMode(MEASURE_TRIGGER_MODE mode)
        {
            try
            {
                string triggerMode = "";
                if (mode == MEASURE_TRIGGER_MODE.TRIGGER_MODE_LEVEL)
                {
                    triggerMode = "LEVEL";
                }
                else
                {
                    triggerMode = "IMMEDIATE";
                }

                this.WSCPI(string.Format(@"CONF:SRW:TRIG {0}", triggerMode));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetTriggerDelayTime(string delay)
        {
            try
            {
                this.WSCPI(string.Format(@"CONF:SRW:TDEL {0}", delay));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetBT_Param()
        {
            try
            {
                this.WSCPI(string.Format(@"CONF:SRW:BT:ADDR \{0}", mGlobalParam.cstrBTAddr));
                this.WSCPI("CONF:SRW:BT:MODE SPEED");

                this.WSCPI(string.Format(@"CONF:SRW:BT:PACK {0}", mGlobalParam.cstrBTPack));
                this.WSCPI("CONF:SRW:BT:PLEN 27");

                this.WSCPI(string.Format(@"CONF:SRW:BT:PAYL {0}", mGlobalParam.cstrBTPayl));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void AutoBTLevel()
        {
            try
            {
                this.WSCPI(@"INIT:SRW:ALEV");
                QueryMeasuermentStatus();
                this.QSCPI(@"CONF:SRW:POW?", 3);
                this.WSCPI(@"INIT:SRW");
                QueryMeasuermentStatus();
                Thread.Sleep(10);
                this.QSCPI(@"FETC:SRW:CINF? 1", 318);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CaptureBT(ref OUT_BT_MEASURE_RESULT result)
        {
            try
            {
                GetBT_Result("BT_ALL", ref result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void GetBT_Result(string pszItemStr, ref OUT_BT_MEASURE_RESULT result)
        {
            try
            {
                result.Clear();
                if (pszItemStr.Equals("BT_ALL"))
                {
                    string value = QueryMeasureSumItemValue(1, EQUIPMENT_QUERY.SUM_BT_POWER, 62);
                    if (0 != value.Length)
                    {
                        result.power.power = value;
                        GetBTPower(ref result);
                    }

                    //value = QueryMeasureSumItemValue(0, EQUIPMENT_QUERY.SUM_BT_FRAN, 3998);
                    //if (0 != value.Length)
                    //{
                    //    result.fran->fran = value;
                    //    GetBTFRAN(result);
                    //}

                    /*value = QueryMeasureSumItemValue(0, EQUIPMENT_QUERY.SUM_BT_BAND, 2046);
                    if (0 != value.Length) {
                        result.band->band = value;
                        GetBTBand(result);
                    }

                    value = QueryMeasureSumItemValue(1, EQUIPMENT_QUERY.SUM_BT_SPEM_ACP, 254);
                    if (0 != value.Length) {
                        result.acp->acp = value;
                        GetBTACP(result);
                    }*/
                }
                else if (pszItemStr.Equals("BLE_ALL"))
                {
                    string value = QueryMeasureSumItemValue(1, EQUIPMENT_QUERY.SUM_BLE_POWER, 126);
                    if (0 != value.Length)
                    {
                        result.power.power = value;
                        GetBTPower(ref result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void GetBTPower(ref OUT_BT_MEASURE_RESULT result)
        {
            try
            {
                string[] splits = result.power.power.Split(',');
                if (splits.Length < 8) {
                    throw new Exception(@"GetBTPower Get Str : " + result.power.power + @" , Index Not Enough 8 Len");
                }

                result.power.peakPower = Convert.ToDouble(splits[3].Trim());
                result.power.avgPower = Convert.ToDouble(splits[2].Trim());
                result.power.minPower = Convert.ToDouble(splits[7].Trim());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool QueryRXArbFileLoad()
        {
            int catchResult = 0;
            const int retryLimit = 30;
            int retry = 0;

            do
            {
                catchResult = 0;
                string r = this.QSCPI(@"SOUR:GPRF:GEN:ARB:FILE:LOAD:STAT?", 1);
                if (null != r)
                    catchResult = Convert.ToInt32(r);
                else
                    catchResult = 1;
                retry++;
                Thread.Sleep(100);
            } while (catchResult != 0 && retry <= retryLimit);

            if (catchResult != 0)
            {
                return false;
            }
            return true;
        }
    }
}
