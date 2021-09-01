using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Acr.UserDialogs;

using System.Windows.Input;
using Xamarin.Forms;

using Plugin.BLE.Abstractions.Contracts;

using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Extensions;
using MvvmCross.ViewModels;

namespace BLE.Client.ViewModels
{
    public class ViewModelFM13DT160LedCtrl : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPCText { get; set; }
        public string entrySelectedPWDText { get; set; }
        public Boolean switchEnableIsToggled { get; set; } = false;
        public string labeluser_access_enText { get; set; } = "";
        public string labelRTCloggingText { get; set; } = "";
        public string labelvdet_process_flagText { get; set; } = "";
        public string labellight_chk_flagText { get; set; } = "";
        public string labelvbat_pwr_flagText { get; set; } = "";
        public ICommand OnReadButtonCommand { protected set; get; }

        public ViewModelFM13DT160LedCtrl(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;

            OnReadButtonCommand = new Command(OnReadButtonButtonClick);

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

            switchEnableIsToggled = false;

            RaisePropertyChanged(() => switchEnableIsToggled);
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
                if (e.access ==  CSLibrary.Constants.FM13DTAccess.LEDCTRL)
                {
                    if (e.success)
                    {
                        labeluser_access_enText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.user_access_en ? " Yes" : "No";
                        labelRTCloggingText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.RTC_logging ? " Yes" : "No";
                        labelvdet_process_flagText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.vdet_process_flag ? " Yes" : "No";
                        labellight_chk_flagText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.light_chk_flag ? " Yes" : "No";
                        labelvbat_pwr_flagText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.vbat_pwr_flag ? " Yes" : "No";

                        updateInfo();
                    }
                    else
                    {
                        _userDialogs.ShowError ("Read Error !!!");
					}
                }
            });
        }

        void updateInfo ()
        {
            RaisePropertyChanged(() => switchEnableIsToggled);
            RaisePropertyChanged(() => labeluser_access_enText);
            RaisePropertyChanged(() => labelRTCloggingText);
            RaisePropertyChanged(() => labelvdet_process_flagText);
            RaisePropertyChanged(() => labellight_chk_flagText);
            RaisePropertyChanged(() => labelvbat_pwr_flagText);
        }


        void OnReadButtonButtonClick()
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                //MessageBox.Show("Reader is busy now, please try later.");
                return;
            }

            labeluser_access_enText = "";
            labelRTCloggingText = "";
            labelvdet_process_flagText = "";
            labellight_chk_flagText = "";
            labelvbat_pwr_flagText = "";
            updateInfo();

            BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
            BleMvxApplication._reader.rfid.Options.TagSelected.bank = CSLibrary.Constants.MemoryBank.EPC;
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMask = new CSLibrary.Structures.S_MASK(BleMvxApplication._SELECT_EPC);
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskOffset = 0x00;
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskLength = (uint)(BleMvxApplication._SELECT_EPC.Length * 4);
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_SELECTED);

            BleMvxApplication._reader.rfid.Options.FM13DTLedCtrl.enable = switchEnableIsToggled;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.FM13DT_LEDCTRL);
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
