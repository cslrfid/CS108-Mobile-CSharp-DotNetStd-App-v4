using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Forms.Views;
using Xamarin.Forms;


namespace BLE.Client.Pages
{
	public partial class PageSettingOperation : MvxContentPage
	{
        List<CSLibrary.Constants.RegionCode> Regions;
        string[] ActiveRegionsTextList;
        double[] ActiveFrequencyList;
        string[] ActiveFrequencyTextList;

        CSLibrary.Constants.RegionCode [] _regionsCode = new CSLibrary.Constants.RegionCode[] {
            CSLibrary.Constants.RegionCode.FCC,
            CSLibrary.Constants.RegionCode.ETSI,
            CSLibrary.Constants.RegionCode.CN,
            CSLibrary.Constants.RegionCode.TW,
            CSLibrary.Constants.RegionCode.KR,
            CSLibrary.Constants.RegionCode.HK,
            CSLibrary.Constants.RegionCode.JP,
            CSLibrary.Constants.RegionCode.AU,
            CSLibrary.Constants.RegionCode.MY,
            CSLibrary.Constants.RegionCode.SG,
            CSLibrary.Constants.RegionCode.IN,
            CSLibrary.Constants.RegionCode.G800,
            CSLibrary.Constants.RegionCode.ZA,
            CSLibrary.Constants.RegionCode.BR1,
            CSLibrary.Constants.RegionCode.BR2,
            CSLibrary.Constants.RegionCode.BR3,
            CSLibrary.Constants.RegionCode.BR4,
            CSLibrary.Constants.RegionCode.BR5,
            CSLibrary.Constants.RegionCode.ID,
            CSLibrary.Constants.RegionCode.TH,
            CSLibrary.Constants.RegionCode.JE,
            CSLibrary.Constants.RegionCode.PH,
            CSLibrary.Constants.RegionCode.ETSIUPPERBAND,
            CSLibrary.Constants.RegionCode.NZ,
            CSLibrary.Constants.RegionCode.UH1,
            CSLibrary.Constants.RegionCode.UH2,
            CSLibrary.Constants.RegionCode.LH,
            CSLibrary.Constants.RegionCode.LH1,
            CSLibrary.Constants.RegionCode.LH2,
            CSLibrary.Constants.RegionCode.VE,
            CSLibrary.Constants.RegionCode.AR,
            CSLibrary.Constants.RegionCode.CL,
            CSLibrary.Constants.RegionCode.CO,
            CSLibrary.Constants.RegionCode.CR,
            CSLibrary.Constants.RegionCode.DO,
            CSLibrary.Constants.RegionCode.PA,
            CSLibrary.Constants.RegionCode.PE,
            CSLibrary.Constants.RegionCode.UY,
            CSLibrary.Constants.RegionCode.BA,
            CSLibrary.Constants.RegionCode.VI
        };
        string[] _regionsName = new string[] {
            "USACanada",
            "Europe",
            "China",
            "Taiwan",
            "Korea",
            "Hong Kong",
            "Japan",
            "Australia",
            "Malaysia",
            "Singapore",
            "India",
            "G800",
            "South Africa",
            "Brazil 915-927",
            "Brazil 902-906, 915-927",
            "Brazil 902-906",
            "Brazil 902-904",
            "Brazil 917-924",
            "Indonesia",
            "Thailand",
            "Israel",
            "Philippine",
            "ETSI Upper Band",
            "New Zealand",
            "UH1",
            "UH2",
            "LH",
            "LH1",
            "LH2",
            "Venezuela",
            "Argentina",
            "Chile",
            "Colombia",
            "Costa Rica",
            "Dominican Republic",
            "Panama",
            "Peru",
            "Uruguay",
            "Bangladesh",
            "Vietnam"
        };

