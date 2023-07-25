using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class PointUtil
    {
        public static Point3d GetPointFromUser(this Document doc,string status)
        {
            return doc.Editor.GetPoint(new PromptPointOptions(status)).Value;
        }
        public static void CreateBreakLine(this Transaction tx, BlockTableRecord blockTableRecord, ObjectId objectId, Point3d startPnt, double height_slab)
        {
            var h_break = 40.0;
            var w_break = 80.0;
            var offset = 50;
            var vecY = new Vector2d(0, 1);
            var startPnt2D = new Point2d(startPnt.X, startPnt.Y);
            var midpoint = new Point3d(startPnt.X, startPnt.Y - height_slab / 2, startPnt.Z);
            var p1 = startPnt2D + offset * vecY;
            var p2 = new Point2d(midpoint.X, midpoint.Y + h_break / 2);
            var p3 = new Point2d(midpoint.X - w_break / 2, midpoint.Y + h_break / 4);
            var p4 = new Point2d(midpoint.X + w_break / 2, midpoint.Y - h_break / 4); ;
            var p5 = new Point2d(midpoint.X, midpoint.Y - h_break / 2); ;
            var p6 = startPnt2D - (height_slab + offset) * vecY;

            var pl = new Polyline();
            pl.AddVertexAt(0, p1, 0, 0, 0);
            pl.AddVertexAt(1, p2, 0, 0, 0);
            pl.AddVertexAt(2, p3, 0, 0, 0);
            pl.AddVertexAt(3, p4, 0, 0, 0);
            pl.AddVertexAt(4, p5, 0, 0, 0);
            pl.AddVertexAt(5, p6, 0, 0, 0);


            pl.SetLayerId(objectId, false);
            blockTableRecord.AppendEntity(pl);
            tx.AddNewlyCreatedDBObject(pl, true);


        }
        public static ObjectId ObjectIdCircleNode(this Transaction tx, BlockTable blockTable, ObjectId objectIdLayer)
        {
            ObjectId objectId = ObjectId.Null;

            ObjectIdCollection obIdCol = new ObjectIdCollection();
            if (!blockTable.Has("LDAT_CircleNode"))
            {
                using (BlockTableRecord bTR = new BlockTableRecord())
                {

                    bTR.Name = "LDAT_CircleNode";
                    bTR.Origin = new Point3d(0, 0, 0);
                    var c = new Circle();
                    c.SetDatabaseDefaults();
                    c.Radius = 15;
                    c.Center = new Point3d(0, 0, 0);
                    c.SetLayerId(objectIdLayer, false);
                    blockTable.UpgradeOpen();
                    blockTable.Add(bTR);

                    bTR.AppendEntity(c);
                    tx.AddNewlyCreatedDBObject(c, true);
                    obIdCol.Add(c.ObjectId);

                    var hatch = new Hatch();
                    bTR.AppendEntity(hatch);
                    tx.AddNewlyCreatedDBObject(hatch, true);

                    hatch.SetDatabaseDefaults();
                    hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                    hatch.Color = Color.FromRgb(255, 0, 0);
                    hatch.Associative = true;

                    hatch.AppendLoop(HatchLoopTypes.Outermost, obIdCol);

                    hatch.EvaluateHatch(true);



                    objectId = bTR.Id;
                }

            }
            else
            {
                objectId = blockTable["LDAT_CircleNode"];
            }

            return objectId;


        }
        public static void CreateCircleBlockRef(this Transaction tx, BlockTableRecord blockTableRecord, List<Point3d> point3Ds, ObjectId objectId)
        {
            foreach (var position in point3Ds)
            {
                if (objectId != ObjectId.Null)
                {
                    var blockReference = new BlockReference(position, objectId);

                    blockTableRecord.AppendEntity(blockReference);
                    tx.AddNewlyCreatedDBObject(blockReference, true);
                }
            }

        }
        public static List<Point3d> GetPoint(this Point3d p, int count, PostionRebar postionRebar, double width, double height, double cover)
        {
            var ps = new List<Point3d>();
            var angleRadius = 20;
            var pnt_X = p.X - width / 2 + cover + angleRadius;
            var pnt_Y = 0.0;
            switch (postionRebar)
            {
                case PostionRebar.Top:
                    pnt_Y = p.Y - cover - angleRadius;
                    break;
                case PostionRebar.Bottom:
                    pnt_Y = p.Y - height + cover + angleRadius;
                    break;
                case PostionRebar.Middle:
                    break;
                default:
                    break;
            }
            var lenght = width - 2 * (cover + angleRadius);
            var v = lenght / (count - 1);
            for (int i = 0; i < count; i++)
            {
                ps.Add(new Point3d(pnt_X + i * v, pnt_Y, p.Z));
            }

            return ps;
        }
        public static Point2d ConvertP3ToP2(this Point3d point3D)
        {
            return new Point2d(point3D.X, point3D.Y);
        }
    }
    public enum PostionRebar
    {
        Top, Bottom, Middle
    }

}
