﻿using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using MvvmCross;
using Plugin.BLE.Abstractions.Contracts;

using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

using BLE.Client.Extensions;

using Plugin.BLE.Abstractions;
using Plugin.Settings.Abstractions;
using Plugin.Permissions.Abstractions;
using MvvmCross.ViewModels;
using MvvmCross.Navigation;

namespace BLE.Client.ViewModels
{
    public class ViewModelMainMenu : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        readonly IPermissions _permissions;
        private IDevice _device;

        public string connectedButton { get; set; }
        public string labelVoltage { get; set; }
        public string labelVoltageTextColor { get { return BleMvxApplication._batteryLow ? "Red" : "Black"; } }
        public string connectedButtonTextColor { get; set; } = "Black";


        public ViewModelMainMenu(IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs, ISettings settings, IPermissions permissions) : base(adapter)
        {
            _userDialogs = userDialogs;
            _permissions = permissions;

            Adapter.DeviceConnectionLost += OnDeviceConnectionLost;

            OnReadWriteButtonCommand = new Command(OnReadWriteButtonClicked);
            OnInventoryButtonCommand = new Command(OnInventoryButtonClicked);
			OnRegisterTagButtonCommand = new Command(OnRegisterTagButtonClicked);
			OnSpecialFuncButtonCommand = new Command(OnSpecialFuncButtonClicked);
			OnGeigerButtonCommand = new Command(OnGeigerButtonClicked);
			OnSettingButtonCommand = new Command(OnSettingButtonClicked);
			OnSecurityButtonCommand = new Command(OnSecurityButtonClicked);
			OnFilterButtonCommand = new Command(OnFilterButtonClicked);
            OnConnectButtonCommand = new Command(OnConnectButtonClicked);

            BleMvxApplication._reader.OnReaderStateChanged += new EventHandler<CSLibrary.Events.OnReaderStateChangedEventArgs>(ReaderStateCChangedEvent);

            GetLocationPermission();
        }

        ~ViewModelMainMenu()
        {
            BleMvxApplication._reader.OnReaderStateChanged -= new EventHandler<CSLibrary.Events.OnReaderStateChangedEventArgs>(ReaderStateCChangedEvent);
        }

        // MUST be geant location permission
        private async void GetLocationPermission()
        {
            //if (await _permissions.CheckPermissionStatusAsync(Permission.Location) != PermissionStatus.Granted)
            //    await _permissions.RequestPermissionsAsync(Permission.Location);
            var status = await _permissions.CheckPermissionStatusAsync<Plugin.Permissions.LocationPermission>();
            if (status != PermissionStatus.Granted)
            {
                var permissionResult = await _permissions.RequestPermissionAsync<Plugin.Permissions.LocationPermission>();

                if (permissionResult != PermissionStatus.Granted)
                {
                    await _userDialogs.AlertAsync("Please allow permission for location");
                    _permissions.OpenAppSettings();
                    return;
                }
            }
        }

        private void CheckConnection ()
        {
            if (BleMvxApplication._reader.Status != CSLibrary.HighLevelInterface.READERSTATE.DISCONNECT)
            {
                connectedButton = "Connected to " + BleMvxApplication._reader.ReaderName + "/Select Another";
                connectedButtonTextColor = "Blue";
            }
            else
            {
                connectedButton = "Press to Scan/Connect Reader";
                connectedButtonTextColor = "Red";
            }

            RaisePropertyChanged(() => connectedButton);
            RaisePropertyChanged(() => connectedButtonTextColor);
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            BleMvxApplication._inventoryEntryPoint = 0;

            //BleMvxApplication._reader.OnReaderStateChanged += new EventHandler<CSLibrary.Events.OnReaderStateChangedEventArgs>(ReaderStateCChangedEvent);
            BleMvxApplication._reader.notification.OnVoltageEvent += new EventHandler<CSLibrary.Notification.VoltageEventArgs>(VoltageEvent);
            BleMvxApplication._reader.notification.OnKeyEvent += new EventHandler<CSLibrary.Notification.HotKeyEventArgs>(HotKeys_OnKeyEvent);

            BleMvxApplication._reader.rfid.OnStateChanged += new EventHandler<CSLibrary.Events.OnStateChangedEventArgs>(StateChangedEvent);

            CheckConnection();

            if (BleMvxApplication._reader.rfid.GetModel() != CSLibrary.Constants.Machine.UNKNOWN)
                BleMvxApplication._reader.rfid.CancelAllSelectCriteria();
            BleMvxApplication._reader.rfid.Options.TagRanging.focus = false;
            BleMvxApplication._reader.rfid.Options.TagRanging.fastid = false;
        }

