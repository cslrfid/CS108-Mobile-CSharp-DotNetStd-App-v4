using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLE.Client.ViewModels;
using MvvmCross.Forms.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BLE.Client.Pages
{
	public partial class PageEM4152SensorControlWord1 : MvxContentPage<ViewModelEM4152SensorControlWord1>
    {
        string[] _SenseAtControlOptionList = new string[] { "No Sense", "Sense At Boot", "Sense At Select", "Sense At Write/BlockWrite" };

        public PageEM4152SensorControlWord1()
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

        public async void labelConfigurationWord1PropertyChanged(object sender, EventArgs e)
        {
/*            if (labelSystemConfigurationWord1 == null)
                return;

            UInt16 value = Convert.ToUInt16(labelSystemConfigurationWord1.Text, 16);

            var a = value >> 14 & 0x03;
            var b = value >> 13 & 0x01;
            var c = value >> 11 & 0x03;
            var d = value >> 9 & 0x03;
            var e1 = value >> 8 & 0x01;
            var f = value >> 7 & 0x01;
            var g = value >> 6 & 0x01;
            var h = value >> 4 & 0x03;

            buttonPadMode.Text = _padModeOptionList[a];
            buttonLegacyPCenable.Text = _legacyPCEnable[b];
            buttonTamperFunction.Text = _tamperFunction[c];
            buttonTNReporting.Text = _tNReporting[d];
            buttonAccessPasswordUntraceablePrivilege.Text = _accessPasswordUntraceable[e1];
            buttonAccessPasswordTNPrivilege.Text = _accessPasswordTNPrivilege[f];
            buttonConfigurationLock.Text = _configurationLock[g];
            buttonBackscatterconfiguration.Text = _backscatterConfiguration[h];
*/        }

        void SetlabelSystemConfigurationWord1Text()
        {
            UInt16 value = 0;
/*
            var a = Array.IndexOf(_padModeOptionList, buttonPadMode.Text);
            var b = Array.IndexOf(_legacyPCEnable, buttonLegacyPCenable.Text);
            var c = Array.IndexOf(_tamperFunction, buttonTamperFunction.Text);
            var d = Array.IndexOf(_tNReporting, buttonTNReporting.Text);
            var e1 = Array.IndexOf(_accessPasswordUntraceable, buttonAccessPasswordUntraceablePrivilege.Text);
            var f = Array.IndexOf(_accessPasswordTNPrivilege, buttonAccessPasswordTNPrivilege.Text);
            var g = Array.IndexOf(_configurationLock, buttonConfigurationLock.Text);
            var h = Array.IndexOf(_backscatterConfiguration, buttonBackscatterconfiguration.Text);

            value = (UInt16)((a << 14) | (b << 13) | (c << 11) | (d << 9) | (e1 << 8) | (f << 7) | (g << 6) | (h << 4));

            labelSystemConfigurationWord1.Text = value.ToString("X04");
*/
        }

        public async void ButtonWriteClicked(object sender, EventArgs e)
        {
            SetlabelSystemConfigurationWord1Text();

            buttonWrite.SetBinding(Button.CommandProperty, new Binding("ButtonWriteCommand"));
            buttonWrite.Command.Execute(1);
            buttonWrite.RemoveBinding(Button.CommandProperty);
        }
    }
}
