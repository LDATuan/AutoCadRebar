using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class DataUtil
    {
        public static void AddReAppTableRecord(this Transaction tx, Database db, string name)
        {
            RegAppTable regAppTable = tx.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;

            if (!regAppTable.Has(name))
            {
                regAppTable.UpgradeOpen();
                RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                regAppTableRecord.Name = name;
                regAppTable.Add(regAppTableRecord);
                tx.AddNewlyCreatedDBObject(regAppTableRecord, true);
            }
        }
        public static Rebar GetDataRebar(this Transaction tx, Document doc)
        {
            var rebar = new Rebar();
            TypedValue[] typedValues = new TypedValue[] { new TypedValue(1001, "LDAT_XData") };
            var rFilter = new SelectionFilter(typedValues);
            var OBs = doc.Editor.GetSelection(new PromptSelectionOptions { MessageForAdding = "Select Rebar" }, rFilter).Value;
            if (OBs != null)
            {
                foreach (SelectedObject ob in OBs)
                {
                    var r = tx.GetObject(ob.ObjectId, OpenMode.ForRead);
                    if (r.GetType().Name == "Polyline")
                    {
                        var pl = r as Polyline;
                        ModelData.Polyline = pl;
                        var obj = tx.GetObject(pl.ObjectId, OpenMode.ForRead);
                        rebar.ObjectIdRebar = pl.ObjectId;
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
                    }
                }
            }

            return rebar;
        }
    }
}
