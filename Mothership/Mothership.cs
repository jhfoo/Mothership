using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Timers;
using FooTools;
using MothershipShared;
using Nancy.Hosting.Self;

namespace MothershipLib
{
    public class Mothership
    {
        private static List<PluginInfo> plugins = new List<PluginInfo>();
        private static int PluginStopTimeout = 3 * 1000;
        private static NancyHost HttpServer = null;
        private static MothershipManifest MotherManifest = null;

        public static int LoadManifest(string filename)
        {
            MotherManifest = new MothershipManifest(filename);
            return 0;
        }

        public static PluginInfo[] Plugins
        {
            get
            {
                return plugins.ToArray();
            }
        }

        public static void Start()
        {
            try
            {
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/plugins/TestPlugin";

                // copy Mothership's version of MothershipShared to plugin
                File.Copy(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/MothershipShared.dll",
                    setup.ApplicationBase + "/MothershipShared.dll", true);

                // load up the plugin manifest
                PluginManifest manifest = new PluginManifest(setup.ApplicationBase + "/manifest.xml");

                // create an AppDomain for the plugin and load it in there
                Log.Normal(string.Format("Starting plugin '{0}' (id:{1})",manifest.Name, manifest.Id));
                AppDomain NewDomain = AppDomain.CreateDomain("NewDomain", null, setup);
                PluginController controller = (PluginController)NewDomain.CreateInstanceFromAndUnwrap(setup.ApplicationBase + "/MothershipShared.dll", "MothershipShared.PluginController");

                // launch the http server
                HttpServer = new Nancy.Hosting.Self.NancyHost(MotherManifest.GetServiceUrlsAsUri());
                HttpServer.Start();

                // start the plugin!
                string DllFilename = setup.ApplicationBase + "/" + manifest.MainLibraryName;
                controller.SetPlugin(DllFilename, manifest.MainClassName);
                controller.Start();

                plugins.Add(new PluginInfo(controller, NewDomain, manifest));
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
