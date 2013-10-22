using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MothershipShared;

namespace MothershipLib
{
    [Serializable()]
    class PluginDisplayInfo
    {
        public string Name = "";
        public string Id = "";
        public StatusItem[] RunningStatus = new StatusItem[] { };
    }
}
