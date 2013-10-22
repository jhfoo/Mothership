using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using FooTools;
using MothershipLib;

namespace MothershipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Log.SetLogInstance(new LogFile(new LogFileConfig()
            //    .SetBasePath("./logs/")));
            //RegisterUrl();

            Log.Normal("Starting application");
            try
            {
                Mothership.LoadManifest("manifest.xml");
                Mothership.Start();
                Log.Normal("Mothership started");
                Console.ReadKey();
                Log.Normal("Closing application");
                Mothership.Stop();
                Log.Normal("Mothership stopped");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void RegisterUrl()
        {
            string ProcessArgs = string.Format(@"http add urlacl url={0} user={1}", "http://192.168.0.106:8088/", "Everyone");

            ProcessStartInfo psi = new ProcessStartInfo("netsh", ProcessArgs);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;

            Process.Start(psi).WaitForExit();

        }
    }
}
