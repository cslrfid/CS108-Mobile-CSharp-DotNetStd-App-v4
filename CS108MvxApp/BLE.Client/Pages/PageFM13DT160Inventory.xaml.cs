/*
Copyright (c) 2018 Convergence Systems Limited

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using BLE.Client.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace BLE.Client.Pages
{
    public partial class PageFM13DT160Inventory : MvxContentPage<ViewModelFM13DT160Inventory>
	{
		public PageFM13DT160Inventory()
		{
			InitializeComponent();
		}

		public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var answer = await DisplayAlert("Select Tag", "Selected Tag for Read/Write and Geiger search", "OK", "Cancel");

			if (answer)
			{
				BLE.Client.ViewModels.ViewModelFM13DT160Inventory.FM13DT160TagInfoViewModel Items = (BLE.Client.ViewModels.ViewModelFM13DT160Inventory.FM13DT160TagInfoViewModel)e.SelectedItem;

				BleMvxApplication._SELECT_EPC = Items.EPC;
				BleMvxApplication._SELECT_TID = Items.TID;
			}
		}
	}
}
