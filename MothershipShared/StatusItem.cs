using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MothershipShared
{
    [Serializable()]
    public class StatusItem
    {
        public string Section = "";
        public string Name = "";
        public string Value = "";

        public StatusItem() { }
        public StatusItem(string Section, string Name, string Value)
        {
            this.Section = Section;
            this.Name = Name;
            this.Value = Value;
        }
    }
}
