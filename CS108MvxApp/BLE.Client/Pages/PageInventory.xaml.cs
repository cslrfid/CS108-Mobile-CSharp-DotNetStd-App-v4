using BLE.Client.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace BLE.Client.Pages
{
    public partial class PageInventory : MvxContentPage
    {
        bool pageView = true;

		public PageInventory()
		{
			InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                this.Icon = new FileImageSource();
                this.Icon.File = "icons8-RFID Tag-104-30x30.png";
            }
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
            var answer = await DisplayAlert("Select Tag", "Selected Tag for Read/Write and Geiger search", "OK", "Cancel");

            if (answer)
            {
				//BLE.Client.ViewModels.ViewModelInventorynScan.TagInfo Items = (BLE.Client.ViewModels.ViewModelInventorynScan.TagInfo)e.SelectedItem;
				BLE.Client.ViewModels.TagInfoViewModel Items = (BLE.Client.ViewModels.TagInfoViewModel)e.SelectedItem;

				BleMvxApplication._SELECT_EPC = Items.EPC;
                BleMvxApplication._SELECT_PC = Items.PC;
            }
        }

		~PageInventory ()
        {
            pageView = false;
        }

        /*
         * public async void OnButtonShareClicked(object sender, EventArgs e)
                {
                    string answer;

                    answer = await DisplayActionSheet("Data Format", "Cancel", null, new string[] { "JSON", "CVS" });

                    if (answer == "Cancel")
                        return;

                    if (answer == "JSON")
                        BleMvxApplication._config.RFID_ShareFormat = 0;
                    else
                        BleMvxApplication._config.RFID_ShareFormat = 1;

                    buttonShare.SetBinding(Button.CommandProperty, new Binding("OnShareDataCommand"));
                    buttonShare.Command.Execute(1);
                    buttonShare.RemoveBinding(Button.CommandProperty);
                }
        */

    }
}
