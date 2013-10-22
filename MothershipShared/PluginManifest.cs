using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace MothershipShared
{
    public class PluginManifest
    {
        public string MainClassName = "";
        public string MainLibraryName = "";
        public string Name = "";
        public string Id = "";

        public PluginManifest(string filename)
        {
            if (!File.Exists("manifest.xml"))
                throw new Exception("File not found: " + Path.GetFullPath("manifest.xml"));

            XmlDocument xml = new XmlDocument();
            xml.Load(filename);

            Id = xml.SelectSingleNode("/PluginManifest/Id").InnerText;
            Name = xml.SelectSingleNode("/PluginManifest/Name").InnerText;
            MainClassName = xml.SelectSingleNode("/PluginManifest/MainClassName").InnerText;
            MainLibraryName = xml.SelectSingleNode("/PluginManifest/MainLibraryName").InnerText;
        }
    }
}
