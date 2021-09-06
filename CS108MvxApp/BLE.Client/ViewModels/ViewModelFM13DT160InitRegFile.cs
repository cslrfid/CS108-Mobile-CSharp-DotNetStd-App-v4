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
    public class ViewModelFM13DT160InitRegFile : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;

        public string entrySelectedTIDText { get; set; }
        public ICommand OnReadButtonCommand { protected set; get; }

        public ViewModelFM13DT160InitRegFile(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
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

            entrySelectedTIDText = BleMvxApplication._SELECT_TID;
        
            RaisePropertyChanged(() => entrySelectedTIDText);
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
                if (e.access ==  CSLibrary.Constants.FM13DTAccess.INITIALREGFILE)
                {
                    if (e.success)
                    {
                        _userDialogs.ShowError("Init RegFile Success!");
                    }
                    else
                    {
                        _userDialogs.ShowError ("Init RegFile Error !!!");
					}
                }
            });
        }

        void OnReadButtonButtonClick()
        {
            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            if (BleMvxApplication._reader.rfid.State != CSLibrary.Constants.RFState.IDLE)
            {
                //MessageBox.Show("Reader is busy now, please try later.");
                return;
            }

            BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
            BleMvxApplication._reader.rfid.Options.TagSelected.bank = CSLibrary.Constants.MemoryBank.TID;
            BleMvxApplication._reader.rfid.Options.TagSelected.Mask = CSLibrary.Tools.Hex.ToBytes(entrySelectedTIDText);
            BleMvxApplication._reader.rfid.Options.TagSelected.MaskOffset = 0x00;
            BleMvxApplication._reader.rfid.Options.TagSelected.MaskLength = (uint)(entrySelectedTIDText.Length * 4);
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_SELECTED);

            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.FM13DT_INITIALREGFILE);
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
