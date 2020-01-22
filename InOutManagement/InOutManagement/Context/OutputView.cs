namespace InOutManagement.Controls
{

    #region using directive

    using InOutManagement.Entity;
    using InOutManagement.SQLHelper;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    #endregion

    public partial class OutputView : UserControl, INotifyPropertyChanged
    {
        #region Title

        private String title = "检出材料";
        public String Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                this.OnPropertyChanged("Title");
            }
        }

        #endregion

        #region Icon

        private ImageSource icon = BitmapFrame.Create(new Uri(@"Resources/Icons/Output.ico", UriKind.RelativeOrAbsolute));
        public ImageSource Icon
        {
            get
            {
                return this.icon;
            }
            set
            {
                this.icon = value;
                this.OnPropertyChanged("Icon");
            }
        }

        #endregion 

        private void Material_DropDownOpened(object sender, EventArgs e)
        {
            var tempText = this.Material.Text;
            this.Material.Items.Clear();
            var input = new Input()
            {
                Supplier = this.Supplier.Text,
            };
            var queryColumn = new List<String>();
            queryColumn.Add("Material");
            var inputResult = this.sqlHelper.Query<Input>(input, queryColumn);
            foreach (var item in inputResult)
            {
                var material = new Material()
                {
                    Identity = item.Material,
                };
                var queryResult = this.sqlHelper.Query<Material>(material);
                foreach (var query in queryResult)
                {
                    this.Material.Items.Add(query.Name);
                }
            }
            this.Material.Text = tempText;
        }

        private void Model_DropDownOpened(object sender, EventArgs e)
        {
            var tempText = this.Model.Text;
            this.Model.Items.Clear();
            var material = new Material()
            {
                Name = this.Material.Text,
            };
            var materialResult = this.sqlHelper.Query<Material>(material);
            foreach (var item in materialResult)
            {
                this.Model.Items.Add(item.Model);
            }
            this.Model.Text = tempText;
        }

        private void Count_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.Count.Text) == false)
            {                
                if (Int32.TryParse(this.Count.Text, out _) == false)
                {
                    this.Count.Text = this.Count.Text.Remove(this.Count.Text.Length - 1 , 1);
                }                
            }
        }

        private Int32 CalculateCount()
        {
            Int32 result = 0;
            var material = new Material()
            {
                Name = this.Material.Text,
                Model = this.Model.Text,
            };
            var materialQuery = this.sqlHelper.Query<Material>(material);
            if (materialQuery.Count > 0)
            {
                var input = new Input()
                {
                    Supplier = this.Supplier.Text,
                    Material = materialQuery[0].Identity,
                };
                var queryResult = this.sqlHelper.Query<Input>(input);
                foreach (var item in queryResult)
                {
                    result += item.Count;
                }
            }
            return result;
        }

        public void OnPropertyChanged(String propertyName)
        {
            var temp = this.PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private SQLHelper sqlHelper = SQLHelper.GetInstance();
    }
}