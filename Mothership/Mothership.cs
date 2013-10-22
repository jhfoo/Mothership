using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Timers;
using FooTools;
using MothershipShared;
using Nancy;
using Nancy.Hosting.Self;

namespace MothershipLib
{
    public class Mothership
    {
        private static List<PluginInfo> plugins = new List<PluginInfo>();
        private static int PluginStopTimeout = 3 * 1000;
        private static NancyHost HttpServer = null;

        public static int LoadManifest(string filename)
        {
            MothershipManifest manifest = new MothershipManifest(filename);

            return 0;
        }


        public static void Start()
        {
            try
            {
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/plugins/TestPlugin";

                PluginManifest manifest = new PluginManifest(setup.ApplicationBase + "/manifest.xml");
                Log.Normal(string.Format("Starting plugin '{0}' (id:{1})",manifest.Name, manifest.Id));

                string DllFilename = setup.ApplicationBase + "/" + manifest.MainLibraryName;
                AppDomain NewDomain = AppDomain.CreateDomain("NewDomain", null, setup);
                PluginController controller = (PluginController)NewDomain.CreateInstanceFromAndUnwrap(setup.ApplicationBase + "/MothershipShared.dll", "MothershipShared.PluginController");

                controller.SetPlugin(DllFilename, manifest.MainClassName);
                controller.Start();

                plugins.Add(new PluginInfo(controller, NewDomain, manifest));

                HttpServer = new NancyHost(new Uri("http://127.0.0.1:8088/"));
                HttpServer.Start();

            }
            catch (Exception e)
            {
                Log.Error("Unable to start plugin");
                Log.Error(e);
            }
        }

        private static void StopTimeout(PluginInfo info)
        {
            Log.Debug("Timeout on domain " + info.domain.FriendlyName);
            AppDomain.Unload(info.domain);
        }

        public static void Stop()
        {
            HttpServer.Stop();

            foreach (PluginInfo info in plugins)
            {
                try
                {
                    // guarantee that the plugin will stop even if behaves badly
                    Timer timer = new Timer();
                    timer.Interval = PluginStopTimeout;
                    timer.Elapsed += delegate
                    {
                        StopTimeout(info);
                    };
                    timer.Start();

                    try
                    {
                        info.controller.Stop();
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex);
                    }
                    info.domain = null;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

        }
    }
}
