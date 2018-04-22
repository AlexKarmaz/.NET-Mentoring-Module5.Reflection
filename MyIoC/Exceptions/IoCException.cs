using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyIoC.Exceptions
{
    [Serializable]
    class IoCException : Exception
    {
        public IoCException()
        {
        }

        public IoCException(string message) 
            : base(message)
        {
        }

        public IoCException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
