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

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Acr.UserDialogs;

using System.Windows.Input;
using Xamarin.Forms;

using Plugin.BLE.Abstractions.Contracts;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCross;

namespace BLE.Client.ViewModels
{
    public class ViewModelQTInventorySlectionMenu : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IMvxNavigationService _navigation;

        public ICommand OnPublicModeInventoryButtonCommand { protected set; get; }
        public ICommand OnPrivateModeInventoryButtonCommand { protected set; get; }


        public ViewModelQTInventorySlectionMenu(IAdapter adapter, IUserDialogs userDialogs, IMvxNavigationService navigation) : base(adapter)
        {
            _userDialogs = userDialogs;
            _navigation = navigation;

            OnPublicModeInventoryButtonCommand = new Command(OnPublicModeInventoryButtonClicked);
            OnPrivateModeInventoryButtonCommand = new Command(OnPrivateModeInventoryButtonClicked);
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            BleMvxApplication._reader.rfid.CancelAllSelectCriteria();
        }

        void OnPublicModeInventoryButtonClicked()
        {
            //ShowViewModel<ViewModelQTPublicModeInventory>(new MvxBundle());
            _navigation.Navigate<ViewModelQTPublicModeInventory>(new MvxBundle());
        }

        void OnPrivateModeInventoryButtonClicked()
        {
            //ShowViewModel<ViewModelQTPrivateModeInventory>(new MvxBundle());
            _navigation.Navigate<ViewModelQTPrivateModeInventory>(new MvxBundle());
        }
    }
}
