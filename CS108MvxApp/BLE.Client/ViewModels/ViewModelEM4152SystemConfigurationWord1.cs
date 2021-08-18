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
    public class ViewModelEM4152SystemConfigurationWord1 : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPCText { get; set; }
        public string entrySelectedPWDText { get; set; }
        public string labelSystemConfigurationWord1Text { get; set; }
        public ICommand ButtonReadCommand { protected set; get; }
        public ICommand ButtonWriteCommand { protected set; get; }

        public ViewModelEM4152SystemConfigurationWord1(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
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
            labelSystemConfigurationWord1Text = "0000";

            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
            RaisePropertyChanged(() => labelSystemConfigurationWord1Text);
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
                                UInt16 value = BleMvxApplication._reader.rfid.Options.TagReadUser.pData.ToUshorts()[0];

 //                               labelSystemConfigurationWord1Text = "0"+value.ToString("X04");
 //                               RaisePropertyChanged(() => labelSystemConfigurationWord1Text);
                                labelSystemConfigurationWord1Text = value.ToString("X04");
                                RaisePropertyChanged(() => labelSystemConfigurationWord1Text);

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
//        public async void ButtonReadClicked(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);

            TagSelected();

            BleMvxApplication._reader.rfid.Options.TagReadUser.accessPassword = Convert.ToUInt32(entrySelectedPWDText, 16);
            BleMvxApplication._reader.rfid.Options.TagReadUser.offset = 0x120; // m_readAllBank.OffsetUser;
            BleMvxApplication._reader.rfid.Options.TagReadUser.count = 1; // m_readAllBank.WordUser;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_READ_USER);
        }

        void ButtonWriteClick(object ind)
//        public async void ButtonWriteClicked(object sender, EventArgs e)
        {
            if (ind == null)
                return;
            if ((int)ind != 1)
                return;

            UInt16 [] value = new UInt16[1];

            RaisePropertyChanged(() => entrySelectedEPCText);
            RaisePropertyChanged(() => entrySelectedPWDText);
            RaisePropertyChanged(() => labelSystemConfigurationWord1Text);

            value[0] = Convert.ToUInt16(labelSystemConfigurationWord1Text, 16);

            //Write User Bank 0x120
            TagSelected();

            BleMvxApplication._reader.rfid.Options.TagWriteUser.accessPassword = Convert.ToUInt32(entrySelectedPWDText, 16);
            BleMvxApplication._reader.rfid.Options.TagWriteUser.offset = 0x120; // m_readAllBank.OffsetUser;
            BleMvxApplication._reader.rfid.Options.TagWriteUser.count = 1; // m_readAllBank.WordUser;
            BleMvxApplication._reader.rfid.Options.TagWriteUser.pData = value;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_WRITE_USER);
        }
    }
}
