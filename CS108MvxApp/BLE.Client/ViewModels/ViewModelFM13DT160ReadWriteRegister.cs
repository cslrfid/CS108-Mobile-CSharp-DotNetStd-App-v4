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
    public class ViewModelFM13DT160ReadWriteRegister : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPCText { get; set; }
        public string entrySelectedPWDText { get; set; }
        public string entryAddressText { get; set; }
        public string entryValueText { get; set; }
        public string entryResultText { get; set; }
        public ICommand ButtonReadCommand { protected set; get; }
        public ICommand ButtonWriteCommand { protected set; get; }

        public ViewModelFM13DT160ReadWriteRegister(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;

            ButtonReadCommand = new Command(ButtonReadClick);
            ButtonWriteCommand = new Command(ButtonWriteClick);

            BleMvxApplication._reader.rfid.CancelAllSelectCriteria();
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
            entryAddressText = "C000";
            entryValueText = "";

            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
            RaisePropertyChanged(() => entryAddressText);
            RaisePropertyChanged(() => entryValueText);
        }

        private void SetEvent(bool enable)
        {
            // Cancel RFID event handler
            BleMvxApplication._reader.rfid.FM13DT160.ClearEventHandler();

            if (enable)
            {
                // RFID event handler
                BleMvxApplication._reader.rfid.FM13DT160.OnAccessCompleted += new EventHandler<CSLibrary.ClassFM13DT160.OnAccessCompletedEventArgs>(TagCompletedEvent);
			}
		}

		public async void TagCompletedEvent(object sender, CSLibrary.ClassFM13DT160.OnAccessCompletedEventArgs e)
		{
			InvokeOnMainThread(() =>
			{
			    switch  (e.operation)
                {
                    case CSLibrary.ClassFM13DT160.Operation.READREGISTER:
                        if (e.success)
                        {
                            entryValueText = BleMvxApplication._reader.rfid.FM13DT160.Options.ReadRegister.value.ToString("X4");
                            RaisePropertyChanged(() => entryValueText);
                            entryResultText = "Read Success";
                        }
                        else
                        {
                            entryResultText = "Read Fail!!!";
                        }
                        break;

                    case CSLibrary.ClassFM13DT160.Operation.WRITEREGISTER:
                        if (e.success)
                        {
                            entryResultText = "Write Success";
                        }
                        else
                        {
                            entryResultText = "Write Fail!!!";
                        }
                        break;
                }
                RaisePropertyChanged(() => entryResultText);
            });
        }

	void TagSelected()
        {
            BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
            BleMvxApplication._reader.rfid.Options.TagSelected.bank = CSLibrary.Constants.MemoryBank.EPC;
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMask = new CSLibrary.Structures.S_MASK(entrySelectedEPCText);
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskOffset = 0;
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskLength = (uint)BleMvxApplication._reader.rfid.Options.TagSelected.epcMask.Length * 8;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_SELECTED);
        }

        void ButtonReadClick()
        {
            entryResultText = "Reading...";
            RaisePropertyChanged(() => entryResultText);

            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
            RaisePropertyChanged(() => entryAddressText);
            RaisePropertyChanged(() => entryValueText);

            TagSelected();

            BleMvxApplication._reader.rfid.FM13DT160.Options.ReadRegister.address = CSLibrary.Tools.Hex.ToUInt16(entryAddressText);
            BleMvxApplication._reader.rfid.FM13DT160.StartOperation(CSLibrary.ClassFM13DT160.Operation.READREGISTER);
        }

        void ButtonWriteClick(object ind)
        {
            entryResultText = "Writeing...";
            RaisePropertyChanged(() => entryResultText);

            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
            RaisePropertyChanged(() => entryAddressText);
            RaisePropertyChanged(() => entryValueText);

            TagSelected();

            BleMvxApplication._reader.rfid.FM13DT160.Options.WriteRegister.address = CSLibrary.Tools.Hex.ToUInt16(entryAddressText);
            BleMvxApplication._reader.rfid.FM13DT160.Options.WriteRegister.value = CSLibrary.Tools.Hex.ToUInt16(entryValueText);
            BleMvxApplication._reader.rfid.FM13DT160.StartOperation(CSLibrary.ClassFM13DT160.Operation.WRITEREGISTER);
        }
    }
}
