using System;
using Android.OS;
using Customers.Droid;

[assembly: Xamarin.Forms.Dependency (typeof (EnvironmentService))]

namespace Customers.Droid
{
    public class EnvironmentService : IEnvironmentService
    {
        #region IEnvironmentService implementation
        public bool IsRealDevice
        {
            get
            {
                string f = Build.Fingerprint;
                return !(f.Contains("vbox") || f.Contains("generic") || f.Contains("vsemu"));
            }
        }
        #endregion
    }
}

