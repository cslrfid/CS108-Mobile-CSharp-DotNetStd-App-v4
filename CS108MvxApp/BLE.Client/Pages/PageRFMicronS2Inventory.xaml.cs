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
    public partial class PageRFMicroS2Inventory : MvxContentPage<ViewModelRFMicroS2Inventory>
    {
		public PageRFMicroS2Inventory()
		{
			InitializeComponent();

            liewViewTagData.ItemSelected += (sender, e) => {
                if (e.SelectedItem == null) return; // don't do anything if we just de-selected the row
                ((ListView)sender).SelectedItem = null; // de-select the row
            };
        }
    }
}
