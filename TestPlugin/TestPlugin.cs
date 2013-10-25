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
            Log.Debug("Initialising TestPlugin");
            count = 3;
        }

        public override void OnLoop()
        {
            //count--;
            if (count > 0)
            {
                Log.Debug("I'm started!");
                Log.Debug(AppDomain.CurrentDomain.FriendlyName);
                System.Threading.Thread.Sleep(5 * 1000);
            }
            else
                _RunStatus = RunStatusType.STOPPING;
                //throw new Exception("There's an error and I gotta bail!");
        }

        public override StatusItem[] OnGetStatus()
        {
            List<StatusItem> list = new List<StatusItem>();
            list.Add(new StatusItem("Info", "RunningTime", "10:20:30"));
            list.Add(new StatusItem("Info", "Current Activity", "Sleeping"));
            return list.ToArray();
        }
    }
}
