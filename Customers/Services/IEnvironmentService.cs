using System;

namespace Customers
{
    public interface IEnvironmentService
    {
        bool IsRealDevice { get; }
    }
}

