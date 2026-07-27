using System;
using System.Collections.Generic;
using System.Text;

namespace Custodian.Domain.Exceptions
{
    public class NotFound: BaseException
    {
        public NotFound(string Name, Object Key): base($"Entity \"{Name}\" with key ({Key}) was not found."){ }
    }
}
