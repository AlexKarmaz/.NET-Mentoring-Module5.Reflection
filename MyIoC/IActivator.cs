using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyIoC
{
    public interface IActivator
    {
        object CreateInstance(Type type, params object[] parameters);
    }
}
