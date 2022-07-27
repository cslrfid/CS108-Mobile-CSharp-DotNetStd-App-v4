﻿using System;
using Acr.UserDialogs;
using Plugin.BLE.Abstractions.Contracts;

using System.Windows.Input;
using Xamarin.Forms;
using MvvmCross.ViewModels;

namespace BLE.Client.ViewModels
{
	public class ViewModelGeiger : BaseViewModel
	{
		private readonly IUserDialogs _userDialogs;

        public ICommand OnStartGeigerButtonCommand { protected set; get; }

        private int _rssi = 0;
        private string _rssiString = "RSSI";
        public string rssiStart { get { return _rssiString; } }

        double _progressbarRSSIValue = 0;
        public double progressbarRSSIValue { get { return _progressbarRSSIValue; } }

        private string _startGeigerButtonText = "Start";
        public string startGeigerButtonText { get { return _startGeigerButtonText; } }

        private int _buttonBank = 1;
        public int buttonBank { get { return _buttonBank; } set { _buttonBank = value; } }

        private string _entryEPC;
        public string entryEPC { get { return _entryEPC; } set { _entryEPC = value; } }

        private uint _power = 300;
        public uint power { get { return _power; } set { _power = value; } }

        private int _Threshold = 0;
        public string labelThresholdText { get { return _Threshold.ToString(); } set { try { _Threshold = int.Parse(value); } catch (Exception ex) { } } }


        // end for test

        bool _InventoryScanning = false;
        private bool _KeyDown = false;
        int _beepSoundCount = 0;
        int _noTagCount = 0;

        public ViewModelGeiger(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
		{
			_userDialogs = userDialogs;

            _entryEPC = BleMvxApplication._SELECT_EPC;

            OnStartGeigerButtonCommand = new Command(StartGeigerButtonClick);

            RaisePropertyChanged(() => entryEPC);

            BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
            if (BleMvxApplication._reader.rfid.GetModelName () == "CS108")
                BleMvxApplication._reader.rfid.SetPowerSequencing(0);
        
            BleMvxApplication._reader.rfid.SetPowerLevel(_power);

            BleMvxApplication._reader.rfid.SetCurrentLinkProfile(BleMvxApplication._config.RFID_Profile);
            
            BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
            if (BleMvxApplication._geiger_Bank == 1) // if EPC
            {
                BleMvxApplication._reader.rfid.Options.TagSelected.bank = CSLibrary.Constants.MemoryBank.EPC;
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMask = new CSLibrary.Structures.S_MASK(_entryEPC);
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskOffset = 0;
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskLength = (uint)(_entryEPC.Length) * 4;
            }
            else
            {
                BleMvxApplication._reader.rfid.Options.TagSelected.bank = (CSLibrary.Constants.MemoryBank)(BleMvxApplication._geiger_Bank);
                BleMvxApplication._reader.rfid.Options.TagSelected.Mask = CSLibrary.Tools.Hex.ToBytes(_entryEPC);
                BleMvxApplication._reader.rfid.Options.TagSelected.MaskOffset = 0;
                BleMvxApplication._reader.rfid.Options.TagSelected.MaskLength = (uint)(_entryEPC.Length) * 4;
            }
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_SELECTED);

            BleMvxApplication._reader.rfid.SetOperationMode(CSLibrary.Constants.RadioOperationMode.CONTINUOUS);
            //BleMvxApplication._reader.rfid.Options.TagSearchOne.avgRssi = cb_averaging.Checked;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_PRESEARCHING);

            _Threshold = BleMvxApplication._config.RFID_DBm ? -47 : 60;

            //SetEvent(true);
        }

        ~ViewModelGeiger()
        {
            //BleMvxApplication._reader.rfid.StopOperation();
            //SetEvent(false);
        }

