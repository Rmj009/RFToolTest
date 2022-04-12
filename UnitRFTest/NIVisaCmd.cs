
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Instrument
{
    abstract class NIVisaCmd
    {
        protected enum ChangeLineType
        {
            CRLF,
            LF
        }

        private ChangeLineType mChangeLineType = ChangeLineType.CRLF;
        private String mUrl = "";

        protected void SetChangeLineType(ChangeLineType type)
        {
            this.mChangeLineType = type;
        }

        protected void OpenDevice(string resourceName)
        {
            if (this.mChangeLineType == ChangeLineType.CRLF)
            {
                NIVisaCmd.SetChangeLineChar_DEVICE(resourceName, new byte[] { 0x0d, 0x0a });
            }
            else
            {
                NIVisaCmd.SetChangeLineChar_DEVICE(resourceName, new byte[] { 0x0a });
            }

            int result = NIVisaCmd.Open_DEVICE(resourceName, 5000);
            if (result != 0)
            {
                throw new Exception(@"OpenDevice error : " + result.ToString());
            }
            this.mUrl = resourceName;
        }

        public void Close()
        {
            NIVisaCmd.Close_DEVICE(this.mUrl);
        }

        protected double QSCPI_FLOAT(string cmd)
        {
            int error = 0;
            double r = QSCPI_FLOAT_DEVICE(this.mUrl, cmd, ref error);
            if (error != 0)
            {
                throw new Exception(@"CMD: " + cmd + ",QSCPI_FLOAT error : " + error.ToString());
            }
            return r;
        }

        protected string QSCPI_FORMAT(string cmd, string format)
        {
            int error = 0;
            IntPtr rStr = QSCPI_FORMAT_DEVICE(this.mUrl, cmd, format, ref error);
            string result = Marshal.PtrToStringAnsi(rStr);
            if (error != 0)
            {
                throw new Exception(@"CMD: " + cmd + ",QSCPI_FORMAT error : " + error.ToString());
            }
            return result;
        }

        protected string QSCPI(string cmd, int returnSize = 1024)
        {
            int error = 0;
            IntPtr rStr = NIVisaCmd.QSCPI_DEVICE(this.mUrl, cmd, ref error, returnSize);
            string result = Marshal.PtrToStringAnsi(rStr);
            if (error != 0)
            {
                throw new Exception(@"CMD: " + cmd + ",QSCPI error : " + error.ToString());
            }

            return result;
        }

        protected void WSCPI(string cmd)
        {
            int result = NIVisaCmd.WSCPI_DEVICE(this.mUrl, cmd);

            if (result != 0)
            {
                throw new Exception(@"WSCPI error : " + result.ToString());
            }
        }

        protected string GetSCPIError(ref int viErrorCode)
        {
            int returnErrorCode = 0;
            IntPtr rStr = NIVisaCmd.GetSCPIError_DEVICE(this.mUrl, ref returnErrorCode, ref viErrorCode);
            string result = Marshal.PtrToStringAnsi(rStr);
            if (returnErrorCode != 0)
            {
                throw new Exception(@"GetSCPIError execute cmd error : " + returnErrorCode.ToString());
            }
            return result;
        }

        protected string QSCPIReadAllError(ref int viErrorCode)
        {
            int returnErrorCode = 0;
            IntPtr rStr = NIVisaCmd.QSCPIReadAllError_DEVICE(this.mUrl, ref returnErrorCode, ref viErrorCode);
            string result = Marshal.PtrToStringAnsi(rStr);
            if (returnErrorCode != 0)
            {
                throw new Exception(@"QSCPIReadAllError_DEVICE execute cmd error : " + returnErrorCode.ToString());
            }
            return result;
        }

        public double CalcSNR(double noiseRMS, double signalRMS)
        {
            return 20 * Math.Log((signalRMS / noiseRMS), 10);
        }

        [DllImport("ASECL_VISA.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 Open_DEVICE([MarshalAs(UnmanagedType.LPStr)] string url, int timeout);

        [DllImport("ASECL_VISA.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Close_DEVICE([MarshalAs(UnmanagedType.LPStr)] string url);

        [DllImport("ASECL_VISA.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 WSCPI_DEVICE([MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string cmd);

        [DllImport("ASECL_VISA.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr QSCPI_DEVICE([MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string cmd, ref int errorCode, int scanEnd = 1024);

        [DllImport("ASECL_VISA.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr QSCPI_FORMAT_DEVICE([MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string cmd, [MarshalAs(UnmanagedType.LPStr)] string format, ref int errorCode);

        [DllImport("ASECL_VISA.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double QSCPI_FLOAT_DEVICE([MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string cmd, ref int errorCode);

        [DllImport("ASECL_VISA.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr QSCPIReadAllError_DEVICE([MarshalAs(UnmanagedType.LPStr)] string url, ref int returnErrorCode, ref int viErrorCode);

        [DllImport("ASECL_VISA.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetSCPIError_DEVICE([MarshalAs(UnmanagedType.LPStr)] string url, ref int returnErrorCode, ref int viErrorCode);

        [DllImport("ASECL_VISA.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetChangeLineChar_DEVICE([MarshalAs(UnmanagedType.LPStr)] string url, byte[] tag);
    }
}
