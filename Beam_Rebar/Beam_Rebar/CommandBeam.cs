using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using View;
using ViewModel;

namespace Beam_Rebar
{
    public class CommandBeam
    {
        [CommandMethod("LDATBeamSection")]
        public static void CreateSectionBeam()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            db.LineWeightDisplay = true;
            LDATLayerAndDIM();
            BeamSectionViewModel.doc = doc;
            BeamSectionViewModel.db = db;

            BeamSectionViewModel vm = new BeamSectionViewModel();
            vm.BeamSectionForm.ShowDialog();
        }
        [CommandMethod("LDATLayer")]
        public static void LDATLayerAndDIM()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            BeamSectionViewModel.doc = doc;
            BeamSectionViewModel.db = db;
            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var layerTb = tx.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                var T_NetBao = LayerUtil.Create_Layer("T_NetBao", tx, layerTb, Color.FromRgb(255, 204, 0), LineWeight.ByLineWeightDefault);
                var T_NetBreakLine = LayerUtil.Create_Layer("T_NetBreakLine", tx, layerTb, Color.FromRgb(255, 0, 0), LineWeight.ByLineWeightDefault);
                var T_CircleNode = LayerUtil.Create_Layer("T_CircleNode", tx, layerTb, Color.FromRgb(0, 102, 102), LineWeight.LineWeight030);
                var T_Rebar = LayerUtil.Create_Layer("T_Rebar", tx, layerTb, Color.FromRgb(255, 0, 0), LineWeight.LineWeight030);
                var T_Tag = LayerUtil.Create_Layer("T_Tag", tx, layerTb, Color.FromRgb(255, 255, 25), LineWeight.LineWeight030);
                var T_TagWhite = LayerUtil.Create_Layer("T_TagWhite", tx, layerTb, Color.FromRgb(255, 255, 255), LineWeight.LineWeight030);

                ModelData.ObjectId_Layer = new List<ObjectId> { T_NetBao, T_NetBreakLine, T_CircleNode, T_Rebar, T_Tag, T_TagWhite };

                ObjectId obID_DimNumber_25 = tx.DimStyleNumber(db, blockTable, 25, Color.FromRgb(255, 0, 0), Color.FromRgb(255, 0, 0));
                ObjectId obID_Leader_25 = tx.DimStyleNumber(db, blockTable, 25, Color.FromRgb(255, 255, 255), Color.FromRgb(255, 255, 255));

