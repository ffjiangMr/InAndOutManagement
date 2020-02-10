namespace InOutManagement.Controls
{
    #region using driective

    using InOutManagement.Common;
    using InOutManagement.Entity;
    using InOutManagement.SQLHelper;
    using InOutManagement.Windows;

    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    #endregion
    public partial class QueryView : INotifyPropertyChanged
    {
        public ObservableCollection<String> MaterialList { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<String> ModelList { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<Query> ViewList { get; set; } = new ObservableCollection<Query>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void Query(object sender, RoutedEventArgs e)
        {
            this.ViewList.Clear();
            var query = new Query()
            {
                Supplier = String.IsNullOrEmpty(this.Supplier.Text) == false ? this.Supplier.Text : String.Empty,
                Status = String.IsNullOrEmpty(this.Status.Text) == false ? this.Status.Text : String.Empty,
                Name = String.IsNullOrEmpty(this.Material.Text) == false ? this.Material.Text : String.Empty,
                Model = String.IsNullOrEmpty(this.Model.Text) == false ? this.Model.Text : String.Empty,
            };
            var queryResult = this.sqlHelper.Query<Query>(query);
            foreach (var item in queryResult)
            {
                if (this.FilterData(item) == false)
                {
                    this.ViewList.Add(new Query()
                    {
                        BillArchive = item.BillArchive,
                        Count = item.Count,
                        Date = item.Date.Substring(0, 10),
                        Model = item.Model,
                        Name = item.Name,
                        Price = item.Price,
                        Status = item.Status,
                        Supplier = item.Supplier,
                        Unit = item.Unit,
                    });
                }
            }
            this.mainWindow.WindowsStatus = MessageEnum.QuerySuccessful;
        }

        private Boolean FilterData(Query query)
        {
            Boolean result = false;
            DateTime date;
            if (DateTime.TryParse(query.Date.Substring(0, 10), out date))
            {
                if (this.StartDate.SelectedDate != null)
                {
                    if (date < this.StartDate.SelectedDate)
                    {
                        result = true;
                    }
                }
                if (this.EndDate.SelectedDate != null)
                {
                    if (date > this.EndDate.SelectedDate)
                    {
                        result = true;
                    }
                }
            }
            return result;
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

        private MainWindow mainWindow = MainWindow.GetInstance();
    }
}