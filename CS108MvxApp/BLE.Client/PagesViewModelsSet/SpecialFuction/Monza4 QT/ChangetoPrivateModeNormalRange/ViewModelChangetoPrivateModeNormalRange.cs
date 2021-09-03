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
using MvvmCross.Navigation;

namespace BLE.Client.ViewModels
{
    public class ViewModelChangetoPrivateModeNormalRange : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPCText { get; set; }
        public string entrySelectedPWDText { get; set; }
        public ICommand onChangeModeButtonCommand { protected set; get; }

        private bool EnableLedSetting = false;

        public ViewModelChangetoPrivateModeNormalRange(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;

            onChangeModeButtonCommand = new Command(OnChangeModeButtonClick);
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

        private void SetEvent(bool enable)
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
/*            InvokeOnMainThread(() =>
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
                }
            });*/
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

        void OnChangeModeButtonClick()
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                //MessageBox.Show("Reader is busy now, please try later.");
                return;
            }

            TagSelected();

            BleMvxApplication._reader.rfid.Options.QTCommand.RW = 1;
            BleMvxApplication._reader.rfid.Options.QTCommand.TP = 1;
            BleMvxApplication._reader.rfid.Options.QTCommand.SR = 0;
            BleMvxApplication._reader.rfid.Options.QTCommand.MEM = 1;
            BleMvxApplication._reader.rfid.Options.QTCommand.accessPassword = UInt32.Parse(entrySelectedPWDText, System.Globalization.NumberStyles.HexNumber);

            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.QT_COMMAND);
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
