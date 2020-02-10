namespace InOutManagement.Controls
{
    using InOutManagement.Common;
    using InOutManagement.Entity;

    #region using directive

    using InOutManagement.SQLHelper;
    using InOutManagement.Windows;

    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    #endregion

    /// <summary>
    /// Interaction logic for InputView.xaml
    /// </summary>
    /// 
    public partial class InputView : UserControl, INotifyPropertyChanged
    {
        #region Binding Property

        #region Title

        private String title = "录入材料";
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

        private ImageSource icon = BitmapFrame.Create(new Uri(@"Resources/Icons/Input.ico", UriKind.RelativeOrAbsolute));
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

        #region Unit

        private String unitContent;
        public String UntiContent
        {
            get
            {
                return unitContent;
            }
            set
            {
                this.unitContent = value;
                this.OnPropertyChanged("UntiContent");
            }
        }

        #endregion

        #region Material 

        private String materialText = String.Empty;
        public String MaterialText
        {
            get
            {
                return this.materialText;
            }
            set
            {
                this.materialText = value;
                this.OnPropertyChanged("MaterialText");
            }
        }

        public ObservableCollection<String> MaterialList { get; set; } = new ObservableCollection<String>();

        #endregion

        #region Model

        private String modelText = String.Empty;
        public String ModelText
        {
            get
            {
                return this.modelText;
            }
            set
            {
                this.modelText = value;
                this.OnPropertyChanged("ModelText");
            }
        }

        public ObservableCollection<String> ModelList { get; set; } = new ObservableCollection<String>();

        #endregion

        #endregion

        #region Notify Event 

        public void OnPropertyChanged(String propertyName)
        {
            var temp = this.PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void Material_DropDownOpened(object sender, EventArgs e)
        {
            var temp = this.MaterialText;
            this.isMaterialClear = true;
            this.MaterialList.Clear();
            var materials = this.sqlHelper.Query<Material>(new Material());
            foreach (var item in materials)
            {
                if (item.Name.Contains(temp.Trim()) == false)
                {
                    continue;
                }
                this.MaterialList.Add(item.Name);
            }
            this.isMaterialClear = false;
            this.Material.Text = temp;
        }

        private void Model_DropDownOpened(object sender, EventArgs e)
        {
            var temp = this.ModelText;
            this.isModelClear = true;
            this.ModelList.Clear();
            if (String.IsNullOrEmpty(this.MaterialText) == false)
            {
                var models = this.sqlHelper.Query<Material>(new Material() { Name = this.MaterialText });
                foreach (var item in models)
                {
                    if (String.IsNullOrEmpty(this.modelText.Trim()) == false)
                    {
                        if (item.Name.Contains(this.modelText.Trim()) == false)
                        {
                            continue;
                        }
                    }
                    this.ModelList.Add(item.Name);
                }
            }
            this.isModelClear = false;
            this.Model.Text = temp;
        }

        private void Count_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.Count.Text) == false)
            {
                if (Int32.TryParse(this.Count.Text, out _) == false)
                {
                    this.Count.Text = this.Count.Text.Remove(this.Count.Text.Length - 1, 1);
                    this.Count.SelectionStart = this.Count.Text.Length;
                }
            }
        }

        private void Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.Price.Text) == false)
            {
                if (Double.TryParse(this.Price.Text, out _) == false)
                {
                    this.Price.Text = this.Price.Text.Remove(this.Price.Text.Length - 1, 1);
                    this.Price.SelectionStart = this.Price.Text.Length;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.Validate() == true)
            {
                if (InsertMaterial() && InsertInput())
                {
                    this.mainWindow.WindowsStatus = MessageEnum.InsertSuccess;
                    MessageBox.Show("录入成功", "提示", MessageBoxButton.OK);
                }
                else
                {
                    this.mainWindow.WindowsStatus = MessageEnum.InsertError;
                    MessageBox.Show("录入失败", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                this.mainWindow.WindowsStatus = MessageEnum.InsertFailed;
                MessageBox.Show("录入失败", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Initial()
        {
            this.SetMaterialEvent();
            this.SetModelEvent();
        }

        private void SetMaterialEvent()
        {
            var materialContent = (TextBox)this.Material.Template.FindName("ComboBoxContent", this.Material);
            if (materialContent != null)
            {
                materialContent.TextChanged += delegate (object sender, TextChangedEventArgs e)
                {
                    var text = (TextBox)sender;
                    if (this.isMaterialClear == false)
                    {
                        if ((this.MaterialText != text.Text))
                        {
                            this.MaterialText = text.Text;
                            this.Material_DropDownOpened(null, null);
                        }
                    }
                    text.SelectionStart = text.Text.Length;
                    this.isMaterialClear = false;
                };
            }
        }

        private void SetModelEvent()
        {
            var modelContent = (TextBox)this.Model.Template.FindName("ComboBoxContent", this.Model);
            if (modelContent != null)
            {
                modelContent.TextChanged += delegate (object sender, TextChangedEventArgs e)
                {
                    var text = (TextBox)sender;
                    if (this.isModelClear == false)
                    {
                        if (this.ModelText != text.Text)
                        {
                            this.ModelText = text.Text;
                            this.Model_DropDownOpened(null, null);
                        }
                    }
                    this.SetCountText();
                    text.SelectionStart = text.Text.Length;
                    this.isModelClear = false;
                };
            }
        }

        private void SetCountText()
        {
            var material = new Material()
            {
                Name = this.MaterialText,
                Model = this.ModelText,
            };
            var queryList = sqlHelper.Query<Material>(material);
            if (queryList.Count > 0)
            {
                this.UntiContent = queryList[0].Unit;
            }
        }

        private Boolean InsertMaterial()
        {
            Boolean result = true;
            var material = new Material()
            {
                Name = this.materialText,
                Model = this.modelText,
                Unit = this.UntiContent,
            };
            var queryResult = sqlHelper.Query<Material>(material);
            if (queryResult?.Count == 0)
            {
                result = sqlHelper.Insert<Material>(material);
            }
            return result;
        }

        private Boolean InsertInput()
        {
            Boolean result = false;
            var material = new Material()
            {
                Name = this.materialText,
                Model = this.modelText,
                Unit = this.UntiContent,
            };
            var queryResult = sqlHelper.Query<Material>(material);
            if (queryResult.Count > 0)
            {
                var input = new Input()
                {
                    Material = queryResult[0].Identity,
                    BillArchive = this.BillArchive.Text,
                    Count = Convert.ToInt32(this.Count.Text),
                    InputDate = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss:ffffff"),
                    Price = Convert.ToDouble(this.Price.Text),
                    Supplier = this.Supplier.Text,
                };
                result = this.sqlHelper.Insert<Input>(input);
            }
            return result;
        }

        private Boolean Validate()
        {
            return (String.IsNullOrEmpty(this.Supplier.Text) == false) &&
                   (String.IsNullOrEmpty(this.Material.Text) == false) &&
                   (String.IsNullOrEmpty(this.Model.Text) == false) &&
                   (String.IsNullOrEmpty(this.Count.Text) == false) &&
                   (String.IsNullOrEmpty(this.Unit.Text) == false) &&
                   (String.IsNullOrEmpty(this.Price.Text) == false);
        }

        #region private 

        private SQLHelper sqlHelper = SQLHelper.GetInstance();

        /// 下拉框下拉获取数据，清空数据导致Text Changed触发
        /// 在TextChanged 事件动态过滤触发下拉事件
        /// 为防止无限递归，保证数据准确设置此控制
        private Boolean isMaterialClear = false;
        private Boolean isModelClear = false;

        private MainWindow mainWindow;

        #endregion
    }
}
