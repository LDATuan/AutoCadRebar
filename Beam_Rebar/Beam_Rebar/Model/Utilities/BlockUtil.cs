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
    public static class BlockUtil
    {
        public static ObjectId TagRebar(this BlockTable blockTable, string nameblock)
        {
            //string name = "BLOCKTAG_T";
            ObjectId objectIdTag = ObjectId.Null;
            var vecX = new Vector3d(1, 0, 0);
            if (!blockTable.Has(nameblock))
            {
                using (BlockTableRecord bTR = new BlockTableRecord())
                {
                    bTR.Name = nameblock;
                    blockTable.UpgradeOpen();
                    blockTable.Add(bTR);
                    objectIdTag = bTR.ObjectId;
                }
            }
            else
            {
                objectIdTag = blockTable[nameblock];
            }
            return objectIdTag;

        }
        public static ObjectId TagRebar(this Transaction tx, BlockTable blockTable, string nameblock, ObjectId objectIdLayer2)
        {
            //string name = "BLOCKTAG_T";
            ObjectId objectIdTag = ObjectId.Null;
            var vecX = new Vector3d(1, 0, 0);
            if (!blockTable.Has(nameblock))
            {
                using (BlockTableRecord bTR = new BlockTableRecord())
                {
                    bTR.Name = nameblock;
                    blockTable.UpgradeOpen();
                    blockTable.Add(bTR);
                    bTR.Origin = new Point3d(0, 0, 0);
                    //Cirle
                    var radius = 2.2;
                    Circle circle = new Circle();
                    circle.Center = new Point3d(2.2, 0, 0);
                    // circle.Center = position + radius * vecX;
                    circle.Radius = radius;
                    circle.SetLayerId(objectIdLayer2, false);

                    bTR.AppendEntity(circle);
                    tx.AddNewlyCreatedDBObject(circle, true);

                    objectIdTag = bTR.ObjectId;

                }
            }
            else
            {
                objectIdTag = blockTable[nameblock];
            }
            return objectIdTag;

        }
        public static BlockReference CreateBlockTagRebar(this Transaction tx,
            BlockTableRecord blockTableRecord, Rebar rebar, ObjectId objectId, ObjectId objectIdText, Point3d position, ObjectId objectIdLayer1, ObjectId objectIdLayer2, double scale)
        {
            var vecX = new Vector3d(1, 0, 0);
            var vecY = new Vector3d(0, 1, 0);
            BlockReference blockRef = null;
            if (objectId != null)
            {
                blockRef = new BlockReference(position, objectId);
                blockTableRecord.AppendEntity(blockRef);
                tx.AddNewlyCreatedDBObject(blockRef, true);

                // rebarNumber
                AttributeReference KiHieu = new AttributeReference();
                KiHieu.Tag = "RebarNumber";

                KiHieu.TextStyleId = objectIdText;
                KiHieu.Height = 2.5 * scale;
                KiHieu.SetLayerId(objectIdLayer1, false);



                // count bardiameter
                AttributeReference countBar = new AttributeReference();
                countBar.Tag = "KIHIEUTHEP";

                countBar.TextStyleId = objectIdText;
                countBar.Height = 2.5 * scale;
                countBar.SetLayerId(objectIdLayer2, false);

                if (blockRef.Name == "BLOCKTAG_T")
                {
                    if (rebar.RebarNumber.Length > 1)
                    {
                        KiHieu.Position = position + 0.6 * scale * vecX - (2.2 * scale / 2) * vecY;
                    }
                    else
                    {
                        KiHieu.Position = position + 1.4 * scale * vecX - (2.2 * scale / 2) * vecY;
                    }
                    countBar.Position = position - 8 * scale * vecX + 0.8 * scale * vecY;
                    KiHieu.TextString = $"{rebar.RebarNumber}";
                    // KiHieu.Justify = AttachmentPoint.MiddleCenter;
                    countBar.TextString = $"{rebar.Count}T{rebar.BarDiameter}";
                }
                else if (blockRef.Name == "BLOCKTAG_T1")
                {
                    KiHieu.Position = position;
                    countBar.Position = position + 5 * scale * vecX;
                    KiHieu.TextString = $"[{rebar.RebarNumber}]";
                    countBar.TextString = $"{rebar.Count}T{rebar.BarDiameter}@{rebar.Spacing}/L={Math.Round(rebar.Length)}";
                }

                blockRef.AttributeCollection.AppendAttribute(KiHieu);
                tx.AddNewlyCreatedDBObject(KiHieu, true);

                blockRef.AttributeCollection.AppendAttribute(countBar);
                tx.AddNewlyCreatedDBObject(countBar, true);

                blockRef.ScaleFactors = new Scale3d(scale);


            }
            return blockRef;
        }
        public static void CreateDataBlock(this Transaction tx, Database db, BlockReference blockReference)
        {
            var name2 = blockReference.Name + "_XData";
            tx.AddReAppTableRecord(db, name2);
            string namerebar = ModelData.Polyline.ObjectId.ToString();
            ResultBuffer resultBuffer = new ResultBuffer(new TypedValue(1001, name2), new TypedValue(1000, namerebar));
            blockReference.XData = resultBuffer;

        }
    }
}
