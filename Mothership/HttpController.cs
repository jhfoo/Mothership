using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;

namespace MothershipLib
{
    class HttpController : NancyModule
    {
        public HttpController()
        {
            Get["/"] = x =>
            {
                return "Welcome to Mothership!";
            };
        }
    }
}
