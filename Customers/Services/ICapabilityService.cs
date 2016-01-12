using System;

namespace Customers
{
    public interface ICapabilityService
    {
        bool CanMakeCalls { get; }
        bool CanSendMessages { get; }
        bool CanSendEmail { get; }
    }
}

