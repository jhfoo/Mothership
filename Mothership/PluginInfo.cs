﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MothershipShared;

namespace MothershipLib
{
    public class PluginInfo
    {
        public PluginController controller = null;
        public AppDomain domain = null;
        public PluginManifest manifest = null;
        public string path = "";

        public PluginInfo() { }

        public PluginInfo(PluginController controller, AppDomain domain, PluginManifest manifest, string path)
        {
            this.controller = controller;
            this.domain = domain;
            this.manifest = manifest;
            this.path = path;
        }
    }
}