                ModelData.ObjectId_DimStyle = new List<ObjectId> { obID_DimNumber_25, obID_Leader_25 };
                tx.Commit();
            }
        }
        [CommandMethod("T_Rebar")]
        public void TRebar()
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            db.LineWeightDisplay = true;
            LDATLayerAndDIM();

            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {

                BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var layerTb = tx.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;

                ObjectId obId_LayerRebar = layerTb.Cast<ObjectId>().SingleOrDefault(x =>
                                    (tx.GetObject(x, OpenMode.ForRead) as LayerTableRecord).Name == "T_Rebar");
                ObjectId obIdCurrent = db.Clayer;
                db.Clayer = layerTb["T_Rebar"];
                ed.Command("._PLINE");
                ObjectId polyLineID = Autodesk.AutoCAD.Internal.Utils.EntLast();

                var polyLine = tx.GetObject(polyLineID, OpenMode.ForWrite);
                Polyline pl = null;
                if (polyLine.GetType().Name == "Polyline")
                {
                    pl = polyLine as Polyline;
                }
                RebarView.DrawAndUpdate = true;
                RebarViewModel vm = new RebarViewModel();

                ModelData.tx = tx;
                ModelData.doc = doc;
                ModelData.Polyline = pl;
                ModelData.ObIdLayerRebar = obId_LayerRebar;

                db.Clayer = obIdCurrent;
                vm.RebarForm.ShowDialog();
                tx.Commit();
            }
        }
        [CommandMethod("T_TagLeader")]
        public void T_TagLeaderRebar()
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            db.LineWeightDisplay = true;
            LDATLayerAndDIM();
            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Rebar rebar = tx.GetDataRebar(doc);
                var p1 = doc.GetPointFromUser("SelectPoint:");
                var p2 = doc.GetPointFromUser("SelectPoint:");
                var p3 = doc.GetPointFromUser("SelectPoint:");
                var p = new Point3d(p3.X, p2.Y, p2.Z);
                ModelData.LastPntTag_T = p;
                tx.CreateLeader(blockTableRecord, p1, p2, p, ModelData.ObjectId_DimStyle[1]);

                var obIDText = tx.CreateTextType(db);
                var obIdtag = tx.TagRebar(blockTable, "BLOCKTAG_T", ModelData.ObjectId_Layer[4]);
                var bloc = tx.CreateBlockTagRebar(blockTableRecord, rebar, obIdtag, obIDText, p, ModelData.ObjectId_Layer[5], ModelData.ObjectId_Layer[4], 25);
                tx.CreateDataBlock(db, bloc);

                tx.Commit();
            }
        }

        //[CommandMethod("T_TagRebar")]
        //public void T_GetTagRebar()
        //{
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;
        //    Editor ed = doc.Editor;
        //    db.LineWeightDisplay = true;
        //    using (Transaction tx = doc.TransactionManager.StartTransaction())
        //    {
        //        BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        //        BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
        //        var layerTb = tx.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
        //        ObjectId obId_LayerTag = layerTb.Cast<ObjectId>().SingleOrDefault(x =>
        //                            (tx.GetObject(x, OpenMode.ForRead) as LayerTableRecord).Name == "T_Tag");
        //        ObjectId obId_LayerTagWhite = layerTb.Cast<ObjectId>().SingleOrDefault(x =>
        //                            (tx.GetObject(x, OpenMode.ForRead) as LayerTableRecord).Name == "T_TagWhite");
        //        var rebar = tx.GetDataRebar(doc);
        //        var obIDText = tx.CreateTextType(db);
        //        var position = doc.GetPointFromUser("Select Point");
        //        var obIdtag = blockTable.TagRebar("BLOCKTAG_T");
        //        var bloc = tx.CreateBlockTagRebar(blockTableRecord, rebar, obIdtag, obIDText, position, obId_LayerTagWhite, obId_LayerTag, 25);
        //        tx.CreateDataBlock(db, bloc);


        //        tx.Commit();
        //    }

        //}
        [CommandMethod("T_TagRSL")]
        public void T_TagRebarSpacingLength()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            db.LineWeightDisplay = true;
            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                var layerTb = tx.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                ObjectId obId_LayerTag = layerTb.Cast<ObjectId>().SingleOrDefault(x =>
                                    (tx.GetObject(x, OpenMode.ForRead) as LayerTableRecord).Name == "T_Tag");
                ObjectId obId_LayerTagWhite = layerTb.Cast<ObjectId>().SingleOrDefault(x =>
                                   (tx.GetObject(x, OpenMode.ForRead) as LayerTableRecord).Name == "T_TagWhite");
                Rebar rebar = tx.GetDataRebar(doc);
                var obIDText = tx.CreateTextType(db);
                var position = doc.GetPointFromUser("Select Point");
                var obIdtag = blockTable.TagRebar("BLOCKTAG_T1");
                var bloc = tx.CreateBlockTagRebar(blockTableRecord, rebar, obIdtag, obIDText, position, obId_LayerTagWhite, obId_LayerTag, 25);
                tx.CreateDataBlock(db, bloc);


                tx.Commit();
            }

        }
        [CommandMethod("T_UpDateTag")]
        public void T_UpDateTag()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            db.LineWeightDisplay = true;
            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {


                var OBs = ed.GetSelection(new PromptSelectionOptions { MessageForAdding = "Select Rebar and Tag" }).Value;
                List<BlockReference> bls = new List<BlockReference>();
                List<Polyline> pls = new List<Polyline>();
                List<Rebar> rebars = new List<Rebar>();
                foreach (SelectedObject ob in OBs)
                {
                    var r = tx.GetObject(ob.ObjectId, OpenMode.ForRead);
                    if (r.GetType().Name == "Polyline")
                    {
                        Rebar rebar = new Rebar();
                        var pl = r as Polyline;
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
                            rebar.Length = pl.Length;
                        }
                        rebars.Add(rebar);
                        pls.Add(pl);
                    }
                    if (r is BlockReference)
                    {
                        bls.Add(r as BlockReference);
                    }
                }
                tx.UpDateTagRebarV2(rebars, bls);




                tx.Commit();
            }

        }
        [CommandMethod("T_UpDateTagAuto")]
        public void T_UpDateTagAuto()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            db.LineWeightDisplay = true;
            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Rebar rebar = new Rebar();

                var pl = ModelData.Polyline;
                RXClass rxc = RXClass.GetClass(typeof(BlockReference));
                string nameblockData1 = "BLOCKTAG_T1_XData";
                string nameblockData2 = "BLOCKTAG_T_XData";
                List<BlockReference> bls = new List<BlockReference>();
                foreach (ObjectId obId in blockTableRecord)
                {
                    if (obId.ObjectClass == rxc)
                    {
                        var obj1 = tx.GetObject(obId, OpenMode.ForWrite);
                        if (obj1 is BlockReference blockReference)
                        {

                            ResultBuffer rsb1 = null;
                            if (blockReference.Name == "BLOCKTAG_T1")
                            {
                                rsb1 = blockReference.GetXDataForApplication(nameblockData1);
                            }
                            else if (blockReference.Name == "BLOCKTAG_T")
                            {
                                rsb1 = blockReference.GetXDataForApplication(nameblockData2);
                            }

                            var arr = rsb1.AsArray();
                            if (arr[1].Value.ToString() == pl.ObjectId.ToString())
                            {
                                bls.Add(blockReference);
                            }
                        }
                    }
                }
                tx.UpdateTagRebar(bls, pl, rebar);

                tx.Commit();
            }
        }

        //[CommandMethod("T_UpDateLengthTag",CommandFlags.UsePickSet)]
        //public void T_UpDateLengthTag()
        //{
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;
        //    Editor ed = doc.Editor;
        //    ModelData.doc = doc;
        //    db.LineWeightDisplay = true;
        //    using (Transaction tx = doc.TransactionManager.StartTransaction())
        //    {
        //        BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        //        BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
        //        ModelData.tx = tx;
        //        RebarView.DrawAndUpdate = false;
        //        ed.Command("._STRETCH");
        //        var rebar = new Rebar();
        //        Polyline pl = null;



        //        ///CACH 1
        //        //var OBs = ed.GetSelection().Value;

        //        //foreach (SelectedObject ob in OBs)
        //        //{
        //        //    var r = tx.GetObject(ob.ObjectId, OpenMode.ForRead);
        //        //    if (r.GetType().Name == "Polyline")
        //        //    {
        //        //         pl = r as Polyline;
        //        //        var obj = tx.GetObject(pl.ObjectId, OpenMode.ForRead);
        //        //        ModelData.Polyline = pl;
        //        //        var rsb = obj.GetXDataForApplication("LDAT_XData");
        //        //        if (rsb != null)
        //        //        {
        //        //            var arr = rsb.AsArray();
        //        //            rebar.RebarNumber = arr[1].Value.ToString();
        //        //            rebar.BarDiameter = int.Parse(arr[2].Value.ToString());
        //        //            rebar.Count = arr[3].Value.ToString();
        //        //            rebar.NameElement = arr[4].Value.ToString();
        //        //            rebar.Spacing = arr[5].Value.ToString();
        //        //            rebar.Comment = arr[6].Value.ToString();
        //        //            rebar.Length = pl.Length;
        //        //            break;
        //        //        }
        //        //    }
        //        //}
        //        ObjectId polyLineID = Autodesk.AutoCAD.Internal.Utils.EntLast();

        //        var polyLine = tx.GetObject(polyLineID, OpenMode.ForWrite);

        //        if (polyLine.GetType().Name == "Polyline")
        //        {
        //            pl = polyLine as Polyline;
        //            var obj = tx.GetObject(pl.ObjectId, OpenMode.ForRead);
        //            var rsb = obj.GetXDataForApplication("LDAT_XData");
        //            if (rsb != null)
        //            {
        //                var arr = rsb.AsArray();
        //                rebar.RebarNumber = arr[1].Value.ToString();
        //                rebar.BarDiameter = int.Parse(arr[2].Value.ToString());
        //                rebar.Count = arr[3].Value.ToString();
        //                rebar.NameElement = arr[4].Value.ToString();
        //                rebar.Spacing = arr[5].Value.ToString();
        //                rebar.Comment = arr[6].Value.ToString();
        //                rebar.Length = pl.Length;
        //            }
        //        }
        //        BlockReference bl = null;

        //        RXClass rxc = RXClass.GetClass(typeof(BlockReference));

        //        foreach (ObjectId obId in blockTableRecord)
        //        {
        //            if (obId.ObjectClass == rxc)
        //            {
        //                var obj1 = tx.GetObject(obId, OpenMode.ForWrite);
        //                if (obj1 is BlockReference blockReference)
        //                {
        //                    var nameblockData = "BLOCKTAG_T1_XData";
        //                    var rsb1 = blockReference.GetXDataForApplication(nameblockData);
        //                    var arr = rsb1.AsArray();
        //                    if (arr[1].Value.ToString() == pl.ObjectId.ToString())
        //                    {
        //                        bl = blockReference;
        //                    }
        //                }
        //            }

        //        }
        //        foreach (ObjectId objectId in bl.AttributeCollection)
        //        {
        //            var ob1 = tx.GetObject(objectId, OpenMode.ForWrite);
        //            if (ob1 is AttributeReference att)
        //            {
        //                if (att.Tag == "RebarNumber")
        //                {
        //                    att.TextString = $"[{rebar.RebarNumber}]";

        //                }
        //                else if (att.Tag == "KIHIEUTHEP")
        //                {
        //                    att.TextString = $"{rebar.Count}T{rebar.BarDiameter}@{rebar.Spacing}/L={Math.Round(rebar.Length)}";
        //                }
        //            }

        //        }

        //        tx.Commit();
        //    }
        //}
        [CommandMethod("T_DimRebarAuto")]
        public void T_DimRebarAuto()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            LDATLayerAndDIM();
            db.LineWeightDisplay = true;
            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                ObjectId obID_DimNumber = ModelData.ObjectId_DimStyle[0];

                var p3DColl = new Point3dCollection();

                var pl = ModelData.Polyline;
                for (int i = 0; i < pl.NumberOfVertices; i++)
                {
                    p3DColl.Add(pl.GetPoint3dAt(i).TransformBy(ed.CurrentUserCoordinateSystem));
                }
                for (int i = 0; i < p3DColl.Count - 1; i++)
                {
                    tx.CreateDimensionNumber(blockTableRecord, p3DColl[i], p3DColl[i + 1], obID_DimNumber);
                }

                tx.Commit();
            }

        }

        [CommandMethod("T_DimRebar")]
        public void T_DimRebar()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            LDATLayerAndDIM();
            db.LineWeightDisplay = true;
            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                ObjectId obID_DimNumber = ModelData.ObjectId_DimStyle[0];

                TypedValue[] typedValues = new TypedValue[] { new TypedValue(1001, "LDAT_XData") };
                var rFilter = new SelectionFilter(typedValues);
                var OBs = ed.GetSelection(new PromptSelectionOptions { MessageForAdding = "Select Rebar" }, rFilter).Value;

                var p3DColl = new Point3dCollection();

                foreach (SelectedObject ob in OBs)
                {
                    var r = tx.GetObject(ob.ObjectId, OpenMode.ForRead);
                    if (r.GetType().Name == "Polyline")
                    {
                        var pl = r as Polyline;
                        for (int i = 0; i < pl.NumberOfVertices; i++)
                        {
                            p3DColl.Add(pl.GetPoint3dAt(i).TransformBy(ed.CurrentUserCoordinateSystem));
                        }


                    }
                }
                for (int i = 0; i < p3DColl.Count - 1; i++)
                {
                    tx.CreateDimensionNumber(blockTableRecord, p3DColl[i], p3DColl[i + 1], obID_DimNumber);
                }

                tx.Commit();
            }
        }
        [CommandMethod("LDAT_DoubleClickAction")]

        public void RegisterDoubleClickActionMethod()
        {

            string mainCuiFile = (string)Application.GetSystemVariable("MENUNAME") + ".cuix";

            CustomizationSection cs = new CustomizationSection(mainCuiFile);
            int index = 0;

            DoubleClickAction DoubleClickAction = null;
            string name = "LDAT_Polyline_DBC ";

            foreach (DoubleClickAction dca in cs.MenuGroup.DoubleClickActions)
            {
                if (dca.Name.Equals(name))
                {
                    cs.MenuGroup.DoubleClickActions.Remove(index);
                    break;
                }
                index++;

            }
            index = 0;

            MacroGroup myMacroGroup = null;
            string namegroup = "LDAT_MacroGroup";
            foreach (MacroGroup mg in cs.MenuGroup.MacroGroups)
            {
                if (mg.Name.Equals(namegroup))
                {
                    cs.MenuGroup.MacroGroups.Remove(index);
                    break;
                }
                index++;
            }
            if (DoubleClickAction == null && myMacroGroup == null)
            {
                DoubleClickAction dblClickAction = new DoubleClickAction(cs.MenuGroup, name, -1);
                dblClickAction.Description = "Double Click Polyline";
                dblClickAction.ElementID = "EID_PL_DBC";
                dblClickAction.DxfName = "LWPOLYLINE";


                DoubleClickCmd dblClickCmd = new DoubleClickCmd(dblClickAction);
                MacroGroup myMacGroup = new MacroGroup(namegroup, cs.MenuGroup);
                MenuMacro macroMyForm = myMacGroup.CreateMenuMacro("LDAT_Pline_DBC", "^C^C_CHANGEINFOREBAR", "ID_Pline_DBC");

                dblClickCmd.MacroID = macroMyForm.ElementID;


                dblClickAction.DoubleClickCmd = dblClickCmd;

                if (cs.IsModified) cs.Save();
            }

        }
        [CommandMethod("ChangeInfoRebar", CommandFlags.UsePickSet)]
        public void ChangeInfoRebar()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            ModelData.doc = doc;
            db.LineWeightDisplay = true;
            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {
                ModelData.tx = tx;
                RebarView.DrawAndUpdate = false;
                RebarViewModel vm = new RebarViewModel();
                var OBs = ed.GetSelection().Value;
                foreach (SelectedObject ob in OBs)
                {
                    var r = tx.GetObject(ob.ObjectId, OpenMode.ForRead);
                    if (r.GetType().Name == "Polyline")
                    {
                        var pl = r as Polyline;
                        var obj = tx.GetObject(pl.ObjectId, OpenMode.ForRead);
                        ModelData.Polyline = pl;
                        var rsb = obj.GetXDataForApplication("LDAT_XData");
                        if (rsb != null)
                        {
                            var arr = rsb.AsArray();
                            vm.RebarView.RebarNumber = arr[1].Value.ToString();
                            vm.RebarView.SelectedBarDiameter = int.Parse(arr[2].Value.ToString());
                            vm.RebarView.Count = arr[3].Value.ToString();
                            vm.RebarView.ElementName = arr[4].Value.ToString();
                            vm.RebarView.Spacing = arr[5].Value.ToString();
                            vm.RebarView.Comment = arr[6].Value.ToString();
                            break;
                        }
                    }
                }
                vm.RebarForm.ShowDialog();


                tx.Commit();
            }
        }

    }
}

