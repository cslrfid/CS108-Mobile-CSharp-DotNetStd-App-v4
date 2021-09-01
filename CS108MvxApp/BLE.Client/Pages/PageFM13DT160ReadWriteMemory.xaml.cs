using Acr.UserDialogs;
using BLE.Client.ViewModels;
using MvvmCross.Forms.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BLE.Client.Pages
{
	public partial class PageFM13DT160ReadWriteMemory : MvxContentPage<ViewModelFM13DT160ReadWriteMemory>
    {
        public PageFM13DT160ReadWriteMemory()
		{
			InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
