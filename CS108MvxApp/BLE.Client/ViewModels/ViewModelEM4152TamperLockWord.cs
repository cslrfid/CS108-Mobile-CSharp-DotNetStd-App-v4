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
    public class ViewModelEM4152TamperLockWord : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedEPC { get; set; }
        public string entrySelectedPWD { get; set; }

        public ICommand OnLockButtonCommand { protected set; get; }

        int _retryCount = 100;

        public ViewModelEM4152TamperLockWord(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;

            OnLockButtonCommand = new Command(OnLockButtonClick);
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            BleMvxApplication._reader.rfid.OnAccessCompleted += new EventHandler<CSLibrary.Events.OnAccessCompletedEventArgs>(TagCompletedEvent);
        }

        public override void ViewDisappearing()
        {
            BleMvxApplication._reader.rfid.OnAccessCompleted -= new EventHandler<CSLibrary.Events.OnAccessCompletedEventArgs>(TagCompletedEvent);
            base.ViewDisappearing();
        }

        protected override void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);

            entrySelectedEPC = BleMvxApplication._SELECT_EPC;
            entrySelectedPWD = "00000000";

            RaisePropertyChanged(() => entrySelectedEPC);
            RaisePropertyChanged(() => entrySelectedPWD);
        }

        void TagCompletedEvent(object sender, CSLibrary.Events.OnAccessCompletedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (e.access == CSLibrary.Constants.TagAccess.READ)
                {
                    if (e.success)
                    {
                        _retryCount = 100;

                    }
                }
            });
        }

        System.Threading.CancellationTokenSource cancelSrc;

        void OnLockButtonClick()
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                //MessageBox.Show("Reader is busy now, please try later.");
                return;
            }

            BleMvxApplication._reader.rfid.Options.TagWriteUser.accessPassword = Convert.ToUInt32(entrySelectedPWD, 16);
            BleMvxApplication._reader.rfid.Options.TagWriteUser.offset = 0x121;
            BleMvxApplication._reader.rfid.Options.TagWriteUser.count = 1;
            BleMvxApplication._reader.rfid.Options.TagWriteUser.pData = new ushort [] { 0x0000 };
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_WRITE_USER);
        }
    }
}