        private void SetEvent(bool enable)
        {
            // Cancel RFID event handler
            BleMvxApplication._reader.rfid.ClearEventHandler();

            // Key Button event handler
            BleMvxApplication._reader.notification.ClearEventHandler();

            if (enable)
            {
                BleMvxApplication._reader.rfid.OnAsyncCallback += new EventHandler<CSLibrary.Events.OnAsyncCallbackEventArgs>(TagSearchOneEvent);
                BleMvxApplication._reader.rfid.OnStateChanged += new EventHandler<CSLibrary.Events.OnStateChangedEventArgs>(StateChangedEvent);

                // Key Button event handler
                BleMvxApplication._reader.notification.OnKeyEvent += new EventHandler<CSLibrary.Notification.HotKeyEventArgs>(HotKeys_OnKeyEvent);
            }
        }

        public override void ViewAppearing()
		{
			base.ViewAppearing();
            //SetEvent(true);
            BleMvxApplication._reader.rfid.OnAsyncCallback += new EventHandler<CSLibrary.Events.OnAsyncCallbackEventArgs>(TagSearchOneEvent);

            // Key Button event handler
            BleMvxApplication._reader.notification.OnKeyEvent += new EventHandler<CSLibrary.Notification.HotKeyEventArgs>(HotKeys_OnKeyEvent);
        }

        public override void ViewDisappearing()
		{
            StopGeiger();
            BleMvxApplication._reader.rfid.OnAsyncCallback -= new EventHandler<CSLibrary.Events.OnAsyncCallbackEventArgs>(TagSearchOneEvent);
            // Key Button event handler
            BleMvxApplication._reader.notification.OnKeyEvent -= new EventHandler<CSLibrary.Notification.HotKeyEventArgs>(HotKeys_OnKeyEvent);

            // don't turn off event handler is you need program work in sleep mode.
            //SetEvent(false);
            base.ViewDisappearing();
        }

        protected override void InitFromBundle(IMvxBundle parameters)
		{
			base.InitFromBundle(parameters);
		}

        void StartGeiger ()
        {
            if (_InventoryScanning)
                return;

            _startGeigerButtonText = "Stop";
            _InventoryScanning = true;

            RaisePropertyChanged(() => entryEPC);
            RaisePropertyChanged(() => power);

            if (BleMvxApplication._reader.rfid.GetModelName() == "CS108")
                BleMvxApplication._reader.rfid.SetPowerSequencing(0);
            BleMvxApplication._reader.rfid.SetPowerLevel(_power);

            BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
            if (BleMvxApplication._geiger_Bank == 1) // if EPC
            {
                BleMvxApplication._reader.rfid.Options.TagSelected.bank = CSLibrary.Constants.MemoryBank.EPC;
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMask = new CSLibrary.Structures.S_MASK(_entryEPC);
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskOffset = 0;
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskLength = (uint)(_entryEPC.Length) * 4;
            }
            else
            {
                BleMvxApplication._reader.rfid.Options.TagSelected.bank = (CSLibrary.Constants.MemoryBank)(BleMvxApplication._geiger_Bank);
                BleMvxApplication._reader.rfid.Options.TagSelected.Mask = CSLibrary.Tools.Hex.ToBytes(_entryEPC);
                BleMvxApplication._reader.rfid.Options.TagSelected.MaskOffset = 0;
                BleMvxApplication._reader.rfid.Options.TagSelected.MaskLength = (uint)(_entryEPC.Length) * 4;
            }
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_SELECTED);

            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_EXESEARCHING);

            RaisePropertyChanged(() => startGeigerButtonText);

            // Create a beep sound timer.
            _beepSoundCount = 0;
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                CSLibrary.Debug.WriteLine("Threshold {0}", _Threshold);

                if (_rssi == 0)
                {
                    _noTagCount++;

                    if (_noTagCount > 2)
                        Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(-1);
                }
                else
//                if (_rssi != 0)
                {
                    if (_beepSoundCount == 0 && _rssi >= 20 && _rssi < 60)
                    //if (_beepSoundCount == 0)
                        Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(3);

                    _beepSoundCount++;

                    if ((!BleMvxApplication._config.RFID_DBm && _rssi >= _Threshold) || (BleMvxApplication._config.RFID_DBm && _rssi >= dBm2dBuV(_Threshold)))
                    {
                        Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(4);
                        _beepSoundCount = 1;
                        _rssi = 0;
                    }
                    else if (_rssi >= 50)
                    {
                        if (_beepSoundCount >= 5)
                        {
                            _beepSoundCount = 0;
                            _rssi = 0;
                        }
                    }
                    else if (_rssi >= 40)
                    {
                        if (_beepSoundCount >= 10)
                        {
                            _beepSoundCount = 0;
                            _rssi = 0;
                        }
                    }
                    else if (_rssi >= 30)
                    {
                        if (_beepSoundCount >= 20)
                        {
                            _beepSoundCount = 0;
                            _rssi = 0;
                        }
                    }
                    else if (_rssi >= 20)
                    {
                        if (_beepSoundCount >= 40)
                        {
                            _beepSoundCount = 0;
                            _rssi = 0;
                        }
                    }
                }

