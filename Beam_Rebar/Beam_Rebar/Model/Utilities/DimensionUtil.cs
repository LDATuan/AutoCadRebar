using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Utilities
{
    public static class DimensionUtil
    {
        public static ObjectId CreateDimTye(this Transaction tx, Database db, BlockTable blockTable, double scale)
        {
            ObjectId objectId = ObjectId.Null;
            string dimTypeName = "T_DIM" + scale.ToString();
            DimStyleTable dimStyleTable = tx.GetObject(db.DimStyleTableId, OpenMode.ForWrite) as DimStyleTable;
            if (!dimStyleTable.Has(dimTypeName))
            {
                DimStyleTableRecord dimType_T = new DimStyleTableRecord();
                dimType_T.Name = dimTypeName;
                dimStyleTable.Add(dimType_T);
                tx.AddNewlyCreatedDBObject(dimType_T, true);
                //Lines
                dimType_T.Dimclrd = Color.FromRgb(255, 0, 0);
                dimType_T.Dimclre = Color.FromRgb(255, 0, 0);
                dimType_T.Dimdle = 1;
                dimType_T.Dimtofl = true;
                dimType_T.Dimdli = 0.8;
                dimType_T.Dimexe = 1.2;
                dimType_T.Dimexo = 0;
                dimType_T.Dimtmove = 2;

                //Symbols and arrows
                dimType_T.Dimasz = 1.6;
                dimType_T.Dimcen = 0;
                if (!blockTable.Has("_ARCHTICK"))
                {
                    Application.SetSystemVariable("DIMBLK", "_ARCHTICK");
                }
                dimType_T.Dimblk = blockTable["_ARCHTICK"];

                //Text
                dimType_T.Dimclrt = Color.FromRgb(255, 255, 0);
                dimType_T.Dimtxsty = tx.CreateTextType(db);
                dimType_T.Dimtxt = 2.5;
                dimType_T.Dimtix = true;
                dimType_T.Dimtih = false;
                dimType_T.Dimgap = 1;
                dimType_T.Dimtoh = false;
                dimType_T.Dimtad = 1;

                dimType_T.Dimscale = scale;
                //Fit
                dimType_T.Dimdec = 0;
                //Unit

                objectId = dimType_T.ObjectId;

            }
            else
            {
                objectId = dimStyleTable[dimTypeName];
            }
            return objectId;


        }
        public static ObjectId DimStyleNumber(this Transaction tx, Database db, BlockTable blockTable, double scale, Color colorLine, Color colorExtend)
        {
            ObjectId objectId = ObjectId.Null;
            string dimTypeName = "T_DIMNumber" + scale.ToString();
            DimStyleTable dimStyleTable = tx.GetObject(db.DimStyleTableId, OpenMode.ForWrite) as DimStyleTable;
            if (!dimStyleTable.Has(dimTypeName))
            {
                DimStyleTableRecord dimType_T = new DimStyleTableRecord();
                dimType_T.Name = dimTypeName;
                dimStyleTable.Add(dimType_T);
                tx.AddNewlyCreatedDBObject(dimType_T, true);
                //Lines
                dimType_T.Dimclrd = colorLine;
                dimType_T.Dimclre = colorExtend;

                dimType_T.Dimsd1 = true;
                dimType_T.Dimsd2 = true;
                dimType_T.Dimse1 = true;
                dimType_T.Dimse2 = true;

                //Symbols and arrows
                dimType_T.Dimasz = 2.5;
                dimType_T.Dimcen = 0;
                if (!blockTable.Has("_NONE"))
                {
                    Application.SetSystemVariable("DIMBLK", "_NONE");
                }
                dimType_T.Dimblk = blockTable["_NONE"];

                //Text
                dimType_T.Dimclrt = Color.FromRgb(15, 202, 40);
                dimType_T.Dimtxsty = tx.CreateTextType(db);
                dimType_T.Dimtxt = 2.5;
                dimType_T.Dimtix = true;
                dimType_T.Dimtih = false;
                dimType_T.Dimgap = 1;
                dimType_T.Dimtoh = false;
                dimType_T.Dimtad = 1;

                dimType_T.Dimscale = scale;
                //Fit
                dimType_T.Dimdec = 0;
                //Unit

                objectId = dimType_T.ObjectId;

            }
            else
            {
                objectId = dimStyleTable[dimTypeName];
            }
            return objectId;
        }
        public static ObjectId CreateTextType(this Transaction tx, Database db)
        {
            ObjectId objectId = ObjectId.Null;
            string textTypeName = "T_TEXT";
            TextStyleTable textStyleTable = tx.GetObject(db.TextStyleTableId, OpenMode.ForWrite) as TextStyleTable;
            if (!textStyleTable.Has(textTypeName))
            {
                TextStyleTableRecord textStyleT = new TextStyleTableRecord();
                textStyleT.Name = textTypeName;
                textStyleTable.Add(textStyleT);
                tx.AddNewlyCreatedDBObject(textStyleT, true);

                textStyleT.Font = new Autodesk.AutoCAD.GraphicsInterface.FontDescriptor("Arial Narrow", false, false, textStyleT.Font.CharacterSet, textStyleT.Font.PitchAndFamily);

                objectId = textStyleT.ObjectId;
            }
            else
            {
                objectId = textStyleTable[textTypeName];
            }
            return objectId;
        }

        public static void CreateDimension(this Transaction tx, BlockTableRecord blockTableRecord, Point3d p1, Point3d p2, Point3d position, ObjectId dimstyle, double rotate)
        {
            RotatedDimension r = new RotatedDimension();
            r.SetDatabaseDefaults();

            r.XLine1Point = p1;
            r.XLine2Point = p2;

            r.Rotation = rotate;
            r.DimLinePoint = position;
            r.DimensionStyle = dimstyle;


            blockTableRecord.AppendEntity(r);
            tx.AddNewlyCreatedDBObject(r, true);
        }
        public static void CreateDimensionNumber(this Transaction tx, BlockTableRecord blockTableRecord, Point3d p1, Point3d p2, ObjectId dimstyle)
        {
            AlignedDimension r = new AlignedDimension();
            r.SetDatabaseDefaults();

            r.XLine1Point = p1;
            r.XLine2Point = p2;



            r.DimLinePoint = new Point3d((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2, (p1.Z + p2.Z) / 2);
            r.DimensionStyle = dimstyle;


            blockTableRecord.AppendEntity(r);
            tx.AddNewlyCreatedDBObject(r, true);
        }

    }
}
