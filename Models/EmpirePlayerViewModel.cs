using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DragonsClaw.Models
{
    public enum Role { Host, ViceHost, Member }

    public class EmpirePlayerViewModel
    {
        public string Ruler { get; set; }
        public string Sector { get; set; }
        public Role Role { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public int Planets { get; set; }
        public int Networth { get; set; }
    }
}