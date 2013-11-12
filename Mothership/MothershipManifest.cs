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
        public List<string> ServiceUrls = new List<string>();
        public List<string> PluginPaths = new List<string>();

        public MothershipManifest(string filename)
        {
            if (!File.Exists(filename))
                throw new Exception("File not found: " + Path.GetFullPath(filename));

            XmlDocument xml = new XmlDocument();
            xml.Load(filename);

            foreach (XmlNode node in xml.SelectNodes("/MothershipManifest/ServiceUrls/ServiceUrl"))
            {
                Log.Debug("Service url: " + node.InnerText);
                ServiceUrls.Add(node.InnerText);
            }

            foreach (XmlNode node in xml.SelectNodes("/MothershipManifest/PluginPaths/path"))
            {
                PluginPaths.Add(node.InnerText);
            }
        }

        public Uri[] GetServiceUrlsAsUri()
        {
            //List<Uri> list = new List<Uri>();
            //foreach (string url in ServiceUrls)
            //{
            //    list.Add(new Uri(url));
            //}
            //return list.ToArray();
            return ServiceUrls.Select(item => new Uri(item)).ToArray();
        }
    }
}
