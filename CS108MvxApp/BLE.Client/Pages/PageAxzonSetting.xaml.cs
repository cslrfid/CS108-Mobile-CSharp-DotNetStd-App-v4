﻿using System;
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
    public partial class PageAxzonSetting : MvxContentPage<ViewModelAxzonSetting>
    {
        string[] _tagTypeOptions = { "Magnus S2", "Magnus S3", "Xerxes" };
        string[] _powerOptions = { "Low (16dBm)", "Mid (23dBm)", "High (30dBm)", "Cycle Power by Trigger Button", "Follow system Setting" };
        string[] _targetOptions = { "A", "B", "Toggle A/B" };
        string[] _indicatorsProfileOptions = { "Hot temperature", "Cold temperature", "Moisture detection" };
        string[] _sensorTypeOptions = { "Humidity", "Temperature" };
        string[] _sensorCodeUnitOptions = { "RAW Sensor Code", "Dry / Wet" };
        string[] _temperatureUnitOptions = { "RAW Average", "ºF", "ºC" };
        int[] _minOCRSSIs = { 0, 5, 10, 10 };
        int[] _maxOCRSSIs = { 21, 18, 21, 21 };
        string[] _thresholdComparisonOptions = { ">", "<" };
        int[] _thresholdValueOptions = { 100, -1, 58 };
        string[] _thresholdColorOptions = { "Red", "Blue" };

        public PageAxzonSetting()
        {
            InitializeComponent();

            buttonTagType.Text = _tagTypeOptions[2];
            buttonPower.Text = _powerOptions[2];
            buttonTarget.Text = _targetOptions[2];
            SetIndicatorsProfile(0);
            entryDelay.Text = "15";
            entryWetDryThreshold.Text = "0";
        }

        protected override void OnAppearing()
        {
            buttonOK.RemoveBinding(Button.CommandProperty);
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            buttonOK.RemoveBinding(Button.CommandProperty);
            base.OnDisappearing();
        }

        public async void buttonTagTypeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Tag Type", "Cancel", null, _tagTypeOptions);

            if (answer != null && answer != "Cancel")
            {
                try
                {
                    buttonTagType.Text = answer;
                }
                catch (Exception ex)
                {
                    CSLibrary.Debug.WriteLine(ex.Message);
				}
/*
                switch (Array.IndexOf(_tagTypeOptions, buttonTagType.Text))
                {
                    case 0: // S2
                        SetIndicatorsProfile(2);
                        stackLayoutS3Options.IsVisible = false;
                        stackLayoutXerxesOption.IsVisible = false;
                        entryWetDryThreshold.Text = "13";
                        break;

                    case 1: // S3
                        stackLayoutS3Options.IsVisible = true;
                        stackLayoutXerxesOption.IsVisible = false;
                        entryWetDryThreshold.Text = "160";
                        break;

                    case 2: // Xerxes
                        SetSensorType(1);
                        //                        SetIndicatorsProfile(0);
                        //                        buttonSensorType.Text = _sensorTypeOptions[1];
                        stackLayoutS3Options.IsVisible = false;
                        stackLayoutXerxesOption.IsVisible = true;
                        break;
                }
*/
            }
        }

        public async void buttonTagTypePropertyChanged(object sender, EventArgs e)
        {
            if (buttonTagType != null)
            {
                switch (Array.IndexOf(_tagTypeOptions, buttonTagType.Text))
                {
                    case 0: // S2
                        SetIndicatorsProfile(2);
                        stackLayoutS3Options.IsVisible = false;
                        stackLayoutXerxesOption.IsVisible = false;
                        entryWetDryThreshold.Text = "13";
                        break;

                    case 1: // S3
                        stackLayoutS3Options.IsVisible = true;
                        stackLayoutXerxesOption.IsVisible = false;
                        entryWetDryThreshold.Text = "160";
                        break;

                    case 2: // Xerxes
                        SetSensorType(1);
//                        SetIndicatorsProfile(0);
//                        buttonSensorType.Text = _sensorTypeOptions[1];
                        stackLayoutS3Options.IsVisible = false;
                        stackLayoutXerxesOption.IsVisible = true;
                        break;
                }
            }
        }

        public async void buttonPowerClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Power", "Cancel", null, _powerOptions);

            if (answer != null && answer != "Cancel")
            {
                buttonPower.Text = answer;
            }
        }

        public async void buttonTargetClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Target", "Cancel", null, _targetOptions);

            if (answer != null && answer != "Cancel")
                buttonTarget.Text = answer;
        }

        public async void buttonSensorTypeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Sensor Type", "Cancel", null, _sensorTypeOptions);

            if (answer != null && answer != "Cancel")
            {
                SetSensorType((uint)Array.IndexOf(_sensorTypeOptions, answer));
            }
        }

        public async void buttonSensorTypePropertyChanged(object sender, EventArgs e)
        {
            if (buttonTagType != null && buttonSensorType != null)
            {
                switch (Array.IndexOf(_sensorTypeOptions, buttonSensorType.Text))
                {
                    case 0: // Sensor type
                        entryMinOCRSSI.Text = _minOCRSSIs[0].ToString();
                        entryMaxOCRSSI.Text = _maxOCRSSIs[0].ToString();
                        buttonSensorUnit.Text = _sensorCodeUnitOptions[1];
                        //stacklayoutWetDryThreshold.IsVisible = false;
                        break;

                    case 1: // Temperature
                        entryMinOCRSSI.Text = _minOCRSSIs[1].ToString();
                        entryMaxOCRSSI.Text = _maxOCRSSIs[1].ToString();
                        buttonSensorUnit.Text = _temperatureUnitOptions[2];
                        //stacklayoutWetDryThreshold.IsVisible = false;
                        break;
                }
            }
        }

        public async void buttonIndicatorsProfileClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Indicators Profile", "Cancel", null, _indicatorsProfileOptions);

            if (answer != null && answer != "Cancel")
            {
                SetIndicatorsProfile((uint)Array.IndexOf(_indicatorsProfileOptions, answer));
            }
        }

        public async void buttonSensorUnitClicked(object sender, EventArgs e)
        {
            string answer;

            if (buttonSensorType.Text == _sensorTypeOptions[0])
                answer = await DisplayActionSheet("Sensor Unit", "Cancel", null, _sensorCodeUnitOptions);
            else
                answer = await DisplayActionSheet("Sensor Unit", "Cancel", null, _temperatureUnitOptions);

            if (answer != null && answer != "Cancel")
            {
                buttonSensorUnit.Text = answer;
            }
        }

        public async void buttonSensorUnitPropertyChanged(object sender, EventArgs e)
        {
            if (buttonTagType != null)
            {
                if (Array.IndexOf(_sensorTypeOptions, buttonSensorType.Text) == 0 && Array.IndexOf(_sensorCodeUnitOptions, buttonSensorUnit.Text) == 1)
                    stacklayoutWetDryThreshold.IsVisible = true;
                else
                    stacklayoutWetDryThreshold.IsVisible = false;
            }
        }

        public async void buttonThresholdComparisonClicked(object sender, EventArgs e)
        {
            string answer = await DisplayActionSheet("Threshold Comparison", "Cancel", null, _thresholdComparisonOptions);

            if (answer != null && answer != "Cancel")
            {
                buttonThresholdComparison.Text = answer;
            }
        }

        public async void buttonThresholdColorClicked(object sender, EventArgs e)
        {
            string answer = await DisplayActionSheet("Threshold Color", "Cancel", null, _thresholdColorOptions);

            if (answer != null && answer != "Cancel")
            {
                buttonThresholdColor.Text = answer;
            }
        }

        public async void ButtonOK_Clicked(object sender, EventArgs e)
        {
            BleMvxApplication._rfMicro_TagType = Array.IndexOf(_tagTypeOptions, buttonTagType.Text);
            BleMvxApplication._rfMicro_Power = Array.IndexOf(_powerOptions, buttonPower.Text);
            BleMvxApplication._rfMicro_Target = Array.IndexOf(_targetOptions, buttonTarget.Text);
            BleMvxApplication._rfMicro_SensorType = Array.IndexOf(_sensorTypeOptions, buttonSensorType.Text);
            switch (BleMvxApplication._rfMicro_SensorType)
            {
                case 0: // Sensor code
                    switch (Array.IndexOf(_sensorCodeUnitOptions, buttonSensorUnit.Text))
                    {
                        case 0: // RAW
                            BleMvxApplication._rfMicro_SensorUnit = 1;
                            break;

                        default: // Dry / Wet
                            BleMvxApplication._rfMicro_SensorUnit = 4;
                            break;
                    }
                    break;
                default: // Temperature
                    switch (Array.IndexOf(_temperatureUnitOptions, buttonSensorUnit.Text))
                    {
                        case 0: // Ave Sensor code
                            BleMvxApplication._rfMicro_SensorUnit = 0;
                            break;

                        case 1: // Temperature F
                            BleMvxApplication._rfMicro_SensorUnit = 2;
                            break;

                        default: // Temperature C
                            BleMvxApplication._rfMicro_SensorUnit = 3;
                            break;
                    }
                    break;
            }
            BleMvxApplication._rfMicro_minOCRSSI = int.Parse(entryMinOCRSSI.Text);
            BleMvxApplication._rfMicro_maxOCRSSI = int.Parse(entryMaxOCRSSI.Text);
            BleMvxApplication._rfMicro_thresholdComparison = Array.IndexOf(_thresholdComparisonOptions, buttonThresholdComparison.Text);
            BleMvxApplication._rfMicro_thresholdValue = int.Parse(entryThresholdValue.Text);
            BleMvxApplication._rfMicro_thresholdColor = buttonThresholdColor.Text;
            BleMvxApplication._rfMicro_WetDryThresholdValue = int.Parse(entryWetDryThreshold.Text);
            BleMvxApplication._xerxes_delay = int.Parse(entryDelay.Text);

            buttonOK.SetBinding(Button.CommandProperty, new Binding("OnOKButtonCommand"));
            buttonOK.Command.Execute(1);
            buttonOK.RemoveBinding(Button.CommandProperty);
        }

        bool SetIndicatorsProfile(uint index)
        {
            switch (index)
            {
                case 0:
                    buttonIndicatorsProfile.Text = _indicatorsProfileOptions[0];
                    SetSensorType(1);
                    buttonSensorUnit.Text = _temperatureUnitOptions[1];
                    buttonThresholdComparison.Text = _thresholdComparisonOptions[0];
                    entryThresholdValue.Text = _thresholdValueOptions[0].ToString();
                    buttonThresholdColor.Text = _thresholdColorOptions[0];
                    break;
                case 1:
                    buttonIndicatorsProfile.Text = _indicatorsProfileOptions[1];
                    SetSensorType(1);
                    buttonSensorUnit.Text = _temperatureUnitOptions[2];
                    buttonThresholdComparison.Text = _thresholdComparisonOptions[1];
                    entryThresholdValue.Text = _thresholdValueOptions[1].ToString();
                    buttonThresholdColor.Text = _thresholdColorOptions[1];
                    break;
                case 2:
                    buttonIndicatorsProfile.Text = _indicatorsProfileOptions[2];
                    SetSensorType(0);
                    buttonSensorUnit.Text = _sensorCodeUnitOptions[1];
                    buttonThresholdComparison.Text = _thresholdComparisonOptions[0];
                    entryThresholdValue.Text = _thresholdValueOptions[2].ToString();
                    buttonThresholdColor.Text = _thresholdColorOptions[1];
                    break;
                default:
                    return false;
            }

            return true;
        }

        bool SetSensorType(uint index)
        {
            if (index >= _sensorTypeOptions.Length)
                return false;

            buttonSensorType.Text = _sensorTypeOptions[index];
            entryMinOCRSSI.Text = _minOCRSSIs[index].ToString();
            entryMaxOCRSSI.Text = _maxOCRSSIs[index].ToString();

            switch (index)
            {
                case 0:
                    buttonSensorUnit.Text = _sensorCodeUnitOptions[1];
                    break;
                default:
                    buttonSensorUnit.Text = _temperatureUnitOptions[2];
                    break;
            }

            return true;
        }
    }
}


