using BLE.Client.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace BLE.Client.Pages
{
    public partial class PageQTPrivateModeInventory : MvxContentPage<ViewModelQTPrivateModeInventory>
    {
		public PageQTPrivateModeInventory()
		{
			InitializeComponent();
		}

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var answer = await DisplayAlert("Select Tag", "Selected Tag for Read/Write and Geiger search", "OK", "Cancel");

            if (answer)
            {
                //BLE.Client.ViewModels.ViewModelInventorynScan.TagInfo Items = (BLE.Client.ViewModels.ViewModelInventorynScan.TagInfo)e.SelectedItem;
                BLE.Client.ViewModels.ViewModelQTPrivateModeInventory.QTTagInfoViewModel Items = (BLE.Client.ViewModels.ViewModelQTPrivateModeInventory.QTTagInfoViewModel)e.SelectedItem;

                BleMvxApplication._SELECT_EPC = Items.EPC;
            }
        }
    }
}
