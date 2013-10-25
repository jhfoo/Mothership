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
        private static Dictionary<string, PluginInfo> plugins = new Dictionary<string, PluginInfo>();
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
                return plugins.Values.ToArray();
            }
        }

        public static PluginInfo GetPluginById(string id)
        {
            foreach (PluginInfo info in plugins.Values)
            {
                if (info.manifest.Id == id)
                    return info;
            }
            return null;
        }

        private static string[] GetPluginFolders()
        {
            List<string> list = new List<string>();
            foreach (string path in MotherManifest.PluginPaths)
            {
                // validity check again
                if (!Directory.Exists(path))
                    continue;

                // look for subfolders containing manifest.xml
                foreach (string subfolder in Directory.GetDirectories(path))
                {
                    if (File.Exists(subfolder + "/manifest.xml"))
                    {
                        list.Add(Path.GetFullPath(subfolder));
                        Log.Debug("Subfolder: " + list[list.Count - 1]);
                    }
                }
            }

            return list.ToArray();
        }

        private static void CreatePlugin(string PluginFolder)
        {
            try
            {
                // load up the plugin manifest
                PluginManifest manifest = new PluginManifest(PluginFolder + "/manifest.xml");

                // make sure the plugin id is unique
                if (plugins.ContainsKey(manifest.Id))
                {
                    Log.Warning("Plugin id '" + manifest.Id + "' already exists. Plugin at "
                        + PluginFolder + "' will not be loaded.");
                    return;
                }

                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = PluginFolder;

                // copy Mothership's version of MothershipShared to plugin
                File.Copy(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/MothershipShared.dll",
                    PluginFolder + "/MothershipShared.dll", true);


                // create an AppDomain for the plugin and load it in there
                Log.Normal(string.Format("Starting plugin '{0}' (id:{1})", manifest.Name, manifest.Id));
                AppDomain NewDomain = AppDomain.CreateDomain(manifest.Id, null, setup);
                PluginController controller = (PluginController)NewDomain.CreateInstanceFromAndUnwrap(
                    PluginFolder + "/MothershipShared.dll", "MothershipShared.PluginController");

                // start the plugin!
                string DllFilename = setup.ApplicationBase + "/" + manifest.MainLibraryName;
                controller.SetPlugin(DllFilename, manifest.MainClassName);
                controller.Start();

                plugins[manifest.Id] = new PluginInfo(controller, NewDomain, manifest);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void Start()
        {
            try
            {
                foreach (string path in GetPluginFolders())
                {
                    CreatePlugin(path);
                }

                // launch the http server
                HttpServer = new Nancy.Hosting.Self.NancyHost(MotherManifest.GetServiceUrlsAsUri());
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

        public static bool StopPluginWithTimeout(PluginInfo info)
        {
            // guarantee that the plugin will stop even if it behaves badly
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
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex);
                return false;
            }
            finally
            {
                info.domain = null;
            }
        }

        public static void Stop()
        {
            HttpServer.Stop();

            foreach (PluginInfo info in plugins.Values)
            {
                try
                {
                    StopPluginWithTimeout(info);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

        }
    }
}
