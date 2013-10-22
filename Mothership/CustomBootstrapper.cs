using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Nancy;
using Nancy.Conventions;

namespace MothershipLib
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        //protected override IRootPathProvider RootPathProvider
        //{
        //    get
        //    {
        //        return new CustomRootPathProvider();
        //    }
        //}

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("www", "www"));
            base.ConfigureConventions(nancyConventions);
        }
    }
}
