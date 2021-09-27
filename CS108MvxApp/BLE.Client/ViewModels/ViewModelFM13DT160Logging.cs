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
    public class ViewModelFM13DT160Logging : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPCText { get; set; }
        public string entrySelectedPWDText { get; set; }
        public string entryTempText { get; set; }
        public string entryResultText { get; set; }
        public ICommand OnGetTempButtonCommand { protected set; get; }

        public ViewModelFM13DT160Logging(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;

            //OnGetTempButtonCommand = new Command(OnGetTempButtonButtonClick);
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
            BleMvxApplication._reader.rfid.FM13DT160.ClearEventHandler();

            if (enable)
            {
                BleMvxApplication._reader.rfid.FM13DT160.OnAccessCompleted += new EventHandler<CSLibrary.ClassFM13DT160.OnAccessCompletedEventArgs>(TagCompletedEvent);
            }
        }

        void TagCompletedEvent(object sender, CSLibrary.ClassFM13DT160.OnAccessCompletedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
/*
                if (e.access ==  CSLibrary.Constants.FM13DTAccess.INITIALREGFILE)
                {
                    if (e.success)
                    {
                        labeluser_access_enText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.user_access_en ? " Yes" : "No";
                        labelRTCloggingText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.RTC_logging ? " Yes" : "No";
                        labelvdet_process_flagText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.vdet_process_flag ? " Yes" : "No";
                        labellight_chk_flagText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.light_chk_flag ? " Yes" : "No";
                        labelvbat_pwr_flagText = BleMvxApplication._reader.rfid.Options.FM13DTOpModeChk.vbat_pwr_flag ? " Yes" : "No";

                        updateInfo();

                        _userDialogs.ShowError("Init RegFile Success!");
                    }
                    else
                    {
                        _userDialogs.ShowError ("Init RegFile Error !!!");
					}
                }
*/            });
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

        void OnStartLoggingButtonClick()
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                //MessageBox.Show("Reader is busy now, please try later.");
                return;
            }

            TagSelected();

            proc = 0;
            StartLoggingProc();
        }

        int proc = 0;
        void StartLoggingProc()
        {
            switch (proc)
            {
                case 0:
                    //write 1
                    break;
                case 1:
                    //write 1
                    break;
                case 2:
                    //write 1
                    break;
                case 3:
                    //write 1
                    break;
                case 4:
                    //write 1
                    break;
            }
        }

        void OnStopLoggingButtonClick()
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                //MessageBox.Show("Reader is busy now, please try later.");
                return;
            }

            TagSelected();

            BleMvxApplication._reader.rfid.FM13DT160.Options.StopLog.password = 0;
            BleMvxApplication._reader.rfid.FM13DT160.StartOperation(CSLibrary.ClassFM13DT160.Operation.STOPLOG);
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
