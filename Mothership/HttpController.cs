using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;

namespace MothershipLib
{
    public class HttpController : NancyModule
    {
        public HttpController()
        {
            Get["/"] = x => Mothership.Plugins.Length.ToString() + " plugins registered";
            Get["/mothership"] = x =>
            {
                return "Welcome to Mothership!";
            };
        }
    }
}
