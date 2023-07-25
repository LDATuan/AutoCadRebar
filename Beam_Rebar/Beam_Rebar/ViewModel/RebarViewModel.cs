using Autodesk.AutoCAD.DatabaseServices;
using Model;
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
    public class RebarViewModel : BaseViewModel.BaseViewModel
    {
        private RebarForm rebarForm;

        public RebarForm RebarForm
        {
            get
            {
                if (rebarForm == null)
                {
                    rebarForm = new RebarForm { DataContext = this };
                }
                return rebarForm;
            }
            set { rebarForm = value; OnPropertyChanged(); }
        }
        private RebarView rebarView;

        public RebarView RebarView
        {
            get
            {
                if (rebarView == null)
                {
                    rebarView = new RebarView();
                }
                return rebarView;
            }
            set { rebarView = value; OnPropertyChanged(); }
        }
        public static LDATSetting LDATSetting { get; set; }
        public ICommand DrawRebarCommand { get; set; }
        public RebarViewModel()
        {
            if (RebarView.DrawAndUpdate == true)
            {
                LoadSetting();
            }
            DrawRebarCommand = new RelayCommand<object>(p => RebarView.BarDiameter != null,
            p =>
            {
                if (RebarView.DrawAndUpdate == true)
                {
                    RebarForm.Hide();

                    var r = new Rebar
                    {
                        RebarNumber = RebarView.RebarNumber,
                        BarDiameter = RebarView.SelectedBarDiameter,
                        Count = RebarView.Count,
                        NameElement = RebarView.ElementName,
                        Spacing = RebarView.Spacing
                    };
                    ModelData.tx.CreateRebar(ModelData.doc.Database,
                        ModelData.Polyline, r, ModelData.ObIdLayerRebar, "LDAT_XData");
                    ModelData.doc.Editor.Command("._T_DimRebarAuto");
                    SaveSetting();
                }
                else
                {
                    RebarForm.Hide();
                    ModelData.tx.UpdateData(ModelData.Polyline, RebarView);
                    ModelData.doc.Editor.Command("._T_UpDateTagAuto");
                }
            });




        }
        public void SaveSetting()
        {
            LDATSetting.ElementName = RebarView.ElementName;
            LDATSetting.RebarNumber = RebarView.RebarNumber;
            LDATSetting.BarDiameter = RebarView.SelectedBarDiameter;
            LDATSetting.Count = RebarView.Count;
            LDATSetting.Spacing = RebarView.Spacing;
            LDATSetting.Comment = RebarView.Comment;


            JsonUtil.Save<LDATSetting>(ModelData.PathFile, LDATSetting);
        }
        public void LoadSetting()
        {
            LDATSetting = JsonUtil.Load<LDATSetting>(ModelData.PathFile);
            RebarView.ElementName = LDATSetting.ElementName;
            RebarView.RebarNumber = LDATSetting.RebarNumber;
            RebarView.SelectedBarDiameter = LDATSetting.BarDiameter;
            RebarView.Count = LDATSetting.Count;
            RebarView.Spacing = LDATSetting.Spacing;
            RebarView.Comment = LDATSetting.Comment;
        }
    }
}
