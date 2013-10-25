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
        private string DllFilename = "";
        private string ClassName = "";

        public void SetPlugin(string DllFilename, string ClassName)
        {
            this.DllFilename = DllFilename;
            this.ClassName = ClassName;

            InstantiatePlugin();
        }

        private void InstantiatePlugin()
        {
            Assembly asm = Assembly.LoadFrom(DllFilename);
            plugin = (IPlugin)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(asm.FullName, ClassName);
        }

        public bool IsRunning
        {
            get
            {
                return MainThread != null && MainThread.IsAlive;
            }
        }

        private void SafeStart()
        {
            try
            {
                Log.Normal("Starting plugin...");
                plugin.Start();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            MainThread = null;
//            plugin = null;
            Log.Normal("Plugin has stopped");
        }

        public void Start()
        {
            Log.Normal("Attempting to start plugin ");

            if (MainThread != null)
                throw new Exception("Plugin already started");

            if (plugin == null)
                InstantiatePlugin();

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
