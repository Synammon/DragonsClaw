using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace DragonsClaw.Models
{
    public class EmpireViewModel
    {
        public int EmpireId { get; set; }
        public string EmpireName { get; set; }
        public string EmpireDescription { get; set; }
        public string Decree { get; set; }
        public string Host { get; set; }
        public int ViceHost1 { get; set; }
        public int ViceHost2 { get; set; }
        public Point Unimatrix { get; set; }

        public List<EmpirePlayerViewModel> Players { get; set; }

        public EmpireViewModel()
        {
            Players = new List<EmpirePlayerViewModel>();
        }
    }
}