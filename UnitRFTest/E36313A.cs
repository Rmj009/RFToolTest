using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instrument
{
    class E36313A : Instrument.NIVisaCmd
    {
        private int mChannel = 1;
        private double mVoltage = 5.0;
        private double mCurrent = 0.1;

        public int Channel {
            get {
                return this.mChannel;
            }

            set {
                this.mChannel = value;
            }
        }

        public double Voltage
        {
            get
            {
                return this.mVoltage;
            }

            set
            {
                this.mVoltage = value;
            }
        }

        public double Current
        {
            get
            {
                return this.mCurrent;
            }

            set
            {
                this.mCurrent = value;
            }
        }

        public void Init(string visaPath)
        {
            try
            {
                this.SetChangeLineType(ChangeLineType.LF);
                this.OpenDevice(visaPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void OnPower() {
            try
            {
                this.WSCPI(string.Format(@"APPL CH{0},{1:0.00},{2:0.00}", Channel, Voltage, Current));
                this.WSCPI(string.Format(@"OUTP ON,(@{0})", Channel));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void OffPower()
        {
            try
            {
                this.WSCPI(string.Format(@"OUTP OFF,(@{0})", Channel));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double GetVoltage() {
            try
            {
                string r = this.QSCPI(string.Format(@"MEAS:VOLT? CH{0}", Channel));
                return Convert.ToDouble(r);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double GetCurrent()
        {
            try
            {
                string r = this.QSCPI(string.Format(@"MEAS:CURR? CH{0}", Channel));
                return Convert.ToDouble(r);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
