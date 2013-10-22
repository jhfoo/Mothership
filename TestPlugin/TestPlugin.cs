using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MothershipShared;
using FooTools;

namespace TestPlugin
{
    public class TestPlugin : IPlugin
    {
        public void Start()
        {
            Log.Debug("I'm started!");
            Log.Debug(AppDomain.CurrentDomain.FriendlyName);
        }

        public void Stop()
        {
            Log.Debug("Taking my time to stop...");
            System.Threading.Thread.Sleep(30 * 1000);
            Log.Debug("I'm stopped!");
        }
    }
}
