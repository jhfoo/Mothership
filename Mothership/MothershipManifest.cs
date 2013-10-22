using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using FooTools;

namespace MothershipLib
{
    class MothershipManifest
    {
        public MothershipManifest(string filename)
        {
            if (!File.Exists("manifest.xml"))
                throw new Exception("File not found: " + Path.GetFullPath("manifest.xml"));

            XmlDocument xml = new XmlDocument();
            xml.Load(filename);

            foreach (XmlNode node in xml.SelectNodes("/MothershipManifest/PluginPaths/path"))
            {
                if (!Directory.Exists(node.InnerText))
                    Log.Normal("Plugin directory '" + node.InnerText + "' does not exist");
                Log.Debug(node.InnerText);
            }
        }
    }
}
