using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Acr.UserDialogs;

using System.Windows.Input;
using Xamarin.Forms;


using Plugin.BLE.Abstractions.Contracts;

using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Extensions;

using Prism.Mvvm;

using Plugin.Share;
using Plugin.Share.Abstractions;
using MvvmCross.ViewModels;

namespace BLE.Client.ViewModels
{
    public class ViewModelQTPrivateModeInventory : BaseViewModel
    {
        public class QTTagInfoViewModel : BindableBase
        {
            private string _EPC;
            public string EPC { get { return this._EPC; } set { this.SetProperty(ref this._EPC, value); } }

            private string _RSSI;
            public string RSSI { get { return this._RSSI; } set { this.SetProperty(ref this._RSSI, value); } }
        }

        private readonly IUserDialogs _userDialogs;

        #region -------------- RFID inventory -----------------

        public ICommand OnStartInventoryButtonCommand { protected set; get; }
        public ICommand OnClearButtonCommand { protected set; get; }

        private ObservableCollection<QTTagInfoViewModel> _TagInfoList = new ObservableCollection<QTTagInfoViewModel>();
        public ObservableCollection<QTTagInfoViewModel> TagInfoList { get { return _TagInfoList; } set { SetProperty(ref _TagInfoList, value); } }

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

        private string _currentPower;
        public string currentPower { get { return _currentPower; } set { _currentPower = value; } }

        bool _cancelVoltageValue = false;

        #endregion

        public ViewModelQTPrivateModeInventory(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;

            RaisePropertyChanged(() => ListViewRowHeight);

            OnStartInventoryButtonCommand = new Command(StartInventoryClick);
            OnClearButtonCommand = new Command(ClearClick);
        }

        ~ViewModelQTPrivateModeInventory()
        {
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

            BleMvxApplication._reader.rfid.Options.TagRanging.QTMode = false;

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

        //private TagInfoViewModel _ItemSelected;
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
            BleMvxApplication._reader.rfid.CancelAllSelectCriteria();                // Confirm cancel all filter

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
            BleMvxApplication._config.RFID_DynamicQParms.toggleTarget = BleMvxApplication._config.RFID_ToggleTarget ? 1U : 0;
            BleMvxApplication._reader.rfid.SetDynamicQParms(BleMvxApplication._config.RFID_DynamicQParms);

            // Setting 4
            BleMvxApplication._config.RFID_FixedQParms.toggleTarget = BleMvxApplication._config.RFID_ToggleTarget ? 1U : 0;
            BleMvxApplication._reader.rfid.SetFixedQParms(BleMvxApplication._config.RFID_FixedQParms);

            // Setting 2
            BleMvxApplication._reader.rfid.SetOperationMode(BleMvxApplication._config.RFID_OperationMode);
            //BleMvxApplication._reader.rfid.SetTagGroup(CSLibrary.Constants.Selected.ASSERTED, CSLibrary.Constants.Session.S1, CSLibrary.Constants.SessionTarget.A);
            BleMvxApplication._reader.rfid.SetCurrentSingulationAlgorithm(BleMvxApplication._config.RFID_Algorithm);
            BleMvxApplication._reader.rfid.SetCurrentLinkProfile(BleMvxApplication._config.RFID_Profile);

            // Multi bank inventory
            BleMvxApplication._reader.rfid.Options.TagRanging.multibanks = 0;
            BleMvxApplication._reader.rfid.Options.TagRanging.compactmode = true;
            BleMvxApplication._reader.rfid.Options.TagRanging.QTMode = true;

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

            // data validation
            if (e.info.Bank1Data.Length != 2 || e.info.Bank2Data.Length != 9)
                return;

            // Check Tag ID
            if (e.info.Bank1Data[0] != 0x8304 && e.info.Bank1Data[0] != 0x8305)
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
                float RSSI = info.rssi;

                for (cnt = 0; cnt < TagInfoList.Count; cnt++)
                {
                    if (TagInfoList[cnt].EPC == EPC)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    TagInfoList[cnt].RSSI = RSSI.ToString();
                }
                else
                {
                    QTTagInfoViewModel item = new QTTagInfoViewModel();

                    item.EPC = EPC;
                    item.RSSI = RSSI.ToString();

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
