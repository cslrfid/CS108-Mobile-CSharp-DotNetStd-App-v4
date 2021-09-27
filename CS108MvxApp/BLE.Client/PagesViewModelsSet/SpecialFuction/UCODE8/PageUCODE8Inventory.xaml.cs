﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLE.Client.ViewModels;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace BLE.Client.Pages
{
    public partial class PageUCODE8Inventory : MvxContentPage<ViewModelUCODE8Inventory>
    {
		public PageUCODE8Inventory()
		{
			InitializeComponent();
		}

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
            var answer = await DisplayAlert("Select Tag", "Selected Tag for Read/Write and Geiger search", "OK", "Cancel");

            if (answer)
            {
				BLE.Client.ViewModels.TagInfoViewModel Items = (BLE.Client.ViewModels.TagInfoViewModel)e.SelectedItem;

                BleMvxApplication._SELECT_EPC = Items.EPC.Substring(0, Items.EPC.Length - 4);
            }
        }
    }
}
