using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using FooTools;

namespace MothershipShared
{
    public class PluginManifest
    {
        public string MainClassName = "";
        public string MainLibraryName = "";
        public string Name = "";
        public string Id = "";
        public bool IsAutoStart = true;

        public PluginManifest(string filename)
        {
            if (!File.Exists(filename))
                throw new Exception("File not found: " + filename);

            XmlDocument xml = new XmlDocument();
            xml.Load(filename);

            Id = GetNodeText(xml, "Id");
            Name = GetNodeText(xml, "Name");
            MainClassName = GetNodeText(xml, "MainClassName");
            MainLibraryName = GetNodeText(xml, "MainLibraryName");
            IsAutoStart = GetNodeText(xml, "IsAutoStart").ToLower().StartsWith("y");
        }

        private string GetNodeText(XmlNode root, string NodeName)
        {
            if (root.SelectSingleNode("/PluginManifest/" + NodeName) == null)
                throw new Exception("Missing XML tag name: " + NodeName);

            return root.SelectSingleNode("/PluginManifest/" + NodeName).InnerText;
        }
    }
}
