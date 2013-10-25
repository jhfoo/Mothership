using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MothershipShared;

namespace MothershipLib
{
    [Serializable()]
    public class PluginDisplayInfo
    {
        public string Name = "";
        public string Id = "";
        public bool IsStarted = false;
        public StatusItem[] RunningStatus = new StatusItem[] { };
    }
}
