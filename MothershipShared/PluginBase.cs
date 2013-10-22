using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FooTools;

namespace MothershipShared
{
    public abstract class PluginBase : IPlugin
    {
        public enum RunStatusType
        {
            STOPPED,
            STARTING,
            STARTED,
            STOPPING
        }

        private RunStatusType _RunStatus = RunStatusType.STOPPED;
        private int LoopIntervalMSec = 1 * 1000;
        private bool IsSleeping = false;

        private void SafeSleep()
        {
            IsSleeping = true;
            for (int i = 0; i < LoopIntervalMSec; i += 500)
            {
                if (_RunStatus != RunStatusType.STARTED)
                    break;
                else
                    Thread.Sleep(500);
            }
            IsSleeping = false;
        }

        public void Start()
        {
            _RunStatus = RunStatusType.STARTING;
            BeforeStart();
            _RunStatus = RunStatusType.STARTED;
            try
            {
                while (_RunStatus == RunStatusType.STARTED)
                {
                    OnLoop();
                    SafeSleep();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            _RunStatus = RunStatusType.STOPPED;
        }

        public void Stop()
        {
            // if app is not in running state assume it's stopping
            if (_RunStatus == RunStatusType.STARTED)
            {
                _RunStatus = RunStatusType.STOPPING;
                while (_RunStatus != RunStatusType.STOPPED)
                {
                    System.Threading.Thread.Sleep(500);
                }
                BeforeShutdown();
            }
        }

        public StatusItem[] GetStatus()
        {
            List<StatusItem> list = new List<StatusItem>(OnGetStatus());
            list.Add(new StatusItem("Info", "RunningStatus", IsSleeping ? "SLEEPING" : "RUNNING"));
            return list.ToArray();
        }

        public abstract void BeforeStart();

        public abstract void OnLoop();

        public abstract void BeforeShutdown();

        public abstract StatusItem[] OnGetStatus();


    }
}
