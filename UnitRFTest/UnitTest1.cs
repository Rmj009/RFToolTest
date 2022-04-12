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
            var tstclass = new Class1();
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
            p.StartInfo = new ProcessStartInfo(@"C:\Python27\python.exe", fileName)
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