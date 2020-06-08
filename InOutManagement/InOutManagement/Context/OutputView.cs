﻿namespace InOutManagement.Controls
{
    #region using directive

    using InOutManagement.Common;

    using InOutManagement.Entity;
    using InOutManagement.SQLHelper;
    using InOutManagement.Windows;

    using log4net;
    using OfficeOpenXml.ConditionalFormatting;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Timers;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    #endregion

    public partial class OutputView : UserControl, INotifyPropertyChanged
    {
        private static ILog Logger = LogManager.GetLogger(typeof(InputView));

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
                       (String.IsNullOrEmpty(this.Warehouse.Text) == false) &&
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
                return "当前库存：" + this.CalculateCount().ToString();
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
                IsDeleated = false,
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

        private void Warehouse_DropDownOpened(object sender, EventArgs e)
        {
            this.isWarehouseClear = true;
            var tempText = this.Warehouse.Text;
            this.Warehouse.Items.Clear();
            if ((String.IsNullOrEmpty(this.Material.Text) == false) &&
                (String.IsNullOrEmpty(this.Model.Text) == false))
            {
                var materialResult = this.sqlHelper.Query<Material>(
                    new Material()
                    {
                        Name = this.Material.Text,
                        Model = this.Model.Text,
                    });
                if (materialResult.Count > 0)
                {
                    var inputResult = this.sqlHelper.Query<Input>(
                    new Input()
                    {
                        Material = materialResult[0].Identity,
                        Supplier = this.Supplier.Text,
                    });
                    foreach (var item in inputResult)
                    {
                        var result = this.sqlHelper.Query<Warehouse>(
                        new Warehouse()
                        {
                            Identity = item.Warehouse,
                            IsDeleated = false,
                        }, new List<String>() { "Name" });
                        if (result.Count > 0)
                        {
                            if (this.Warehouse.Items.Contains(result[0].Name) == false)
                            {
                                this.Warehouse.Items.Add(result[0].Name);
                            }
                        }
                    }
                }
            }
            this.warehouseText = tempText;
            this.Warehouse.Text = tempText;
            this.isWarehouseClear = false;
        }

        private void Count_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.Count.Text) == false)
            {
                Double currentCount = 0;
                if (Double.TryParse(this.Count.Text, out currentCount) == false)
                {
                    this.Count.Text = this.Count.Text.Remove(this.Count.Text.Length - 1, 1);
                }
                if (currentCount > this.CalculateCount())
                {
                    this.Count.Text = this.Count.Text.Remove(this.Count.Text.Length - 1, 1);
                    this.Count.SelectionStart = this.Count.Text.Length;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var message = new PromptMessageBox();
            if (message.ShowMessage(this.ToString()) == true)
            {
                if (this.Validate() == true)
                {
                    var inputIdentities = this.ObtainInputIdentity();
                    var tempCount = Convert.ToDouble(this.Count.Text);
                    foreach (var identity in inputIdentities)
                    {
                        if (tempCount > 0)
                        {
                            var tempIndentity = identity;
                            var count = this.CalculateInputLaveCount(ref tempIndentity);
                            if (count > 0)
                            {
                                Double realCount = tempCount > count ? count : tempCount;
                                tempCount -= realCount;
                                if (this.InsertOutput(ref tempIndentity, ref realCount) == false)
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
                    Logger.Error("数据验证失败");
                    this.mainWindow.WindowsStatus = MessageEnum.OutputFailed;
                    MessageBox.Show("检出失败", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private Boolean InsertOutput(ref Int32 inputIdentity, ref Double materialCount)
        {
            Boolean result = false;
            var inputResult = this.sqlHelper.Query<Input>(new Input()
            {
                Identity = inputIdentity,
                IsDeleated = false
            });
            if (inputResult.Count > 0)
            {
                var output = new Output()
                {
                    Count = materialCount,
                    OutputDate = this.StartDate.SelectedDate != null ?
                                 this.StartDate.SelectedDate?.ToString("yyyy-MM-dd HH-mm-ss:ffffff") :
                                 DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss:ffffff"),
                    Pickup = this.Pickup.Text,
                    Input = inputIdentity,
                    Warehouse = inputResult[0].Warehouse,
                    IsDeleated = false,
                };
                result = this.sqlHelper.Insert<Output>(output);
            }
            return result;
        }

        private List<Int32> ObtainInputIdentity()
        {
            Logger.Info("Func in.");
            List<Int32> result = new List<Int32>();
            if ((String.IsNullOrEmpty(this.Material.Text.Trim()) == false) &&
               (String.IsNullOrEmpty(this.Model.Text.Trim()) == false) &&
               (String.IsNullOrEmpty(this.Warehouse.Text.Trim()) == false))
            {
                var materialResult = this.sqlHelper.Query<Material>(new Material()
                {
                    Name = this.Material.Text,
                    Model = this.Model.Text,
                });
                var warehouseResult = this.sqlHelper.Query<Warehouse>(new Warehouse()
                {
                    Name = this.Warehouse.Text,
                    IsDeleated = false,
                });
                foreach (var material in materialResult)
                {
                    Input input = new Input()
                    {
                        Supplier = this.Supplier.Text,
                        Material = material.Identity,
                        IsDeleated = false,
                        Warehouse = warehouseResult.Count > 0 ? warehouseResult[0].Identity : -1,
                    };
                    var inputResult = this.sqlHelper.Query<Input>(input);
                    foreach (var item in inputResult)
                    {
                        result.Add(item.Identity);
                    }
                }
            }
            Logger.Info("Func out.");
            return result;
        }

        private Double CalculateInputLaveCount(ref Int32 inputIdentity)
        {
            Logger.Info("Func in.");
            Double result = 0;
            var inputResult = this.sqlHelper.Query<Input>(new Input()
            {
                IsDeleated = false,
                Identity = inputIdentity,
            });
            var outputResult = this.sqlHelper.Query<Output>(new Output()
            {
                IsDeleated = false,
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
            Logger.Info("Func out.");
            return result;
        }

        private Double CalculateCount()
        {
            Double result = 0;
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
            this.Warehouse.Text = String.Empty;
        }

        private void Initial()
        {
            this.SetMaterialEvent();
            this.SetWarehouseEvent();
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

        private void SetWarehouseEvent()
        {
            var warehouseContent = (TextBox)this.Warehouse.Template.FindName("ComboBoxContent", this.Warehouse);
            if (warehouseContent != null)
            {
                warehouseContent.TextChanged += delegate (object sender, TextChangedEventArgs e)
                {
                    var text = (TextBox)sender;
                    if (this.isWarehouseClear == false)
                    {
                        if (this.warehouseText != text.Text)
                        {
                            this.warehouseText = text.Text;
                        }
                    }
                    this.isWarehouseClear = false;
                };
            }
        }

        private Boolean Validate()
        {
            return (String.IsNullOrEmpty(this.Supplier.Text) == false) &&
                   (String.IsNullOrEmpty(this.Material.Text) == false) &&
                   (String.IsNullOrEmpty(this.Model.Text) == false) &&
                   (String.IsNullOrEmpty(this.Warehouse.Text) == false) &&
                   (String.IsNullOrEmpty(this.Pickup.Text) == false) &&
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
        private Boolean isWarehouseClear = false;
        private String warehouseText = String.Empty;

        private Timer timer = new Timer(100);
        private MainWindow mainWindow;
        #endregion


        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("出库时间: ").Append(this.StartDate.SelectedDate?.ToShortDateString()).Append(Environment.NewLine);
            result.Append("出库类别: ").Append(this.Supplier.Text).Append(Environment.NewLine);
            result.Append("出库材料: ").Append(this.Material.Text).Append(Environment.NewLine);
            result.Append("仓库名称: ").Append(this.Warehouse.Text).Append(Environment.NewLine);
            result.Append("材料规格: ").Append(this.Model.Text).Append(Environment.NewLine);
            result.Append("出库数量: ").Append(this.Count.Text).Append(Environment.NewLine);
            result.Append("取货人: ").Append(this.Pickup.Text).Append(Environment.NewLine);
            return result.ToString();
        }
    }
}