        //        string[] _profileList = { "0 for Fade Resistance", "1 for Range", "2 for Range & Throughput", "3 for Max Throughput" };
        string[] _profileList = {
            "0. Multipath Interference Resistance",
            "1. Range/Dense Reader",
            "2. Range/Throughput/Dense Reader",
            "3. Max Throughput"
        };

        string[] _freqOrderOptions;

        string[] _RFLNAcompressionmodeList = { "0", "1" };
        string[] _RFLNAGainList = { "1", "7", "13" };
        string[] _IFLNAGainList = { "24", "18", "12", "6" };
        string[] _AGCGainList = { "-12", "-6", "0", "6" };

        public PageSettingOperation()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                this.Icon = new FileImageSource();
                this.Icon.File = "icons8-Settings-50-1-30x30.png";
            }

            stackLayoutInventoryDuration.IsVisible = stackLayoutPower.IsVisible = (BleMvxApplication._reader.rfid.GetAntennaPort() == 1);

        var countryCode = BleMvxApplication._reader.rfid.GetCountryCode();

            if (countryCode == "-2")
                _regionsName[0] = "FCC";

            switch (countryCode)
            {
                case "-1":
                case "-8":
                    _freqOrderOptions = new string[] { "Fixed" };
                    break;

                default:
                    _freqOrderOptions = new string[] { "Hopping" };
                    break;
            }

            Regions = BleMvxApplication._reader.rfid.GetActiveRegionCode();
            ActiveRegionsTextList = Regions.OfType<object>().Select(o => _regionsName[(int)o - 1]).ToArray();

            ActiveFrequencyList = BleMvxApplication._reader.rfid.GetAvailableFrequencyTable(BleMvxApplication._config.RFID_Region);
            ActiveFrequencyTextList = ActiveFrequencyList.OfType<object>().Select(o => o.ToString()).ToArray();

            buttonRegion.Text = _regionsName[(int)BleMvxApplication._config.RFID_Region - 1];
            if (Regions.Count == 1)
                buttonRegion.IsEnabled = false;
            switch (BleMvxApplication._config.RFID_FrequenceSwitch)
            {
                case 0:
                    buttonFrequencyOrder.Text = "Hopping";
                    break;
                case 1:
                    buttonFrequencyOrder.Text = "Fixed";
                    break;
                case 2:
                    buttonFrequencyOrder.Text = "Agile";
                    break;
            }
            if (_freqOrderOptions.Length == 1)
                buttonFrequencyOrder.IsEnabled = false;
            buttonFixedChannel.Text = ActiveFrequencyTextList[BleMvxApplication._config.RFID_FixedChannel];
            checkbuttonFixedChannel();
            entryPower.Text = BleMvxApplication._config.RFID_Antenna_Power[0].ToString();
            entryTagPopulation.Text = BleMvxApplication._config.RFID_TagPopulation.ToString();
            if (BleMvxApplication._config.RFID_QOverride)
            {
                entryQOverride.IsEnabled = true;
                buttonQOverride.Text = "Reset";
            }
            else
            {
                entryQOverride.IsEnabled = false;
                buttonQOverride.Text = "Override";
            }
            buttonSession.Text = BleMvxApplication._config.RFID_TagGroup.session.ToString();
            if (BleMvxApplication._config.RFID_ToggleTarget)
            {
                buttonTarget.Text = "Toggle A/B";
            }
            else
            {
                buttonTarget.Text = BleMvxApplication._config.RFID_TagGroup.target.ToString();
            }
            buttonAlgorithm.Text = BleMvxApplication._config.RFID_Algorithm.ToString();
            buttonProfile.Text = _profileList[BleMvxApplication._config.RFID_Profile];

            SetQvalue();

