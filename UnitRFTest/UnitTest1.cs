using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnitRFTest
{
    //public partial class From1 : Form // cannot load
    //{

    //}
    public class Tests
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern bool Beep(UInt32 dwFreq, UInt32 dwDuration);

        [DllImport("KailkuFFT.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FFT_VppAndFreqAndSNR(double sampleRate, UInt32 windowsFunc, double windowFuncParam, double[] inADCValues, int inAdcLen, ref double vpp, ref double outFreq, ref double outSNDR, int inSNDRDCSpan);
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            //  Arrange
            var PushingMsg = new PushingMsg();
            //  Act
            run_cmd();
            //  Assert
            Assert.IsFalse(true);
            Assert.Pass();
        }
        private void run_cmd() // running python3
        {

            string fileName = @"C:\sample_script.py";

            System.Diagnostics.Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Python3\python.exe", fileName) // mounting python3
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            Console.WriteLine(output);

            Console.ReadLine();

        }


        #region TOYOTA Script
        private void button1_Click(object sender, EventArgs e)
        {
            int time = 0;
            //Instrument.MT8872 mT8872 = new Instrument.MT8872();
            //mT8872.Init(@"TCPIP0::192.168.1.1::56001::SOCKET");
//----------------------------------------------------------------------------------------------------------------------
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                //---------- daq970a
                Instrument.DAQ970A dAQ970A = new Instrument.DAQ970A();


                while ((time++) < 10000)
                {
                    try
                    {
                        dAQ970A.Init(@"TCPIP0::192.168.1.72::inst0::INSTR");
                        dAQ970A.Channel = "101";
                        double sampleDataRate = 6250 / 512;

                        Instrument.DAQ970A.OUT_DAQ_RESULT signal = new Instrument.DAQ970A.OUT_DAQ_RESULT();
                        dAQ970A.GetResult(ref signal);
                        Console.WriteLine("DAQ --> Signal RMS --> " + signal.RMS + ", Freq --> " + signal.FREQ);
                        Console.WriteLine("DAQ --> THDNoise --> " + signal.THDNoise + ", THD --> " + signal.THD);

                        //Instrument.DAQ970A.OUT_FFT_RESULT fftSignal = new Instrument.DAQ970A.OUT_FFT_RESULT();
                        //dAQ970A.GetFreqRate((int)512, ref fftSignal);

                        ////double sampleDataRate = (signal.SampleRate / 2) / (signal.SampleSize / 2);
                        //Instrument.DAQ970A.OUT_FFT_RESULT fftSignal = new Instrument.DAQ970A.OUT_FFT_RESULT();
                        //dAQ970A.GetFreqRate(Instrument.DAQ970A.FFT_WINDOW_FLAT, 12500, 512, ref fftSignal);

                        //double[] adcs = fftSignal.Adcs.ToArray();

                        //double vpp = 0;
                        //double freq = 0;
                        //double snr = 0;

                        //FFT_VppAndFreqAndSNR(12500, 6, -2, adcs, adcs.Length, ref vpp, ref freq, ref snr, 0);
                        //Console.WriteLine("DAQ --> vpp --> " + vpp);
                        //Console.WriteLine("DAQ --> freq --> " + freq);
                        //Console.WriteLine("DAQ --> snr --> " + snr);



                        //double ccc = 0.0;

                        //foreach(var a in fftSignal.Adcs)
                        //{
                        //    ccc += (a * a);
                        //}

                        //double rms = Math.Sqrt(ccc / 512);

                        //Console.WriteLine("DAQ --> Signal RMS2 --> " + rms);



                        //int maxIndex = 0;
                        //double maxV = -1000.0;
                        //List<double> freqs = new List<double>();
                        //double totalSignal = 0;
                        //double onlySignal = 0;
                        //double noiseSignal = 0;
                        ////16,777,216
                        //double normalizeValue = ((double)512 / Math.Sqrt(2));

                        ////StreamWriter sw = new StreamWriter(@"D:\abcdddd.txt");

                        //for (int i = 0; i < fftSignal.Adcs.Count; i++)
                        //{
                        //    if (maxV < fftSignal.Adcs[i])
                        //    {
                        //        maxV = fftSignal.Adcs[i];
                        //        maxIndex = i;
                        //    }

                        //    freqs.Add(i * sampleDataRate);

                        //    //sw.WriteLine(fftSignal.Adcs[i]);

                        //    //   double nor = fftSignal.Adcs[i] / normalizeValue;

                        //    //  10 ^ (A / 10)
                        //    totalSignal += (Math.Pow(10, fftSignal.Adcs[i] / 10) * 1000);
                        //}

                        //Console.WriteLine("Freq --> " + freqs[maxIndex]);

                        //int leftPadding = maxIndex;
                        //int rightPadding = maxIndex;

                        //while (true) {
                        //    if (fftSignal.Adcs[leftPadding] < fftSignal.Adcs[leftPadding - 1]) {
                        //        break;
                        //    }

                        //    if (fftSignal.Adcs[rightPadding] < fftSignal.Adcs[rightPadding + 1])
                        //    {
                        //        break;
                        //    }

                        //    leftPadding--;
                        //    rightPadding++;
                        //}


                        //Console.WriteLine("leftPadding --> " + leftPadding + " , rightPadding --> " + rightPadding);

                        //for (int i = leftPadding; i <= rightPadding; i++) {
                        //   // double nor1 = fftSignal.Adcs[i] / normalizeValue;
                        //    onlySignal += (Math.Pow(10, fftSignal.Adcs[i] / 10) * 1000);
                        //}


                        //noiseSignal = totalSignal - onlySignal;

                        //double snr = 10 * Math.Log10( (onlySignal / noiseSignal));
                        //Console.WriteLine("snr --> " + snr);

                        //sw.Close();

                        dAQ970A.Close();
                        System.Threading.Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Daq err : " + dAQ970A.GetError());
                    }
                }
            });

            t.IsBackground = true;
            t.Start();
            //----------------------------------------------------------------------------------------------------------------------
            //System.Threading.Thread t1 = new System.Threading.Thread(() => {
            //    //---------- daq970a
            //    Instrument.MT8872 mT8872 = new Instrument.MT8872();

            //    while ((time++) < 100)
            //    {
            //        try
            //        {
            //            mT8872.Init(@"TCPIP0::192.168.1.1::56001::SOCKET");
            //            Instrument.OUT_BT_MEASURE_RESULT btMeasureResult = new Instrument.OUT_BT_MEASURE_RESULT();
            //            mT8872.GetEquipmentBTMeasure(@"DH1", 39, ref btMeasureResult);

            //            Console.WriteLine("RF power : " + btMeasureResult.power);

            //            mT8872.Close();
            //            System.Threading.Thread.Sleep(800);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine("RF err : " + ex.Message);
            //        }
            //    }
            //});

            //t1.IsBackground = true;
            //t1.Start();
            //  Instrument.DAQ970A dAQ970A = new Instrument.DAQ970A();

            // dAQ970A.Close();

            //  MessageBox.Show("a");

            //   dAQ970A = new Instrument.DAQ970A();
            //  dAQ970A.Init(@"TCPIP0::169.254.48.161::inst0::INSTR", "101");

            ///---------
            //---------- power
            //Instrument.E36313A e36313A = new Instrument.E36313A();
            //e36313A.Init(@"TCPIP0::192.168.10.5::inst0::INSTR");
            //e36313A.Channel = 1;
            //e36313A.Voltage = 5;
            //e36313A.Current = 0.1;

            //e36313A.OnPower();

            //double a = e36313A.GetVoltage();
            //double b = e36313A.GetCurrent();

            //MessageBox.Show("aaa --> " + a.ToString() + "\r\n" + "bbb --> " + b.ToString());

            //System.Threading.Thread.Sleep(5000);

            //e36313A.OffPower();

            //a = e36313A.GetVoltage();
            //b = e36313A.GetCurrent();

            //MessageBox.Show("aaa --> " + a.ToString() + "\r\n" + "bbb --> " + b.ToString());
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Instrument.DAQ970A dAQ970A = new Instrument.DAQ970A();
            dAQ970A.Init("TCPIP0::192.168.1.72::inst0::INSTR");
            dAQ970A.Channel = "101";
            while (true)
            {
                Instrument.DAQ970A.OUT_DAQ_RESULT signal = new Instrument.DAQ970A.OUT_DAQ_RESULT();
                dAQ970A.GetResult(ref signal);
                Console.WriteLine("DAQ --> Signal RMS --> " + signal.RMS + ", Freq --> " + signal.FREQ);
            }
            dAQ970A.Close();
        }
#endregion
    }
}