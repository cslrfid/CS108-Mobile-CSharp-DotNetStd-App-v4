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
    public class ViewModelFM13DT160ReadWriteMemory : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPCText { get; set; }
        public string entrySelectedPWDText { get; set; }
        public string entryOffsetText { get; set; }
        public string entrySizeText { get; set; }
        public string entryValueText { get; set; }
        public string entryResultText { get; set; }
        public ICommand ButtonReadCommand { protected set; get; }
        public ICommand ButtonWriteCommand { protected set; get; }

        public ViewModelFM13DT160ReadWriteMemory(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
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
            entryOffsetText = "0";
            entrySizeText = "4";
            entryValueText = "";

            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
            RaisePropertyChanged(() => entryOffsetText);
            RaisePropertyChanged(() => entrySizeText);
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
                    case CSLibrary.ClassFM13DT160.Operation.READMEMORY:
                        if (e.success)
                        {
                            entryValueText = CSLibrary.Tools.Hex.ToString(BleMvxApplication._reader.rfid.FM13DT160.Options.ReadMemory.data);
                            RaisePropertyChanged(() => entryValueText);
                            entryResultText = "Read Success";
                        }
                        else
                        {
                            entryResultText = "Read Fail!!!";
                        }
                        break;

                    case CSLibrary.ClassFM13DT160.Operation.WRITEMEMORY:
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
            RaisePropertyChanged(() => entryOffsetText);
            RaisePropertyChanged(() => entrySizeText);
            RaisePropertyChanged(() => entryValueText);

            TagSelected();

            BleMvxApplication._reader.rfid.FM13DT160.Options.ReadMemory.offset = CSLibrary.Tools.Hex.ToUInt16(entryOffsetText);
            BleMvxApplication._reader.rfid.FM13DT160.Options.ReadMemory.count = CSLibrary.Tools.Hex.ToUInt32(entrySizeText);
            BleMvxApplication._reader.rfid.FM13DT160.StartOperation(CSLibrary.ClassFM13DT160.Operation.READMEMORY);
        }

        void ButtonWriteClick(object ind)
        {
            entryResultText = "Writeing...";
            RaisePropertyChanged(() => entryResultText);

            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
            RaisePropertyChanged(() => entryOffsetText);
            RaisePropertyChanged(() => entrySizeText);
            RaisePropertyChanged(() => entryValueText);

            byte[] value = CSLibrary.Tools.Hex.ToBytes(entryValueText) ;

            TagSelected();

            BleMvxApplication._reader.rfid.FM13DT160.Options.WriteMemory.offset = CSLibrary.Tools.Hex.ToUInt16(entryOffsetText);
            BleMvxApplication._reader.rfid.FM13DT160.Options.WriteMemory.count = CSLibrary.Tools.Hex.ToUInt32(entrySizeText);
            BleMvxApplication._reader.rfid.FM13DT160.Options.WriteMemory.data = value;
            BleMvxApplication._reader.rfid.FM13DT160.StartOperation(CSLibrary.ClassFM13DT160.Operation.WRITEMEMORY);
        }
    }
}
