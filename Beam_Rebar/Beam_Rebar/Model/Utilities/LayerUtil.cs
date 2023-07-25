using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class LayerUtil
    {
        public static bool CheckNameLayer(this Transaction transaction, LayerTable layerTable, string name)
        {
            bool result = false;
            foreach (var ob in layerTable)
            {
                LayerTableRecord layerRecord = transaction.GetObject(ob, OpenMode.ForRead) as LayerTableRecord;
                if (layerRecord.Name == name)
                {
                    result = true;
                }
            }
            return result;
        }
        public static ObjectId Create_Layer(string nameLayer,Transaction tx, LayerTable layerTb, Color color,LineWeight lineWeight)
        {
            ObjectId objectId = ObjectId.Null;
            if (!tx.CheckNameLayer(layerTb, nameLayer))
            {
                LayerTableRecord layerTableRecord = new LayerTableRecord();
                layerTableRecord.Name = nameLayer;
                layerTableRecord.Color = color;
                layerTableRecord.LineWeight = lineWeight;

                layerTb.Add(layerTableRecord);
                tx.AddNewlyCreatedDBObject(layerTableRecord, true);
                objectId = layerTableRecord.ObjectId;
            }
            else
            {
                objectId = layerTb[nameLayer];
            }
            return objectId;
        }
    }
   
}
