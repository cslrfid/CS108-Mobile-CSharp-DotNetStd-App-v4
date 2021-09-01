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
	public partial class PageEM4152SystemConfigurationWord1 : MvxContentPage<ViewModelEM4152SystemConfigurationWord1>
    {
        string [] _padModeOptionList = new string [] {"Disable", "Tamper Loop (open)", "Package Test", "Tamper Loop (closed)" };
        string [] _legacyPCEnable = new string[] {"New PC Word", "Old PC Word" };
        string [] _tamperFunction = new string[] { "Reported but does not modify the normal Tag behavior", "disables Tag and if tamper detection was logged", "RFU0", "RFU1" };
        string [] _tNReporting = new string[] { "TN bit writeable", "Report sensing detection" };
        string [] _accessPasswordUntraceable = new string[] { "Does not have the Unteaceable privilege", "Does have the Unteaceable privilege" };
        string [] _accessPasswordTNPrivilege = new string[] { "Does not have the TN privilege", "Does have the TN privilege" };
        string [] _configurationLock = new string[] { "Block 1 unlocked", "Block 1 locked" };
        string [] _backscatterConfiguration = new string[] { "RFU0", "RFU1", "Min.backscatter strength", "Max. backscatter strength" };

        public PageEM4152SystemConfigurationWord1()
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

        public async void buttonPadModeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Pad Mode", "Cancel", null, _padModeOptionList);

            if (answer != null && answer != "Cancel")
            {
                buttonPadMode.Text = answer;
                SetlabelSystemConfigurationWord1Text();
            }
        }

        public async void buttonLegacyPCenableClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Legacy PC enable", "Cancel", null, _legacyPCEnable);

            if (answer != null && answer != "Cancel")
            { 
                buttonLegacyPCenable.Text = answer;
                SetlabelSystemConfigurationWord1Text();
            }
        }

        public async void buttonTamperFunctionClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Tamper Function", "Cancel", null, _tamperFunction);

            if (answer != null && answer != "Cancel")
            { 
                buttonTamperFunction.Text = answer;
                SetlabelSystemConfigurationWord1Text();
            }
        }

    public async void buttonTNReportingClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("TN Reporting", "Cancel", null, _tNReporting);

            if (answer != null && answer != "Cancel")
            { 
                buttonTNReporting.Text = answer;
                SetlabelSystemConfigurationWord1Text();
            }
        }

        public async void buttonAccessPasswordUntraceablePrivilegeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Access Password Untraceable Privilege", "Cancel", null, _accessPasswordUntraceable);

            if (answer != null && answer != "Cancel")
            {
                buttonAccessPasswordUntraceablePrivilege.Text = answer;
                SetlabelSystemConfigurationWord1Text();
            }
        }

        public async void buttonAccessPasswordTNPrivilegeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Access Password TN Privilege", "Cancel", null, _accessPasswordTNPrivilege);

            if (answer != null && answer != "Cancel")
            {
                buttonAccessPasswordTNPrivilege.Text = answer;
                SetlabelSystemConfigurationWord1Text();
            }
        }

        public async void buttonConfigurationLockClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Configuration Lock", "Cancel", null, _configurationLock);

            if (answer != null && answer != "Cancel")
            {
                buttonConfigurationLock.Text = answer;
                SetlabelSystemConfigurationWord1Text();
            }
        }

        public async void buttonBackscatterconfigurationClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Backscatter configuration", "Cancel", null, _backscatterConfiguration);

            if (answer != null && answer != "Cancel")
            {
                buttonBackscatterconfiguration.Text = answer;
                SetlabelSystemConfigurationWord1Text();
            }
        }

        public async void labelConfigurationWord1PropertyChanged(object sender, EventArgs e)
        {
            if (labelSystemConfigurationWord1 == null)
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
        }

        void SetlabelSystemConfigurationWord1Text ()
        {
            UInt16 value = 0;

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




#if NOUSE
namespace BLE.Client.Pages
{
	public partial class PageEM4152SystemConfigurationWord1
    {
        string [] _padModeOptionList = new string [] {"Disable", "Tamper Loop (open)", "Package Test", "Tamper Loop (closed)" };
        string [] _legacyPCEnable = new string[] {"New PC Word", "Old PC Word" };
        string [] _tamperFunction = new string[] { "Reported but does not modify the normal Tag behavior", "disables Tag and if tamper detection was logged", "RFU", "RFU" };
        string [] _tNReporting = new string[] { "TN bit writeable", "Report sensing detection" };
        string [] _accessPasswordUntraceable = new string[] { "Does not have the Unteaceable privilege", "Does have the Unteaceable privilege" };
        string [] _accessPasswordTNPrivilege = new string[] { "Does not have the TN privilege", "Does have the TN privilege" };
        string [] _configurationLock = new string[] { "Block 1 unlocked", "Block 1 locked" };
        string [] _backscatterConfiguration = new string[] { "RFU", "RFU", "Min.backscatter strength", "Max. backscatter strength" };

        public PageEM4152SystemConfigurationWord1()
		{
			InitializeComponent();

            entrySelectedEPC.Text = BleMvxApplication._SELECT_EPC;
            entryPassword.Text = "00000000";
            buttonPadMode.Text = _padModeOptionList[0];
            buttonLegacyPCenable.Text = _legacyPCEnable[0];
            buttonTamperFunction.Text = _tamperFunction[0];
            buttonTNReporting.Text = _tNReporting[0];
            buttonAccessPasswordUntraceablePrivilege.Text = _accessPasswordUntraceable[0];
            buttonAccessPasswordTNPrivilege.Text = _accessPasswordTNPrivilege[0];
            buttonConfigurationLock.Text = _configurationLock[0];
            buttonBackscatterconfiguration.Text = _backscatterConfiguration[0];

            BleMvxApplication._reader.rfid.CancelAllSelectCriteria();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetEvent(true);
        }

        protected override void OnDisappearing()
        {
            SetEvent(false);
            base.OnDisappearing();
        }

        public async void buttonPadModeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Pad Mode", "Cancel", null, _padModeOptionList);

            if (answer != null && answer != "Cancel")
                buttonPadMode.Text = answer;
        }

        public async void buttonLegacyPCenableClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Legacy PC enable", "Cancel", null, _legacyPCEnable);

            if (answer != null && answer != "Cancel")
                buttonLegacyPCenable.Text = answer;
        }

        public async void buttonTamperFunctionClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Tamper Function", "Cancel", null, _tamperFunction);

            if (answer != null && answer != "Cancel")
                buttonTamperFunction.Text = answer;
        }

        public async void buttonTNReportingClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("TN Reporting", "Cancel", null, _tNReporting);

            if (answer != null && answer != "Cancel")
                buttonTNReporting.Text = answer;
        }

        public async void buttonAccessPasswordUntraceablePrivilegeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Access Password Untraceable Privilege", "Cancel", null, _accessPasswordUntraceable);

            if (answer != null && answer != "Cancel")
                buttonAccessPasswordUntraceablePrivilege.Text = answer;
        }

        public async void buttonAccessPasswordTNPrivilegeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Access Password TN Privilege", "Cancel", null, _accessPasswordTNPrivilege);

            if (answer != null && answer != "Cancel")
                buttonAccessPasswordTNPrivilege.Text = answer;
        }

        public async void buttonConfigurationLockClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Configuration Lock", "Cancel", null, _configurationLock);

            if (answer != null && answer != "Cancel")
                buttonConfigurationLock.Text = answer;
        }

        public async void buttonBackscatterconfigurationClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Backscatter configuration", "Cancel", null, _backscatterConfiguration);

            if (answer != null && answer != "Cancel")
                buttonBackscatterconfiguration.Text = answer;
        }

        public async void ButtonReadClicked(object sender, EventArgs e)
        {
            TagSelected();

            BleMvxApplication._reader.rfid.Options.TagReadUser.accessPassword = 0x00000000;
            BleMvxApplication._reader.rfid.Options.TagReadUser.offset = 0x120; // m_readAllBank.OffsetUser;
            BleMvxApplication._reader.rfid.Options.TagReadUser.count = 1; // m_readAllBank.WordUser;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_READ_USER);
        }

        public async void ButtonWriteClicked(object sender, EventArgs e)
        {
            UInt16 value = 0;

            var a = Array.IndexOf(_padModeOptionList, buttonPadMode.Text);
            var b = Array.IndexOf(_legacyPCEnable, buttonLegacyPCenable.Text);
            var c = Array.IndexOf(_tamperFunction, buttonTamperFunction.Text);
            var d = Array.IndexOf(_tNReporting, buttonTNReporting.Text);
            var e1 = Array.IndexOf(_accessPasswordUntraceable, buttonAccessPasswordUntraceablePrivilege.Text);
            var f = Array.IndexOf(_accessPasswordTNPrivilege, buttonAccessPasswordTNPrivilege.Text);
            var g = Array.IndexOf(_configurationLock, buttonConfigurationLock.Text);
            var h = Array.IndexOf(_backscatterConfiguration, buttonBackscatterconfiguration.Text);

            value = (UInt16)((a << 14) | (b << 13) | (c << 11) | (d << 9) | (e1 << 8) | (f << 7) | (g << 6) | (h << 4));

            //Write User Bank 0x120
            TagSelected();

            BleMvxApplication._reader.rfid.Options.TagWriteUser.accessPassword = Convert.ToUInt32(entryPassword.Text, 16);
            BleMvxApplication._reader.rfid.Options.TagWriteUser.offset = 0x120; // m_readAllBank.OffsetUser;
            BleMvxApplication._reader.rfid.Options.TagWriteUser.count = 1; // m_readAllBank.WordUser;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_WRITE_USER);
        }

        public async void labelConfigurationWord1PropertyChanged(object sender, EventArgs e)
        {
            if (labelConfigurationWord1 == null)
                return;

            UInt16 value = Convert.ToUInt16(labelConfigurationWord1.Text, 16);

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
        }

        private void SetEvent(bool enable)
        {
            // Cancel RFID event handler
            BleMvxApplication._reader.rfid.ClearEventHandler();

            if (enable)
            {
                // RFID event handler
                BleMvxApplication._reader.rfid.OnAccessCompleted += new EventHandler<CSLibrary.Events.OnAccessCompletedEventArgs>(TagCompletedEvent);
            }
        }

        public async void TagCompletedEvent(object sender, CSLibrary.Events.OnAccessCompletedEventArgs e)
        {
            if (e.access == CSLibrary.Constants.TagAccess.READ)
            {
                switch (e.bank)
                {
                    case CSLibrary.Constants.Bank.USER:
                        if (e.success)
                        {
                            UInt16 value = BleMvxApplication._reader.rfid.Options.TagReadUser.pData.ToUshorts()[0];

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

                            await DisplayAlert("", "Read Sucess", "OK");
                        }
                        else
                        {
                            await DisplayAlert("", "Read Fail!!!", "OK");
                        }
                        break;
                }
            }

            if (e.access == CSLibrary.Constants.TagAccess.WRITE)
            {
                switch (e.bank)
                {
                    case CSLibrary.Constants.Bank.USER:
                        if (e.success)
                        {
                            await DisplayAlert("", "Write Sucess", "OK");
                        }
                        else
                        {
                            await DisplayAlert("", "Write Fail!!!", "OK");
                        }
                        break;
                }
            }
        }

        void TagSelected()
        {
            BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
            BleMvxApplication._reader.rfid.Options.TagSelected.bank = CSLibrary.Constants.MemoryBank.EPC;
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMask = new CSLibrary.Structures.S_MASK(entrySelectedEPC.Text);
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskOffset = 0;
            BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskLength = (uint)BleMvxApplication._reader.rfid.Options.TagSelected.epcMask.Length * 8;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_SELECTED);
        }

    }
}
#endif
