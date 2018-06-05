using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Components.Exceptions
{
    public class UnregisteredComponentException: WarningException
    {
        public UnregisteredComponentException() : base() { }
        public UnregisteredComponentException(string message) : base(message) { }
        public UnregisteredComponentException(string message, Exception inner) : base(message, inner) { }
    }
}
