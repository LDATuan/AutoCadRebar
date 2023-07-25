using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View;

namespace Utilities
{
    public static class LineUtil
    {
        public static void AddLineToDrawing(this Transaction tx, BlockTableRecord blockTableRecord, List<Line> lines, ObjectId objectId)
        {
            foreach (var line in lines)
            {
                line.SetLayerId(objectId, false);
                blockTableRecord.AppendEntity(line);
                tx.AddNewlyCreatedDBObject(line, true);
            }
        }
        public static void CreateStirrup(this Transaction tx, BlockTableRecord blockTableRecord, Point3d p, double width, double height, double cover, ObjectId objectIdLayer)
        {
            var vecX = new Vector2d(1, 0);
            var vecY = new Vector2d(0, 1);
            var angleRadius = 20;

            var p1 = new Point2d(p.X - width / 2 + cover + angleRadius, p.Y - cover);
            var p2 = new Point2d(p.X - width / 2 + cover, p.Y - cover - angleRadius);
            var p3 = new Point2d(p.X - width / 2 + cover, p.Y - height + (cover + angleRadius));
            var p4 = new Point2d(p.X - width / 2 + cover + angleRadius, p.Y - height + cover);

            var w1 = width - 2 * (cover + angleRadius);
            var w2 = width - 2 * cover;
            var p11 = p1 + w1 * vecX;
            var p22 = p2 + w2 * vecX;
            var p33 = p3 + w2 * vecX;
            var p44 = p4 + w1 * vecX;

            var pl = new Polyline();
            pl.AddVertexAt(0, p1, Math.Tan(Math.PI / 8), 0, 0);
            pl.AddVertexAt(1, p2, 0, 0, 0);
            pl.AddVertexAt(2, p3, Math.Tan(Math.PI / 8), 0, 0);
            pl.AddVertexAt(3, p4, 0, 0, 0);

            pl.AddVertexAt(4, p44, Math.Tan(Math.PI / 8), 0, 0);
            pl.AddVertexAt(5, p33, 0, 0, 0);
            pl.AddVertexAt(6, p22, Math.Tan(Math.PI / 8), 0, 0);
            pl.AddVertexAt(7, p11, 0, 0, 0);
            pl.AddVertexAt(8, p1, 0, 0, 0);

            pl.SetLayerId(objectIdLayer, false);
            blockTableRecord.AppendEntity(pl);
            tx.AddNewlyCreatedDBObject(pl, true);
        }
        public static void CreateRebar(this Transaction tx, Database db, Polyline pl, Rebar rebar, ObjectId LayerID, string nameData)
        {
            pl.SetLayerId(LayerID, false);

            tx.AddReAppTableRecord(db, nameData);
            ResultBuffer resultBuffer = new ResultBuffer(new TypedValue(1001, nameData),
                                                         new TypedValue(1000, rebar.RebarNumber),
                                                         new TypedValue(1070, rebar.BarDiameter),
                                                         new TypedValue(1000, rebar.Count),
                                                         new TypedValue(1000, rebar.NameElement),
                                                         new TypedValue(1000, rebar.Spacing),
                                                         new TypedValue(1000, rebar.Comment));

            pl.XData = resultBuffer;
        }
        public static void UpdateData(this Transaction tx, Polyline pl, RebarView rebarView)
        {

            var obj = tx.GetObject(pl.ObjectId, OpenMode.ForWrite);
            ModelData.Polyline = pl;
            var rsb = obj.GetXDataForApplication("LDAT_XData");
            TypedValue[] data = rsb.AsArray();

            data[1] = new TypedValue(1000, rebarView.RebarNumber);
            data[2] = new TypedValue(1000, rebarView.SelectedBarDiameter);
            data[3] = new TypedValue(1000, rebarView.Count);
            data[4] = new TypedValue(1000, rebarView.ElementName);
            data[5] = new TypedValue(1000, rebarView.Spacing);
            data[6] = new TypedValue(1000, rebarView.Comment);
            rsb = new ResultBuffer(data);
            pl.XData = rsb;

        }
        public static void CreateLeader(this Transaction tx, BlockTableRecord blockTableRecord, Point3d p1, Point3d p2, Point3d p3, ObjectId objectIdDimSty)
        {

            Leader leader = new Leader();
            leader.SetDatabaseDefaults();

            leader.AppendVertex(p1);
            leader.AppendVertex(p2);
            leader.AppendVertex(p3);
            leader.HasArrowHead = true;
            leader.DimensionStyle = objectIdDimSty;
           


            blockTableRecord.AppendEntity(leader);
            tx.AddNewlyCreatedDBObject(leader, true);

        }

    }
}
