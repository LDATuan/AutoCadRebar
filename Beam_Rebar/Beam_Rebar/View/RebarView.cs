using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View
{
    public class RebarView : BaseViewModel.BaseViewModel
    {
        private string elementName;

        public string ElementName
        {
            get { return elementName; }
            set { elementName = value; OnPropertyChanged(); }
        }
        private string rebarNumber;

        public string RebarNumber
        {
            get { return rebarNumber; }
            set
            {
                rebarNumber = value;
                this.PreViewUpdate();
                OnPropertyChanged();
            }
        }
        private ObservableCollection<int> barDiameter;

        public ObservableCollection<int> BarDiameter
        {
            get
            {
                if (barDiameter == null)
                {
                    barDiameter = new ObservableCollection<int> { 6, 8, 10, 12, 14, 16, 18, 20, 22, 25, 30, 35 };
                }
                return barDiameter;
            }
            set { barDiameter = value; OnPropertyChanged(); }
        }
        private int selectedBarDiameter;

        public int SelectedBarDiameter
        {
            get { return selectedBarDiameter; }
            set
            {
                selectedBarDiameter = value;
                this.PreViewUpdate();
                OnPropertyChanged();
            }
        }
        private string count;

        public string Count
        {
            get { return count; }
            set { count = value; this.PreViewUpdate(); OnPropertyChanged(); }
        }
        private string spacing;

        public string Spacing
        {
            get { return spacing; }
            set { spacing = value; this.PreViewUpdate(); OnPropertyChanged(); }
        }
        private string contentButton;

        public string ContentButton
        {
            get
            {
                if (DrawAndUpdate == true)
                {
                    contentButton = "OK";
                }
                else
                {
                    contentButton = "Update";
                }
                return contentButton;
            }
            set { contentButton = value; OnPropertyChanged(); }
        }

        public static bool DrawAndUpdate { get; set; }

        private string comment;

        public string Comment
        {
            get { return comment; }
            set { comment = value; OnPropertyChanged(); }
        }
        private string previewText;

        public string PreviewText
        {
            get
            {
                if (previewText == null)
                {
                    previewText = $"[{RebarNumber}] {Count}T{SelectedBarDiameter}@{Spacing}";
                }
                return previewText;
            }
            set { previewText = value; OnPropertyChanged(); }
        }
        private string title;

        public string Title
        {
            get
            {
                if (DrawAndUpdate == true)
                {
                    title = "Create Rebar";
                }
                else
                {
                    title = "Info Rebar";
                }
                return title;
            }
            set { title = value; OnPropertyChanged(); }
        }

    }
}
