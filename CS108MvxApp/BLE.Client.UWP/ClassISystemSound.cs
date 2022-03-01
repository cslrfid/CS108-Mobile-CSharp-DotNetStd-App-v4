using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(BLE.Client.UWP.SystemSound_UWP))]
namespace BLE.Client.UWP
{
    public class SystemSound_UWP : BLE.Client.ISystemSound
    {
        static public void Initialization()
        {
        }

        public void SystemSound(int id)
        {
        }
    }
}
