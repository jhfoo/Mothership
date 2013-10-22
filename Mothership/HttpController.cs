using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using MothershipShared;

namespace MothershipLib
{
    public class HttpController : NancyModule
    {
        public HttpController()
        {
            Get["/"] = x => Mothership.Plugins.Length.ToString() + " plugins registered";
            Get["/test"] = x =>
            {
                StatusItem item = new StatusItem();
                item.Name = "Hello!";
                return View["TestRazor.cshtml", item];
            };
            Get["/mothership"] = x =>
            {
                List<PluginDisplayInfo> list = new List<PluginDisplayInfo>();
                foreach (PluginInfo info in Mothership.Plugins)
                {
                    PluginDisplayInfo DisplayInfo = new PluginDisplayInfo();
                    DisplayInfo.Name = info.manifest.Name;
                    DisplayInfo.Id = info.manifest.Id;
                    DisplayInfo.RunningStatus = Mothership.Plugins[0].controller.GetStatus();
                    list.Add(DisplayInfo);
                }
                return Response.AsJson(list);
            };
        }
    }
}
