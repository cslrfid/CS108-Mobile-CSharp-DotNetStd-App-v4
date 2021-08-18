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
    public class ViewModelFM13DT160ReadWriteMemory : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPCText { get; set; }
        public string entrySelectedPWDText { get; set; }
        public string entryOffsetText { get; set; }
        public string entrySizeText { get; set; }
        public string entryValueText { get; set; }
        public ICommand ButtonReadCommand { protected set; get; }
        public ICommand ButtonWriteCommand { protected set; get; }

        public ViewModelFM13DT160ReadWriteMemory(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;

            ButtonReadCommand = new Command(ButtonReadClick);
            ButtonWriteCommand = new Command(ButtonWriteClick);

            BleMvxApplication._reader.rfid.CancelAllSelectCriteria();
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
            BleMvxApplication._reader.rfid.ClearEventHandler();

            if (enable)
            {
                // RFID event handler
                BleMvxApplication._reader.rfid.OnAccessCompleted += new EventHandler<CSLibrary.Events.OnAccessCompletedEventArgs>(TagCompletedEvent);
            }
        }

        public async void TagCompletedEvent(object sender, CSLibrary.Events.OnAccessCompletedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (e.access == CSLibrary.Constants.TagAccess.READ)
                {
                    switch (e.bank)
                    {
                        case CSLibrary.Constants.Bank.USER:
                            if (e.success)
                            {
                                entryValueText = BleMvxApplication._reader.rfid.Options.FM13DTReadMemory.data.ToString("X04");
                                RaisePropertyChanged(() => entryValueText);

                                _userDialogs.ShowSuccess("Read Sucess");
                            }
                            else
                            {
                                _userDialogs.ShowError("Read Fail!!!");
                            }
                            break;
                    }
                }

                if (e.access == CSLibrary.Constants.TagAccess.WRITE)
                {
                    switch (e.bank)
                    {
                        case CSLibrary.Constants.Bank.USER:
                            if (e.success)
                            {
                                _userDialogs.ShowSuccess("Write Sucess");
                            }
                            else
                            {
                                _userDialogs.ShowError("Write Fail!!!");
                            }
                            break;
                    }
                }
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
            entryValueText = "";

            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
            RaisePropertyChanged(() => entryOffsetText);
            RaisePropertyChanged(() => entrySizeText);
            RaisePropertyChanged(() => entryValueText);

            TagSelected();

            BleMvxApplication._reader.rfid.Options.FM13DTReadMemory.offset = uint.Parse(entryOffsetText);
            BleMvxApplication._reader.rfid.Options.FM13DTReadMemory.count = uint.Parse(entrySizeText);
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.FM13DT_READMEMORY);
        }

        void ButtonWriteClick(object ind)
        {
            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
            RaisePropertyChanged(() => entryOffsetText);
            RaisePropertyChanged(() => entrySizeText);
            RaisePropertyChanged(() => entryValueText);

            TagSelected();

            BleMvxApplication._reader.rfid.Options.FM13DTWriteMemory.offset = uint.Parse(entryOffsetText);
            BleMvxApplication._reader.rfid.Options.FM13DTWriteMemory.count = uint.Parse(entrySizeText);
            BleMvxApplication._reader.rfid.Options.FM13DTWriteMemory.data = Convert.ToUInt16(entryValueText, 16);
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.FM13DT_WRITEMEMORY);
        }
    }
}
