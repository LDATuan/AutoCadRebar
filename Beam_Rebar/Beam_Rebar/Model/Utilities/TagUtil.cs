using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class TagUtil
    {
        public static void UpdateTagRebar(this Transaction tx, List<BlockReference> bls, Polyline pl, Rebar rebar)
        {
            var vecX = new Vector3d(1, 0, 0);
            var vecY = new Vector3d(0, 1, 0);
            foreach (var bl in bls)
            {
                var obj = tx.GetObject(pl.ObjectId, OpenMode.ForRead);

                var rsb = obj.GetXDataForApplication("LDAT_XData");
                if (rsb != null)
                {

                    var arr = rsb.AsArray();
                    rebar.RebarNumber = arr[1].Value.ToString();
                    rebar.BarDiameter = int.Parse(arr[2].Value.ToString());
                    rebar.Count = arr[3].Value.ToString();
                    rebar.NameElement = arr[4].Value.ToString();
                    rebar.Spacing = arr[5].Value.ToString();
                    rebar.Comment = arr[6].Value.ToString();
                    rebar.Length = pl.Length;

                }
                foreach (ObjectId objectId in bl.AttributeCollection)
                {
                    var ob1 = tx.GetObject(objectId, OpenMode.ForWrite);
                    if (ob1 is AttributeReference att)
                    {
                        if (att.Tag == "RebarNumber")
                        {
                            if (bl.Name == "BLOCKTAG_T1")
                            {
                                att.TextString = $"[{rebar.RebarNumber}]";
                            }
                            else if (bl.Name == "BLOCKTAG_T")
                            {
                                att.TextString = $"{rebar.RebarNumber}";
                                //if (rebar.RebarNumber.Length > 1)
                                //{
                                //    att.Position = position + 0.6 * scale * vecX - (2.2 * scale / 2) * vecY;
                                //}
                                //else
                                //{
                                //    att.Position = position + 1.4 * scale * vecX - (2.2 * scale / 2) * vecY;
                                //}
                            }
                        }
                        else if (att.Tag == "KIHIEUTHEP")
                        {

                            if (bl.Name == "BLOCKTAG_T1")
                            {
                                att.TextString = $"{rebar.Count}T{rebar.BarDiameter}@{rebar.Spacing}/L={Math.Round(rebar.Length)}";
                            }
                            else if (bl.Name == "BLOCKTAG_T")
                            {
                                att.TextString = $"{rebar.Count}T{rebar.BarDiameter}";
                            }
                        }
                    }
                }


            }

        }
        public static void UpDateTagRebarV2(this Transaction tx, List<Rebar> rebars, List<BlockReference> bls)
        {
            for (int i = 0; i < rebars.Count; i++)
            {
                var rebar = rebars[i];
                foreach (var bl in bls)
                {
                    var rsb = bl.GetXDataForApplication("BLOCKTAG_T1_XData");

                    if (rsb != null && rsb.AsArray()[1].Value.ToString() == rebar.ObjectIdRebar.ToString())
                    {
                        foreach (ObjectId objectId in bl.AttributeCollection)
                        {
                            var ob1 = tx.GetObject(objectId, OpenMode.ForWrite);
                            if (ob1 is AttributeReference att)
                            {
                                if (att.Tag == "KIHIEUTHEP")
                                {
                                    if (bl.Name == "BLOCKTAG_T1")
                                    {
                                        att.TextString = $"{rebar.Count}T{rebar.BarDiameter}@{rebar.Spacing}/L={Math.Round(rebar.Length)}";
                                    }

                                }
                            }
                        }
                    }

                }
            }

        }
    }
}
