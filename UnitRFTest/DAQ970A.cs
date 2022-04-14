using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instrument
{
    class DAQ970A : Instrument.NIVisaCmd
    {
        private string mChannel = "101";
        private int mDelay = 150;

        public string Channel {
            get {
                return mChannel;
            }

            set {
                this.mChannel = value;
            }
        }

        public int Delay {
            get {
                return mDelay;
            }

            set {
                mDelay = value;
            }
        }

        public class OUT_DAQ_RESULT
        {
            public double FREQ { get; set; }
            public double RMS { get; set; }
            public double THDNoise { get; set; }

            public double THD { get; set; }
        }

        public class OUT_FREQ_RESULT
        {
            public double SampleSize { get; set; }
            public double SampleRate { get; set; }
        }

        public class OUT_FFT_RESULT
        {
            public List<double> Adcs = new List<double>();
        }


        public void Init(string visaPath)
        {
            try
            {
                this.SetChangeLineType(ChangeLineType.LF);
                this.OpenDevice(visaPath);
                this.WSCPI(@"FORM:READ:UNIT ON");
                this.WSCPI(@"FORMat3:BORDer NORMAL");
                this.WSCPI(@"FORM:DATA ASC");
               
                this.WSCPI(@"ARM3:COUNt DEF,(@" + Channel + ")");
                this.WSCPI(@"ARM3:SOUR IMM,(@" + Channel + ")");
                this.WSCPI(@"INST:DMM ON");
                this.WSCPI(@"ACQ3:VOLT MIN,DEF,DEF,DEF,DEF,DEF,(@" + Channel + ")");

            }
            catch (Exception ex) {
                throw ex;
            }
        }

        private string FORMAT = "%t";

        public string GetError() {
            int a = 0;
            return this.GetSCPIError(ref a);
        }

        public static string FFT_WINDOW_FLAT = "FLAT";
        public static string FFT_WINDOW_HANNing = "HANNing";

        public void GetFreqRate(string windowFunc, int sampleRate, int sampleSize, ref OUT_FFT_RESULT result) {
            int tryCount = 5;
            int errorCount = 0;

            this.WSCPI(@"*RST");
            this.WSCPI(@"*WAI");
            this.WSCPI(@"ACQ3:VOLT DEF,DEF,DEF,FREQ," + sampleSize + "," + sampleRate + ",(@101)");
            this.WSCPI(@"*WAI");
            this.WSCPI(@"INIT3 (@101)");
            this.WSCPI(@"*WAI");
            System.Threading.Thread.Sleep(Delay);

            do
            {
                //ACQuire3 ?\s(@101)
                try
                {

                    //this.WSCPI(@"SAMP3:COUN 512,(@" + Channel + ")");
                    //this.WSCPI(@"SAMP3:RATE 12500,(@" + Channel + ")");
                    //this.WSCPI(@"*WAI");
                    //string r = this.QSCPI_FORMAT(@"FETC3:FFTR:MAGN? "+ windowFunc + ",1,2," + sampleSize + ",(@101)", FORMAT);
                    string r = this.QSCPI_FORMAT(@"FETCh3? (@101)", FORMAT);
                    string[] split = r.Split(',');
                    result.Adcs.Clear();
                    foreach (string str in split)
                    {
                        result.Adcs.Add(Convert.ToDouble(str));
                    }





                    //string[] split = r.Split(',');
                    //string sr = split[5].Replace("\"", "");
                    //string ss = split[4];
                    //result.SampleRate = Convert.ToDouble(sr);
                    //result.SampleSize = Convert.ToDouble(ss);
                    break;
                }
                catch (Exception ex)
                {
                    if (errorCount < (tryCount - 1))
                    {
                        System.Threading.Thread.Sleep(5);
                        continue;
                    }
                    throw ex;
                }
            } while (++errorCount < tryCount);

        }

        public void GetResult(ref OUT_DAQ_RESULT result) {
            int tryCount = 5;
            int errorCount = 0;

            this.WSCPI(@"*RST");
            this.WSCPI(@"INIT3 (@" + Channel + ")");
            this.WSCPI(@"*WAI");
            System.Threading.Thread.Sleep(Delay);

            do {
              
                try
                {
                    string r = this.QSCPI_FORMAT(@"FETCh3:FREQ? 1,2,500,(@" + Channel + ")", FORMAT);
                    result.FREQ = Convert.ToDouble(r);

                    r = this.QSCPI_FORMAT(@"FETCh3:RMS? 1,2,500,(@" + Channel + ")", FORMAT);
                    result.RMS = Convert.ToDouble(r);

                    r = this.QSCPI_FORMAT(@"FETCh3:THDNoise? 1,1,512,(@" + Channel + ")", FORMAT);
                    result.THDNoise = Convert.ToDouble(r);

                    r = this.QSCPI_FORMAT(@"FETCh3:THD? 1,1,512,(@" + Channel + ")", FORMAT);
                    result.THD = Convert.ToDouble(r);

                    break;
                }
                catch (Exception ex)
                {
                    if (errorCount < (tryCount - 1)) {
                        System.Threading.Thread.Sleep(5);
                        continue;
                    }
                    throw ex;
                }
            } while (++errorCount < tryCount);
        }

        public void GetFFTMAGNitude(int sampleSize, ref OUT_FFT_RESULT result) {
            int tryCount = 5;
            int errorCount = 0;

            this.WSCPI(@"*RST");
            this.WSCPI(@"INIT3 (@" + Channel + ")");
            this.WSCPI(@"*WAI");
            System.Threading.Thread.Sleep(Delay);

            do
            {
                //ACQuire3 ?\s(@101)
                try
                {
                    // string r = this.QSCPI_FORMAT(@"FETC3:FFTR:MAGN? FLAT,1,2," + sampleSize  + ",(@" + Channel + ")", FORMAT);
                    string r = this.QSCPI_FORMAT(@"FETCh3? (@" + Channel + ")", FORMAT);
                    string[] split = r.Split(',');
                    result.Adcs.Clear();
                    foreach (string str in split) {
                        result.Adcs.Add(Convert.ToDouble(str));
                    } 
                    break;
                }
                catch (Exception ex)
                {
                    if (errorCount < (tryCount - 1))
                    {
                        System.Threading.Thread.Sleep(5);
                        continue;
                    }
                    throw ex;
                }
            } while (++errorCount < tryCount);
        }

        public double CalcSNR(double noiseRMS, double signalRMS) {
            return 20 * Math.Log((signalRMS / noiseRMS), 10);
        }
    }
}
