using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utilities;
using View;

namespace ViewModel
{
    public class BeamSectionViewModel : BaseViewModel.BaseViewModel
    {
        public static Document doc { get; set; }
        public static Database db { get; set; }
        private BeamSectionForm beamSectionForm;

        public BeamSectionForm BeamSectionForm
        {
            get
            {
                if (beamSectionForm == null)
                {
                    beamSectionForm = new BeamSectionForm { DataContext = this };
                }
                return beamSectionForm;
            }
            set { beamSectionForm = value; }
        }
        private BeamSectionView beamSectionView;

        public BeamSectionView BeamSectionView
        {
            get
            {
                if (beamSectionView == null)
                {
                    beamSectionView = new BeamSectionView();
                }
                return beamSectionView;
            }
            set { beamSectionView = value; OnPropertyChanged(); }
        }
        public ICommand DrawCommand { get; set; }
        public BeamSectionViewModel()
        {
            LoadSetting();

            DrawCommand = new RelayCommand<object>(p => BeamSectionView.SelectedBot != 0 && BeamSectionView.SelectedTop != 0,
                p =>
                {
                    SaveSetting();
                    BeamSectionForm.Hide();

                    var width = double.Parse(BeamSectionView.Width);
                    var height = double.Parse(BeamSectionView.Height);
                    var height_slab = double.Parse(BeamSectionView.ThicknessSlab);
                    var cover = double.Parse(BeamSectionView.Cover);


                    CreateBeamSection(doc, db, width, height, height_slab, cover, BeamSectionView.SelectedTop, BeamSectionView.SelectedBot);

                });
        }
        public void CreateBeamSection(Document doc, Database db, double width, double height, double height_slab, double cover, int n_top, int n_bot)
        {
            using (Transaction tx = doc.TransactionManager.StartTransaction())
            {

                BlockTable blockTable = tx.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = tx.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var layerTb = tx.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;

                ObjectId obId_NetBao = layerTb.Cast<ObjectId>().SingleOrDefault(x =>
                                    (tx.GetObject(x, OpenMode.ForRead) as LayerTableRecord).Name == "T_NetBao");
                ObjectId obId_NetBreakLine = layerTb.Cast<ObjectId>().SingleOrDefault(x =>
                                    (tx.GetObject(x, OpenMode.ForRead) as LayerTableRecord).Name == "T_NetBreakLine");
                ObjectId obIdLayer_CircleNode = layerTb.Cast<ObjectId>().SingleOrDefault(x =>
                                    (tx.GetObject(x, OpenMode.ForRead) as LayerTableRecord).Name == "T_CircleNode");

                var p = doc.GetPointFromUser("Select Point");
                #region INPUT
                //double width = 300;
                //double height = 600;
                //double height_slab = 150;
                double offset = 200;
                double offset_dim = 100;
                //double cover = 25.0;
                //int n_top = 4;
                //int n_bot = 3;
                #endregion


                var vecX = new Vector3d(1, 0, 0);
                var vecY = new Vector3d(0, 1, 0);
                //ve net bao dam
                var lines_NetBao = new List<Line>();
                //net tren san
                var p_s1 = new Point3d(p.X - width / 2 - offset, p.Y, p.Z);
                var p_s2 = new Point3d(p.X + width / 2 + offset, p.Y, p.Z);
                var line_s = new Line(p_s1, p_s2);
                lines_NetBao.Add(line_s);

                //net dam
                var p_b1 = new Point3d(p_s1.X, p_s1.Y - height_slab, p_s1.Z);
                var p_b2 = p_b1 + offset * vecX;
                var p_b3 = p_b2 - (height - height_slab) * vecY;
                var p_b4 = p_b3 + width * vecX;
                var p_b5 = p_b2 + width * vecX;
                var p_b6 = p_b5 + offset * vecX;

                var line_b1 = new Line(p_b1, p_b2);
                var line_b2 = new Line(p_b2, p_b3);
                var line_b3 = new Line(p_b3, p_b4);
                var line_b4 = new Line(p_b4, p_b5);
                var line_b5 = new Line(p_b5, p_b6);

                lines_NetBao.Add(line_b1);
                lines_NetBao.Add(line_b2);
                lines_NetBao.Add(line_b3);
                lines_NetBao.Add(line_b4);
                lines_NetBao.Add(line_b5);
                //ve breakline
                tx.CreateBreakLine(blockTableRecord, obId_NetBreakLine, p_s1, height_slab);
                tx.CreateBreakLine(blockTableRecord, obId_NetBreakLine, p_s2, height_slab);
                //add line to drawing
                tx.AddLineToDrawing(blockTableRecord, lines_NetBao, obId_NetBao);


                // lay objectid cua thep
                ObjectId obIdCircle = tx.ObjectIdCircleNode(blockTable, obId_NetBreakLine);
                var pnt_top = p.GetPoint(n_top, PostionRebar.Top, width, height, cover);
                var pnt_Bot = p.GetPoint(n_bot, PostionRebar.Bottom, width, height, cover);
                //
                PointUtil.CreateCircleBlockRef(tx, blockTableRecord, pnt_top, obIdCircle);
                PointUtil.CreateCircleBlockRef(tx, blockTableRecord, pnt_Bot, obIdCircle);
                ////Create stirrup
                LineUtil.CreateStirrup(tx, blockTableRecord, p, width, height, cover, obIdLayer_CircleNode);

                ObjectId obId_Dim25 = tx.CreateDimTye(db, blockTable, 25);

                // dim ngang
                tx.CreateDimension(blockTableRecord, p_b3 - offset_dim * vecY, p_b4 - offset_dim * vecY, p_b3 - (100 + offset_dim) * vecY, obId_Dim25, 0);
                // dim doc
                var p_v1 = p_s1 - offset_dim * vecX;
                var p_v2 = p_b1 - offset_dim * vecX;
                var p_v3 = p_b3 - (offset_dim + offset) * vecX;


                tx.CreateDimension(blockTableRecord, p_v1, p_v2, p_v1 - 100 * vecX, obId_Dim25, Math.PI / 2);

                tx.CreateDimension(blockTableRecord, p_v2, p_v3, p_v1 - 100 * vecX, obId_Dim25, Math.PI / 2);

                tx.CreateDimension(blockTableRecord, p_v1, p_v3, p_v1 - 250 * vecX, obId_Dim25, Math.PI / 2);





                //doc.SendStringToExecute("._zoom _all ", true, false, false);
                //var f = new BeamSectionForm();
                //f.ShowDialog();
                tx.Commit();
            }
        }
        public void SaveSetting()
        {
            Beam_Rebar.Properties.Settings.Default.Width = BeamSectionView.Width;
            Beam_Rebar.Properties.Settings.Default.Height = BeamSectionView.Height;
            Beam_Rebar.Properties.Settings.Default.ThicnessSlab = BeamSectionView.ThicknessSlab;
            Beam_Rebar.Properties.Settings.Default.Cover = BeamSectionView.Cover;
            Beam_Rebar.Properties.Settings.Default.n_top = BeamSectionView.SelectedTop;
            Beam_Rebar.Properties.Settings.Default.n_bot = BeamSectionView.SelectedBot;
        }
        public void LoadSetting()
        {
            BeamSectionView.Width = Beam_Rebar.Properties.Settings.Default.Width;
            BeamSectionView.Height = Beam_Rebar.Properties.Settings.Default.Height;
            BeamSectionView.ThicknessSlab = Beam_Rebar.Properties.Settings.Default.ThicnessSlab;
            BeamSectionView.Cover = Beam_Rebar.Properties.Settings.Default.Cover;
            BeamSectionView.SelectedTop = Beam_Rebar.Properties.Settings.Default.n_top;
            BeamSectionView.SelectedBot = Beam_Rebar.Properties.Settings.Default.n_bot;
        }
    }
}
