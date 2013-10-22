using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MothershipShared;
using FooTools;

namespace TestPlugin
{
    public class TestPlugin : PluginBase
    {
        private int count = 3;
        public override void BeforeShutdown()
        {
            Log.Debug("Taking my time to stop...");
            System.Threading.Thread.Sleep(30 * 1000);
            Log.Debug("I'm stopped!");
        }

        public override void BeforeStart()
        {
        }
        
        public override void OnLoop()
        {
            count--;
            if (count > 0)
            {
                Log.Debug("I'm started!");
                Log.Debug(AppDomain.CurrentDomain.FriendlyName);
            }

            else
                throw new Exception("There's an error and I gotta bail!");
        }
    }
}
