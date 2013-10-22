using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using FooTools;

namespace MothershipShared
{
    public class PluginController : MarshalByRefObject
    {
        private IPlugin plugin = null;
        private Thread MainThread = null;

        public void SetPlugin(string DllFilename, string ClassName)
        {
            Assembly asm = Assembly.LoadFrom(DllFilename);
            this.plugin = (IPlugin)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(asm.FullName, ClassName);
        }

        private void SafeStart()
        {
            try
            {
                plugin.Start();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void Start()
        {
            if (MainThread != null)
                throw new Exception("Plugin already started");

            MainThread = new Thread(new ThreadStart(SafeStart));
            MainThread.Start();
        }

        public StatusItem[] GetStatus()
        {
            return plugin.GetStatus();
        }

        public virtual void Stop()
        {
            try
            {
                plugin.Stop();
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }

            MainThread = null;
        }
    }
}
