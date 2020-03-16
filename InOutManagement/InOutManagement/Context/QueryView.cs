namespace InOutManagement.Controls
{
    #region using driective

    using InOutManagement.Common;
    using InOutManagement.Entity;
    using InOutManagement.SQLHelper;
    using InOutManagement.Windows;

    using log4net;

    using Microsoft.Win32;

    using OfficeOpenXml;

    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;


    #endregion
    public partial class QueryView : INotifyPropertyChanged
    {
        private static ILog Logger = LogManager.GetLogger(typeof(QueryView));

        public ObservableCollection<String> MaterialList { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<String> ModelList { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<String> PickuplList { get; set; } = new ObservableCollection<String>();
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
                Pickup = String.IsNullOrEmpty(this.Pickup.Text) == false ? this.Pickup.Text : String.Empty,
            };
            var queryResult = this.sqlHelper.Query<Query>(query);
            foreach (var item in queryResult)
            {
                if (this.FilterData(item) == false)
                {
                    this.ViewList.Add(new Query()
                    {
                        Count = item.Count,
                        Date = item.Date.Substring(0, 10),
                        Model = item.Model,
                        Name = item.Name,
                        Price = item.Price,
                        Status = item.Status,
                        Supplier = item.Supplier,
                        Unit = item.Unit,
                        Pickup = item.Pickup,
                        Identity = item.Identity,
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

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewList[this.QueryListView.SelectedIndex].Status == "入库")
            {
                Output outRecord = new Output
                {
                    Input = this.ViewList[this.QueryListView.SelectedIndex].Identity,
                };
                Input record = new Input()
                {
                    Identity = this.ViewList[this.QueryListView.SelectedIndex].Identity,
                };
                this.sqlHelper.Delete<Input>(record);
            }
            else if (this.ViewList[this.QueryListView.SelectedIndex].Status == "出库")
            {
                Output record = new Output()
                {
                    Identity = this.ViewList[this.QueryListView.SelectedIndex].Identity,
                };
                this.sqlHelper.Delete<Output>(record);
            }
            this.Query(this, null);
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

        private void Pickup_DropDownOpened(object sender, EventArgs e)
        {
            this.PickuplList.Clear();
            var materials = this.sqlHelper.Query<Output>(new Output() { IsDeleated = false, });
            foreach (var item in materials)
            {
                this.PickuplList.Add(item.Pickup);
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage excel = new ExcelPackage();
            Task task = new Task(() =>
            {
                this.isImported = false;
                while (this.isImported == false)
                {
                    this.OnUpdateProcess(this.message, this.current / this.total);
                    Thread.Sleep(1);
                }
            });
            var processBar = new ProcessBar();
            this.UpdateProcesEvent += processBar.UpdateProcess;
            task.Start();
            Action saveAcation = new Action(() =>
            {
                this.SaveInputToExcel(excel);
                this.SaveOutputToExcel(excel);
                this.SaveQueryToExcel(excel);
                processBar.CloseDialog();
                this.isImported = true;
            });
            processBar.ShowSchedule(saveAcation);
            this.UpdateProcesEvent -= processBar.UpdateProcess;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Microsoft Excel|*.xlsx";
            if (saveDialog.ShowDialog() == true)
            {
                var filePath = saveDialog.FileName;
                filePath = filePath.Contains(".xlsx") == false ? filePath + ".xlsx" : filePath;
                FileInfo file = new FileInfo(filePath);
                excel.SaveAs(file);
            }            
            this.mainWindow.WindowsStatus = MessageEnum.ImportSuccessful;
            MessageBox.Show("导出完成.", "提示", MessageBoxButton.OK);
        }

        private void SaveInputToExcel(in ExcelPackage excel)
        {
            if (excel != null)
            {
                var inputSheet = excel.Workbook.Worksheets.Add("录入");
                inputSheet.DefaultColWidth = 20;
                /// 设置Header
                inputSheet.Cells[1, 1].Value = "时间";
                inputSheet.Cells[1, 2].Value = "类别";
                inputSheet.Cells[1, 3].Value = "材料";
                inputSheet.Cells[1, 4].Value = "型号";
                inputSheet.Cells[1, 5].Value = "单位";
                inputSheet.Cells[1, 6].Value = "总数量";
                inputSheet.Cells[1, 7].Value = "剩余数量";
                Int32 rowNum = 2;
                this.message = "导出录入信息";
                this.total = this.ViewList.Count;
                this.current = 0;
                foreach (var item in this.ViewList)
                {
                    this.current++;
                    if (item.Status == "入库")
                    {
                        inputSheet.Cells[rowNum, 1].Value = item.Date;
                        inputSheet.Cells[rowNum, 2].Value = item.Supplier;
                        inputSheet.Cells[rowNum, 3].Value = item.Name;
                        inputSheet.Cells[rowNum, 4].Value = item.Model;
                        inputSheet.Cells[rowNum, 5].Value = item.Unit;
                        inputSheet.Cells[rowNum, 6].Value = item.Count;
                        Int32 inputIdentity = item.Identity;
                        inputSheet.Cells[rowNum, 7].Value = this.CalculateInputLaveCount(ref inputIdentity).ToString();
                        rowNum++;
                    }
                }
            }
        }

        private void SaveOutputToExcel(in ExcelPackage excel)
        {
            if (excel != null)
            {
                this.message = "导出检出信息";
                this.total = this.ViewList.Count;
                this.current = 0;
                var outputSheet = excel.Workbook.Worksheets.Add("检出");
                outputSheet.DefaultColWidth = 20;
                /// 设置Header
                outputSheet.Cells[1, 1].Value = "时间";
                outputSheet.Cells[1, 2].Value = "类别";
                outputSheet.Cells[1, 3].Value = "材料";
                outputSheet.Cells[1, 4].Value = "型号";
                outputSheet.Cells[1, 5].Value = "单位";
                outputSheet.Cells[1, 6].Value = "提取人";
                outputSheet.Cells[1, 7].Value = "提取数量";
                Int32 rowNum = 2;
                foreach (var item in this.ViewList)
                {
                    current++;
                    if (item.Status == "出库")
                    {
                        outputSheet.Cells[rowNum, 1].Value = item.Date;
                        outputSheet.Cells[rowNum, 2].Value = item.Supplier;
                        outputSheet.Cells[rowNum, 3].Value = item.Name;
                        outputSheet.Cells[rowNum, 4].Value = item.Model;
                        outputSheet.Cells[rowNum, 5].Value = item.Unit;
                        outputSheet.Cells[rowNum, 6].Value = item.Pickup;
                        outputSheet.Cells[rowNum, 7].Value = item.Count;
                        rowNum++;
                    }
                }
            }
        }

        private void SaveQueryToExcel(in ExcelPackage excel)
        {
            if (excel != null)
            {
                this.message = "导出统计信息";
                this.total = this.ViewList.Count;
                this.current = 0;
                var querySheet = excel.Workbook.Worksheets.Add("统计");
                querySheet.DefaultColWidth = 20;
                /// 设置Header
                querySheet.Cells[1, 1].Value = "时间";
                querySheet.Cells[1, 2].Value = "类别";
                querySheet.Cells[1, 3].Value = "材料";
                querySheet.Cells[1, 4].Value = "型号";
                querySheet.Cells[1, 5].Value = "单位";
                querySheet.Cells[1, 6].Value = "价格";
                querySheet.Cells[1, 7].Value = "数量";
                querySheet.Cells[1, 8].Value = "状态";
                querySheet.Cells[1, 9].Value = "提取人";
                Int32 rowNum = 2;
                foreach (var item in this.ViewList)
                {
                    this.current++;
                    querySheet.Cells[rowNum, 1].Value = item.Date;
                    querySheet.Cells[rowNum, 2].Value = item.Supplier;
                    querySheet.Cells[rowNum, 3].Value = item.Name;
                    querySheet.Cells[rowNum, 4].Value = item.Model;
                    querySheet.Cells[rowNum, 5].Value = item.Unit;
                    querySheet.Cells[rowNum, 6].Value = item.Pickup;
                    querySheet.Cells[rowNum, 7].Value = item.Count;
                    querySheet.Cells[rowNum, 8].Value = item.Status;
                    querySheet.Cells[rowNum, 9].Value = item.Pickup;
                    rowNum++;
                }
            }
        }

        private Int32 CalculateInputLaveCount(ref Int32 inputIdentity)
        {
            Logger.Info("Func in.");
            Int32 result = 0;
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

        private SQLHelper sqlHelper = SQLHelper.GetInstance();

        private MainWindow mainWindow;
        private Int32 total = 1;
        private Int32 current = 0;
        private String message = String.Empty;
        private bool isImported = false;

        public delegate void UpdateProcess(String message, Double process);
        public event UpdateProcess UpdateProcesEvent;

        private void OnUpdateProcess(String message, Double process)
        {
            var temp = this.UpdateProcesEvent;
            if (temp != null)
            {
                temp(message, process);
            }
        }

    }
}