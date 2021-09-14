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
    public class ViewModelEM4325PassiveReadTemp : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPCText { get; set; }
        public string entrySelectedPWDText { get; set; }
        public string labelResultText { get; set; }
        public ICommand onChangeModeButtonCommand { protected set; get; }
        public ICommand onDisableBAPButtonCommand { protected set; get; }
        

        public ViewModelEM4325PassiveReadTemp(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;

            onChangeModeButtonCommand = new Command(OnChangeModeButtonClick);
            onDisableBAPButtonCommand = new Command(OnDisableBAPButtonClick);
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
            BleMvxApplication._reader.rfid.EM4325.ClearEventHandler();

            if (enable)
            {
                BleMvxApplication._reader.rfid.OnAccessCompleted += new EventHandler<CSLibrary.Events.OnAccessCompletedEventArgs>(TagCompletedEvent);
                BleMvxApplication._reader.rfid.EM4325.OnAccessCompleted += new EventHandler<CSLibrary.ClassEM4325.OnAccessCompletedEventArgs>(EM4325TagCompletedEvent);
            }
        }

        void TagCompletedEvent(object sender, CSLibrary.Events.OnAccessCompletedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                switch (e.access)
                {
                    case CSLibrary.Constants.TagAccess.WRITE:
                    {
                        if (e.bank == CSLibrary.Constants.Bank.USER)
                        {
                            if (e.success)
                            {
                                labelResultText = "BAP Mode Disabled";
                                GetSensorData();
                            }
                                else
                            {
                                labelResultText = "Set BAP Mode Error!";
                            }
                            RaisePropertyChanged(() => labelResultText);
                        }
                    }
                    break;
                }
            });
        }

        void EM4325TagCompletedEvent(object sender, CSLibrary.ClassEM4325.OnAccessCompletedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                switch (e.operation)
                {
                    case CSLibrary.ClassEM4325.Operation.GETSENSORDATA:
                        if (e.success)
                        {
                            labelResultText = BleMvxApplication._reader.rfid.EM4325.Options.GetSensorData.temperatureC.ToString();
                        }
                        else
                        {
                            labelResultText = "Error reading!";
                        }
                        break;
                }

                RaisePropertyChanged(() => labelResultText);
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

        void GetSensorData()
        {
            TagSelected();

            BleMvxApplication._reader.rfid.EM4325.Options.GetSensorData.sendUID = false;
            BleMvxApplication._reader.rfid.EM4325.Options.GetSensorData.newSample = false;
            BleMvxApplication._reader.rfid.EM4325.StartOperation(CSLibrary.ClassEM4325.Operation.GETSENSORDATA);
        }

        void OnGetSensorDataButtonClick()
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                labelResultText = "Reader is busy now, please try later.";
                RaisePropertyChanged(() => labelResultText);
                return;
            }

            labelResultText = "Reading...";
            RaisePropertyChanged(() => labelResultText);

            GetSensorData();
        }

        void OnChangeModeButtonClick()
        {
            OnDisableBAPButtonClick();
        }

        void DisableBAP()
        {
            TagSelected();

            BleMvxApplication._reader.rfid.Options.TagWriteUser.offset = 269;
            BleMvxApplication._reader.rfid.Options.TagWriteUser.count = 1;
            BleMvxApplication._reader.rfid.Options.TagWriteUser.pData = new UInt16[1];
            BleMvxApplication._reader.rfid.Options.TagWriteUser.pData[0] = 0x0000;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_WRITE_USER);
        }

        void OnDisableBAPButtonClick()
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                labelResultText = "Reader is busy now, please try later.";
                RaisePropertyChanged(() => labelResultText);
                return;
            }

            labelResultText = "Disable BAP...";
            RaisePropertyChanged(() => labelResultText);

            DisableBAP();
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
