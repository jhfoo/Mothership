using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
