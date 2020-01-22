namespace InOutManagement.Controls
{
    #region using driective

    using InOutManagement.Entity;
    using InOutManagement.Windows;
    using InOutManagement.SQLHelper;

    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    #endregion
    public partial class QueryView: INotifyPropertyChanged
    {              

        public ObservableCollection<String> MaterialList { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<String> ModelList { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<ListviewContent> ViewList { get; set; } = new ObservableCollection<ListviewContent>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void Query(object sender, RoutedEventArgs e)
        {
            this.sqlHelper.Query();
            this.MaterialList.Add("asdadwa");
            this.ViewList.Add(new ListviewContent()
            {
                BillArchive = "asdasdasd",
                Count = 22,
                Date = DateTime.Now.ToShortDateString(),
                Model = "DZ47LE-39-32A",
                Name = "大汉港白乳胶（A货）",
                Price = 1111,
                Status = "进库",
                Supplier = "甲供",
                Unit = "个",
            });
        }

        private void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.StartDate.SelectedDate != null)
            {
                if (this.EndDate.SelectedDate != null)
                {
                    if (this.EndDate.SelectedDate < this.StartDate.SelectedDate)
                    {
                        MainWindow.GetInstance().Tips = "结束时间小于开始时间，请重新设置时间范围！";
                        this.EndDate.SelectedDate = null;
                    }
                }
            }
        }

        private void Material_DropDownOpened(object sender, EventArgs e)
        {
            this.MaterialList.Clear();
            var materials = this.sqlHelper.Query<Material>(new Material());
            foreach (var item in materials)
            {
                this.MaterialList.Add(item.Name);
            }
        }

        public void OnPropertyChanged(String propertyName)
        {
            var temp = this.PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Model_DropDownOpened(object sender, EventArgs e)
        {
            this.ModelList.Clear();            
            var materials = this.sqlHelper.Query<Material>(new Material() { Name = this.Material.Text });
            foreach (var item in materials)
            {
                this.ModelList.Add(item.Name);
            }
        }

        private SQLHelper sqlHelper = SQLHelper.GetInstance();
    }
}