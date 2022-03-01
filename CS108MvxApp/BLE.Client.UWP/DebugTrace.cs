using System;
using System.Diagnostics;
using MvvmCross.Logging;

namespace BLE.Client.UWP
{
    public class DebugTrace : IMvxLog
    {
        public bool IsLogLevelEnabled(MvxLogLevel logLevel)
        {
            //To be implemented
            return true;
        }

        public bool Log(MvxLogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            //To be implemented
            return true;
        }
    }
}
