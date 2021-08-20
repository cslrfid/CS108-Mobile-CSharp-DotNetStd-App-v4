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
	public partial class PageEM4152SensorCalibrationWord : MvxContentPage<ViewModelEM4152SensorCalibrationWord>
    {
        string[] _CalibrationLockOptionsList = new string[] { "Unlock", "Locked" };
        string[] _ValibrationMarginOptionsList = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32" };

        public PageEM4152SensorCalibrationWord()
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

        public async void buttonCalibrationLockClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Calibration Lock", "Cancel", null, _CalibrationLockOptionsList);

            if (answer != null && answer != "Cancel")
            {
                buttonCalibrationLock.Text = answer;
                SetSensorCalibrationWordText();
            }
        }
        
        public async void buttonValibrationMarginClicked(object sender, EventArgs e)
        {
            var answer = await DisplayActionSheet("Calibration Lock", "Cancel", null, _ValibrationMarginOptionsList);

            if (answer != null && answer != "Cancel")
            {
                buttonValibrationMargin.Text = answer;
                SetSensorCalibrationWordText();
            }
        }

        public async void labelSensorCalibrationWordPropertyChanged(object sender, EventArgs e)
        {
            if (labelSensorCalibrationWord == null)
                return;

            UInt16 value = Convert.ToUInt16(labelSensorCalibrationWord.Text, 16);

            var a = value >> 15 & 0x01;
            var b = value >> 10 & 0x1f;
            var c = value & 0xff;

            buttonCalibrationLock.Text = _CalibrationLockOptionsList[a];
            buttonValibrationMargin.Text = _ValibrationMarginOptionsList[b];
            extryCalibrationData.Text = c.ToString("X02");
        }

        void SetSensorCalibrationWordText ()
        {
            UInt16 value = 0;

            var a = Array.IndexOf(_CalibrationLockOptionsList, buttonCalibrationLock.Text);
            var b = Array.IndexOf(_ValibrationMarginOptionsList, buttonValibrationMargin.Text);
            var c = Convert.ToUInt16(extryCalibrationData.Text, 16);

            value = (UInt16)((a << 15) | (b << 10) | (c) );

            labelSensorCalibrationWord.Text = value.ToString("X04");
        }

        public async void ButtonWriteClicked(object sender, EventArgs e)
        {
            SetSensorCalibrationWordText();

            buttonWrite.SetBinding(Button.CommandProperty, new Binding("ButtonWriteCommand"));
            buttonWrite.Command.Execute(1);
            buttonWrite.RemoveBinding(Button.CommandProperty);
        }
    }
}
