using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View
{
    public static class BeamSectionViewUtil
    {
        public static ObservableCollection<int> GetCount(this BeamSectionView bSView)
        {
            
            var cs = new ObservableCollection<int>();
            if (bSView.Width == null || bSView.Width == "")
            {
                for (int i = 1; i < 6; i++)
                {
                    cs.Add(i);
                }
            }
            else
            {
                bSView.Numbers.Clear();
                var count_max = int.Parse(Math.Round(double.Parse(bSView.Width) / 60).ToString());
                for (int i = 1; i <= count_max; i++)
                {
                    cs.Add(i);
                }
            }


            return cs;
        }
        public static void PreViewUpdate(this RebarView rebarView)
        {
            rebarView.PreviewText = $"[{rebarView.RebarNumber}] {rebarView.Count}T{rebarView.SelectedBarDiameter}@{rebarView.Spacing}";
        }
    }
}
