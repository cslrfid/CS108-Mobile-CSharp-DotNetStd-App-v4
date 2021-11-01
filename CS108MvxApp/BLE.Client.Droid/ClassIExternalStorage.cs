﻿using Android.App;

[assembly: Xamarin.Forms.Dependency(typeof(BLE.Client.Droid.AndroidImplementation))]
namespace BLE.Client.Droid
{

        public class AndroidImplementation : IExternalStorage
        {
            public string GetPath()
            {
                Android.Content.Context context = Android.App.Application.Context;
                var filePath = context.GetExternalFilesDir("");
                return filePath.Path;
            }
        }



/*
    public class Version_Android : Activity, BLE.Client.IAppVersion
    {
        public string GetVersion()
        {
            var context = global::Android.App.Application.Context;

            Android.Content.PM.PackageManager manager = context.PackageManager;
            Android.Content.PM.PackageInfo info = manager.GetPackageInfo(context.PackageName, 0);

            return info.VersionName;
        }

        public int GetBuild()
        {
            var context = global::Android.App.Application.Context;

            Android.Content.PM.PackageManager manager = context.PackageManager;
            Android.Content.PM.PackageInfo info = manager.GetPackageInfo(context.PackageName, 0);

            return info.VersionCode;
        }
    }
*/
}