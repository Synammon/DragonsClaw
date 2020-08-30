using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DragonsClaw.Models
{
    public class CreatePlayerModel
    {
        public string Name { get; set; }
        public string SectorName { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public bool Observer { get; set; }

        public SelectList RaceList { get; internal set; }
        public SelectList ClassList { get; internal set; }
        public string Email { get; internal set; }
    }
}