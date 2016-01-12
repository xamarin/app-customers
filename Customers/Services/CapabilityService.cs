using System;
using Xamarin.Forms;
using Customers;

[assembly: Xamarin.Forms.Dependency (typeof (CapabilityService))]

namespace Customers
{
    public class CapabilityService : ICapabilityService
    {
        readonly IEnvironmentService _EnvironmentService;

        public CapabilityService()
        {
            _EnvironmentService = DependencyService.Get<IEnvironmentService>();
        }

        #region ICapabilityService implementation

        public bool CanMakeCalls
        {
            get
            {
                return _EnvironmentService.IsRealDevice || (Device.OS != TargetPlatform.iOS);
            }
        }

        public bool CanSendMessages
        {
            get
            {
                return _EnvironmentService.IsRealDevice || (Device.OS != TargetPlatform.iOS);
            }
        }

        public bool CanSendEmail
        {
            get
            {
                return _EnvironmentService.IsRealDevice || (Device.OS != TargetPlatform.iOS);
            }
        }

        #endregion
    }
}

