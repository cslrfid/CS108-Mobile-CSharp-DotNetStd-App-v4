﻿using System;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;

using Android.Content;

using Android.App;
using Android.Media;

[assembly: Dependency(typeof(BLE.Clinet.Droid.SystemSound_Android))]
namespace BLE.Clinet.Droid
{
    public class SystemSound_Android : Activity, BLE.Client.ISystemSound
    {
        static Context _applicationContext;
        static MediaPlayer _player2;
        static MediaPlayer _player3;
        static MediaPlayer _player4;

        static public void Initialization(Context applicationContext)
        {
            _applicationContext = applicationContext;

            _player2 = MediaPlayer.Create(_applicationContext, BLE.Client.Droid.Resource.Raw.beeplow);
            _player3 = MediaPlayer.Create(_applicationContext, BLE.Client.Droid.Resource.Raw.beephigh);
            _player4 = MediaPlayer.Create(_applicationContext, BLE.Client.Droid.Resource.Raw.beep3s1khz);
        }

        //CSLibraryv4: updated
        static public void Initialization()
        {
            var context = global::Android.App.Application.Context;

            _player2 = MediaPlayer.Create(context, BLE.Client.Droid.Resource.Raw.beeplow);
            _player3 = MediaPlayer.Create(context, BLE.Client.Droid.Resource.Raw.beephigh);
            _player4 = MediaPlayer.Create(context, BLE.Client.Droid.Resource.Raw.beep3s1khz);

        }


        public void SystemSound(int id)
        {
            try
            {
                switch (id)
                {
                    case 2:
                        if (_player2.IsPlaying)
                            _player2.Pause();
                        if (_player3.IsPlaying)
                            _player3.Pause();
                        if (_player4.IsPlaying)
                            _player4.Pause();
                        _player2.Start();
                        break;

                    case 1:
                    case 3:
                        if (_player2.IsPlaying)
                            _player2.Pause();
                        if (_player3.IsPlaying)
                            _player3.Pause();
                        if (_player4.IsPlaying)
                            _player4.Pause();
                        _player3.Start();
                        break;

                    case 4:
                        if (_player2.IsPlaying)
                            _player2.Pause();
                        if (_player3.IsPlaying)
                            _player3.Pause();

                        if (!_player4.IsPlaying)
                            _player4.Start();
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
