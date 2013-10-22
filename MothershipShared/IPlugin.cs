using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MothershipShared
{
    public interface IPlugin
    {
        void Start();
        void Stop();
        StatusItem[] GetStatus();
    }
}
