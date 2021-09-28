using BLE.Client.ViewModels;
using MvvmCross.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BLE.Client.Pages
{
    public partial class PageEM4325PassiveInventory : MvxContentPage<ViewModelEM4325PassiveInventory>
    {
		public PageEM4325PassiveInventory()
		{
			InitializeComponent();
		}

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var answer = await DisplayAlert("Select Tag", "Selected Tag for Read/Write and Geiger search", "OK", "Cancel");

            if (answer)
            {
                BLE.Client.ViewModels.ViewModelEM4325PassiveInventory.EM4325PassiveTagInfoViewModel Items = (BLE.Client.ViewModels.ViewModelEM4325PassiveInventory.EM4325PassiveTagInfoViewModel)e.SelectedItem;

                BleMvxApplication._SELECT_EPC = Items.EPC;
            }
        }
    }
}