            entryTagDelay.Text = BleMvxApplication._config.RFID_TagDelayTime.ToString();
            entryInventoryDuration.Text = BleMvxApplication._config.RFID_Antenna_Dwell[0].ToString();
            //switchNewTagLocation.IsToggled = BleMvxApplication._config.RFID_NewTagLocation;
            switchFocus.IsToggled = BleMvxApplication._config.RFID_Focus;
            buttonRFLNAcompression.Text = BleMvxApplication._config.RFID_RFLNAcompression.ToString();
            buttonRFLNAGain.Text = BleMvxApplication._config.RFID_RFLNAGain.ToString();
            buttonIFLNAGain.Text = BleMvxApplication._config.RFID_IFLNAGain.ToString();
            buttonAGCGain.Text = BleMvxApplication._config.RFID_AGCGain.ToString();
            if (buttonRFLNAGain.Text == "13")
            {
                buttonRFLNAcompression.Text = "0";
                buttonRFLNAcompression.IsEnabled = false;
            }
            else
            {
                buttonRFLNAcompression.IsEnabled = true;
            }
        }

        protected override void OnAppearing()
        {
            if (BleMvxApplication._settingPage1TagPopulationChanged)
            {
                BleMvxApplication._settingPage1TagPopulationChanged = false;
                entryTagPopulation.Text = BleMvxApplication._config.RFID_TagPopulation.ToString();
            }

            base.OnAppearing();
        }

        void checkbuttonFixedChannel()
        {
            if (buttonFrequencyOrder.Text == "Fixed")
                buttonFixedChannel.IsEnabled = true;
            else
                buttonFixedChannel.IsEnabled = false;
        }

        public async void btnOKClicked(object sender, EventArgs e)
        {
            int cnt;

            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(1);

            for (cnt = 0; cnt < _regionsName.Length; cnt++)
            {
                if (_regionsName[cnt] == buttonRegion.Text)
                {
                    BleMvxApplication._config.RFID_Region = _regionsCode[cnt];
                    break;
                }
            }
            if (cnt == _regionsName.Length)
                BleMvxApplication._config.RFID_Region = CSLibrary.Constants.RegionCode.UNKNOWN;

            switch (buttonFrequencyOrder.Text)
            {
                case "Hopping":
                    BleMvxApplication._config.RFID_FrequenceSwitch = 0;
                    break;
                case "Fixed":
                    BleMvxApplication._config.RFID_FrequenceSwitch = 1;
                    break;
                case "Agile":
                    BleMvxApplication._config.RFID_FrequenceSwitch = 2;
                    break;
            }

            for (cnt = 0; cnt < ActiveFrequencyTextList.Length; cnt++)
            {
                if (buttonFixedChannel.Text == ActiveFrequencyTextList[cnt])
                {
                    BleMvxApplication._config.RFID_FixedChannel = (uint)cnt;
                    break;
                }
            }
            if (cnt == ActiveFrequencyTextList.Length)
                BleMvxApplication._config.RFID_FixedChannel = 0;

            BleMvxApplication._config.RFID_Antenna_Power[0] = UInt16.Parse(entryPower.Text);

            BleMvxApplication._config.RFID_TagPopulation = UInt16.Parse(entryTagPopulation.Text);

            BleMvxApplication._config.RFID_QOverride = entryQOverride.IsEnabled;

            switch (buttonSession.Text)
            {
                case "S0":
                    BleMvxApplication._config.RFID_TagGroup.session = CSLibrary.Constants.Session.S0;
                    break;

                case "S1":
                    BleMvxApplication._config.RFID_TagGroup.session = CSLibrary.Constants.Session.S1;
                    break;

                case "S2":
                    BleMvxApplication._config.RFID_TagGroup.session = CSLibrary.Constants.Session.S2;
                    break;

                case "S3":
                    BleMvxApplication._config.RFID_TagGroup.session = CSLibrary.Constants.Session.S3;
                    break;
            }

            switch (buttonTarget.Text)
            {
                case "A":
                    BleMvxApplication._config.RFID_ToggleTarget = false;
                    BleMvxApplication._config.RFID_TagGroup.target = CSLibrary.Constants.SessionTarget.A;
                    BleMvxApplication._config.RFID_FixedQParms.toggleTarget = 0;
                    BleMvxApplication._config.RFID_DynamicQParms.toggleTarget = 0;
                    break;
                case "B":
                    BleMvxApplication._config.RFID_ToggleTarget = false;
                    BleMvxApplication._config.RFID_TagGroup.target = CSLibrary.Constants.SessionTarget.B;
                    BleMvxApplication._config.RFID_FixedQParms.toggleTarget = 0;
                    BleMvxApplication._config.RFID_DynamicQParms.toggleTarget = 0;
                    break;
                default:
                    BleMvxApplication._config.RFID_ToggleTarget = true;
                    BleMvxApplication._config.RFID_DynamicQParms.toggleTarget = 1;
                    BleMvxApplication._config.RFID_FixedQParms.toggleTarget = 1;
                    break;
            }

            if (buttonAlgorithm.Text == "DYNAMICQ")
            {
                BleMvxApplication._config.RFID_Algorithm = CSLibrary.Constants.SingulationAlgorithm.DYNAMICQ;
            }
            else
            {
                BleMvxApplication._config.RFID_Algorithm = CSLibrary.Constants.SingulationAlgorithm.FIXEDQ;
            }

            BleMvxApplication._config.RFID_Profile = UInt16.Parse(buttonProfile.Text.Substring(0, 1));

            BleMvxApplication._config.RFID_DynamicQParms.startQValue = uint.Parse(entryQOverride.Text);
            BleMvxApplication._config.RFID_FixedQParms.qValue = uint.Parse(entryQOverride.Text);

            /*
            switch (BleMvxApplication._config.RFID_Algorithm)
            {
                case CSLibrary.Constants.SingulationAlgorithm.DYNAMICQ:
                    BleMvxApplication._config.RFID_DynamicQParms.startQValue = uint.Parse(entryQOverride.Text);
                    break;

                case CSLibrary.Constants.SingulationAlgorithm.FIXEDQ:
                    BleMvxApplication._config.RFID_FixedQParms.qValue = uint.Parse(entryQOverride.Text);
                    break;
            }
            */
            
            BleMvxApplication._config.RFID_TagDelayTime = int.Parse(entryTagDelay.Text);
            BleMvxApplication._config.RFID_Antenna_Dwell[0] = UInt32.Parse(entryInventoryDuration.Text);
            //BleMvxApplication._config.RFID_NewTagLocation = switchNewTagLocation.IsToggled;
            BleMvxApplication._config.RFID_Focus = switchFocus.IsToggled;
            BleMvxApplication._config.RFID_RFLNAcompression = int.Parse(buttonRFLNAcompression.Text);
            BleMvxApplication._config.RFID_RFLNAGain = int.Parse(buttonRFLNAGain.Text);
            BleMvxApplication._config.RFID_IFLNAGain = int.Parse(buttonIFLNAGain.Text);
            BleMvxApplication._config.RFID_AGCGain = int.Parse(buttonAGCGain.Text);

            BleMvxApplication.SaveConfig();

            switch (BleMvxApplication._config.RFID_FrequenceSwitch)
            {
                case 0:
                    BleMvxApplication._reader.rfid.SetHoppingChannels(BleMvxApplication._config.RFID_Region);
                    break;
                case 1:
                    BleMvxApplication._reader.rfid.SetFixedChannel(BleMvxApplication._config.RFID_Region, BleMvxApplication._config.RFID_FixedChannel);
                    break;
                case 2:
                    BleMvxApplication._reader.rfid.SetAgileChannels(BleMvxApplication._config.RFID_Region);
                    break;
            }
        }

        public async void buttonRegionClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Regions", "Cancel", null, ActiveRegionsTextList);

            if (answer != null && answer !="Cancel")
            {
                int cnt;

                buttonRegion.Text = answer;

                for (cnt = 0; cnt < _regionsName.Length; cnt++)
                {
                    if (_regionsName[cnt] == answer)
                    {
                        ActiveFrequencyList = BleMvxApplication._reader.rfid.GetAvailableFrequencyTable(_regionsCode[cnt]);
                        break;
                    }
                }
                if (cnt == _regionsName.Length)
                    ActiveFrequencyList = new double[1] { 0.0 };

                ActiveFrequencyTextList = ActiveFrequencyList.OfType<object>().Select(o => o.ToString()).ToArray();
                buttonFixedChannel.Text = ActiveFrequencyTextList[0];
            }
        }

        public async void buttonFrequencyOrderClicked(object sender, EventArgs e)
        {
            string answer;
            
            answer = await DisplayActionSheet("Frequence Channel Order", "Cancel", null, _freqOrderOptions);

            if (answer != null && answer !="Cancel")
                buttonFrequencyOrder.Text = answer;

            checkbuttonFixedChannel();
        }

        public async void buttonFixedChannelClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Frequence Channel Order", "Cancel", null, ActiveFrequencyTextList);

            if (answer != null && answer !="Cancel")
                buttonFixedChannel.Text = answer;
        }

        public async void entryPowerCompleted(object sender, EventArgs e)
        {
            uint value;

            try
            {
                value = uint.Parse(entryPower.Text);
                if (value < 0 || value > 330)
                    throw new System.ArgumentException("Value not valid", "tagPopulation");
                entryPower.Text = value.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("", "Value not valid!!!", "OK");
                entryPower.Text = "300";
            }
        }

        public async void entryTagPopulationCompleted(object sender, EventArgs e)
        {
            uint tagPopulation;

            try
            {
                tagPopulation = uint.Parse(entryTagPopulation.Text);
                if (tagPopulation < 1 || tagPopulation > 8192)
                    throw new System.ArgumentException("Value not valid", "tagPopulation");
                entryTagPopulation.Text = tagPopulation.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("", "Value not valid!!!", "OK");
                tagPopulation = 30;
                entryTagPopulation.Text = "30";
            }

            if (!entryQOverride.IsEnabled)
                entryQOverride.Text = ((uint)(Math.Log((tagPopulation * 2), 2)) + 1).ToString();
        }

        public async void entryQOverrideCompiled(object sender, EventArgs e)
        {
            uint Q;
            try
            {
                Q = uint.Parse(entryQOverride.Text);
                if (Q < 0 || Q > 15)
                    throw new System.ArgumentException("Value not valid", "tagPopulation");
                entryQOverride.Text = Q.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("", "Value not valid!!!", "OK");
                Q = 6;
                entryQOverride.Text = "6";
            }

            //entryTagPopulation.Text = (1U << (int)Q).ToString();
        }

        public async void buttonQOverrideClicked(object sender, EventArgs e)
        {
            if (entryQOverride.IsEnabled)
            {
                entryQOverride.IsEnabled = false;
                buttonQOverride.Text = "Override";
                entryTagPopulationCompleted(null, null);
            }
            else
            {
                entryQOverride.IsEnabled = true;
                buttonQOverride.Text = "Reset";
            }
        }

        public async void entryTagDelayCompleted(object sender, EventArgs e)
        {
            uint value;

            try
            {
                value = uint.Parse(entryTagDelay.Text);
                if (value < 0 || value > 15)
                    throw new System.ArgumentException("Value not valid", "tagPopulation");
                entryTagDelay.Text = value.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("", "Value not valid!!!", "OK");
                entryTagDelay.Text = "0";
            }
        }

        public async void entryInventoryDurationCompleted(object sender, EventArgs e)
        {
            uint value;

            try
            {
                value = uint.Parse(entryInventoryDuration.Text);
                if (value < 0 || value > 3000)
                    throw new System.ArgumentException("Value not valid", "tagPopulation");
                entryInventoryDuration.Text = value.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("", "Value not valid!!!", "OK");
                entryInventoryDuration.Text = "0";
            }
        }

        public async void buttonSessionClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Session", "Cancel", null, "S0", "S1", "S2", "S3"); // S2 S3

            if (answer != null && answer !="Cancel")
                buttonSession.Text = answer;
        }

        public async void buttonTargetClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet(null, "Cancel", null, "A", "B", "Toggle A/B");

            if (answer != null && answer !="Cancel")
                buttonTarget.Text = answer;
        }

        public async void buttonAlgorithmClicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Algorithm", "", "DYNAMICQ", "FIXEDQ");
            buttonAlgorithm.Text = answer ? "DYNAMICQ" : "FIXEDQ";
        }

        void SetQvalue ()
        {
            switch (buttonAlgorithm.Text)
            {
                default:
                    entryQOverride.Text = "0";
                    break;

                case "DYNAMICQ":
                    entryQOverride.Text = BleMvxApplication._config.RFID_DynamicQParms.startQValue.ToString();
                    break;

                case "FIXEDQ":
                    entryQOverride.Text = BleMvxApplication._config.RFID_FixedQParms.qValue.ToString();
                    break;
            }
        }

        public async void buttonProfileClicked(object sender, EventArgs e)
        {
            int cnt;
            CSLibrary.Constants.RegionCode region = CSLibrary.Constants.RegionCode.UNKNOWN;

            for (cnt = 0; cnt < _regionsName.Length; cnt++)
            {
                if (_regionsName[cnt] == buttonRegion.Text)
                {
                    region = _regionsCode[cnt];
                    break;
                }
            }

            var currentProfileList = BleMvxApplication._reader.rfid.GetActiveLinkProfile(region);

            string[] profileList = new string[currentProfileList.Length];

            for (cnt = 0; cnt < currentProfileList.Length; cnt++)
                profileList[cnt] = _profileList[cnt];

            var answer = await DisplayActionSheet(null, "Cancel", null, profileList);

            if (answer != null && answer !="Cancel")
                buttonProfile.Text = answer;
        }

        public async void buttonRFLNAcompressionClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet(null, "Cancel", null, "0", "1");

            if (answer != null && answer != "Cancel")
                buttonRFLNAcompression.Text = answer;
        }

        public async void buttonRFLNAGainClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet(null, "Cancel", null, "1", "7", "13");

            if (answer != null && answer != "Cancel")
            {
                buttonRFLNAGain.Text = answer;

                if (answer == "13")
                {
                    buttonRFLNAcompression.Text = "0";
                    buttonRFLNAcompression.IsEnabled = false;
                }
                else
                {
                    buttonRFLNAcompression.IsEnabled = true;
                }
            }
        }

        public async void buttonIFLNAGainClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet(null, "Cancel", null, "24", "18", "12", "6");

            if (answer != null && answer != "Cancel")
                buttonIFLNAGain.Text = answer;
        }

        public async void buttonAGCGainClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet(null, "Cancel", null, "-12", "-6", "0", "6");

            if (answer != null && answer != "Cancel")
                buttonAGCGain.Text = answer;
        }

        public async void switchFocusPropertyChanged(object sender, EventArgs e)
        {
            if (switchFocus == null)
                return;

            if (switchFocus.IsToggled)
            {
                buttonSession.Text = "S1";
                buttonTarget.Text = "A";
                entryTagDelay.Text = "0";
                entryInventoryDuration.Text = "2000";
                buttonSession.IsEnabled = false;
                buttonTarget.IsEnabled = false;
                entryTagDelay.IsEnabled = false;
                entryInventoryDuration.IsEnabled = false;
            }
            else
            {
                buttonSession.IsEnabled = true;
                buttonTarget.IsEnabled = true;
                entryTagDelay.IsEnabled = true;
                entryInventoryDuration.IsEnabled = true;
            }

        }

    }
}
