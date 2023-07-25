using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Model
{
    public class Rebar
    {
        public string RebarNumber { get; set; }
        public int BarDiameter { get; set; }
        public string Count { get; set; }
        public string Spacing { get; set; }
        public double Length { get; set; }
        public ObjectId ObjectIdRebar { get; set; }
        public string NameElement { get; set; }
        public string Comment { get; set; }
    }
}
