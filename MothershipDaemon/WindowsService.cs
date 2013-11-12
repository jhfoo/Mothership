using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Reflection;
using System.IO;
using FooTools;
using MothershipLib;

namespace WindowsService
{
    class WindowsService : ServiceBase
    {
        private bool IsStopping = false;


        public WindowsService()
        {
            this.ServiceName = "My Windows Service";
            this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            this.CanHandlePowerEvent = false;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = false;
            this.CanStop = true;
        }

        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main()
        {
            try
            {
                string LocalPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string LogPath = Directory.Exists(LocalPath + "/../../logs")
                    ? Path.GetFullPath(LocalPath + "/../../logs")
                    : LocalPath;

                Log.SetLogInstance(new LogFile(new LogFileConfig()
                    .SetFilename("MothershipDaemon")
                    .SetFileExtension("log")
                    .SetBasePath(LogPath)));
                Log.Debug(LogPath);
                Log.Normal("Service started");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            ServiceBase.Run(new WindowsService());
        }

        /// <summary>
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether
        ///    or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void ThreadRun()
        {
            IsStopping = false;
            while (!IsStopping)
            {
                Log.Debug("Running some tasks");
                System.Threading.Thread.Sleep(5 * 1000);
            }
        }

        /// <summary>
        /// OnStart(): Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                Log.Normal("OnStart() event received");

                base.OnStart(args);
                Mothership.LoadManifest("manifest.xml");
                Mothership.Start();

                //Thread ServiceThread = new Thread(new ThreadStart(ThreadRun));
                //ServiceThread.Start();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// OnStop(): Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            Log.Normal("OnStop() event received");

            base.OnStop();
            IsStopping = true;

            Mothership.Stop();
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            Log.Normal("OnPause() event received");

            base.OnPause();
        }

        /// <summary>
        /// OnContinue(): Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            Log.Normal("OnContinue() event received");

            base.OnContinue();
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();
        }


    }
}