using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using BaseViewModel;

namespace View
{
    public class BeamSectionView : BaseViewModel.BaseViewModel
    {
        private string width;

        public string Width
        {
            get { return width; }
            set
            {
                width = value;
                Numbers = this.GetCount();
                OnPropertyChanged();
            }
        }
        private string height;

        public string Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }
        private string thicknessSlab;

        public string ThicknessSlab
        {
            get { return thicknessSlab; }
            set
            {
                thicknessSlab = value;
                OnPropertyChanged();
            }

        }
        private string cover;

        public string Cover
        {
            get { return cover; }
            set
            {
                cover = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<int> numbers;

        public ObservableCollection<int> Numbers
        {
            get
            {
                if (numbers == null)
                {
                    numbers = new ObservableCollection<int> { 1, 2, 3, 4, 5 };
                }
                return numbers;
            }
            set { numbers = value; OnPropertyChanged(); }
        }
        private int selectedTop;

        public int SelectedTop
        {
            get { return selectedTop; }
            set { selectedTop = value; OnPropertyChanged(); }
        }

        private int selectedBot;

        public int SelectedBot
        {
            get { return selectedBot; }
            set { selectedBot = value; OnPropertyChanged(); }
        }
    }
}
