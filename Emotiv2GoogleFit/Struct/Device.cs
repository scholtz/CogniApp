using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emotiv2GoogleFit.Struct
{
    public class Device
    {
        public string Manufacturer { get; set; } = "Emotiv";
        public string Model { get; set; }
        public string Type { get; set; } = "scale";
        public string Uid { get; set; }
        public string Version { get; set; } = "1.0";
    }
}