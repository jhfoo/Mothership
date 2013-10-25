using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using MothershipShared;
using FooTools;

namespace MothershipLib
{
    public class HttpController : NancyModule
    {
        public HttpController()
        {
            Get["/"] = x =>
            {
                ViewBag.Text = "Hello!";
                ViewBag.PluginDisplayInfoList = GetPluginDisplayInfo();
                StatusItem item = new StatusItem();
                item.Name = "Object works!";
                ViewBag.TestObj = item;
                return View["index.cshtml"];
            };
            Get["/test"] = x =>
            {
                StatusItem item = new StatusItem();
                item.Name = "Hello!";
                return View["TestRazor.cshtml", item];
            };
            Get["/cmd/StartPlugin/{PluginId}"] = x =>
            {
                try
                {
                    PluginInfo info = Mothership.GetPluginById(x.PluginId);

                    if (info == null)
                        return Response.AsJson(new SimpleResponse(SimpleResponse.ResultType.ERROR, "Invalid plugin id"));

                    if (info.controller.IsRunning)
                    return Response.AsJson(new SimpleResponse(SimpleResponse.ResultType.ERROR, "Plugin already started"));

                    info.controller.Start();
                    return Response.AsJson(new SimpleResponse(SimpleResponse.ResultType.OK));
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return new SimpleResponse(SimpleResponse.ResultType.ERROR, e.Message);
                }
            };
            Get["/cmd/StopPlugin/{PluginId}"] = x =>
            {
                try
                {
                    PluginInfo info = Mothership.GetPluginById(x.PluginId);

                    if (info == null)
                        return Response.AsJson(new SimpleResponse(SimpleResponse.ResultType.ERROR, "Invalid plugin id"));

                    if (!info.controller.IsRunning)
                        return Response.AsJson(new SimpleResponse(SimpleResponse.ResultType.ERROR, "Plugin already stopped"));

                    Mothership.StopPluginWithTimeout(info);
                    return Response.AsJson(new SimpleResponse(SimpleResponse.ResultType.OK));
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return new SimpleResponse(SimpleResponse.ResultType.ERROR, e.Message);
                }
            };
            Get["/mothership"] = x =>
            {
                return Response.AsJson(GetPluginDisplayInfo());
            };
        }

        private PluginDisplayInfo[] GetPluginDisplayInfo()
        {
            List<PluginDisplayInfo> list = new List<PluginDisplayInfo>();
            foreach (PluginInfo info in Mothership.Plugins)
            {
                PluginDisplayInfo DisplayInfo = new PluginDisplayInfo();
                DisplayInfo.Name = info.manifest.Name;
                DisplayInfo.Id = info.manifest.Id;
                DisplayInfo.IsStarted = info.controller.IsRunning;
                if (DisplayInfo.IsStarted)
                    DisplayInfo.RunningStatus = info.controller.GetStatus();
                list.Add(DisplayInfo);
            }

            return list.ToArray();
        }
    }
}
