using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Acr.UserDialogs;
using MvvmCross.ViewModels;

using System.Windows.Input;
using Xamarin.Forms;

using Plugin.BLE.Abstractions.Contracts;

using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Extensions;

namespace BLE.Client.ViewModels
{
    public class ViewModelFM13DT160LedCtrl : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPCText { get; set; }
        public string entrySelectedPWDText { get; set; }
        public ICommand OnSetLedOnButtonCommand { protected set; get; }
        public ICommand OnSetLedOffButtonCommand { protected set; get; }
        public ICommand OnInitLedSettingButtonCommand { protected set; get; }
        public ICommand OnDisableLedSettingButtonCommand { protected set; get; }

        private bool EnableLedSetting = false;

        public ViewModelFM13DT160LedCtrl(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;

            OnSetLedOnButtonCommand = new Command(OnSetLedOnButtonClick);
            OnSetLedOffButtonCommand = new Command(OnSetLedOffButtonClick);
            OnInitLedSettingButtonCommand = new Command(OnInitLedSettingButtonClick);
            OnDisableLedSettingButtonCommand = new Command(OnDisableLedSettingButtonClick);

            SetEvent(true);
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            SetEvent(true);
        }

        public override void ViewDisappearing()
        {
            SetEvent(false);
            base.ViewDisappearing();
        }

        protected override void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);

            entrySelectedEPCText = BleMvxApplication._SELECT_EPC;
            entrySelectedPWDText = "00000000";

            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
        }

        private void SetEvent (bool enable)
        {
            // Cancel RFID event handler
            BleMvxApplication._reader.rfid.ClearEventHandler();

            if (enable)
            {
                BleMvxApplication._reader.rfid.OnFM13DTAccessCompleted += new EventHandler<CSLibrary.Events.OnFM13DTAccessCompletedEventArgs>(TagCompletedEvent);
            }
        }

        void TagCompletedEvent(object sender, CSLibrary.Events.OnFM13DTAccessCompletedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                switch (e.access)
                {
                    case CSLibrary.Constants.FM13DTAccess.LEDCTRL:
                        if (e.success)
                        {
                        }
                        else
                        {
                            _userDialogs.ShowError("Read Error !!!");
                        }
                        break;

                    case CSLibrary.Constants.FM13DTAccess.READMEMORY:
                        if (e.success)
                        {
                            WriteLedSetting();
                        }
                        else
                        {
                            _userDialogs.ShowError("Read Error !!!");
                        }
                        break;

                    case CSLibrary.Constants.FM13DTAccess.WRITEMEMORY:
                        if (e.success)
                        {

                        }
                        else
                        {
                            _userDialogs.ShowError("Read Error !!!");
                        }
                        break;
                }
            });
        }

        void TagSelected()
        {
            BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
            BleMvxApplication._reader.rfid.Options.TagSelected.bank = CSLibrary.Constants.MemoryBank.EPC;
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMask = new CSLibrary.Structures.S_MASK(BleMvxApplication._SELECT_EPC);
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskOffset = 0x00;
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskLength = (uint)(BleMvxApplication._SELECT_EPC.Length * 4);
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_SELECTED);
        }

        void OnSetLedOnButtonClick ()
        {
            SetLed(true);
        }

        void OnSetLedOffButtonClick()
        {
            SetLed(false);        
        }

        void SetLed(bool enable)
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                //MessageBox.Show("Reader is busy now, please try later.");
                return;
            }

            TagSelected();

            BleMvxApplication._reader.rfid.Options.FM13DTLedCtrl.enable = enable;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.FM13DT_LEDCTRL);
        }

        void OnInitLedSettingButtonClick ()
        {
            EnableLedSetting = true;
            OnInitLedButtonButtonClick();
        }

        void OnDisableLedSettingButtonClick ()
        {
            EnableLedSetting = false;
            OnInitLedButtonButtonClick();
        }

        void OnInitLedButtonButtonClick()
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                //MessageBox.Show("Reader is busy now, please try later.");
                return;
            }

            // Reader b040 ~ 0xb043 = 4D B2 29 D6
            BleMvxApplication._reader.rfid.Options.FM13DTReadMemory.offset = 0xb040;
            BleMvxApplication._reader.rfid.Options.FM13DTReadMemory.count = 4;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.FM13DT_READMEMORY);
        }

        void WriteLedSetting ()
        {
            byte [] paras = BleMvxApplication._reader.rfid.Options.FM13DTReadMemory.data;

            paras[0] &= 0xfc; // ~0x03
            if (EnableLedSetting)
            {
                paras[0] |= 0x01;
                paras[1] = (byte)~paras[0];
            }
            BleMvxApplication._reader.rfid.Options.FM13DTWriteMemory.offset = 0xb040;
            BleMvxApplication._reader.rfid.Options.FM13DTWriteMemory.data = paras;
            //BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.FM13DT_WRITEMEMORY);
        }

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
                await System.Threading.Tasks.Task.Delay(3000);
            }
        }
    }
}
