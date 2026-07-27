using System;
using System.Collections.Generic;
using System.Text;

namespace Custodian.Domain.Exceptions
{
    public abstract class BaseException: Exception
    {
        protected BaseException(string message): base(message) {}
    }
}
