namespace InOutManagement.Controls
{
    using InOutManagement.Common;

    #region using directive

    using InOutManagement.Entity;
    using InOutManagement.SQLHelper;
    using InOutManagement.Windows;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Timers;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    #endregion

    public partial class OutputView : UserControl, INotifyPropertyChanged
    {
        #region Binding Property

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

        #region MaxCountVisiblity

        public Visibility MaxCountVisiblity
        {
            get
            {
                return (String.IsNullOrEmpty(this.Supplier.Text) == false) &&
                       (String.IsNullOrEmpty(this.Material.Text) == false) &&
                       (String.IsNullOrEmpty(this.Model.Text) == false) ?
                       Visibility.Visible :
                       Visibility.Hidden;

            }
            set
            {
                this.OnPropertyChanged("MaxCountVisiblity");
            }
        }

        public String MaxCountContent
        {
            get
            {
                return "但前库存：" + this.CalculateCount().ToString();
            }
            set
            {
                this.OnPropertyChanged("MaxCountContent");
            }
        }


        #endregion

        #endregion
        private void Material_DropDownOpened(object sender, EventArgs e)
        {
            var tempText = this.materialText;
            this.isMaterialClear = true;
            this.Material.Items.Clear();
            var input = new Input()
            {
                Supplier = this.Supplier.Text,
            };
            var queryColumn = new List<String>() { "Material" };
            var inputResult = this.sqlHelper.Query<Input>(input, queryColumn);
            foreach (var item in inputResult)
            {
                var queryResult = this.sqlHelper.Query<Material>(
                    new Material()
                    {
                        Identity = item.Material,
                    });
                foreach (var query in queryResult)
                {
                    this.Material.Items.Add(query.Name);
                }
            }
            this.materialText = tempText;
            this.Material.Text = tempText;
            this.isMaterialClear = false;
        }

        private void Model_DropDownOpened(object sender, EventArgs e)
        {
            var tempText = this.Model.Text;
            this.Model.Items.Clear();
            var materialResult = this.sqlHelper.Query<Material>(
                new Material()
                {
                    Name = this.Material.Text,
                });
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
                    this.Count.Text = this.Count.Text.Remove(this.Count.Text.Length - 1, 1);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.Validate() == true)
            {
                var inputIdentities = this.ObtainInputIdentity();
                var tempCount = Convert.ToInt32(this.Count.Text);
                foreach (var identity in inputIdentities)
                {
                    if (tempCount > 0)
                    {
                        var tempIndentity = identity;
                        var count = this.CalculateInputLaveCount(ref tempIndentity);
                        if (count > 0)
                        {
                            Int32 realCount = tempCount > count ? count : tempCount;
                            tempCount -= realCount;
                            if (this.InsertOutput(ref tempIndentity, ref realCount))
                            {
                                this.mainWindow.WindowsStatus = MessageEnum.OutputError;
                                MessageBox.Show("检出失败", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                                break;
                            }
                        }
                    }
                }
                this.mainWindow.WindowsStatus = MessageEnum.OutputSuccess;
                MessageBox.Show("检出成功", "提示", MessageBoxButton.OK);
            }
            else
            {
                this.mainWindow.WindowsStatus = MessageEnum.OutputFailed;
                MessageBox.Show("检出失败", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private Boolean InsertOutput(ref Int32 inputIdentity, ref Int32 materialCount)
        {
            Boolean result = false;
            var output = new Output()
            {
                Count = materialCount,
                OutputDate = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss:ffffff"),
                BillArchive = this.BillArchive.Text,
                Input = inputIdentity,
            };
            result = this.sqlHelper.Insert<Output>(output);
            return result;
        }

        private List<Int32> ObtainInputIdentity()
        {
            List<Int32> result = new List<Int32>();
            var materialResult = this.sqlHelper.Query<Material>(new Material()
            {
                Name = this.Material.Text,
                Model = this.Model.Text,
            });
            foreach (var material in materialResult)
            {
                Input input = new Input()
                {
                    Supplier = this.Supplier.Text,
                    Material = material.Identity,
                };
                var inputResult = this.sqlHelper.Query<Input>(input);
                foreach (var item in inputResult)
                {
                    result.Add(item.Identity);
                }
            }
            return result;
        }

        private Int32 CalculateInputLaveCount(ref Int32 inputIdentity)
        {
            Int32 result = 0;
            var inputResult = this.sqlHelper.Query<Input>(new Input()
            {
                Identity = inputIdentity,
            });
            var outputResult = this.sqlHelper.Query<Output>(new Output()
            {
                Input = inputIdentity,
            });
            foreach (var input in inputResult)
            {
                result += input.Count;
            }
            foreach (var output in outputResult)
            {
                result -= output.Count;
            }
            return result;
        }

        private Int32 CalculateCount()
        {
            Int32 result = 0;
            if (String.IsNullOrEmpty(this.Model.Text) == false)
            {
                var inputIdentities = this.ObtainInputIdentity();
                foreach (var identity in inputIdentities)
                {
                    var tempIndentity = identity;
                    result += this.CalculateInputLaveCount(ref tempIndentity);
                }
            }
            return result;
        }

        private void Supplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Material.Text = String.Empty;
            this.Model.Text = String.Empty;
        }

        private void Initial()
        {
            this.SetMaterialEvent();
            this.timer.Elapsed += delegate
            {
                this.MaxCountVisiblity = Visibility.Hidden;
                this.MaxCountContent = String.Empty;
            };
            this.timer.Start();
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
                        if (this.materialText != text.Text)
                        {
                            this.materialText = text.Text;
                            this.Model.Text = String.Empty;
                        }
                    }
                    this.isMaterialClear = false;
                };
            }
        }

        private Boolean Validate()
        {
            return (String.IsNullOrEmpty(this.Supplier.Text) == false) &&
                   (String.IsNullOrEmpty(this.Material.Text) == false) &&
                   (String.IsNullOrEmpty(this.Model.Text) == false) &&
                   (String.IsNullOrEmpty(this.Count.Text) == false);                   
        }

        #region NotifyEnevt

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

        #region private 

        private SQLHelper sqlHelper = SQLHelper.GetInstance();
        private Boolean isMaterialClear = false;
        private String materialText = String.Empty;
        private Timer timer = new Timer(100);
        private MainWindow mainWindow;
        #endregion
    }
}