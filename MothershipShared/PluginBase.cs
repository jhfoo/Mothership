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

        private void SafeSleep()
        {
            for (int i = 0; i < LoopIntervalMSec; i += 500)
            {
                if (_RunStatus != RunStatusType.STARTED)
                    break;
                else
                    Thread.Sleep(500);
            }
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

        public abstract void BeforeStart();

        public abstract void OnLoop();

        public abstract void BeforeShutdown();
    }
}
