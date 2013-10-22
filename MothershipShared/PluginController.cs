using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using FooTools;

namespace MothershipShared
{
    public class PluginController : MarshalByRefObject
    {
        private IPlugin plugin = null;

        public void SetPlugin(string DllFilename, string ClassName)
        {
            Assembly asm = Assembly.LoadFrom(DllFilename);
            this.plugin = (IPlugin)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(asm.FullName, ClassName);
        }

        public void Start() {
            try
            {
                plugin.Start();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public virtual void Stop() {
            try
            {
                plugin.Stop();
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }
        }
    }
}
