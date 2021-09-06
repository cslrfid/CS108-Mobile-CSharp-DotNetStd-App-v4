﻿/*
Copyright (c) 2018 Convergence Systems Limited

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Prism.Mvvm;
using Acr.UserDialogs;
using Plugin.BLE.Abstractions.Contracts;

namespace BLE.Client.ViewModels
{
    public class ViewModelFM13DT160Inventory : BaseViewModel
    {
        public class FM13DT160TagInfoViewModel : BindableBase
        {
            private string _EPC;
            public string EPC { get { return this._EPC; } set { this.SetProperty(ref this._EPC, value); } }
            private string _TID;
            public string TID { get { return this._TID; } set { this.SetProperty(ref this._TID, value); } }
            private string _RSSI;
            public string RSSI { get { return this._RSSI; } set { this.SetProperty(ref this._RSSI, value); } }
        }

        private readonly IUserDialogs _userDialogs;
        private readonly IMvxNavigationService _navigation;

        #region -------------- RFID inventory -----------------



        public ICommand OnStartInventoryButtonCommand { protected set; get; }
        public ICommand OnClearButtonCommand { protected set; get; }
        public ICommand OnInitialRegfileButtonCommand { protected set; get; }
        public ICommand OnGetTemperatureButtonCommand { protected set; get; }
        public ICommand OnLoggingButtonCommand { protected set; get; }
        public ICommand OnReadWriteMemoryButtonCommand { protected set; get; }
        public ICommand OnReadWriteRegButtonCommand { protected set; get; }
        public ICommand OnAuthButtonCommand { protected set; get; }
        public ICommand OnDeepSleepButtonCommand { protected set; get; }
        public ICommand OnOpModeCheckButtonCommand { protected set; get; }
        public ICommand OnLedCtrlButtonCommand { protected set; get; }


        private ObservableCollection<FM13DT160TagInfoViewModel> _TagInfoList = new ObservableCollection<FM13DT160TagInfoViewModel>();
        public ObservableCollection<FM13DT160TagInfoViewModel> TagInfoList { get { return _TagInfoList; } set { SetProperty(ref _TagInfoList, value); } }

        private string _startInventoryButtonText = "Start Inventory";
        public string startInventoryButtonText { get { return _startInventoryButtonText; } }

        bool _tagCount = false;

        private string _tagPerSecondText = "0 tags/s";
        public string tagPerSecondText { get { return _tagPerSecondText; } }
        private string _numberOfTagsText = "0 tags";
        public string numberOfTagsText { get { return _numberOfTagsText; } }
        private string _labelVoltage = "";
        public string labelVoltage { get { return _labelVoltage; } }

        private int _ListViewRowHeight = -1;
        public int ListViewRowHeight { get { return _ListViewRowHeight; } set { _ListViewRowHeight = value; } }

        public bool _startInventory = true;

        public int tagsCount = 0;
        int _tagCountForAlert = 0;
        bool _newTagFound = false;

        DateTime InventoryStartTime;
        private double _InventoryTime = 0;
        public string InventoryTime { get { return ((uint)_InventoryTime).ToString() + "s"; } }

        bool _cancelVoltageValue = false;

        #endregion

        public ViewModelFM13DT160Inventory(IAdapter adapter, IUserDialogs userDialogs, IMvxNavigationService navigation) : base(adapter)
        {
            _userDialogs = userDialogs;
            _navigation = navigation;

            RaisePropertyChanged(() => ListViewRowHeight);

            OnStartInventoryButtonCommand = new Command(StartInventoryClick);
            OnClearButtonCommand = new Command(ClearClick);
            OnInitialRegfileButtonCommand = new Command(OnInitialRegfileButtonClick);
            OnGetTemperatureButtonCommand = new Command(OnGetTemperatureButtonClick);
            OnLoggingButtonCommand = new Command(OnSenseCalibrateButtonClick);
            OnReadWriteMemoryButtonCommand = new Command(OnSystemConfigurationWord1ButtonClick);
            OnReadWriteRegButtonCommand = new Command(OnTamperLockWordButtonClick);
            OnAuthButtonCommand = new Command(OnSensorCalibrationWordButtonClick);
            OnDeepSleepButtonCommand = new Command(OnSensorCalibrationWordButtonClick);
            OnOpModeCheckButtonCommand = new Command(OnOpModeCheckButtonClick);
            OnLedCtrlButtonCommand = new Command(OnLedCtrlButtonClick);

            SetEvent(true);
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            SetEvent(true);
            InventorySetting();
        }

        public override void ViewDisappearing()
        {
            BleMvxApplication._reader.rfid.StopOperation();
            ClassBattery.SetBatteryMode(ClassBattery.BATTERYMODE.IDLE);

            SetEvent(false);
            base.ViewDisappearing();
        }

        protected override void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);
        }

        private void SetEvent(bool enable)
        {
            // Cancel RFID event handler
            BleMvxApplication._reader.rfid.ClearEventHandler();

            // Key Button event handler
            BleMvxApplication._reader.notification.ClearEventHandler();

            if (enable)
            {
                // RFID event handler
                BleMvxApplication._reader.rfid.OnStateChanged += new EventHandler<CSLibrary.Events.OnStateChangedEventArgs>(StateChangedEvent);
                BleMvxApplication._reader.rfid.OnAsyncCallback += new EventHandler<CSLibrary.Events.OnAsyncCallbackEventArgs>(TagInventoryEvent);

                // Key Button event handler
                BleMvxApplication._reader.notification.OnKeyEvent += new EventHandler<CSLibrary.Notification.HotKeyEventArgs>(HotKeys_OnKeyEvent);
                BleMvxApplication._reader.notification.OnVoltageEvent += new EventHandler<CSLibrary.Notification.VoltageEventArgs>(VoltageEvent);
            }
        }

        private void ClearClick()
        {
            InvokeOnMainThread(() =>
            {
                lock (TagInfoList)
                {
                    TagInfoList.Clear();
                    _numberOfTagsText = _TagInfoList.Count.ToString() + " tags";
                    RaisePropertyChanged(() => numberOfTagsText);

                    tagsCount = 0;
                    _tagPerSecondText = tagsCount.ToString() + " tags/s";
                    RaisePropertyChanged(() => tagPerSecondText);
                }
            });
        }

        private void OnInitialRegfileButtonClick()
        {
            //ShowViewModel<ViewModelFM13DT160InitRegFile>(new MvxBundle());
            _navigation.Navigate<ViewModelFM13DT160InitRegFile>(new MvxBundle());
        }

        private void OnGetTemperatureButtonClick()
        {
            //ShowViewModel<ViewModelFM13DT160GetTemperature>(new MvxBundle());
            _navigation.Navigate<ViewModelFM13DT160GetTemperature>(new MvxBundle());
        }

        private void OnSenseCalibrateButtonClick()
        {
            //ShowViewModel<ViewModelFM13DT160GetTemperature>(new MvxBundle());
            //_navigation.Navigate<ViewModelFM13DT160GetTemperature>(new MvxBundle());
        }

        private void OnSystemConfigurationWord1ButtonClick()
        {
            //ShowViewModel<ViewModelEM4152SystemConfigurationWord1>(new MvxBundle());
            _navigation.Navigate<ViewModelEM4152SystemConfigurationWord1>(new MvxBundle());
        }

        private void OnTamperLockWordButtonClick()
        {
            //ShowViewModel<ViewModelEM4152TamperLockWord>(new MvxBundle());
            _navigation.Navigate<ViewModelEM4152TamperLockWord>(new MvxBundle());
        }

        private void OnSensorCalibrationWordButtonClick()
        {
            //ShowViewModel< ViewModelEM4152SensorCalibrationWord>(new MvxBundle());
            _navigation.Navigate<ViewModelEM4152SensorCalibrationWord>(new MvxBundle());
        }

        private void OnOpModeCheckButtonClick()
        {
            //ShowViewModel <ViewModelFM13DT160OpModeCheck>(new MvxBundle());
            _navigation.Navigate<ViewModelFM13DT160OpModeCheck>(new MvxBundle());
        }

        private void OnLedCtrlButtonClick()
        {
            //ShowViewModel<ViewModelFM13DT160LedCtrl>(new MvxBundle());
            _navigation.Navigate<ViewModelFM13DT160LedCtrl>(new MvxBundle());
        }

        void SetConfigPower()
        {
            if (BleMvxApplication._reader.rfid.GetAntennaPort() == 1)
            {
                if (BleMvxApplication._config.RFID_PowerSequencing_NumberofPower == 0)
                {
                    BleMvxApplication._reader.rfid.SetPowerSequencing(0);
                    BleMvxApplication._reader.rfid.SetPowerLevel(BleMvxApplication._config.RFID_Antenna_Power[0]);
                }
                else
                    BleMvxApplication._reader.rfid.SetPowerSequencing(BleMvxApplication._config.RFID_PowerSequencing_NumberofPower, BleMvxApplication._config.RFID_PowerSequencing_Level, BleMvxApplication._config.RFID_PowerSequencing_DWell);
            }
            else
            {
                for (uint cnt = BleMvxApplication._reader.rfid.GetAntennaPort() - 1; cnt >= 0; cnt--)
                {
                    BleMvxApplication._reader.rfid.SetPowerLevel(BleMvxApplication._config.RFID_Antenna_Power[cnt], cnt);
                }
            }
        }

        void InventorySetting()
        {
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

            BleMvxApplication._reader.rfid.Options.TagRanging.flags = CSLibrary.Constants.SelectFlags.ZERO;

            // Setting 1
            SetConfigPower();

            // Setting 3  // MUST SET for RFMicro
            if (BleMvxApplication._reader.rfid.Options.TagRanging.focus)
                BleMvxApplication._config.RFID_DynamicQParms.toggleTarget = 0;
            else
                BleMvxApplication._config.RFID_DynamicQParms.toggleTarget = BleMvxApplication._config.RFID_ToggleTarget ? 1U : 0;
            BleMvxApplication._reader.rfid.SetDynamicQParms(BleMvxApplication._config.RFID_DynamicQParms);

            // Setting 4
            if (BleMvxApplication._reader.rfid.Options.TagRanging.focus)
                BleMvxApplication._config.RFID_FixedQParms.toggleTarget = 0;
            else
                BleMvxApplication._config.RFID_FixedQParms.toggleTarget = BleMvxApplication._config.RFID_ToggleTarget ? 1U : 0;
            BleMvxApplication._reader.rfid.SetFixedQParms(BleMvxApplication._config.RFID_FixedQParms);

            BleMvxApplication._reader.rfid.SetOperationMode(BleMvxApplication._config.RFID_OperationMode);
            BleMvxApplication._reader.rfid.SetTagGroup(BleMvxApplication._config.RFID_TagGroup);
            BleMvxApplication._reader.rfid.SetCurrentSingulationAlgorithm(BleMvxApplication._config.RFID_Algorithm);
            BleMvxApplication._reader.rfid.SetCurrentLinkProfile(BleMvxApplication._config.RFID_Profile);

            {
                BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
                BleMvxApplication._reader.rfid.Options.TagSelected.bank = CSLibrary.Constants.MemoryBank.TID;
                BleMvxApplication._reader.rfid.Options.TagSelected.Mask = new byte []{ 0xE2, 0x82, 0x70, 0x01 };
                BleMvxApplication._reader.rfid.Options.TagSelected.MaskOffset = 0;
                BleMvxApplication._reader.rfid.Options.TagSelected.MaskLength = 32;
                BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_PREFILTER);

                BleMvxApplication._reader.rfid.Options.TagRanging.flags |= CSLibrary.Constants.SelectFlags.SELECT;
            }

            BleMvxApplication._reader.rfid.Options.TagRanging.compactmode = false;
            BleMvxApplication._reader.rfid.Options.TagRanging.multibanks = 1;
            BleMvxApplication._reader.rfid.Options.TagRanging.bank1 = CSLibrary.Constants.MemoryBank.TID;
            BleMvxApplication._reader.rfid.Options.TagRanging.offset1 = 0;
            BleMvxApplication._reader.rfid.Options.TagRanging.count1 = 4;

            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_PRERANGING);
        }

        void StartInventory()
        {
            if (_startInventory == false)
                return;

            InventorySetting(); // reset inventory setting

            StartTagCount();
            {
                _startInventory = false;
                _startInventoryButtonText = "Stop Inventory";
            }

            InventoryStartTime = DateTime.Now;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_EXERANGING);
            ClassBattery.SetBatteryMode(ClassBattery.BATTERYMODE.INVENTORY);
            _cancelVoltageValue = true;

            RaisePropertyChanged(() => startInventoryButtonText);
        }

        void StopInventory()
        {
            _startInventory = true;
            _startInventoryButtonText = "Start Inventory";

            _tagCount = false;
            BleMvxApplication._reader.rfid.StopOperation();
            RaisePropertyChanged(() => startInventoryButtonText);
        }

        void StartInventoryClick()
        {
            if (_startInventory)
            {
                StartInventory();
            }
            else
            {
                StopInventory();
            }
        }

        void StartTagCount()
        {
            tagsCount = 0;
            _tagCount = true;

            // Create a timer that waits one second, then invokes every second.
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {
                _InventoryTime = (DateTime.Now - InventoryStartTime).TotalSeconds;
                RaisePropertyChanged(() => InventoryTime);

                _tagCountForAlert = 0;

                _numberOfTagsText = _TagInfoList.Count.ToString() + " tags";
                RaisePropertyChanged(() => numberOfTagsText);

                _tagPerSecondText = tagsCount.ToString() + " tags/s";
                RaisePropertyChanged(() => tagPerSecondText);
                tagsCount = 0;

                if (_tagCount)
                    return true;

                return false;
            });
        }

        void StopInventoryClick()
        {
            BleMvxApplication._reader.rfid.StopOperation();
        }

        void TagInventoryEvent(object sender, CSLibrary.Events.OnAsyncCallbackEventArgs e)
        {
            if (e.type != CSLibrary.Constants.CallbackType.TAG_RANGING)
                return;

            InvokeOnMainThread(() =>
            {
                _tagCountForAlert++;
                if (_tagCountForAlert == 1)
                {
                    if (BleMvxApplication._config.RFID_InventoryAlertSound)
                    {
                        if (_newTagFound)
                            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(3);
                        else
                            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(2);
                        _newTagFound = false;
                    }
                }
                else if (_tagCountForAlert >= 5)
                    _tagCountForAlert = 0;

                AddOrUpdateTagData(e.info);
                tagsCount++;
            });
        }

        void StateChangedEvent(object sender, CSLibrary.Events.OnStateChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                switch (e.state)
                {
                    case CSLibrary.Constants.RFState.IDLE:
                        ClassBattery.SetBatteryMode(ClassBattery.BATTERYMODE.IDLE);
                        _cancelVoltageValue = true;
                        switch (BleMvxApplication._reader.rfid.LastMacErrorCode)
                        {
                            case 0x00:  // normal end
                                break;

                            case 0x0309:    // 
                                _userDialogs.Alert("Too near to metal, please move CS108 away from metal and start inventory again.");
                                break;

                            default:
                                _userDialogs.Alert("Mac error : 0x" + BleMvxApplication._reader.rfid.LastMacErrorCode.ToString("X4"));
                                break;
                        }
                        break;
                }
            });
        }

        private void AddOrUpdateTagData(CSLibrary.Structures.TagCallbackInfo info)
        {
            bool found = false;
            int cnt;

            lock (TagInfoList)
            {
                string EPC = info.epc.ToString();
                string TID = CSLibrary.Tools.Hex.ToString(info.Bank1Data);

                for (cnt = 0; cnt < TagInfoList.Count; cnt++)
                {
                    if (TagInfoList[cnt].TID == TID)
                    {
                        found = true;
                        TagInfoList[cnt].RSSI = info.rssi.ToString("#");
                        break;
                    }
                }

                if (!found)
                {
                    FM13DT160TagInfoViewModel item = new FM13DT160TagInfoViewModel();

                    item.EPC = EPC;
                    item.TID = TID;
                    item.RSSI = info.rssi.ToString("#");
                    TagInfoList.Insert(0, item);
                }
            }
        }

        void VoltageEvent(object sender, CSLibrary.Notification.VoltageEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (e.Voltage == 0xffff)
                {
                    _labelVoltage = "CS108 Bat. ERROR"; //			3.98v
                }
                else
                {
                    // to fix CS108 voltage bug
                    if (_cancelVoltageValue)
                    {
                        _cancelVoltageValue = false;
                        return;
                    }

                    switch (BleMvxApplication._config.BatteryLevelIndicatorFormat)
                    {
                        case 0:
                            _labelVoltage = "CS108 Bat. " + ((double)e.Voltage / 1000).ToString("0.000") + "v"; //			v
                            break;

                        default:
                            _labelVoltage = "CS108 Bat. " + ClassBattery.Voltage2Percent((double)e.Voltage / 1000).ToString("0") + "%"; //			%
                            break;
                    }
                }

                RaisePropertyChanged(() => labelVoltage);
            });
		}















#region Key_event

        void HotKeys_OnKeyEvent(object sender, CSLibrary.Notification.HotKeyEventArgs e)
        {
            if (e.KeyCode == CSLibrary.Notification.Key.BUTTON)
            {
                if (e.KeyDown)
                {
                    StartInventory();
                }
                else
                {
                    StopInventory();
                }
            }
        }
#endregion

        async void ShowDialog(string Msg)
        {
            var config = new ProgressDialogConfig()
            {
                Title = Msg,
                IsDeterministic = true,
                MaskType = MaskType.Gradient,
            };

            using (var progress = _userDialogs.Progress(config))
            {
                progress.Show();
                await System.Threading.Tasks.Task.Delay(1000);
            }
        }
    }
}