                if (_InventoryScanning)
                    return true;

                // Stop all sound
                Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(-1);
                return false;
            });
        }

        void StopGeiger ()
        {
            if (!_InventoryScanning)
                return;

            _InventoryScanning = false;
            _startGeigerButtonText = "Start";
            BleMvxApplication._reader.rfid.StopOperation();
            RaisePropertyChanged(() => startGeigerButtonText);
        }

        void StartGeigerButtonClick()
        {
            if (!_InventoryScanning)
            {
                StartGeiger();
            }
            else
            {
                StopGeiger();
            }
        }

        public void TagSearchOneEvent(object sender, CSLibrary.Events.OnAsyncCallbackEventArgs e)
        {
            switch (e.type)
            {
                case CSLibrary.Constants.CallbackType.TAG_SEARCHING:

                    //Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(3);

                    _rssi = (int)(Math.Round(((CSLibrary.Structures.TagCallbackInfo)e.info).rssi));
                    _noTagCount = 0;

                    //_progressbarRSSIValue = ((CSLibrary.Structures.TagCallbackInfo)e.info).rssi / 100;
                    if (BleMvxApplication._config.RFID_DBm)
                    {
                        // Range -90 ~ -10 (16.98 ~ 96.98)
                        double displayRSSI = dBuV2dBm(((CSLibrary.Structures.TagCallbackInfo)e.info).rssi);
                        if (displayRSSI < -90)
                            _progressbarRSSIValue = 0;
                        else if (displayRSSI > -10)
                            _progressbarRSSIValue = 1;
                        else
                            _progressbarRSSIValue = (displayRSSI + 90) / 80;
                        _rssiString = displayRSSI.ToString();
                    }
                    else
                    {
                        _progressbarRSSIValue = ((CSLibrary.Structures.TagCallbackInfo)e.info).rssi / 100;
                        _rssiString = _rssi.ToString();
                    }

                    RaisePropertyChanged(() => rssiStart);
                    RaisePropertyChanged(() => progressbarRSSIValue);
                    break;
            }
        }

        double dBuV2dBm (double dBuV)
        {
            // Range -90 ~ -10 (16.98 ~ 96.98)
            return (Math.Round(dBuV - 106.98));
        }

        double dBm2dBuV(double dBm)
        {
            // Range -90 ~ -10 (16.98 ~ 96.98)
            return (dBm + 106.98);
        }

        void StateChangedEvent(object sender, CSLibrary.Events.OnStateChangedEventArgs e)
        {
            switch (e.state)
            {
                case CSLibrary.Constants.RFState.IDLE:
                    break;
            }
        }

        bool CheckPageActive()
        {
            try
            {
                if (Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
                {
                    var currPage = Application.Current.MainPage.Navigation.NavigationStack[Application.Current.MainPage.Navigation.NavigationStack.Count - 1];

                    if (currPage.Title == "Geiger")
                        return true;
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }


        void HotKeys_OnKeyEvent(object sender, CSLibrary.Notification.HotKeyEventArgs e)
        {
            Page currentPage;

            if (!CheckPageActive())
                return;

            if (e.KeyCode == CSLibrary.Notification.Key.BUTTON)
            {
                if (e.KeyDown)
                {
                    if (!_KeyDown)
                        StartGeiger();
                    _KeyDown = true;
                }
                else
                {
                    if (_KeyDown)
                        StopGeiger();
                    _KeyDown = false;
                }
            }
        }
    }
}