        public override void ViewDisappearing()
        {
            //BleMvxApplication._reader.rfid.OnStateChanged -= new EventHandler<CSLibrary.Events.OnStateChangedEventArgs>(StateChangedEvent);

            BleMvxApplication._reader.notification.OnKeyEvent -= new EventHandler<CSLibrary.Notification.HotKeyEventArgs>(HotKeys_OnKeyEvent);
            BleMvxApplication._reader.notification.OnVoltageEvent -= new EventHandler<CSLibrary.Notification.VoltageEventArgs>(VoltageEvent);
            //BleMvxApplication._reader.OnReaderStateChanged -= new EventHandler<CSLibrary.Events.OnReaderStateChangedEventArgs>(ReaderStateCChangedEvent);
            base.ViewDisappearing();
        }

        protected override void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);

            _device = GetDeviceFromBundle(parameters);

            if (_device == null)
            {
                var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
                navigation.Close(this);
            }
		}

        private async void Disconnect()
        {
            BleMvxApplication._reader.DisconnectAsync();
        }

        void StateChangedEvent(object sender, CSLibrary.Events.OnStateChangedEventArgs e)
        {
            if (e.state == CSLibrary.Constants.RFState.INITIALIZATION_COMPLETE)
            {
                BleMvxApplication._batteryLow = false;
                RaisePropertyChanged(() => labelVoltageTextColor);

                // Set Country and Region information
                if (BleMvxApplication._config.RFID_Region == CSLibrary.Constants.RegionCode.UNKNOWN || BleMvxApplication._config.readerModel != BleMvxApplication._reader.rfid.GetModel())
                {
                    BleMvxApplication._config.readerModel = BleMvxApplication._reader.rfid.GetModel();
                    BleMvxApplication._config.RFID_Region = BleMvxApplication._reader.rfid.SelectedRegionCode;

                    if (BleMvxApplication._reader.rfid.IsFixedChannel)
                    {
                        BleMvxApplication._config.RFID_FrequenceSwitch = 1;
                        BleMvxApplication._config.RFID_FixedChannel = BleMvxApplication._reader.rfid.SelectedChannel;
                    }
                    else
                    {
                        BleMvxApplication._config.RFID_FrequenceSwitch = 0; // Hopping
                    }
                }

                // the library auto cancel the task if the setting no change
                switch (BleMvxApplication._config.RFID_FrequenceSwitch)
                {
                    case 0:
                        BleMvxApplication._reader.rfid.SetHoppingChannels(BleMvxApplication._config.RFID_Region);
                        break;
                    case 1:
                        BleMvxApplication._reader.rfid.SetFixedChannel(BleMvxApplication._config.RFID_Region, BleMvxApplication._config.RFID_FixedChannel);
                        break;
                    case 2:
                        BleMvxApplication._reader.rfid.SetAgileChannels(BleMvxApplication._config.RFID_Region);
                        break;
                }

                uint portNum = BleMvxApplication._reader.rfid.GetAntennaPort();
                for (uint cnt = 0; cnt < portNum; cnt++)
                {
                    BleMvxApplication._reader.rfid.SetAntennaPortState(cnt, BleMvxApplication._config.RFID_AntennaEnable[cnt] ? CSLibrary.Constants.AntennaPortState.ENABLED : CSLibrary.Constants.AntennaPortState.DISABLED);
                    BleMvxApplication._reader.rfid.SetPowerLevel(BleMvxApplication._config.RFID_Antenna_Power[cnt], cnt);
                    BleMvxApplication._reader.rfid.SetInventoryDuration(BleMvxApplication._config.RFID_Antenna_Dwell[cnt], cnt);
                }

                if ((BleMvxApplication._reader.bluetoothIC.GetFirmwareVersion() & 0x0F0000) != 0x030000) // ignore CS463
                if (BleMvxApplication._reader.rfid.GetFirmwareVersion() < 0x0002061D || BleMvxApplication._reader.siliconlabIC.GetFirmwareVersion() < 0x00010009 || BleMvxApplication._reader.bluetoothIC.GetFirmwareVersion() < 0x0001000E)
                {
                    _userDialogs.AlertAsync("Firmware too old" + Environment.NewLine + 
                                            "Please upgrade firmware to at least :" + Environment.NewLine +
                                            "RFID Processor firmware: V2.6.29" + Environment.NewLine +
                                            "SiliconLab Firmware: V1.0.9" + Environment.NewLine +
                                            "Bluetooth Firmware: V1.0.14");
                }

                ClassBattery.SetBatteryMode(ClassBattery.BATTERYMODE.IDLE);
                BleMvxApplication._reader.battery.SetPollingTime(BleMvxApplication._config.RFID_BatteryPollingTime);
            }
        }

        void ReaderStateCChangedEvent(object sender, CSLibrary.Events.OnReaderStateChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                Trace.Message(e.type.ToString());

                switch (e.type)
                {
                    case CSLibrary.Constants.ReaderCallbackType.COMMUNICATION_ERROR:
                        {
                            _userDialogs.AlertAsync("BLE protocol error, Please reset reader");

                            //                        _userDialogs.HideLoading();
                            //                        _userDialogs.ErrorToast("Error", $"BLE protocol error, Please reset reader", TimeSpan.MaxValue);
                        }
                        break;

                    case CSLibrary.Constants.ReaderCallbackType.CONNECTION_LOST:
                        break;

                    default:
                        break;
                }

                CheckConnection();
            });
        }

        DateTime _keyPressStartTime;

        void HotKeys_OnKeyEvent(object sender, CSLibrary.Notification.HotKeyEventArgs e)
        {
            if (e.KeyCode == CSLibrary.Notification.Key.BUTTON)
            {
                if (e.KeyDown)
                {
                    _keyPressStartTime = DateTime.Now;
                }
                else
                {
                    double duration = (DateTime.Now - _keyPressStartTime).TotalMilliseconds;

                    for (int cnt = 0; cnt < BleMvxApplication._config.RFID_Shortcut.Length; cnt++)
                    {
                        if (duration >= BleMvxApplication._config.RFID_Shortcut[cnt].DurationMin && duration <= BleMvxApplication._config.RFID_Shortcut[cnt].DurationMax)
                        {
                            switch (BleMvxApplication._config.RFID_Shortcut[cnt].Function)
                            {
                                case CONFIG.MAINMENUSHORTCUT.FUNCTION.INVENTORY:
                                    BleMvxApplication._inventoryEntryPoint = 0;
                                    OnInventoryButtonClicked();
                                    break;

                                case CONFIG.MAINMENUSHORTCUT.FUNCTION.BARCODE:
                                    BleMvxApplication._inventoryEntryPoint = 1;
                                    OnInventoryButtonClicked();
                                    break;
                            }

                            break;
                        }
                    }
                }
            }
        }

        bool _firstTimeBatteryLowAlert = true;

        void VoltageEvent(object sender, CSLibrary.Notification.VoltageEventArgs e)
		{
			if (e.Voltage == 0xffff)
			{
				labelVoltage = "CS108 Bat. ERROR"; //			3.98v
			}
			else
			{
                double voltage = (double)e.Voltage / 1000;

                {
                    var batlow = ClassBattery.BatteryLow(voltage);

                    if (BleMvxApplication._batteryLow && batlow == ClassBattery.BATTERYLEVELSTATUS.NORMAL)
                    {
                        BleMvxApplication._batteryLow = false;
                        RaisePropertyChanged(() => labelVoltageTextColor);
                    }
                    else
                    if (!BleMvxApplication._batteryLow && batlow != ClassBattery.BATTERYLEVELSTATUS.NORMAL)
                    {
                        BleMvxApplication._batteryLow = true;

                        if (batlow == ClassBattery.BATTERYLEVELSTATUS.LOW)
                            _userDialogs.AlertAsync("20% Battery Life Left, Please Recharge CS108 or Replace Freshly Charged CS108B");
                        //else if (batlow == ClassBattery.BATTERYLEVELSTATUS.LOW_17)
                        //    _userDialogs.AlertAsync("8% Battery Life Left, Please Recharge CS108 or Replace with Freshly Charged CS108B");

                        RaisePropertyChanged(() => labelVoltageTextColor);
                    }
                }

                switch (BleMvxApplication._config.BatteryLevelIndicatorFormat)
                {
                    case 0:
                        labelVoltage = "CS108 Bat. " + voltage.ToString("0.000") + "v"; //			v
                        break;

                    default:
                        labelVoltage = "CS108 Bat. " + ClassBattery.Voltage2Percent(voltage).ToString("0") + "%" + " " + voltage.ToString("0.000") + "v"; //			%
                        break;
                }
            }

            RaisePropertyChanged(() => labelVoltage);
		}

		public ICommand OnInventoryButtonCommand { protected set; get; }

        void OnInventoryButtonClicked()
        {
            if (BleMvxApplication._reader.BLEBusy)
            {
                _userDialogs.ShowSuccess("Configuring Reader, Please Wait", 1000);
            }
            else
            {
                if (BleMvxApplication._reader.Status == CSLibrary.HighLevelInterface.READERSTATE.DISCONNECT)
                {
                    ShowConnectionWarringMessage();
                    return;
                }

                //ShowViewModel<ViewModelInventorynScan>(new MvxBundle());
                var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
                navigation.Navigate<ViewModelInventorynScan>(new MvxBundle());
            }
        }

        public ICommand OnReadWriteButtonCommand { protected set; get; }

        void OnReadWriteButtonClicked()
        {
            if (BleMvxApplication._reader.Status == CSLibrary.HighLevelInterface.READERSTATE.DISCONNECT)
            {
                ShowConnectionWarringMessage();
                return;
            }

            //ShowViewModel<ViewModelReadWrite>(new MvxBundle());
            var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigation.Navigate<ViewModelReadWrite>(new MvxBundle());
        }

		public ICommand OnRegisterTagButtonCommand { protected set; get; }

		void OnRegisterTagButtonClicked()
		{
            if (BleMvxApplication._reader.Status == CSLibrary.HighLevelInterface.READERSTATE.DISCONNECT)
            {
                ShowConnectionWarringMessage();
                return;
            }

            //ShowViewModel<ViewModelRegisterTag>(new MvxBundle());
            var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigation.Navigate<ViewModelRegisterTag>(new MvxBundle());
        }

		public ICommand OnSpecialFuncButtonCommand { protected set; get; }

		void OnSpecialFuncButtonClicked()
		{
            if (BleMvxApplication._reader.Status == CSLibrary.HighLevelInterface.READERSTATE.DISCONNECT)
            {
                ShowConnectionWarringMessage();
                return;
            }

            //ShowViewModel<ViewModelSpecialFunctionsMenu>(new MvxBundle());
            var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigation.Navigate<ViewModelSpecialFunctionsMenu>(new MvxBundle());
        }

		public ICommand OnGeigerButtonCommand { protected set; get; }

		void OnGeigerButtonClicked()
		{
            if (BleMvxApplication._reader.Status == CSLibrary.HighLevelInterface.READERSTATE.DISCONNECT)
            {
                ShowConnectionWarringMessage();
                return;
            }

            //ShowViewModel<ViewModelGeiger>(new MvxBundle());
            var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigation.Navigate<ViewModelGeiger>(new MvxBundle());
        }

		public ICommand OnSettingButtonCommand { protected set; get; }

        void OnSettingButtonClicked()
        {
            if (BleMvxApplication._reader.BLEBusy)
            {
                _userDialogs.ShowSuccess("Configuring Reader, Please Wait", 1000);
            }
            else
            {
                if (BleMvxApplication._reader.Status == CSLibrary.HighLevelInterface.READERSTATE.DISCONNECT)
                {
                    ShowConnectionWarringMessage();
                    return;
                }

                //ShowViewModel<ViewModelSetting>(new MvxBundle());
                var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
                navigation.Navigate<ViewModelSetting>(new MvxBundle());
            }
        }

		public ICommand OnSecurityButtonCommand { protected set; get; }

		void OnSecurityButtonClicked()
		{
            if (BleMvxApplication._reader.Status == CSLibrary.HighLevelInterface.READERSTATE.DISCONNECT)
            {
                ShowConnectionWarringMessage();
                return;
            }

            //ShowViewModel<ViewModelSecurity>(new MvxBundle());
            var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigation.Navigate<ViewModelSecurity>(new MvxBundle());
        }

		public ICommand OnFilterButtonCommand { protected set; get; }

		void OnFilterButtonClicked()
		{
            if (BleMvxApplication._reader.Status == CSLibrary.HighLevelInterface.READERSTATE.DISCONNECT)
            {
                ShowConnectionWarringMessage();
                return;
            }

            //ShowViewModel<ViewModelFilter>(new MvxBundle());
            var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigation.Navigate<ViewModelFilter>(new MvxBundle());
        }

        public ICommand OnConnectButtonCommand { protected set; get; }

        void OnConnectButtonClicked()
        {
            if (BleMvxApplication._reader.BLEBusy)
            {
                _userDialogs.ShowSuccess("Configuring Reader, Please Wait", 1000);
                return;
            }

            // for Geiger and Read/Write
            BleMvxApplication._SELECT_EPC = "";
            //BleMvxApplication._SELECT_EPC = "E280115020001144766E1800"; // for testing
            BleMvxApplication._SELECT_PC = 3000;

            // for PreFilter
            BleMvxApplication._PREFILTER_MASK_EPC = "";
            BleMvxApplication._PREFILTER_MASK_Offset = 0;
            BleMvxApplication._PREFILTER_MASK_Truncate = 0;
            BleMvxApplication._PREFILTER_Enable = false;

            // for Post Filter
            BleMvxApplication._POSTFILTER_MASK_EPC = "";
            BleMvxApplication._POSTFILTER_MASK_Offset = 0;
            BleMvxApplication._POSTFILTER_MASK_MatchNot = false;
            BleMvxApplication._POSTFILTER_MASK_Enable = false;

            labelVoltage = "";
            RaisePropertyChanged(() => labelVoltage);

            //ShowViewModel<DeviceListViewModel>(new MvxBundle());
            var navigation = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigation.Navigate<DeviceListViewModel>(new MvxBundle());

            CheckConnection();
        }

        async void ShowConnectionWarringMessage ()
        {
            string connectWarringMsg = "Reader NOT connected\n\nPlease connect to reader first!!!";

            _userDialogs.ShowSuccess (connectWarringMsg, 2500);
        }

        private void OnDeviceConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            CheckConnection();
        }

    }
}
