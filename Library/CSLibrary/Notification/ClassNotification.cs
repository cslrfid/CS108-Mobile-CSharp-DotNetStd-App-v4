﻿using System;

using CSLibrary.Barcode;
using CSLibrary.Barcode.Constants;
using CSLibrary.Barcode.Structures;

namespace CSLibrary
{
    public partial class Notification
    {
        bool _currentAutoReportStatus = false;
        uint _batteryLevel = 0;

        // RFID event code
        private class DOWNLINKCMD
        {
            public static readonly byte[] GETVOLTAGE = { 0xA0, 0x00 };
            public static readonly byte[] GETTRIGGERSTATE = { 0xA0, 0x01 };
            public static readonly byte[] STARTAUTOREPORTING = { 0xA0, 0x02 };
            public static readonly byte[] STOPAUTOREPORTING = { 0xA0, 0x03 };
            public static readonly byte[] STARTTRIGGERSTATEAUTOREPORTING = { 0xA0, 0x08 };
            public static readonly byte[] STOPTRIGGERSTATEAUTOREPORTING = { 0xA0, 0x09 };
        }

        private HighLevelInterface _deviceHandler;

        /// <summary>
        /// HotKey Event Argument
        /// </summary>
        public class HotKeyEventArgs : EventArgs
        {
            Key m_KeyCode = Key.BUTTON;
            bool m_KeyDown = false;

            public Key KeyCode { get { return m_KeyCode; } }
            public bool KeyDown { get { return m_KeyDown; } }

            public HotKeyEventArgs(Key KeyCode, bool KeyDown)
            {
                m_KeyCode = KeyCode;
                m_KeyDown = KeyDown;
            }
        }

        public class VoltageEventArgs : EventArgs
        {
            uint m_Voltage = 0;

            public uint Voltage { get { return m_Voltage; } }

            public VoltageEventArgs(uint voltage)
            {
                m_Voltage = voltage;
            }
        }

        public event EventHandler<VoltageEventArgs> OnVoltageEvent;
        public event EventHandler<HotKeyEventArgs> OnKeyEvent;

        /// <summary>
        /// Current Supported Virtual Key
        /// </summary>
        public enum Key : uint
        {
            /// <summary>
            /// Button
            /// </summary>
            BUTTON,
        }

        internal Notification(HighLevelInterface handler)
        {
            _deviceHandler = handler;
        }

        internal void SetAutoReport(bool OnOff)
        {
            if (OnOff)
            {
//                if (!_currentAutoReportStatus)
                {
                    _deviceHandler.SendAsync(0, 2, DOWNLINKCMD.STARTAUTOREPORTING, null, HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE);
                    _currentAutoReportStatus = true;
                }
            }
            else
            {
  //              if (_currentAutoReportStatus)
                {
                    _deviceHandler.SendAsync(0, 2, DOWNLINKCMD.STOPAUTOREPORTING, null, HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE);
                    _currentAutoReportStatus = false;
                }
            }
        }

        
        internal void SetTriggerStateAutoReporting(bool OnOff)
        {
            if (OnOff)
            {
                //                if (!_currentAutoReportStatus)
                {
                    _deviceHandler.SendAsync(0, 2, DOWNLINKCMD.STARTTRIGGERSTATEAUTOREPORTING, new byte [1]{1}, HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE);
                    _currentAutoReportStatus = true;
                }
            }
            else
            {
                //              if (_currentAutoReportStatus)
                {
                    _deviceHandler.SendAsync(0, 2, DOWNLINKCMD.STOPTRIGGERSTATEAUTOREPORTING, null, HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.WAIT_BTAPIRESPONSE);
                    _currentAutoReportStatus = false;
                }
            }
        }

        internal void GetCurrentBatteryVoltage()
        {
            _deviceHandler.SendAsync(0, 2, DOWNLINKCMD.GETVOLTAGE, null, HighLevelInterface.BTWAITCOMMANDRESPONSETYPE.NOWAIT);
        }


        internal void DeviceRecvVoltage(uint voltagemV)
        {
            _batteryLevel = voltagemV;

            if (OnVoltageEvent == null)
                return;

            OnVoltageEvent(_deviceHandler, new Notification.VoltageEventArgs(voltagemV));
        }

        bool _receiveOffWithin1s = false;
        bool _receiveOnWithinOffcycle = false;

        internal void DeviceRecvState(int value)
        {
            if (OnKeyEvent == null)
                return;

            switch (value)
            {
                case 0: // button on
                    OnKeyEvent(_deviceHandler, new Notification.HotKeyEventArgs(Key.BUTTON, true));
                    break;

                case 1: // button off
                    OnKeyEvent(_deviceHandler, new Notification.HotKeyEventArgs(Key.BUTTON, false));
                    break;
            }

            /*
                        switch (value)
                        {
                            case 0: // button on
                                if (!_receiveOffWithin1s)
                                {
                                    OnKeyEvent(_deviceHandler, new Notification.HotKeyEventArgs(Key.BUTTON, true));
                                }
                                else
                                {
                                    _receiveOnWithinOffcycle = true;
                                }
                                break;

                            case 1: // button off
                                if (!_receiveOffWithin1s)
                                {
                                    _receiveOffWithin1s = true;
                                    OnKeyEvent(_deviceHandler, new Notification.HotKeyEventArgs(Key.BUTTON, false));

                                    System.Threading.Tasks.Task.Run(async () =>
                                    {
                                        await System.Threading.Tasks.Task.Delay(1000);
                                        _receiveOffWithin1s = false;
                                        if (_receiveOnWithinOffcycle)
                                        {
                                            OnKeyEvent(_deviceHandler, new Notification.HotKeyEventArgs(Key.BUTTON, true));
                                            _receiveOnWithinOffcycle = false;
                                        }
                                    });

                                }
                                break;
                        }
            */

        }

        public uint GetCurrentBatteryLevel()
        {
            return _batteryLevel;
        }

        public void ClearEventHandler()
        {
            //OnVoltageEvent = delegate { };
            //OnKeyEvent = delegate { };
            OnVoltageEvent = null;
            OnKeyEvent = null;
        }

    }
}
