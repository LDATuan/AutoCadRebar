using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ModelData
    {
        public static string PathFile { get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\LDATSettings.json"; } }
        public static Document doc { get; set; }
        public static Transaction tx { get; set; } 
        public static Polyline Polyline { get; set; }
        public static ObjectId ObIdLayerRebar { get; set; }
        public static Rebar Rebar { get; set; }
        public static List<ObjectId> ObjectId_Layer { get; set; }
        public static List<ObjectId> ObjectId_DimStyle { get; set; }
        public static Point3d LastPntTag_T { get; set; }
    }
    public class LDATSetting
    {
        #region Rebar
        public string ElementName { get; set; }
        public string RebarNumber { get; set; }
        public int BarDiameter { get; set; }
        public string Count { get; set; }
        public string Spacing { get; set; }
        public string Comment { get; set; }
        #endregion

        #region BeamSection
        public string Width { get; set; }
        public string Height { get; set; }
        public string ThicknessSlab { get; set; }
        public string Cover { get; set; }
        public int RebarTop { get; set; }
        public int RebarBottom { get; set; }
        #endregion

    }
}