#if oldcode

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BLE.Client.Pages
{
    public partial class PageAxzonSetting
    {
        string[] _tagTypeOptions = { "Magnus S2", "Magnus S3", "Xerxes" };
        string[] _powerOptions = { "Low (16dBm)", "Mid (23dBm)", "High (30dBm)", "Cycle Power by Trigger Button", "Follow system Setting" };
        string[] _targetOptions = { "A", "B", "Toggle A/B"};
        string[] _indicatorsProfileOptions = { "Hot temperature", "Cold temperature", "Moisture detection" };
        string[] _sensorTypeOptions = { "Sensor Code", "Temperature" };
        string[] _sensorCodeUnitOptions = { "RAW", "Dry / Wet" };
        string[] _temperatureUnitOptions = { "code", "ºF", "ºC" };
        int[] _minOCRSSIs = { 0, 5, 10, 10 };
        int [] _maxOCRSSIs = { 21, 18, 21, 21 };
        int[,] _OCRSSIRange = new int[,] { { 0, 21 }, { 8, 18 }, { 10, 21 }, { 10, 21 } };

        string[] _thresholdComparisonOptions = { ">", "<" };
        int[] _thresholdValueOptions = { 100, -1, 58 };
        string [] _thresholdColorOptions = { "Red", "Blue"};

        public PageAxzonSetting()
        {
            InitializeComponent();

            buttonTagType.Text = _tagTypeOptions[2];
            buttonPower.Text = _powerOptions[2];
            buttonTarget.Text = _targetOptions[2];
            SetIndicatorsProfile(0);
            entryWetDryThreshold.Text = "0";

            SetTagTypeProfile();
        }

        protected override void OnAppearing()
        {
            buttonOK.RemoveBinding(Button.CommandProperty);
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            buttonOK.RemoveBinding(Button.CommandProperty);
            base.OnDisappearing();
        }

        public async void buttonTagTypeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Tag Type", "Cancel", null, _tagTypeOptions);

            if (answer != null && answer !="Cancel")
            {
                buttonTagType.Text = answer;

                SetTagTypeProfile();
            }
        }
        void SetTagTypeProfile()
        {
            switch (Array.IndexOf(_tagTypeOptions, buttonTagType.Text))
            {
                case 0: // S2
                    stackLayout1.IsVisible = true;
                    stackLayout2.IsVisible = false;
                    stackLayout3.IsVisible = false;
                    stackLayout4.IsVisible = true;
                    SetIndicatorsProfile(2);
                    buttonIndicatorsProfile.IsEnabled = false;
                    buttonSensorType.IsEnabled = false;
                    break;

                case 1: // S3
                    stackLayout1.IsVisible = true;
                    stackLayout2.IsVisible = true;
                    stackLayout3.IsVisible = false;
                    stackLayout4.IsVisible = true;
                    buttonIndicatorsProfile.IsEnabled = true;
                    buttonSensorType.IsEnabled = true;
                    break;

                default: // Xerxes
                    stackLayout1.IsVisible = false;
                    stackLayout2.IsVisible = false;
                    stackLayout3.IsVisible = true;
                    stackLayout4.IsVisible = false;
                    buttonSensorType.Text = _sensorTypeOptions[1];

                    break;
            }
        }
        public async void buttonPowerClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Power", "Cancel", null, _powerOptions);

            if (answer != null && answer !="Cancel")
            {
                buttonPower.Text = answer;
            }
        }

        public async void buttonTargetClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Target", "Cancel", null, _targetOptions);

            if (answer != null && answer !="Cancel")
                buttonTarget.Text = answer;
        }

        public async void buttonSensorTypeClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Sensor Type", "Cancel", null, _sensorTypeOptions);

            if (answer != null && answer !="Cancel")
            {
                SetSensorType((uint)Array.IndexOf(_sensorTypeOptions, answer));
            }
        }

        public async void buttonIndicatorsProfileClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Indicators Profile", "Cancel", null, _indicatorsProfileOptions);

            if (answer != null && answer !="Cancel")
            {
                SetIndicatorsProfile((uint)Array.IndexOf(_indicatorsProfileOptions, answer));
            }
        }

        public async void buttonSensorUnitClicked(object sender, EventArgs e)
        {
            string answer;

            if (buttonSensorType.Text == _sensorTypeOptions[0])
                answer = await DisplayActionSheet("Sensor Unit", "Cancel", null, _sensorCodeUnitOptions);
            else
                answer = await DisplayActionSheet("Sensor Unit", "Cancel", null, _temperatureUnitOptions);

            if (answer != null && answer !="Cancel")
            {
                buttonSensorUnit.Text = answer;
            }
        }

        public async void buttonSensorUnitPropertyChanged(object sender, EventArgs e)
        {
            if (buttonTagType != null)
            {
                if (Array.IndexOf(_tagTypeOptions, buttonTagType.Text) == 0)
                {
                    if (Array.IndexOf(_sensorCodeUnitOptions, buttonSensorUnit.Text) == 3)
                    {
                        entryMinOCRSSI.Text = _minOCRSSIs[2].ToString();
                        entryMaxOCRSSI.Text = _maxOCRSSIs[2].ToString();
                        stacklayoutWetDryThreshold.IsVisible = true;
                    }
                    else
                    {
                        stacklayoutWetDryThreshold.IsVisible = false;
                    }
                }
            }
        }

        public async void buttonThresholdComparisonClicked(object sender, EventArgs e)
        {
            string answer = await DisplayActionSheet("Threshold Comparison", "Cancel", null, _thresholdComparisonOptions);

            if (answer != null && answer !="Cancel")
            {
                buttonThresholdComparison.Text = answer;
            }
        }

        public async void buttonThresholdColorClicked(object sender, EventArgs e)
        {
            string answer = await DisplayActionSheet("Threshold Color", "Cancel", null, _thresholdColorOptions);

            if (answer != null && answer !="Cancel")
            {
                buttonThresholdColor.Text = answer;
            }
        }

        public async void ButtonOK_Clicked(object sender, EventArgs e)
        {
            BleMvxApplication._rfMicro_TagType = Array.IndexOf(_tagTypeOptions, buttonTagType.Text);
            BleMvxApplication._rfMicro_Power = Array.IndexOf(_powerOptions, buttonPower.Text);
            BleMvxApplication._rfMicro_Target = Array.IndexOf(_targetOptions, buttonTarget.Text); 
            BleMvxApplication._rfMicro_SensorType = Array.IndexOf(_sensorTypeOptions, buttonSensorType.Text);
            switch (BleMvxApplication._rfMicro_SensorType)
            {
                case 0:
                    BleMvxApplication._rfMicro_SensorUnit = Array.IndexOf(_sensorCodeUnitOptions, buttonSensorUnit.Text);
                    if (BleMvxApplication._rfMicro_SensorUnit != 0)
                        BleMvxApplication._rfMicro_SensorUnit += 2;
                    break;
                default:
                    BleMvxApplication._rfMicro_SensorUnit = Array.IndexOf(_temperatureUnitOptions, buttonSensorUnit.Text);
                    break;
            }
            BleMvxApplication._rfMicro_minOCRSSI = int.Parse(entryMinOCRSSI.Text);
            BleMvxApplication._rfMicro_maxOCRSSI = int.Parse(entryMaxOCRSSI.Text);
            BleMvxApplication._rfMicro_thresholdComparison = Array.IndexOf(_thresholdComparisonOptions, buttonThresholdComparison.Text);
            BleMvxApplication._rfMicro_thresholdValue = int.Parse(entryThresholdValue.Text);
            BleMvxApplication._rfMicro_thresholdColor = buttonThresholdColor.Text;
            BleMvxApplication._rfMicro_WetDryThresholdValue = int.Parse(entryWetDryThreshold.Text);
            BleMvxApplication._xerxes_delay = int.Parse(entryDelay.Text);

            buttonOK.SetBinding(Button.CommandProperty, new Binding("OnOKButtonCommand"));
            buttonOK.Command.Execute(1);
            buttonOK.RemoveBinding(Button.CommandProperty);
        }

        bool SetIndicatorsProfile(uint index)
        {
            switch (index)
            {
                case 0:
                    buttonIndicatorsProfile.Text = _indicatorsProfileOptions[0];
                    SetSensorType(1);
                    buttonSensorUnit.Text = _temperatureUnitOptions[1];
                    buttonThresholdComparison.Text = _thresholdComparisonOptions[0];
                    entryThresholdValue.Text = _thresholdValueOptions[0].ToString();
                    buttonThresholdColor.Text = _thresholdColorOptions[0];
                    break;
                case 1:
                    buttonIndicatorsProfile.Text = _indicatorsProfileOptions[1];
                    SetSensorType(1);
                    buttonSensorUnit.Text = _temperatureUnitOptions[2];
                    buttonThresholdComparison.Text = _thresholdComparisonOptions[1];
                    entryThresholdValue.Text = _thresholdValueOptions[1].ToString();
                    buttonThresholdColor.Text = _thresholdColorOptions[1];
                    break;
                case 2:
                    buttonIndicatorsProfile.Text = _indicatorsProfileOptions[2];
                    SetSensorType(0);
                    buttonSensorUnit.Text = _sensorCodeUnitOptions[1];
                    buttonThresholdComparison.Text = _thresholdComparisonOptions[0];
                    entryThresholdValue.Text = _thresholdValueOptions[2].ToString();
                    buttonThresholdColor.Text = _thresholdColorOptions[1];
                    break;
                default:
                    return false;
            }

            return true;
        }

        bool SetSensorType(uint index)
        {
            if (index >= _sensorTypeOptions.Length)
                return false;

            buttonSensorType.Text = _sensorTypeOptions[index];
            entryMinOCRSSI.Text = _minOCRSSIs[index].ToString();
            entryMaxOCRSSI.Text = _maxOCRSSIs[index].ToString();

            switch (index)
            {
                case 0:
                    buttonSensorUnit.Text = _sensorCodeUnitOptions[0];
                    break;
                default:
                    buttonSensorUnit.Text = _temperatureUnitOptions[2];
                    break;
            }

            return true;
        }
    }
}
#endif
