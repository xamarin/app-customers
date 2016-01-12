using ObjCRuntime;
using Customers.iOS;

[assembly: Xamarin.Forms.Dependency (typeof (EnvironmentService))]

namespace Customers.iOS
{
    public class EnvironmentService : IEnvironmentService
    {
        #region IEnvironmentService implementation
        public bool IsRealDevice
        {
            get { return Runtime.Arch == Arch.DEVICE; }
        }
        #endregion
    }
}

