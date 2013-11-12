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
            // do not depend on working directory. always reference from the assembly location 
            Log.Debug("Loading: " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + filename);
            MotherManifest = new MothershipManifest(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + filename);
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
            string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (string path in MotherManifest.PluginPaths)
            {
                // validity check again
                string NormalisedPath = path.StartsWith(".") ? Path.GetFullPath(AssemblyPath + "/" + path) : path;
                Log.Debug("Looking for folder: " + NormalisedPath);
                if (!Directory.Exists(NormalisedPath))
                {
                    Log.Warning("Plugin directory '" + NormalisedPath + "' does not exist");
                    continue;
                }

                // look for subfolders containing manifest.xml
                foreach (string subfolder in Directory.GetDirectories(NormalisedPath))
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

        private static void StartPlugin(string PluginId)
        {
            if (!plugins.ContainsKey(PluginId))
                throw new Exception("Plugin '" + PluginId + "' does not exist");

            if (plugins[PluginId].domain != null)
                throw new Exception("Plugin '" + PluginId + "' is already started");

            PluginInfo info = plugins[PluginId];

            // load up the plugin manifest
            // this overrides the existing manifest created during registration
            info.manifest = new PluginManifest(info.path + "/manifest.xml");

            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = info.path;

            // copy Mothership's version of MothershipShared to plugin
            File.Copy(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/MothershipShared.dll",
                info.path + "/MothershipShared.dll", true);

            // remove MothershipShared.pdb file in the plugin folder
            if (File.Exists(info.path + "/MothershipShared.pdb"))
                File.Delete(info.path + "/MothershipShared.pdb");

            // create an AppDomain and load the plugin in there
            Log.Normal(string.Format("Starting plugin '{0}' (id:{1})", info.manifest.Name, info.manifest.Id));
            info.domain = AppDomain.CreateDomain(info.manifest.Id, null, setup);
            info.controller = (PluginController)info.domain.CreateInstanceFromAndUnwrap(
                info.path + "/MothershipShared.dll", "MothershipShared.PluginController");

            // start the plugin!
            string DllFilename = setup.ApplicationBase + "/" + info.manifest.MainLibraryName;
            info.controller.SetPlugin(DllFilename, info.manifest.MainClassName);
            info.controller.Start();

        }

        private static void RegisterPlugin(string PluginFolder)
        {
            try
            {
                // load up the plugin manifest
                Log.Debug("Registering plugin at: " + PluginFolder); 
                PluginManifest manifest = new PluginManifest(PluginFolder + "/manifest.xml");

                // make sure the plugin id is unique
                if (plugins.ContainsKey(manifest.Id))
                {
                    Log.Warning("Plugin id '" + manifest.Id + "' already exists. Plugin at "
                        + PluginFolder + "' will not be loaded.");
                    return;
                }

                plugins[manifest.Id] = new PluginInfo();
                plugins[manifest.Id].path = PluginFolder;
                plugins[manifest.Id].manifest = manifest;

                if (manifest.IsAutoStart)
                    StartPlugin(manifest.Id);
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
                    RegisterPlugin(path);
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
